using QApp.Database.Models;
using QApp.Web;
using QApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QApp.Web.Controllers.Helpers;

namespace QApp.Web.Controllers {
    [InjectDependencies]
    public class UserController : Controller {
        /// <summary>
        /// Returns a list of the existing users within the system.
        /// </summary>
        [HttpGet]
        [Route("api/v1/{token}/User")]
        public async Task<Web_AuthResponse<List<Web_User>>> UserList(string token) {
            return await AuthHelper.WithUser(token, async (user, db) => {
                if (!user.IsAdmin) {
                    throw new AuthException("User must be an administrator.");
                }
                var allUsersDb = await db.Users.OrderBy(x => x.Name).ToListAsync();

                return allUsersDb.Select(x => {
                    var userSummary = new Web_User();
                    userSummary.PullData(x);
                    return userSummary;
                }).ToList();
            });
        }

        /// <summary>
        /// Returns an existing user by id.
        /// </summary>
        [HttpGet]
        [Route("api/v1/{token}/User/{id}")]
        public async Task<Web_AuthResponse<Web_User>> UserGet(string token, int id) {
            return await AuthHelper.WithUser(token, async (user, db) => {
                if (!user.IsAdmin || user.Id != id) {
                    throw new AuthException("User must be an administrator.");
                }
                var dbUser = await db.Users.FirstAsync(x => x.Id == id);

                var userSummary = new Web_User();
                userSummary.PullData(dbUser);
                return userSummary;
            });
        }

        /// <summary>
        /// Edit an existing user. Fields which are not specified or are set to null will not be changed.
        /// The user with all fields populated will be returned.
        /// </summary>
        [HttpPut]
        [Route("api/v1/{token}/User/{id}")]
        public async Task<Web_AuthResponse<Web_User>> UserEdit(string token, int id, [FromBody]Web_User user) {
            return await AuthHelper.WithUser(token, async (currentUser, db) => {
                var self = this;
                if (!currentUser.IsAdmin || user.id != id) {
                    throw new AuthException("User must be an administrator.");
                }

                if (!currentUser.IsAdmin) {
                    //We could do this with a reflection attribute, it would put this in the web model
                    user.isAdmin = null;
                    user.email = null;
                    user.enabled = null;
                }

                user.email = user.email?.Trim().ToLower();
                if (user.email == "") {
                    throw new ArgumentException("Empty email set.");
                }

                var existingUser = await db.Users.FirstAsync(x => x.Id == id);
                if (existingUser.Id == 1) {
                    user.enabled = true;
                    user.isAdmin = true;
                }
                else {
                    var isDuplicateEmail = await db.Users
                    .AnyAsync(x => x.Id != id && x.Email != null && x.Email == user.email);

                    if (isDuplicateEmail) {
                        throw new ArgumentException("Email is already in use.");
                    }
                }

                user.PushData(existingUser);
                await db.SaveChangesAsync();
                user.PullData(existingUser);

                return user;
            });
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        [HttpPost]
        [Route("api/v1/{token}/User")]
        public async Task<Web_AuthResponse<Web_User>> UserCreate(string token) {
            return await AuthHelper.WithUser(token, async (currentUser, db) => {
                if (!currentUser.IsAdmin) {
                    throw new AuthException("User must be an administrator.");
                }
                var newUser = new User() {
                    Name = "_New User_",
                    SessionToken = (Util.GetCryptoGuid().ToString() + Util.GetCryptoGuid().ToString()).Replace("-", "").ToLower(),
                    PasswordHash = Util.GetCryptoGuid().ToString().Replace("-", ""),
                    PasswordSalt = Util.GetCryptoGuid(),
                };
                db.Users.Add(newUser);
                await db.SaveChangesAsync();

                var ret = new Web_User();
                ret.PullData(newUser);
                return ret;
            });
        }
    }
}
