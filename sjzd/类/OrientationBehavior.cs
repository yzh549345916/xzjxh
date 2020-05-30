using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace sjzd
{
    public class OrientationBehavior : DependencyObject
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation",
            typeof(Orientation),
            typeof(OrientationBehavior),
            new PropertyMetadata(Orientation.Vertical, OrientationBehavior.OnOrientationChanged));

        public static Orientation GetOrientation(DependencyObject obj)
        {
            return (Orientation)obj.GetValue(OrientationProperty);
        }

        public static void SetOrientation(DependencyObject obj, Orientation value)
        {
            obj.SetValue(OrientationProperty, value);
        }

        private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RadCartesianChart chart = sender as RadCartesianChart;
            CartesianAxis horizontalAxis = chart.HorizontalAxis;
            CartesianAxis verticalAxis = chart.VerticalAxis;
            chart.HorizontalAxis = null;
            chart.VerticalAxis = null;
            chart.HorizontalAxis = verticalAxis;
            chart.VerticalAxis = horizontalAxis;
        }
    }

    public class OrientationToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Orientation orientation = (Orientation)Enum.Parse(typeof(Orientation), value.ToString(), true);
            if (orientation == Orientation.Vertical)
                return false;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isChecked = (bool)value;

            if (isChecked)
                return Orientation.Horizontal;

            return Orientation.Vertical;
        }
    }
}
