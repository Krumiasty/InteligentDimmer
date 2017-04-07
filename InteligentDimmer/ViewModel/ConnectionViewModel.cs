using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InteligentDimmer.Extensions;
using InteligentDimmer.Model;
using InteligentDimmer.Utility;
using InteligentDimmer.View;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace InteligentDimmer.ViewModel
{
    public class ConnectionViewModel : ViewModelsWrapper, INotifyPropertyChanged
    {
       public ConnectionViewModel()
        {
            LoadCommands();
            FindBluetooths();
        }

        private void SetupSerialPort()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                _serialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
                try
                {
                    _serialPort.Open();
                    MessageBoxResult result = MessageBox.Show("Success", "Press Yes", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                    {
                        break;
                    }
                }
                catch
                {
                    if (_serialPort != null)
                    {
                        _serialPort.Close();
                    }
                }
            }
        }

        private async void FindBluetooths()
        {
            List<Bluetooth> devices = new List<Bluetooth>();
            BluetoothClient bluetoothClientc = new BluetoothClient();
            
            // TODO flag to show animation

            await Task.Run(() =>
            {
                var foundDevices = bluetoothClientc.DiscoverDevices();
                int count = foundDevices.Length;

                foreach (var foundDevice in foundDevices)
                {
                    Bluetooth device = new Bluetooth(foundDevice);
                    devices.Add(device);
                }

                Bluetooths = devices.ToObservableCollection();
            });
           
            // TODO stop animation
        }

        private void LoadCommands()
        {
            ConnectWithDeviceCommand = new CustomCommand(ConnectWithDevice, CanConnect);
            RefreshCommand = new CustomCommand(Refresh, CanRefresh);
        }

        private void Refresh(object obj)
        {
            FindBluetooths();
        }

        private bool CanRefresh(object obj)
        {
            return true;
        }

        private void ConnectWithDevice(object obj)
        {
            SetupSerialPort();

            var macAddressString = SelectedBluetooth.GetMacAddress();
            var macAddress = BluetoothAddress.Parse(macAddressString);
            var device = new BluetoothDeviceInfo(macAddress);
            var bluetoothClient = new BluetoothClient();

            const string pin = "1111";
            var isPaired = BluetoothSecurity.PairRequest(macAddress, pin);

            if (!isPaired)
            {
                MessageBox.Show("Pairing failed");
                return;
            }        

            if (!device.Authenticated)
            {
                MessageBox.Show("Authentication failed");
                SerialPort.Close();
                return;
            }

            foreach (var service in device.InstalledServices)
            {
                try
                {
                    bluetoothClient.Connect(macAddress, service);
                    break;
                }
                catch
                {
                    
                }
            }

            if (!bluetoothClient.Connected)
            {
                MessageBox.Show("Connection failed");
                return;                
            }       

            var stream = bluetoothClient.GetStream();
            stream.Write(new byte[] { 0, 0, 0}, 0, 0);
            _serialPort.DataReceived += OnDataReceived;

            //     ControlViewModel controlViewModel = new ControlViewModel(SerialPort);
            ControlView controlWindow = new ControlView();
            Application.Current.MainWindow.Close();
            controlWindow.Show();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = sender as SerialPort;
            if (serialPort == null)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(serialPort.ReadExisting());
            var receivedString = sb.ToString();
            if (!string.IsNullOrEmpty(receivedString))
            {
                Debug.WriteLine(receivedString);
            }
        }

        private bool CanConnect(object obj)
        {
               if (SelectedBluetooth != null)
                   return true;
               return false;
        }  
    } 
}
