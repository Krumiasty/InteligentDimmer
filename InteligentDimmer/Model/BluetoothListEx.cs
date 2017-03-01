using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InteligentDimmer.Model
{
    public class BluetoothListEx : List<Bluetooth>
    {
        public static List<Bluetooth> bluetoothList;

        public BluetoothListEx()
        {
            Init();
        }

        public static void Init()
        {
            bluetoothList = new List<Bluetooth>()
            {
                new Bluetooth()
                {
                    Id = 0,
                    Name = "device1"
                },
                new Bluetooth()
                {
                    Id = 1,
                    Name = "testdevice"
                },
                new Bluetooth()
                {
                    Id = 2,
                    Name = "myBluetooth"
                },
                 new Bluetooth()
                {
                    Id = 3,
                    Name = "ttttttt"
                }
            };
            
        }
    }
}
