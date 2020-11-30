using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace sjzd
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Items = new ObservableCollection<NavigationItemModel>();

            Items.Add(CreateItem("乡镇精细化预报", "&#xe813;", string.Empty, ""));
            Items.Add(CreateItem("呼和浩特市精细化", "&#xe703;", string.Empty, "Images/7.JPG"));
            /* this.Items.Add(CreateItem("地图", "&#xe500;", string.Empty, "Images/7.JPG"));*/
            Items.Add(CreateItem("预报产品制作发布", "&#xe921;", string.Empty, "Images/7.JPG"));
        }

        public ObservableCollection<NavigationItemModel> Items { get; set; }

        private static NavigationItemModel CreateItem(string title, string iconGlyph, string text,
            params string[] imagePaths)
        {
            var item = new NavigationItemModel();
            item.Title = title;
            item.Text = text;
            item.IconGlyph = iconGlyph;
            item.ImagePaths = imagePaths;

            return item;
        }
    }
}