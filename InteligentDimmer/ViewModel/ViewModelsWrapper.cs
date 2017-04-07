using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InteligentDimmer.Model;
using InteligentDimmer.ViewModel.Interfaces;

namespace InteligentDimmer.ViewModel
{
    public class ViewModelsWrapper : IConnectionViewModel
    {
        protected Bluetooth _selectedBluetooth;

        protected ObservableCollection<Bluetooth> _bluetooths;

        protected SerialPort _serialPort;

        public ICommand WindowCloseCommand { get; set; }
        public ICommand ConnectWithDeviceCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public Bluetooth SelectedBluetooth
        {
            get { return _selectedBluetooth; }
            set
            {
                _selectedBluetooth = value;
                RaisePropertyChanged("SelectedBluetooth");
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

        public SerialPort SerialPort => _serialPort;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Singleton
    {
        private static Singleton instance;

        public ConnectionViewModel connectionViewModel;
        public ControlViewModel controlViewModel;

        private Singleton() { }

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
}
