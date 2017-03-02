using System;
using InTheHand.Net.Sockets;

namespace InteligentDimmer.Model
{
    public class Bluetooth
    {
        public string Name { get; set; }
        public bool Authenticated { get; set; }
        public bool Connected { get; set; }
        public ushort Nap { get; set; }
        public uint Sap { get; set; }
        public DateTime LastSeen { get; set; }
        public DateTime LastUsed { get; set; }
        public bool Remembered { get; set; }

        public Bluetooth(BluetoothDeviceInfo deviceInfo)
        {
            this.Authenticated = deviceInfo.Authenticated;
            this.Connected = deviceInfo.Connected;
            this.Name = deviceInfo.DeviceName;
            this.LastSeen = deviceInfo.LastSeen;
            this.LastUsed = deviceInfo.LastUsed;
            this.Nap = deviceInfo.DeviceAddress.Nap;
            this.Sap = deviceInfo.DeviceAddress.Sap;
            this.Remembered = deviceInfo.Remembered;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
