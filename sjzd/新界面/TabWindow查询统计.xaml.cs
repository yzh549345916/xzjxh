using Telerik.Windows.Controls;
using Telerik.Windows.Controls.TabbedWindow;

namespace sjzd
{
    /// <summary>
    /// TabWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class TabWindow查询统计 : RadTabbedWindow
    {
        public TabWindow查询统计()
        {
            InitializeComponent();
            RadialMenuCommands.Show.Execute(null, this);
        }

        private void TabWindow1_OnAddingNewTab(object sender, AddingNewTabEventArgs e)
        {
            e.Cancel = true;
            RadialMenuCommands.Show.Execute(null, this);
        }
    }
}
