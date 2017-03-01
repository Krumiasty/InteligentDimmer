using System.ComponentModel;
using InteligentDimmer.Model;

namespace InteligentDimmer.ViewModel
{
    public class ControlViewModel: INotifyPropertyChanged
    {
        private Bluetooth _selectedBluetooth;

        public event PropertyChangedEventHandler PropertyChanged;

        public Bluetooth SelectedBluetooth
        {
            get { return _selectedBluetooth; }
            set
            {
                _selectedBluetooth = value;
                RaisePropertyChanged("SelectedBluetooth");
            }
        }
        public ControlViewModel()
        {
            
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
