using App.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Controllers {
    public static class AuthHelper {
        public static async Task<Web_AuthResponse<T>> WithUser<T>(string token, Func<UserAuthDetails, AppDbContext, Task<T>> doThis) {
            return await AppDbContext.WithContext(async db => {
                var user = await db.Users.Select(x => new UserAuthDetails {
                    Id = x.Id,
                    IsAdmin = x.IsAdmin,
                    SessionToken = x.SessionToken,
                }).FirstOrDefaultAsync(x => x.SessionToken == token);
                if (user == null) {
                    return new Web_AuthResponse<T> {
                        tokenNotFound = true
                    };
                }

                try {
                    var result = await doThis(user, db);
                    return new Web_AuthResponse<T>() {
                        data = result,
                    };
                }
                catch (AuthException ex) {
                    Util.Log.Error(user.Id.ToString(), "AuthHelper.NotAuthorized", ex);
                    return new Web_AuthResponse<T>() {
                        notAuthorized = true
                    };
                }
            });
        }

        internal static string GetDoubleHashedPassword(string password) {
            //Make sure the submitted hash isn't the same as the database hash.
            //The whole goal is to prevent reversing the hash and getting the original
            //password, so adding this and then doing an SHA256 makes it harder to do that.
            var hardCodedSalt = "Error: Unhandled Login Exception";
            password += hardCodedSalt;
            using (var sha = SHA256.Create()) {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = "";
                foreach (var b in bytes) {
                    password += b.ToString("x2");
                }
                return password;
            }
        }
    }

    public class UserAuthDetails {
        public int Id { get; set; }
        public bool IsAdmin { get; set; }
        public string SessionToken { get; set; }
    }

}
