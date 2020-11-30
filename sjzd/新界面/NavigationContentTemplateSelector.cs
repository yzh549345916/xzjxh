using System.Windows;
using System.Windows.Controls;

namespace sjzd
{
    public class NavigationContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Template { get; set; }
        public DataTemplate TemplateAlternative { get; set; }
        public DataTemplate ConfigContentTemplate { get; set; }
        public DataTemplate XZJXHContentTemplate { get; set; }
        public DataTemplate MapContentTemplate { get; set; }
        public DataTemplate CPZZTemplate { get; set; }
        public DataTemplate Property1ContentTemplate { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                NavigationItemModel model = (NavigationItemModel)item;
                if (model.Title == "乡镇精细化预报")
                {
                    return this.XZJXHContentTemplate;
                }
                else if (model.Title == "地图")
                {
                    return this.MapContentTemplate;
                }
                else if (model.Title == "呼和浩特市精细化")
                {
                    return this.Property1ContentTemplate;
                }
                else if (model.Title == "预报产品制作发布")
                {
                    return this.CPZZTemplate;
                }
                else if (!string.IsNullOrEmpty(model.Text))
                {
                    return this.TemplateAlternative;
                }
            }

            return this.Template;
        }
    }
}
