using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SzoftMod
{
    internal class Controller
    {
        protected static Loader loader;
        protected static Monitor monitor;
        protected static Dictionary<string, SensorData> sensorDatas;
        protected static GreenHouseList greenHousesList;
        protected static Driver driver;
        protected static Dictionary<double, double> sprinklerTable;
        static void Main(string[] args)
        {
            initialize();
            loadSensorData();

            Console.WriteLine("GreenHouses:");
            foreach (var item in sensorDatas)
            {
                Console.WriteLine("{0} {1} {2} {3} {4}",item.Key, item.Value.temperature_act, item.Value.humidity_act ,item.Value.sprinkler_on, item.Value.boiler_on);
            }

            Console.WriteLine();

            for (int i = 0; i < greenHousesList.greenhouseList.Count; i++)
            {
                Console.WriteLine("GreenHouse ID:{0} - Code:{1}",greenHousesList.greenhouseList[i].ghId ,Convert.ToString(supervise(greenHousesList.greenhouseList[i], sensorDatas[greenHousesList.greenhouseList[i].ghId])));
            }



            Console.ReadKey();
        }

        public static void loadSensorData()
        {
            for (int i = 0; i < greenHousesList.greenhouseList.Count; i++)
            {
                sensorDatas.Add(greenHousesList.greenhouseList[i].ghId, monitor.getSensorData(greenHousesList.greenhouseList[i].ghId));
            }
        }

        public static void initialize()
        {
            loader = new Loader();
            monitor = new Monitor();
            sensorDatas = new Dictionary<string, SensorData>();
            greenHousesList = loader.loadGreenHouses();
            driver = new Driver();
            sprinklerTable = new Dictionary<double, double>();
            sprinklerTable[20] = 17.3;
            sprinklerTable[21] = 18.5;
            sprinklerTable[22] = 19.7;
            sprinklerTable[23] = 20.9;
            sprinklerTable[24] = 22.1;
            sprinklerTable[25] = 23.3;
            sprinklerTable[26] = 24.7;
            sprinklerTable[27] = 26.1;
            sprinklerTable[28] = 27.5;
            sprinklerTable[29] = 28.9;
            sprinklerTable[30] = 30.3;
            sprinklerTable[31] = 31.9;
            sprinklerTable[32] = 33.5;
            sprinklerTable[33] = 35.1;
            sprinklerTable[34] = 36.7;
            sprinklerTable[35] = 38.3;
        }

        public static int supervise(Greenhouse greenhouse, SensorData sensorData)
        {
            int res = 0;

            if (sensorData.sprinkler_on || sensorData.boiler_on)
            {
                res = driver.sendCommand(greenhouse, sensorData.token, 0, 0);
                Console.WriteLine("Valamelyik eszkoz parancsot hajt vegre! GreenHouse ID:" + greenhouse.ghId);
                return res;
            }
            else if (!sensorData.boiler_on && !sensorData.sprinkler_on)
            {
                if (sensorData.temperature_act >= greenhouse.temperature_min && sensorData.temperature_act<=greenhouse.temperature_opt && sensorData.humidity_act>=greenhouse.humidity_min)
                {
                    res = driver.sendCommand(greenhouse, sensorData.token, 0, 0);
                    Console.WriteLine("Minden ertek optimalis! GreenHouse ID:" + greenhouse.ghId);
                    return res;
                }
                else if (sensorData.temperature_act-greenhouse.temperature_opt>=5 || greenhouse.temperature_min-sensorData.temperature_act>=5 || greenhouse.humidity_min-sensorData.humidity_act>=20 || sensorData.humidity_act-greenhouse.humidity_min>=20)
                {
                    using (StreamWriter writer = File.AppendText("log.txt"))
                    {
                        writer.WriteLine("Hibas mukodes! GreenHouse ID:" + greenhouse.ghId);
                    }
                    Console.WriteLine("Hibas mukodes! GreenHouse ID:" + greenhouse.ghId);
                    return res;
                }
                else if (sensorData.temperature_act < greenhouse.temperature_min)
                {

                    double rise_temp = greenhouse.temperature_opt - sensorData.temperature_act;
                    Console.WriteLine("Rise Temp: {0}", rise_temp);
                    double current_huminity = sprinklerTable[sensorData.temperature_act]*(sensorData.humidity_act / 100);
                    Console.WriteLine("Current Humanity: {0}", current_huminity);
                    double aim_huminity = sprinklerTable[greenhouse.temperature_opt];
                    Console.WriteLine("Aim Humanity: {0}", aim_huminity);
                    double temp_huminity_percentage = (current_huminity / aim_huminity)*100;
                    Console.WriteLine("Temp Humanity percentage: {0}", temp_huminity_percentage);

                    if (temp_huminity_percentage<greenhouse.humidity_min)
                    {
                        double wanted_huminity = sprinklerTable[greenhouse.temperature_opt]*(greenhouse.humidity_min/100);
                        Console.WriteLine("Wanted Humanity: {0}", wanted_huminity);
                        double rise_hunminity = (((wanted_huminity - current_huminity)/0.01)*greenhouse.volume)/1000;
                        Console.WriteLine("Rise Humanity: {0}", rise_hunminity);
                        res = driver.sendCommand(greenhouse, sensorData.token, rise_temp, rise_hunminity);
                    }
                    else
                    {
                        res = driver.sendCommand(greenhouse, sensorData.token, rise_temp, 0);
                    }
                    return res;

                }
                else if (sensorData.humidity_act<greenhouse.humidity_min)
                {
                    /*
                    double current_huminity = sprinklerTable[sensorData.temperature_act] * (sensorData.humidity_act / 100);
                    Console.WriteLine("Current Humanity: {0}", current_huminity);
                    double aim_huminity = sprinklerTable[sensorData.temperature_act];
                    Console.WriteLine("Aim Humanity: {0}", aim_huminity);

                    double rise_hunminity = (((aim_huminity - current_huminity) / 0.01) * greenhouse.volume) / 1000;
                    Console.WriteLine("Rise Humanity: {0}", rise_hunminity);
                    res = driver.sendCommand(greenhouse, sensorData.token, 0, rise_hunminity);
                    */
                    double rise_temp = greenhouse.temperature_opt - sensorData.temperature_act;
                    Console.WriteLine("Rise Temp: {0}", rise_temp);
                    double current_huminity = sprinklerTable[sensorData.temperature_act] * (sensorData.humidity_act / 100);
                    Console.WriteLine("Current Humanity: {0}", current_huminity);
                    double aim_huminity = sprinklerTable[greenhouse.temperature_opt];
                    Console.WriteLine("Aim Humanity: {0}", aim_huminity);
                    double temp_huminity_percentage = (current_huminity / aim_huminity) * 100;
                    Console.WriteLine("Temp Humanity percentage: {0}", temp_huminity_percentage);

                    double wanted_huminity = sprinklerTable[greenhouse.temperature_opt] * (greenhouse.humidity_min / 100);
                    Console.WriteLine("Wanted Humanity: {0}", wanted_huminity);
                    double rise_hunminity = (((wanted_huminity - current_huminity) / 0.01) * greenhouse.volume) / 1000;
                    Console.WriteLine("Rise Humanity: {0}", rise_hunminity);
                    res = driver.sendCommand(greenhouse, sensorData.token, rise_temp, rise_hunminity);
                }
            }

            return res;

        }
    }
}
