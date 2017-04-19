using System;

namespace InteligentDimmer.Controls
{
    /// <summary>
    /// Interaction logic for ProgressWait.xaml
    /// </summary>
    public partial class ProgressWait : ISplashScreen
    {
        public ProgressWait()
        {
            InitializeComponent();
        }

        public void AddMessage(string message)
        {

        }

        public void LoadComplete()
        {
            Dispatcher.InvokeShutdown();
        }   
    }

    public interface ISplashScreen
    {
        void AddMessage(string message);
        void LoadComplete();
    }
}
