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
        public ICommand IncreaseCommand => 
            new CustomCommand(o =>
            {
                SliderValue++;
            },
                o => true);
            
        public ICommand DecreaseCommand =>
            new CustomCommand(o =>
            {
                SliderValue--;
            },
                o => true);

        private string _turnModeText;

        public string TurnModeText
        {
            get { return _turnModeText; }
            set
            {
                _turnModeText = value;
                RaisePropertyChanged(nameof(TurnModeText));
            }
        }

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
                if (value > 100)
                {
                    _sliderValue = 100;
                }
                else if (value < 0)
                {
                    _sliderValue = 0;
                    CurrentPowerStatus = PowerMode.Off;
                }
                else
                {
                    if (value == 0)
                    {
                        CurrentPowerStatus = PowerMode.Off;
                        //     TurnModeText = "Turn Off";
                    }
                    else
                    {
                        CurrentPowerStatus = PowerMode.On;
                    }
                    _sliderValue = value;
                }
                
                RaisePropertyChanged("SliderValue");
            }
        }

        private PowerMode _currentPowerStatus;

        public PowerMode CurrentPowerStatus
        {
            get { return _currentPowerStatus; }
            set
            {
                _currentPowerStatus = value;
                if (_currentPowerStatus == PowerMode.On)
                {
                    TurnModeText = "Turn Off";
                }
                else
                {
                    TurnModeText = "Turn On";
                }
                RaisePropertyChanged(nameof(CurrentPowerStatus)); }
        }

        public SerialPort SerialPort { get; }
        public ICommand WindowCloseCommand { get; set; }
        public ICommand PowerDeviceCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionViewModel ConnectionViewModel { get; private set; }

        public ControlViewModel()
        {
            StartTimer();
            LoadCommands();
            ConnectionViewModel = ViewModelLocator.GetViewModel<ConnectionViewModel>();
            IsConnected = ConnectionViewModel.IsConnected;
            SelectedBluetooth = ConnectionViewModel.SelectedBluetooth;
            SerialPort = ConnectionViewModel.SerialPort;

        //    TurnModeText = "Turn On";
            CurrentPowerStatus = PowerMode.Off;
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
         //   WindowCloseCommand = new CustomCommand(WindowClose, CanClose);
            PowerDeviceCommand = new CustomCommand(ControlPowerDevice, CanChangePowerStatus);
        }

        private void ControlPowerDevice(object obj)
        {
            var isActionSucceed = SliderValue == 0 ? PowerOn() : PowerOff();
            if (isActionSucceed == 1)
            {
                MessageBox.Show("Action Success");
            }
        }

        private int PowerOn()
        {
            // send data to bluetooth if action success:
            if (true)
            {
           //     TurnModeText = "Turn Off";
           //     CurrentPowerStatus = PowerMode.On;
                SliderValue = 100;
                return 1;
            }
            return 0;
        }


        public int PowerOff()
        {
            // send data to bluetooth if action success:
            if (true)
            {
             //   TurnModeText = "Turn On";
             //   CurrentPowerStatus = PowerMode.Off;
                SliderValue = 0;
                return 1;
            }
            return 0;
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
