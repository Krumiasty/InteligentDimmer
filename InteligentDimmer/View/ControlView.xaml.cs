using System.Windows;
using System.Windows.Input;

namespace InteligentDimmer.View
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlView : Window
    {
        int sliderValue = 0;
        public ControlView()
        {
            InitializeComponent();
            
        }

        //protected void CloseClick(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Application.Current.Shutdown();
        }

        private void PowerSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sliderValue = (int)e.NewValue;
            //  this.SliderValueLabel.Content = this.sliderValue.ToString() + " %";
            SunImage.Opacity = (double)this.sliderValue / 100;

            DisplayPowerValueLabel.Content = this.sliderValue.ToString() + " %";
        }

        private void IncreasePowerClick(object sender, RoutedEventArgs e)
        {
            if(this.sliderValue <= 100)
            {
                this.sliderValue++;
            }

        }

        private void DecreasePowerClick(object sender, RoutedEventArgs e)
        {
            if(this.sliderValue >= 0)
            {
                this.sliderValue--;
            }
        }
    }
}
