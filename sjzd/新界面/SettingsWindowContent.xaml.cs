using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// SettingsWindowContent.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindowContent : UserControl
    {

        public SettingsWindowContent()
        {
            InitializeComponent();



        }
        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            RadWindow radWindow = base.Parent as RadWindow;

            if (radWindow != null)
            {
                radWindow.Close();
            }
        }


    }
}
