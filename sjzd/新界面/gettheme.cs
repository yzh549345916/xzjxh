using Telerik.Windows.Controls;

namespace sjzd
{
    static class Gettheme
    {
        static public Theme GetMyTheme(string name)
        {
            string myName = name.ToLower();
            if (myName.Contains("crystal"))
            {
                return new CrystalTheme();
            }
            else if (myName.Contains("fluent"))
            {
                return new FluentTheme();
            }
            else if (myName.Contains("material"))
            {
                return new MaterialTheme();
            }
            else if (myName.Contains("office2016touch"))
            {
                return new Office2016TouchTheme();
            }
            else if (myName.Contains("office2016"))
            {
                return new Office2016Theme();
            }
            else if (myName.Contains("green"))
            {
                return new GreenTheme();
            }
            else if (myName.Contains("office2013"))
            {
                return new Office2013Theme();
            }
            else if (myName.Contains("visualstudio2013"))
            {
                return new VisualStudio2013Theme();
            }
            else if (myName.Contains("windows8touch"))
            {
                return new Windows8TouchTheme();
            }
            else if (myName.Contains("windows8"))
            {
                return new Windows8Theme();
            }
            else if (myName.Contains("office_black"))
            {
                return new Office_BlackTheme();
            }
            else if (myName.Contains("office_blue"))
            {
                return new Office_BlueTheme();
            }
            else if (myName.Contains("office_silver"))
            {
                return new Office_SilverTheme();
            }
            else if (myName.Contains("summer"))
            {
                return new SummerTheme();
            }
            else if (myName.Contains("vista"))
            {
                return new VistaTheme();
            }
            else if (myName.Contains("transparent"))
            {
                return new TransparentTheme();
            }
            else if (myName.Contains("windows7"))
            {
                return new Windows7Theme();
            }
            else if (myName.Contains("expression_dark"))
            {
                return new Expression_DarkTheme();
            }
            return new CrystalTheme();
        }
    }
}