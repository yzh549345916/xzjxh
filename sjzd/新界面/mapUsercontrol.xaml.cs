using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Map;

namespace sjzd
{
    /// <summary>
    /// mapUsercontrol.xaml 的交互逻辑
    /// </summary>
    public partial class mapUsercontrol : UserControl
    {
        bool bsBool = false;
        public mapUsercontrol()
        {
            LocalizationManager.Manager.Culture = new CultureInfo("zh-cn");
            InitializeComponent();
            ArcGisMapProvider provider = new ArcGisMapProvider();
            provider.Mode = ArcGisMapMode.Aerial;
            this.RadMap1.Provider = provider;
            //InitializeBingMapProvider("SZawYWTET2Xl8lkOsT1E~bZ2aR1LXZTcp72a0g7jziw~AhBURoXw9esbv4I3QVM2T0mBt_qDW5IDAxPbMvGLc-17nhNearJfv3R698_VUhRo");
            //AvzM4FgDkpuZwkwP9DPDUwq15NUTJxHNyyUHGSXiA9JwAtAinnlPS31PvwB3hcWh
        }
        private void InitializeBingMapProvider(string VEKey)
        {
            if (string.IsNullOrEmpty(VEKey))
                return;

            this.RadMap1.Provider = new BingRestMapProvider(MapMode.Aerial, true, VEKey);
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            if (!bsBool)
            {
                InitializeBingMapProvider("AvzM4FgDkpuZwkwP9DPDUwq15NUTJxHNyyUHGSXiA9JwAtAinnlPS31PvwB3hcWh");
            }
            else
            {
                ArcGisMapProvider provider = new ArcGisMapProvider();
                provider.Mode = ArcGisMapMode.Aerial;
                this.RadMap1.Provider = provider;
            }
            bsBool = !bsBool;


        }
    }
}

