using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace GPSFilter
{
    public class LatLongSet
    {
        public List<double> LatList { get; set; }
        public List<double> LongList { get; set; }
        public int Count { get
        {
            if (LatList?.Count == LongList?.Count)
                {
                    return LatList.Count;
                }
            return -1;
        } }


    }
}
