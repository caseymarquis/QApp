using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Web.Models {
    public class Web_AuthWrapper {
        /// <summary>
        /// A token for interacting in the API in the future.
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// The friendly name of the user for display purposes.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The id of the user with the returned token.
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Should the user be shown admin pages?
        /// </summary>
        public bool isAdmin { get; set; }
    }
}
