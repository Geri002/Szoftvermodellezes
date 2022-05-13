using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzoftMod
{
    public interface IMonitor
    {
        SensorData getSensorData(string ghId);
    }
}
