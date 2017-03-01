using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using InteligentDimmer.Model;
using InteligentDimmer.Utility;
using InteligentDimmer.View;
using InteligentDimmer.ViewModel.Interfaces;

namespace InteligentDimmer.ViewModel
{
    public class ConnectionViewModel : IConnectionViewModel, INotifyPropertyChanged
    {
        private Bluetooth _selectedBluetooth;

        private ObservableCollection<Bluetooth> _bluetooths;

        public event PropertyChangedEventHandler PropertyChanged;

        
        public ICommand ConnectWithDeviceCommand { get; set; }

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
            LoadBluetoothMock();
            LoadCommands();
        }

        private void LoadBluetoothMock()
        {
            Bluetooths = new ObservableCollection<Bluetooth>()
            {
                new Bluetooth()
                {
                    Id = 0,
                    Name = "device1"
                },
                new Bluetooth()
                {
                    Id = 1,
                    Name = "testdevice"
                },
                new Bluetooth()
                {
                    Id = 2,
                    Name = "myBluetooth"
                },
                 new Bluetooth()
                {
                    Id = 3,
                    Name = "ttttttt"
                }
            };

        }

        private void LoadCommands()
        {
            ConnectWithDeviceCommand = new CustomCommand(ConnectWithDevice, CanConnect);
        }

        private void ConnectWithDevice(object obj)
        {
            ControlView controlWindow = new ControlView();
            Application.Current.MainWindow.Close();
            controlWindow.Show();
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
