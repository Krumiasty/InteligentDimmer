using System.Windows;
using InteligentDimmer.Configuration;

namespace InteligentDimmer.View
{
    public partial class ConnectionView : Window
    { 
        public ConnectionView()
        {
            InitializeComponent();

            ConnectWithDeviceButton.Content = Constants.ConnectWithDevice;
            SearchingForDevices.Content = Constants.SearchingForDevices;
        }
    }
}
