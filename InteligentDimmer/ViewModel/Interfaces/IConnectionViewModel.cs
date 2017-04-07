using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InteligentDimmer.Model;

namespace InteligentDimmer.ViewModel.Interfaces
{
    public interface IConnectionViewModel
    {
        ICommand ConnectWithDeviceCommand { get; set; }
        ObservableCollection<Bluetooth> Bluetooths { get; set; }
        Bluetooth SelectedBluetooth { get; set; }
        SerialPort SerialPort { get; }
    }
}
