using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InteligentDimmer.Extensions;
using InteligentDimmer.Model;
using InteligentDimmer.Services;
using InteligentDimmer.Utility;
using InteligentDimmer.View;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace InteligentDimmer.ViewModel
{
    public class ConnectionViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<Bluetooth> _bluetooths;
        private Bluetooth _selectedBluetooth;
        private Visibility _progressBar;

        public BluetoothClient BluetoothClient { get; set; }
        public string Response { get; set; }
        public bool IsConnected { get; set; }

        public ICommand ConnectWithDeviceCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public Visibility ProgressBar
        {
            get { return _progressBar; }
            set
            {
                _progressBar = value;
                RaisePropertyChanged("ProgressBar");
            }
        }

        public ObservableCollection<Bluetooth> Bluetooths
        {
            get { return _bluetooths; }
            set
            {
                _bluetooths = value;
                RaisePropertyChanged("Bluetooths");
            }
        }
        public Bluetooth SelectedBluetooth
        {
            get { return _selectedBluetooth; }
            set
            {
                _selectedBluetooth = value;
                RaisePropertyChanged("SelectedBluetooth");
            }
        }

        private SerialPort _serialPort;
        public SerialPort SerialPort
        {
            get { return _serialPort; }
            set { _serialPort = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionViewModel()
        {
            LoadCommands();
            FindBluetooths();
        }

        private void SetupSerialPort()
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                SerialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
                try
                {
                    SerialPort.Open();
                    MessageBoxResult result = MessageBox.Show("Success", "Press Yes", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                    {
                        break;
                    }
                }
                catch
                {
                    if (SerialPort != null)
                    {
                        SerialPort.Close();
                    }
                }
            }
        }

        private async void FindBluetooths()
        {
            var devices = new List<Bluetooth>();
            var bluetoothClientc = new BluetoothClient();

            ProgressBar = Visibility.Visible;
            SelectedBluetooth = null;
               await Task.Run(() =>
            {
                var foundDevices = bluetoothClientc.DiscoverDevices();

                foreach (var foundDevice in foundDevices)
                {
                    Bluetooth device = new Bluetooth(foundDevice);
                    devices.Add(device);
                }

                Bluetooths = devices.ToObservableCollection();
            });

            if (Bluetooths.Count == 0)
            {
                BluetoothDeviceInfo empytDevice = new BluetoothDeviceInfo(new BluetoothAddress(0));
                var item = new Bluetooth(empytDevice);
                item.DeviceName = "No devices found";
                Bluetooths.Add(item);
            }

            ProgressBar = Visibility.Hidden;
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
            #region testing

            SetupSerialPort();

            var macAddressString = SelectedBluetooth.GetMacAddress();
            var macAddress = BluetoothAddress.Parse(macAddressString);
            var device = new BluetoothDeviceInfo(macAddress);
            BluetoothClient = new BluetoothClient();

            const string pin = "1234";
            var isPaired = BluetoothSecurity.PairRequest(macAddress, pin);

            if (!isPaired)
            {
                MessageBox.Show("Pairing failed");
                return;
            }

            if (!device.Authenticated)
            {
                //MessageBox.Show("Authentication failed");
                SerialPort.Close();
                return;
            }

            foreach (var service in device.InstalledServices)
            {
                try
                {
                    BluetoothClient.Connect(macAddress, service);
                    break;
                }
                catch (Exception e)
                {

                }
            }

            //if (!BluetoothClient.Connected)
            //{
            //    MessageBox.Show("Connection failed");
            //    return;
            //}

            PrepareDataService.PrepareData(0x00, 0x00);
            //var stream = BluetoothClient.GetStream();

            SerialPort.Write(new byte[]
            {
                ControlData.StartByte,
                ControlData.CommandByte,
                ControlData.SeparatorByte,
                ControlData.DataByte,
                ControlData.EndByte
            }, 0, 5);

            SerialPort.DataReceived += OnDataReceived;

            if (!string.IsNullOrEmpty(Response))
            {
                IsConnected = false;
                return;

            }
            #endregion

            IsConnected = true;
            ControlView controlWindow = new ControlView();
            Application.Current.MainWindow.Close();
            controlWindow.Show();
            SerialPort.DataReceived -= OnDataReceived;
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
            Response = receivedString;
        }

        private bool CanConnect(object obj)
        {
               if (SelectedBluetooth != null && SelectedBluetooth.DeviceName != "No devices found")
                   return true;
               return false;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    } 
}
