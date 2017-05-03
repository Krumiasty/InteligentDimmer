using System.Windows;

namespace InteligentDimmer
{
    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    partial class MyDialog : Window
    {

        public MyDialog()
        {
            InitializeComponent();
        }

        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        private void OKButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
