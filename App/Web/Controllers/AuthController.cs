using QApp.Database;
using QApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QApp.Web.Controllers {
    public class AuthController : Controller {

        private static SemaphoreSlim bruteForceLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Submit credentials and receive an auth token in return. 
        /// </summary>
        /// <param name="token">This is ignored, and here to ease frontend use of this function.</param>
        /// <param name="username">The UserName for the user.</param>
        /// <param name="password">
        ///     An irreversible hash of the user's password with salt.
        ///     If somehow a person is transmitting without SSL, this provides
        ///     a safeguard.
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/{token}/Auth/LoginUser")]
        public async Task<Web_AuthWrapper> LoginUser(string token, string username, string password) {
            var ret = new Web_AuthWrapper();
            await AppDbContext.WithContext(async db => {
                if (username == null || password == null) {
                    throw new ArgumentNullException("username or password was null");
                }

                //Make sure the submitted hash isn't the same as the database hash.
                //The whole goal is to prevent reversing the hash and getting the original
                //password, so adding this and then doing an SHA256 makes it harder to do that.
                password = AuthHelper.GetDoubleHashedPassword(password);

                //Rate limit brute force attempts.
                await bruteForceLock.WaitAsync();
                try {
                    await Task.Delay(new Random().Next(25) + 100); //No guessing things from the timeout period.
                }
                finally {
                    bruteForceLock.Release();
                }

                //Get actual work done:
                username = username.ToUpper().Trim();
                var user = await db.Users.FirstOrDefaultAsync(x => x.Email.ToUpper() == username);
                if (user == null) {
                    return;
                }
                if (user.ResetPassword) {
                    user.PasswordHash = password;
                    user.ResetPassword = false;
                    await db.SaveChangesAsync();
                }

                if (user.PasswordHash != password) {
                    return;
                }
                else {
                    var utcNow = DateTime.UtcNow;
                    if (user.SessionTokenExpiresUtc < utcNow || user.SessionToken == null) {
                        user.SessionToken = "";
                        var r = RandomNumberGenerator.Create();
                        for (var i = 0; i < 4; i++) {
                            var cryptoBytes = new byte[4];
                            r.GetNonZeroBytes(cryptoBytes);
                            var num = 0;
                            for (int j = 0; j < cryptoBytes.Length; j++) {
                                num += (cryptoBytes[j] << (j * 8));
                            }
                            user.SessionToken += num.ToString();
                        }
                        user.SessionTokenExpiresUtc = utcNow.AddDays(30);
                        await db.SaveChangesAsync();
                    }
                    ret.token = user.SessionToken;
                    ret.id = user.Id;
                    ret.name = user.Name;
                    ret.isAdmin = user.IsAdmin;
                }
            });
            return ret;
        }

        /// <summary>
        /// Submit a token from a previous log in. If the token is still valid,
        /// the user's information will be returned.
        /// </summary>
        [HttpPost]
        [Route("api/v1/{token}/Auth/GetUserFromToken")]
        public async Task<Web_AuthWrapper> GetUserFromToken(string token) {
            if (token == null) {
                throw new ArgumentNullException("Token cannot be null.");
            }

            return await AppDbContext.WithContext(async db => {
                var utcNow = DateTime.UtcNow;
                var user = await db.Users.Select(x => new {
                    name = x.Name,
                    token = x.SessionToken,
                    tokenExpires = x.SessionTokenExpiresUtc,
                    id = x.Id,
                    isAdmin = x.IsAdmin
                }).FirstOrDefaultAsync(x => x.token == token && x.tokenExpires > utcNow);
                if (user == null) {
                    return null;
                }

                return new Web_AuthWrapper() {
                    name = user.name,
                    token = user.token.ToString(),
                    id = user.id,
                    isAdmin = user.isAdmin
                };
            });
        }

        /// <summary>
        /// The user with the given token is logged out everywhere. This is accomplished by changing their session key.
        /// </summary>
        [HttpPost]
        [Route("api/v1/{token}/Auth/LogoutUserEverywhere")]
        public async Task<Web_AuthResponse<bool>> LogoutUser(string token) {
            return await AuthHelper.WithUser(token, async (user, db) => {
                var fullUser = await db.Users.FirstAsync(x => x.Id == user.Id);
                fullUser.SessionToken = Util.GetCryptoGuid().ToString() + Util.GetCryptoGuid().ToString();
                await db.SaveChangesAsync();
                return true;
            });
        }

        /// <summary>
        /// Returns a user specific salt which allows client
        /// side password hashing to be done more effectively. The salt is combined with
        /// the user's password to increase its strength. It means that an
        /// attacker can't use a dictionary of common password MD5s to figure
        /// out a user's password. The goal isn't to prevent impersonation. The goal
        /// is to prevent an attacker from guessing the user's password,
        /// which may be used in other more critical systems, if it is somehow
        /// transmitted without SSL.
        /// </summary>
        /// <param name="username">The user whose salt we want.</param>
        /// <param name="token">This is ignored and just here for compatibility.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/{token}/Auth/Salt")]
        public async Task<string> Salt(string token, string username) {
            return await AppDbContext.WithContext(async db => {
                var pair = await db.Users.Select(x => new {
                    Id = x.Id,
                    Email = x.Email,
                    PasswordSalt = x.PasswordSalt
                }).FirstOrDefaultAsync(x => x.Email.ToUpper() == username.ToUpper());

                if (pair == null) {
                    return null;
                }

                if (pair.PasswordSalt == Guid.Empty) {
                    var salt = Guid.NewGuid();
                    var user = await db.Users.FirstOrDefaultAsync(x => x.Id == pair.Id);
                    user.PasswordSalt = salt;
                    await db.SaveChangesAsync();
                    return salt.ToString().ToLower();
                }
                return pair.PasswordSalt.ToString().ToLower();
            });
        }

        [HttpPut]
        [Route("api/v1/{token}/Auth/SetPassword/{id}")]
        public async Task<Web_AuthResponse<bool>> SetPassword(string token, int id, string password) {
            return await AuthHelper.WithUser(token, async (user, db) => {
                if (id != user.Id && !user.IsAdmin) {
                    throw new AuthException("User does not have permission to reset this password.");
                }
                var dbUser = await db.Users.FirstAsync(x => x.Id == id);

                password = AuthHelper.GetDoubleHashedPassword(password);
                dbUser.PasswordHash = password;
                await db.SaveChangesAsync();

                return true;
            });
        }
    }
}
