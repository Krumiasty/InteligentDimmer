using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InteligentDimmer.Extensions;
using InteligentDimmer.Model;
using InteligentDimmer.Utility;
using InteligentDimmer.View;
using InteligentDimmer.ViewModel.Interfaces;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace InteligentDimmer.ViewModel
{
    public class ConnectionViewModel : IConnectionViewModel, INotifyPropertyChanged
    {
        private Bluetooth _selectedBluetooth;

        private ObservableCollection<Bluetooth> _bluetooths;

        public event PropertyChangedEventHandler PropertyChanged;

        private SerialPort _serialPort;

        public SerialPort SerialPort
        {
            get { return _serialPort; }
        }

        public ICommand ConnectWithDeviceCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
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

        public ConnectionViewModel()
        {
            SetupSerialPort();
            LoadCommands();
            FindBluetooths();
        }

        private void SetupSerialPort()
        {
            _serialPort = new SerialPort
            {
                PortName = "COM7",  //Com Port Name   
                BaudRate = Convert.ToInt32("921600"), //COM Port Sp
                Handshake = System.IO.Ports.Handshake.None,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 200,
                WriteTimeout = 50
            };

            _serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(Recieve);
        }
        
        private void Recieve(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            // Collecting the characters received to our 'buffer' (string).
            var recieved_data = _serialPort.ReadExisting();
          
        }

        private async void FindBluetooths()
        {
            List<Bluetooth> devices = new List<Bluetooth>();
            BluetoothClient bluetoothClientc = new BluetoothClient();
            
            // flag to show animation

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
           
            // stop animation
        }

        //private void LoadBluetoothMock()
        //{
        //    Bluetooths = new ObservableCollection<Bluetooth>()
        //    {
        //        new Bluetooth()
        //        {
        //            Id = 0,
        //            Name = "device1"
        //        },
        //        new Bluetooth()
        //        {
        //            Id = 1,
        //            Name = "testdevice"
        //        },
        //        new Bluetooth()
        //        {
        //            Id = 2,
        //            Name = "myBluetooth"
        //        },
        //         new Bluetooth()
        //        {
        //            Id = 3,
        //            Name = "ttttttt"
        //        }
        //    };

        //}

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

        public static SerialPort serialPort;// = new SerialPort("COM4", 9600, Parity.None, 8, StopBits.One);
        public string message, message1;
        public string message2;

        public void OpenConnection()
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                    message = "Closing port, because it was already open!";
                }
                else
                {
                    serialPort.Open();
                    serialPort.ReadTimeout = 1000;
                    //message =
                    Debug.WriteLine("Port Opened!");
                }
            }
            else
            {
                if (serialPort.IsOpen)
                {
                    Debug.WriteLine("Port is already open");
                }
                else
                {
                    Debug.WriteLine("Port == null");
                }
            }
        }




        void OnApplicationQuit()
        {
            serialPort.Close();
        }

        private void ConnectWithDevice(object obj)
        {
           

            var macAddressString = SelectedBluetooth.GetMacAddress();
            var macAddress = BluetoothAddress.Parse(macAddressString);
            var device = new BluetoothDeviceInfo(macAddress);

            BluetoothEndPoint localEndpoint = new BluetoothEndPoint( macAddress, BluetoothService.SerialPort, 4);
            BluetoothClient localClient = new BluetoothClient();
        //    localClient.Connect(localEndpoint);
        //    BluetoothComponent localComponent = new BluetoothComponent(localClient);

            string pin = "1111";
            var isPaired = BluetoothSecurity.PairRequest(macAddress, pin);

            if (!isPaired)
            {
                MessageBox.Show("Pairing failed");
                return;
            }

            string[] ports = SerialPort.GetPortNames();
            //foreach (var port in ports)
            //{
            //    SerialPort inPort = new SerialPort(port, 9600);
            //    try
            //    {
            //        inPort.Open();
            //        return;
            //    }
            //    catch
            //    {
            //    }
            //}

            serialPort = new SerialPort(ports[0], 9600, Parity.None, 8, StopBits.One);

            OpenConnection();
         

            if (device.Authenticated)
            {
               // localClient.BeginConnect(device.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(Connect), device);
            }
            OnApplicationQuit();


            //if (device.Authenticated)
            //{
            //    localClient.SetPin(DEVICE_PIN);
            //    // async connection method
            //    localClient.BeginConnect(device.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(Connect), device);
            //}
            //        device.SetServiceState(BluetoothService.SerialPort, true, true);

            ControlView controlWindow = new ControlView();
            Application.Current.MainWindow.Close();
            controlWindow.Show();
        }

        private void Connect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                // client is connected now :)
            }
        }

        private bool CanConnect(object obj)
        {
               if (SelectedBluetooth != null)
                   return true;
               return false;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    

}
