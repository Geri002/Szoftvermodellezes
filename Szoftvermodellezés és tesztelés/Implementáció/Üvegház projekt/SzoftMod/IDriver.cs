using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzoftMod
{
    public interface IDriver
    {
        int sendCommand(Greenhouse gh, string token, double boilerValue, double sprinklerValue);

    }
}
