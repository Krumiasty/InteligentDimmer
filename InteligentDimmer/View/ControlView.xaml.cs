using System.Windows;
using InteligentDimmer.ViewModel;

namespace InteligentDimmer.View
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlView : Window
    {
        public ControlView()
        {
            InitializeComponent();
            var ff = this.DataContext as ControlViewModel;        
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Application.Current.Shutdown();
        }

    }
}
