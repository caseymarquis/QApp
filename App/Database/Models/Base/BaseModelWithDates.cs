using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Database.Models {
    public class BaseModelWithDates : BaseModel {
        public BaseModelWithDates() {
            this.CreatedUtc = DateTime.UtcNow;
            this.ModifiedUtc = CreatedUtc;
        }

        public DateTime CreatedUtc { get; set; }
        public DateTime ModifiedUtc { get; set; }
    }
}
