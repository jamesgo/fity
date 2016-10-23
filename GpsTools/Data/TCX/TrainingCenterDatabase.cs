using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsTools.Data.TCX
{
    public class TrainingCenterDatabase
    {
        public IEnumerable<Activity> Activities { get; set; }

        public Author Author { get; set; }
    }
}
