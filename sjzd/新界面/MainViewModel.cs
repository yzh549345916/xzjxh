using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace sjzd
{
    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<NavigationItemModel> Items { get; set; }

        public MainViewModel()
        {
            this.Items = new ObservableCollection<NavigationItemModel>();

            this.Items.Add(CreateItem("乡镇精细化预报", "&#xe813;", string.Empty, ""));
            this.Items.Add(CreateItem("呼和浩特市精细化", "&#xe703;", string.Empty, "Images/7.JPG"));
            this.Items.Add(CreateItem("地图", "&#xe500;", string.Empty, "Images/7.JPG"));

        }

        private static NavigationItemModel CreateItem(string title, string iconGlyph, string text, params string[] imagePaths)
        {
            NavigationItemModel item = new NavigationItemModel();
            item.Title = title;
            item.Text = text;
            item.IconGlyph = iconGlyph;
            item.ImagePaths = imagePaths;

            return item;
        }
    }
}
