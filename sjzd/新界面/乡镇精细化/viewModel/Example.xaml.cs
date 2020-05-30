using System.Windows;
using System.Windows.Controls;


namespace sjzd
{
    /// <summary>
    /// Interaction logic for Example.xaml
    /// </summary>
    public partial class Example : UserControl
    {
        Telerik.Windows.Controls.RadCarouselPanel panel;
        public Example()
        {
            InitializeComponent();
            this.DataContext = new Stations();
            sampleRadCarousel.ReflectionSettings.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.sampleRadCarousel.ReflectionSettings.Visibility = Visibility.Hidden;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.sampleRadCarousel.ReflectionSettings.Visibility = Visibility.Visible;
        }

        private void opacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.Opacity = e.NewValue;
        }

        private void offsetXSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.OffsetX = e.NewValue;
        }

        private void offsetYSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.OffsetY = e.NewValue;
        }

        private void hiddenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.HiddenPercentage = e.NewValue;
        }

        private void angleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.Angle = e.NewValue;
        }

        private void offsetHSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.HeightOffset = e.NewValue;
        }

        private void offsetWSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.sampleRadCarousel.ReflectionSettings.WidthOffset = e.NewValue;
        }

    }
}