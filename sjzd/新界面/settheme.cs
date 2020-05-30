using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;

namespace sjzd
{
    class Settheme
    {
        private static readonly Dictionary<string, ResourceDictionary> cachedResourceDictionaries = new Dictionary<string, ResourceDictionary>();

        public void setTheme(FrameworkElement element, string themeName)
        {
            string[] array = ((IEnumerable<string>)defaultReferencesNamesForApplication).ToArray<string>();
            Application.Current.Resources.MergedDictionaries.Except<ResourceDictionary>(cachedResourceDictionaries.Where<KeyValuePair<string, ResourceDictionary>>(keyValuePair => keyValuePair.Key.Contains("Telerik.Windows.Themes.")).Select<KeyValuePair<string, ResourceDictionary>, ResourceDictionary>(keyValuePair => keyValuePair.Value)).ToList<ResourceDictionary>();
            element.Resources.MergedDictionaries.Clear();
            foreach (string str1 in array)
            {
                char[] chArray = new char[1] { ',' };
                string str2 = str1.Split(chArray)[0].ToLower(CultureInfo.InvariantCulture) + ".xaml";
                Uri uri = new Uri("/Telerik.Windows.Themes." + themeName + ";component/themes/" + str2, UriKind.RelativeOrAbsolute);
                ResourceDictionary resourceDictionary = new ResourceDictionary()
                {
                    Source = uri
                };
                element.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        public void setTheme(string themeName)
        {
            string[] array = ((IEnumerable<string>)defaultReferencesNamesForApplication).ToArray<string>();
            Application.Current.Resources.MergedDictionaries.Except<ResourceDictionary>(cachedResourceDictionaries.Where<KeyValuePair<string, ResourceDictionary>>(keyValuePair => keyValuePair.Key.Contains("Telerik.Windows.Themes.")).Select<KeyValuePair<string, ResourceDictionary>, ResourceDictionary>(keyValuePair => keyValuePair.Value)).ToList<ResourceDictionary>();
            Application.Current.Resources.MergedDictionaries.Clear();
            foreach (string str1 in array)
            {
                char[] chArray = new char[1] { ',' };
                string str2 = str1.Split(chArray)[0].ToLower(CultureInfo.InvariantCulture) + ".xaml";
                Uri uri = new Uri("/Telerik.Windows.Themes." + themeName + ";component/themes/" + str2, UriKind.RelativeOrAbsolute);
                ResourceDictionary resourceDictionary = new ResourceDictionary()
                {
                    Source = uri
                };
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
        public string setLightOrDark(string themeName)
        {
            if (themeName.Contains("Office2013"))
            {
                switch (themeName)
                {
                    case "Office2013_LightGray":
                        Office2013Palette.LoadPreset(Office2013Palette.ColorVariation.LightGray);
                        break;
                    case "Office2013_DarkGray":
                        Office2013Palette.LoadPreset(Office2013Palette.ColorVariation.DarkGray);
                        break;
                    default:
                        Office2013Palette.LoadPreset(Office2013Palette.ColorVariation.White);
                        break;
                }
                return "Office2013";
            }
            if (themeName.Contains("VisualStudio2013"))
            {
                switch (themeName)
                {
                    case "VisualStudio2013_Blue":
                        VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Blue);
                        break;
                    case "VisualStudio2013_Dark":
                        VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);
                        break;
                    default:
                        VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Light);
                        break;
                }
                return "VisualStudio2013";
            }
            if (themeName.Contains("Green"))
            {
                switch (themeName)
                {
                    case "Green_Light":
                        GreenPalette.LoadPreset(GreenPalette.ColorVariation.Light);
                        break;
                    case "Green_Dark":
                        GreenPalette.LoadPreset(GreenPalette.ColorVariation.Dark);
                        break;
                    default:
                        GreenPalette.LoadPreset(GreenPalette.ColorVariation.Dark);
                        break;
                }
                return "Green";
            }
            if (themeName.Contains("Fluent"))
            {
                switch (themeName)
                {
                    case "Fluent_Dark":
                        FluentPalette.LoadPreset(FluentPalette.ColorVariation.Dark);
                        break;
                    default:
                        FluentPalette.LoadPreset(FluentPalette.ColorVariation.Light);
                        break;
                }
                return "Fluent";
            }
            if (themeName.Contains("Crystal"))
            {
                if (themeName.Contains("Crystal_Dark"))
                {
                    CrystalPalette.LoadPreset(CrystalPalette.ColorVariation.Dark);
                }
                else
                {
                    CrystalPalette.LoadPreset(CrystalPalette.ColorVariation.Light);
                }
                return "Crystal";
            }
            return themeName;
        }
        private static readonly string[] defaultReferencesNamesForApplication = new string[]
        {
            "System.Windows",
            "Telerik.Windows.Controls.Navigation",
            "Telerik.Windows.Controls",
            //"Telerik.Windows.Controls.Pivot",
            //"Telerik.Windows.Controls.PivotFieldList",
            //"Telerik.Windows.Controls.RibbonView",
            //"Telerik.Windows.Controls.RichTextBoxUI",
            //"Telerik.Windows.Controls.ScheduleView",
            //"Telerik.Windows.Controls.Spreadsheet",
            //"Telerik.Windows.Controls.SpreadsheetUI",
            //"Telerik.Windows.Controls.VirtualGrid",
            //"Telerik.Windows.Documents.Proofing",
            //"Telerik.Windows.Documents",
            //"Telerik.Windows.Cloud.Controls",
            //"Telerik.Windows.Controls.Chart",
            //"Telerik.Windows.Controls.ConversationalUI",
            "Telerik.Windows.Controls.Data",
            "Telerik.Windows.Controls.DataVisualization",
            //"Telerik.Windows.Controls.Diagrams.Extensions",
            //"Telerik.Windows.Controls.Diagrams.Ribbon",
            //"Telerik.Windows.Controls.Diagrams",
            //"Telerik.Windows.Controls.Docking",
            //"Telerik.Windows.Controls.Expressions",
            //"Telerik.Windows.Controls.FileDialogs",
            //"Telerik.Windows.Controls.FixedDocumentViewers",
            //"Telerik.Windows.Controls.FixedDocumentViewersUI",
            //"Telerik.Windows.Controls.GanttView",
            "Telerik.Windows.Controls.GridView",
            "Telerik.Windows.Controls.ImageEditor",
            "Telerik.Windows.Controls.Input",
        };
    }
}