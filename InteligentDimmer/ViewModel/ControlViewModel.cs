using System.ComponentModel;
using System.IO.Ports;
using System.Net.Mime;
using System.Windows;
using System.Windows.Input;
using InteligentDimmer.Model;
using InteligentDimmer.Utility;

namespace InteligentDimmer.ViewModel
{
    public class ControlViewModel: ViewModelsWrapper, INotifyPropertyChanged
    {
        private Bluetooth _selectedBluetooth;
        public ICommand WindowCloseCommand { get; set; }
        
        public Bluetooth SelectedBluetooth
        {
            get { return _selectedBluetooth; }
            set
            {
                _selectedBluetooth = value;
                RaisePropertyChanged("SelectedBluetooth");
            }
        }

        public SerialPort SerialPort { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ControlViewModel()
        {
                // TODO check which ctor is being used
        }

        public ControlViewModel(SerialPort serialPort)
        {
            SerialPort = serialPort;
            LoadCommands();
        }

        private void LoadCommands()
        {
            WindowCloseCommand = new CustomCommand(WindowClose, CanClose);
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
