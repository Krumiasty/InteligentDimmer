using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using InteligentDimmer.Model;
using InteligentDimmer.Utility;

namespace InteligentDimmer.ViewModel
{
    public class ControlViewModel : INotifyPropertyChanged
    {
        public ICommand GetCurrentTime => new CustomCommand(o =>
        {
            FromHours = DateTime.Now.Hour.ToString();
            FromMinutes = DateTime.Now.Minute.ToString();
        }, o => true);

        private string _setPower;

        public string SetPower
        {
            get
            {
                return _setPower ?? 100.ToString();
            }
            set
            {
                _setPower = value;
                int powerValueFromSetter;
                if (!int.TryParse(value, out powerValueFromSetter))
                {
                    ValidationColor = new SolidColorBrush(Colors.Red);
                    throw new ApplicationException("Wrong minutes number");
                }
                else
                {
                    RaisePropertyChanged(nameof(SetPower));
                    SliderValue = int.Parse(_setPower);
                    ValidationColor = new SolidColorBrush(Colors.White);
                }
            }
        }

        private Brush _validationColor;

        public Brush ValidationColor
        {
            set
            {
                _validationColor = value;
                RaisePropertyChanged(nameof(ValidationColor));
            }
            get { return _validationColor; }
        }

        public bool IsFromMinutesValid;

        private string _fromMinutes;

        public string FromMinutes
        {
            get { return _fromMinutes; }
            set
            {
                _fromMinutes = value;
                int fromMinutesInt;
                if (!int.TryParse(value, out fromMinutesInt))
                {
                    ValidationColor = new SolidColorBrush(Colors.Red);
                    throw new ApplicationException("Wrong minutes number");
                }
                else
                {
                    RaisePropertyChanged(nameof(FromMinutes));
                    ValidationColor = new SolidColorBrush(Colors.White);
                }
            }
        }

        private string _fromHours;
        public string FromHours
        {
            get { return _fromHours; }
            set
            {
                _fromHours = value;
                int fromHoursInt;
                if (!int.TryParse(value, out fromHoursInt))
                {
                    ValidationColor = new SolidColorBrush(Colors.Red);
                    throw new ApplicationException("Wrong hours number");
                }
                else
                {
                    RaisePropertyChanged(nameof(FromHours));
                    ValidationColor = new SolidColorBrush(Colors.White);
                }
            }
        }

        private string _toHours;

        public string ToHours
        {
            get { return _toHours; }
            set
            {
                _toHours = value;
            }
        }

        private string _toMinutes;

        public string ToMinutes
        {
            get { return _toMinutes; }
            set
            {
                _toMinutes = value;
            }
        }

        public ICommand IncreaseCommand => 
            new CustomCommand(o =>
            {
                SliderValue++;
            }, o => true);
            
        public ICommand DecreaseCommand =>
            new CustomCommand(o =>
            {
                SliderValue--;
            }, o => true);

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
                RaisePropertyChanged(nameof(CurrentTime));
            }
        }

        private Bluetooth _selectedBluetooth;

        public Bluetooth SelectedBluetooth
        {
            get { return _selectedBluetooth; }
            set
            {
                _selectedBluetooth = value;
                RaisePropertyChanged(nameof(SelectedBluetooth));
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
                    CurrentPowerStatus = value == 0 ? PowerMode.Off : PowerMode.On;
                    _sliderValue = value;
                }
                
                RaisePropertyChanged(nameof(SliderValue));
            }
        }

        private PowerMode _currentPowerStatus;

        public PowerMode CurrentPowerStatus
        {
            get { return _currentPowerStatus; }
            set
            {
                _currentPowerStatus = value;
                TurnModeText = _currentPowerStatus == PowerMode.On ? Constants.TurnOff : Constants.TurnOn;
                RaisePropertyChanged(nameof(CurrentPowerStatus)); }
        }

        public SerialPort SerialPort { get; }
        public ICommand WindowCloseCommand { get; set; }
        public ICommand PowerDeviceCommand { get; set; }
        public ICommand SetTimeCommand { get; set; }

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

            CurrentPowerStatus = PowerMode.Off;
        }


        private void StartTimer()
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
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
            PowerDeviceCommand = new CustomCommand(ControlPowerDevice, CanChangePowerStatus);
            SetTimeCommand = new CustomCommand(SetTime, CanSetTime);
        }

        private void SetTime(object obj)
        {
            
        }

        private bool CanSetTime(object obj)
        {
            
            return DataValidator();
        }


        private bool DataValidator()
        {
            //validate it
            if(!string.IsNullOrEmpty(FromHours) && !string.IsNullOrEmpty(FromMinutes) 
                && !string.IsNullOrEmpty(ToHours) && !string.IsNullOrEmpty(ToMinutes))
            {
                return true;

            }
            return false;
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
                SliderValue = 0;
                return 1;
            }
            return 0;
        }

        private bool CanChangePowerStatus(object obj)
        {
            return IsConnected;
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
