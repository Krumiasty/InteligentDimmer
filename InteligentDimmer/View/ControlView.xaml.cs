using System.Windows;
using InteligentDimmer.Configuration;
using InteligentDimmer.ViewModel;

namespace InteligentDimmer.View
{
    public partial class ControlView : Window
    {
        public ControlView()
        {
            InitializeComponent();

            //var ff = this.DataContext as ControlViewModel;

            CurrentDeviceColonLabel.Content = Constants.Colon;
            CurrentDeviceLabel.Content = Constants.CurrentDevice;
            FromColonLabel.Content = Constants.Colon;
            FromLabel.Content = Constants.From;
            NowButton.Content = Constants.Now;
            SetTimerButton.Content = Constants.SetTimer;
            PowerLabel.Content = Constants.Power;
            PowerColonLabel.Content = Constants.Colon;
            SetCurrencyLabel.Content = Constants.SetCurrencyIntensityTimer;
            ToColonLabel.Content = Constants.Colon;
            ToLabel.Content = Constants.To;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Application.Current.Shutdown();
        }
    }
}
