using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Database.Models {
    public class BaseModel {
        [Key]
        public int Id { get; set; }
    }
}
