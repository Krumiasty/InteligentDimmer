using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using InteligentDimmer.Configuration;
using InteligentDimmer.Model;
using InteligentDimmer.Services;
using InteligentDimmer.Utility;
using InTheHand.Net.Sockets;

namespace InteligentDimmer.ViewModel
{
    public class ControlViewModel : INotifyPropertyChanged
    {
        private int _powerToSet;
        public int PowerToSet
        {
            get { return _powerToSet; }
            set
            {
                _powerToSet = value;
                PrepareDataService.PrepareData(0x01, (byte)_powerToSet, 0x00);
                SendData();
            }
        }

        // TODO change data to frame format
        public int HourFromToSet { get; set; }
        public int MinuteFromToSet { get; set; }
        public int HourToToSet { get; set; }
        public int MinuteToToSet { get; set; }
        //

        public ConnectionViewModel ConnectionViewModel { get; private set; }
        public bool IsConnected { get; }
      

        public SerialPort SerialPort { get; }
        public BluetoothClient BluetoothClient { get; }

        private PowerMode _currentPowerStatus;
        public PowerMode CurrentPowerStatus
        {
            get { return _currentPowerStatus; }
            set
            {
                _currentPowerStatus = value;
                TurnModeText = _currentPowerStatus == PowerMode.On ? Constants.TurnOff : Constants.TurnOn;
                RaisePropertyChanged(nameof(CurrentPowerStatus));
            }
        }

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

        private string _daysSetter;

        public string DaysSetter
        {
            get { return _daysSetter; }
            set
            {
                _daysSetter = value;
                int daysSetterInt;
                if (!int.TryParse(value, out daysSetterInt))
                {
                    ValidationColor = new SolidColorBrush(Colors.Red);
                    throw new ApplicationException("Wrong days number");
                }
                else
                {
                    RaisePropertyChanged(nameof(DaysSetter));
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

        private Action<object> SetCurrentTime()
        {
            return o =>
            {
                FromHours = DateTime.Now.Hour.ToString();
                FromMinutes = (DateTime.Now.Minute + 1).ToString();
            };
        }

        private bool _shouldRepeat;

        public bool ShouldRepeat
        {
            get { return _shouldRepeat; }
            set {
                _shouldRepeat = value;
                RaisePropertyChanged(nameof(ShouldRepeat));
            }
        }

        private string _response;
        public string Response
        {
            get { return _response; }
            set
            {
                _response = value;
            }
        }

        private string _setPower;
        public string SetPower
        {
            get
            {
                return _setPower ?? 100.ToString();
            }
            set
            {
                string tmpstr = value;
                var lastChar = tmpstr.Last();

                if (lastChar == '%')
                {
                    while (tmpstr.Last() == '%')
                    {
                        tmpstr = tmpstr.Remove(tmpstr.Length - 1);
                    }

                    _setPower = tmpstr;
                }
                else
                {
                    tmpstr = value;
                }
                int powerValueFromSetter;
                if (!int.TryParse(tmpstr, out powerValueFromSetter))
                {
                    ValidationColor = new SolidColorBrush(Colors.Red);
                    throw new ApplicationException("Wrong power number");
                }
                else
                {
                    if (powerValueFromSetter >= 0 && powerValueFromSetter <= 100)
                    {
                        RaisePropertyChanged(nameof(SetPower));
                        ValidationColor = new SolidColorBrush(Colors.White);
                        PowerToSet = powerValueFromSetter;
                    }
                    else
                    {
                        throw new ApplicationException("Wrong power number");
                    }
                }
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

                PowerToSet = SliderValue;
                RaisePropertyChanged(nameof(SliderValue));
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

        public ICommand GetCurrentTime =>
              new CustomCommand(SetCurrentTime(), o => true);
        public ICommand PowerDeviceCommand { get; set; }
        public ICommand SetTimeCommand { get; set; }
        public ICommand WindowCloseCommand { get; set; }

        private NetworkStream _stream;
        public NetworkStream Stream { get; private set; }

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

        public event PropertyChangedEventHandler PropertyChanged;

        public ControlViewModel()
        {
            StartTimer();
            LoadCommands();
            ConnectionViewModel = ViewModelLocator.GetViewModel<ConnectionViewModel>();
            IsConnected = ConnectionViewModel.IsConnected;
            SelectedBluetooth = ConnectionViewModel.SelectedBluetooth;
            SerialPort = ConnectionViewModel.SerialPort;
            BluetoothClient = ConnectionViewModel.BluetoothClient;

          //  Stream = BluetoothClient.GetStream();
            SerialPort.DataReceived += OnDataReceived;

            CurrentPowerStatus = PowerMode.Off;
            Response = null;
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
            var currentTime = DateTime.Now;

            var startTimeHour = int.Parse(FromHours);
            var startTimeMinute = int.Parse(FromMinutes);
            var endTimeHour = int.Parse(ToHours);
            var endTimeMinute = int.Parse(ToMinutes);

            int days;
            if (!int.TryParse(DaysSetter, out days))
            {
                days = 0;
            }

            var brightness = int.Parse(SetPower);

            // ON
            PrepareDataService.PrepareData((byte)Command.FirstTimeStamp,
                                (byte)DataForTimeStampStructure.Minutes,
                                (byte)startTimeMinute);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.FirstTimeStamp,
                                (byte)DataForTimeStampStructure.Hours,
                                (byte)startTimeHour);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.FirstTimeStamp,
                                (byte)DataForTimeStampStructure.Days,
                                (byte)currentTime.Day);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.FirstTimeStamp,
                                (byte)DataForTimeStampStructure.Weekdays,
                                (byte)currentTime.DayOfWeek);
            SendDataService.SendData(SerialPort);

            //brightness
            PrepareDataService.PrepareData((byte)Command.FirstTimeStamp,
                                (byte)DataForTimeStampStructure.Function,
                                (byte)Command.SetPower);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.FirstTimeStamp,
                (byte)DataForTimeStampStructure.FunctionValue,
                (byte)brightness);
            SendDataService.SendData(SerialPort);
            ////
            PrepareDataService.PrepareData((byte)Command.WriteStructureToDevice,
                0x00,
                0x00);
            SendDataService.SendData(SerialPort);



            // OFF
            PrepareDataService.PrepareData((byte)Command.SecondTimeStamp,
                                (byte)DataForTimeStampStructure.Minutes,
                                (byte)endTimeMinute);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.SecondTimeStamp,
                                (byte)DataForTimeStampStructure.Hours,
                                (byte)endTimeHour);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.SecondTimeStamp,
                                (byte)DataForTimeStampStructure.Days,
                                (byte)currentTime.Day);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.SecondTimeStamp,
                                (byte)DataForTimeStampStructure.Weekdays,
                                (byte)currentTime.DayOfWeek);
            SendDataService.SendData(SerialPort);

            PrepareDataService.PrepareData((byte)Command.WriteStructureToDevice,
                                0x00,
                                0x00);

            SendDataService.SendData(SerialPort);

            if (days > 0)
            {
                PrepareDataService.PrepareData((byte)Command.SetAlarm,
                                (byte)(days + 1),
                                0x01);
                SendDataService.SendData(SerialPort);
            }
            else
            {
                PrepareDataService.PrepareData((byte)Command.SetAlarm,
                                0x01,
                                0x01);
                SendDataService.SendData(SerialPort);
            }

            MessageBox.Show("Action Success");


            // PrepareData();
            // SendData();
            //// if responose == 0xAA
            //if (true)
            //{
            //    MessageBox.Show("Action Success");
            //}
        }

        private bool CanSetTime(object obj)
        {
            return DataValidator();
        }

        private bool DataValidator()
        {
            return !string.IsNullOrEmpty(FromHours) && !string.IsNullOrEmpty(FromMinutes) 
                   && !string.IsNullOrEmpty(ToHours) && !string.IsNullOrEmpty(ToMinutes);
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
            PrepareDataService.PrepareData(0x00, 0x01, 0x00);
            SendData();
            if (string.IsNullOrEmpty(Response))
            {
                SliderValue = 100;
                return 1;
            }
            return 0;
        }

        public int PowerOff()
        {
            PrepareDataService.PrepareData(0x00, 0x00, 0x00);
            SendData();
            if (string.IsNullOrEmpty(Response))
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
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Response = null;
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

        public void SendData()
        {
            Response = null;
            SerialPort.Write(new byte[]
             {
                ControlData.StartByte,
                ControlData.CommandByte,
                ControlData.SeparatorByte1,
                ControlData.DataByte1,
                ControlData.SeparatorByte2,
                ControlData.DataByte2,
                ControlData.EndByte
             }, 0, Constants.BytesNumber);
        }

        public void CloseSerialPort()
        {
            SerialPort.Close();
        }
    }
}
