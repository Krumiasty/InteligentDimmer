using System.Windows;
using InteligentDimmer.ViewModel;

namespace InteligentDimmer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Singleton.Instance.connectionViewModel = new ConnectionViewModel();
            Singleton.Instance.controlViewModel = new ControlViewModel();
        }
    }

  
}
