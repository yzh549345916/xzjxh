using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;

namespace sjzd
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public static string MyDir = Environment.CurrentDirectory + @"\设置文件\";
        public Window1()
        {
            InitializeComponent();
        }
        private void btnSimple_Click(object sender, EventArgs e)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            //添加包含默认数据的图表。您可以指定不同的图表类型和大小。
            var shape = builder.InsertChart(ChartType.Column, 432, 252);
            //Shape的Chart属性包含所有与图表相关的选项。
            var chart = shape.Chart;

            //chart.Title = "cs";

            //获取图表系列集合
            var seriesColl = chart.Series;

            //删除默认生成的系列。
            seriesColl.Clear();

            //Create category names array, in this example we have two categories.
            //创建类别名称数组，在此示例中我们有两个类别。
            string[] categories = { "第一赛季", "第二赛季", "第三赛季" };

            //添加新系列。数据数组不能为空，并且数组的大小必须相同。
            seriesColl.Add("AW Series 1", categories, new double[] { 1, 2, 2 });
            seriesColl.Add("AW Series 2", categories, new double[] { 3, 4, 44 });
            seriesColl.Add("AW Series 3", categories, new double[] { 5, 6, 22 });
            seriesColl.Add("AW Series 4", categories, new double[] { 7, 81, 3 });
            seriesColl.Add("AW Series 5", categories, new double[] { 9, 10, 44 });


            var wordPath = MyDir+"a.docx";
            doc.Save(wordPath);
        }

        private void btnColumn_Click(object sender, EventArgs e)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            //条形图
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            //使用此重载可将系列添加到任何类型的条形图，柱形图，折线图和曲面图。
            chart.Series.Add("AW Series 1", new string[] { "AW Category 1", "AW Category 2" }, new double[] { 1, 2 });


            doc.Save(MyDir + @"TestInsertColumnChart.docx");
        }

        private void btnScatter_Click(object sender, EventArgs e)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Scatter, 432, 252);
            Chart chart = shape.Chart;

            //使用此重载可将系列添加到任何类型的散点图
            chart.Series.Add("AW Series 1", new double[] { 0.7, 1.8, 2.6 }, new double[] { 2.7, 3.2, 0.8 });

            doc.Save(MyDir + @"TestInsertScatterChart.docx");
        }

        private void btnArea_Click(object sender, EventArgs e)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Area, 432, 252);
            Chart chart = shape.Chart;

            chart.Series.Add("AW Series 1", new DateTime[] {
                                            new DateTime(2002, 05, 01),
                                            new DateTime(2002, 06, 01),
                                            new DateTime(2002, 07, 01),
                                            new DateTime(2002, 08, 01),
                                            new DateTime(2002, 09, 01)}, new double[] { 32, 32, 28, 12, 15 });

            doc.Save(MyDir + @"TestInsertAreaChart.docx");
        }

        private void btnBubble_Click(object sender, EventArgs e)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
           
            Shape shape = builder.InsertChart(ChartType.Line3D, 432, 252);
            shape.BehindText = true;
            Chart chart = shape.Chart;
           chart.Title.Text ="测试一下哈哈哈哈哈哈";
            
            chart.Series.Add("AW Series 1", new double[] { 0.7, 1.8, 2.6 }, new double[] { 2.7, 3.2, 0.8 }, new double[] { 10, 4, 8 });

            // chart.Legend.Overlay = true;
            doc.Save(MyDir + @"TestInsertBubbleChart.docx");
        }


    }

}
