using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppNS
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class ReflectionMark : System.Attribute
    {
        public ReflectionMark(params string[] textValues) {
            textValues = textValues ?? new string[] { };
            this.TextValues = textValues;
        }

        public string[] TextValues { get; set; }
    }
}
