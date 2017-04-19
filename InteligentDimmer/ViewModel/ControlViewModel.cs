using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using InteligentDimmer.Model;
using InteligentDimmer.Utility;

namespace InteligentDimmer.ViewModel
{
    public class ControlViewModel: INotifyPropertyChanged
    {
        public bool IsConnected { get; }

        private string _currentTime;

        public string CurrentTime
        {
            get { return _currentTime; }
            set
            {
                _currentTime = value;
                RaisePropertyChanged("CurrentTime");
            }
        }

        private Bluetooth _selectedBluetooth;

        public Bluetooth SelectedBluetooth
        {
            get { return _selectedBluetooth; }
            set
            {
                _selectedBluetooth = value;
                RaisePropertyChanged("SelectedBluetooth");
            }
        }

        private int _sliderValue;

        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                _sliderValue = value;
                RaisePropertyChanged("SliderValue");
            }
        }

        public SerialPort SerialPort { get; }
        public ICommand WindowCloseCommand { get; set; }
        public ICommand PowerDeviceCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionViewModel ConnectionViewModel { get; private set; }

        public ControlViewModel()
        {
            StartTimer();

            ConnectionViewModel = ViewModelLocator.GetViewModel<ConnectionViewModel>();
            IsConnected = ConnectionViewModel.IsConnected;
            SelectedBluetooth = ConnectionViewModel.SelectedBluetooth;
            SerialPort = ConnectionViewModel.SerialPort;         
        }

        private void StartTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
        }

        private void LoadCommands()
        {
            WindowCloseCommand = new CustomCommand(WindowClose, CanClose);
            PowerDeviceCommand = new CustomCommand(PowerDevice, CanChangePowerStatus);
        }

        private void PowerDevice(object obj)
        {
            var isActionSucceed = IsConnected == true ? PowerOn() : PowerOff();
            if (isActionSucceed == 1)
            {
                MessageBox.Show("Success");
            }
        }

        private static int PowerOn()
        {
            return 1;
        }

        private static int PowerOff()
        {
            return 1;
        }

        private bool CanChangePowerStatus(object obj)
        {
            return IsConnected;
        }

        private bool CanClose(object obj)
        {
            return true;
        }

        private void WindowClose(object obj)
        {
          //  SerialPort.Close();
            Application.Current.Shutdown();
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
