using System;
using System.Linq;
using System.Text;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace InteligentDimmer.Model
{
    public class Bluetooth
    {
        public string DeviceName { get; set; }
        public bool Authenticated { get; set; }
        public ClassOfDevice ClassOfDevice { get; set; }
        public bool Connected { get; set; }
        public BluetoothAddress DeviceAddress { get; set; }
        public Guid[] InstalledServices { get; set; }
        public ushort Nap { get; set; }
        public uint Sap { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime LastUsed { get; set; }
        public bool Remembered { get; set; }
        public int Rssi { get; set; }

        public Bluetooth(BluetoothDeviceInfo deviceInfo)
        {
            this.Authenticated = deviceInfo.Authenticated;
            this.ClassOfDevice = deviceInfo.ClassOfDevice;
            this.Connected = deviceInfo.Connected;
            this.DeviceName = deviceInfo.DeviceName;
            this.DeviceAddress = deviceInfo.DeviceAddress;
            this.InstalledServices = deviceInfo.InstalledServices;
            this.LastSeen = deviceInfo.LastSeen;
            this.LastUsed = deviceInfo.LastUsed;
            this.Nap = deviceInfo.DeviceAddress.Nap;
            this.Sap = deviceInfo.DeviceAddress.Sap;
            this.Remembered = deviceInfo.Remembered;
            this.Rssi = deviceInfo.Rssi;
        }

        public override string ToString()
        {
            return this.DeviceName;
        }

        public string GetMacAddress()
        {
            var macCharArray = DeviceAddress.ToString().ToCharArray();
            int putColon = 0;
            StringBuilder sb = new StringBuilder();
            foreach (char c in macCharArray)
            {      
                if (putColon % 2 == 0)
                {
                    sb.Append(":");
                    putColon = 0;
                }
                sb.Append(c);
                putColon++;
            }
            sb.Remove(0, 1);
            return sb.ToString();
        }
    }
}
