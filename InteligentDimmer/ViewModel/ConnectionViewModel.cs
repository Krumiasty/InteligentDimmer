using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InteligentDimmer.Configuration;
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

        private string _progressBarText;

        public string ProgressBarText
        {
            get { return _progressBarText; }
            set
            {
                _progressBarText = value;
                RaisePropertyChanged("ProgressBarText");
            }
        }

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
            //   SetupSerialPort();
            FindBluetooths();
        }

        private bool SetupSerialPort()
        {
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports)
            {
                SerialPort = new SerialPort(port, 9600, Parity.None, 8, StopBits.One);
                try
                {
                    SerialPort.Open();
                    MessageBoxResult result = MessageBox.Show("Success!",
                                                            "SerialPort Setup",
                                                            MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    if (port == ports.Last())
                    {
                        var answer = MessageBox.Show("Make sure you have proper drivers installed",
                                            "Port opening failed",
                                            MessageBoxButton.OK);

                        if (answer == MessageBoxResult.OK)
                        {
                            if (SerialPort != null)
                            {
                                if (SerialPort.IsOpen)
                                {
                                    SerialPort.Close();
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private void SetupProgressBarLayout(Visibility visibility, string text)
        {
            ProgressBar = visibility;
            ProgressBarText = text;
        }

        private async void FindBluetooths()
        {
            SetupProgressBarLayout(Visibility.Visible, Constants.SearchingForDevices);

            var devices = new List<Bluetooth>();
            BluetoothClient bluetoothClient;
            try
            {
                bluetoothClient = new BluetoothClient();

            }
            catch (Exception e)
            {
                MessageBox.Show("Please make sure your bluetooth is enabled!",
                                "Bluetooth Error!",
                                MessageBoxButton.OK);
                SetupProgressBarLayout(Visibility.Hidden, Constants.SearchingForDevices);
                return;
            }

            ProgressBar = Visibility.Visible;
            SelectedBluetooth = null;

            await Task.Run( () =>
            {
                var foundDevices = bluetoothClient.DiscoverDevices();

                ProgressBar = Visibility.Hidden;

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

            SetupProgressBarLayout(Visibility.Hidden, Constants.SearchingForDevices);
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

        private async void ConnectWithDevice(object obj)
        {
            SetupProgressBarLayout(Visibility.Visible, Constants.ConnectingToTheDevice);

            var isSetupSucceed = await Task.Run<bool>( ()  =>
            {
                if (!SetupSerialPort())
                {
               
                    return false;
                }

                var macAddressString = SelectedBluetooth.GetMacAddress();
                var macAddress = BluetoothAddress.Parse(macAddressString);
                var device = new BluetoothDeviceInfo(macAddress);
                BluetoothClient = new BluetoothClient();

                var isPaired = BluetoothSecurity.PairRequest(macAddress, Constants.Pin);
            
                if (!isPaired)
                {
                    MessageBox.Show("Pairing failed");
                    return false;
                }

                //foreach (var service in device.InstalledServices)
                //{
                //    try
                //    {
                //        //BluetoothClient.Connect(macAddress, service);
                //        BluetoothClient.BeginConnect(macAddress, BluetoothService.SerialPort,
                //                    new AsyncCallback(BCCCallback), BluetoothClient);
                //        break;
                //    }
                //    catch (Exception e)
                //    {
                //        if (service == device.InstalledServices.Last())
                //        {
                //            return false;
                //        }
                //    }
                //}
                
                if (!device.Authenticated)
                {
                    MessageBox.Show("Authentication failed");
                    SerialPort.Close();
                    return false;
                }

                return true;
            });

            if (!isSetupSucceed)
            {
                SetupProgressBarLayout(Visibility.Hidden, Constants.ConnectingToTheDevice);

                return;
            }

            SerialPort.DataReceived += OnDataReceived;

            try
            {
                SynchronizeTimeOnDevice();
            }
            catch (Exception e)
            {
                SetupProgressBarLayout(Visibility.Hidden, Constants.ConnectingToTheDevice);
            }

            if (!string.IsNullOrEmpty(Response))
            {
                MessageBox.Show("Error");
                SerialPort.Close();
                return;
            }
            IsConnected = true;
            SerialPort.DataReceived -= OnDataReceived;
            ControlView controlWindow = new ControlView();
            Application.Current.MainWindow.Close();
            controlWindow.Show();
        }

        private void SynchronizeTimeOnDevice()
        {
            var currentTime = DateTime.Now;

            PrepareDataService.PrepareData(
                                (byte)Command.SetStructureTime,
                                (byte)DataForSetTimeStructure.Minutes,
                                (byte)currentTime.Minute);

            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData(
                                (byte)Command.SetStructureTime,
                                (byte)DataForSetTimeStructure.Hours,
                                (byte)currentTime.Hour);

            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData(
                                (byte)Command.SetStructureTime,
                                (byte)DataForSetTimeStructure.Days,
                                (byte)currentTime.Day);

            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData(
                                (byte)Command.SetStructureTime,
                                (byte)DataForSetTimeStructure.Weekdays,
                                (byte)currentTime.DayOfWeek);

            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData(
                                (byte)Command.SetStructureTime,
                                (byte)DataForSetTimeStructure.Months,
                                (byte)currentTime.Month);

            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData(
                                (byte)Command.SetStructureTime,
                                (byte)DataForSetTimeStructure.Years,
                                (byte)(currentTime.Year % 2000));

            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData(
                                (byte)Command.WriteStructureToDevice,
                                0x00,
                                0x00);

            SendDataService.SendData(SerialPort);
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
            if (SelectedBluetooth != null
             && SelectedBluetooth.DeviceName != "No devices found")
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