using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTools.Data.TCX
{
    public class Activity
    {
        public string Sport { get; set; }

        public string Id { get; set; }

        public string Notes { get; set; }

        public Lap Lap { get; set; }
    }
}
