using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzoftMod
{
    public class Greenhouse
    {
        public string ghId { get; set; }
        public string description { get; set; }
        public double temperature_min { get; set; }
        public double temperature_opt { get; set; }
        public double humidity_min { get; set; }
        public double volume { get; set; }
    }
}
