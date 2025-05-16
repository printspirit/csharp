using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System;
using System.Diagnostics;



using SpiritLabelLibrary;

namespace demo_win_gui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
	
	// 事件处理程序
	private void Button1_Click(object sender, RoutedEventArgs e)
	{
		var vars = new Dictionary<string, object>
        {
            { "AA", "BB" },
            { "CC", "DD" }
        };

        var p = SpiritLabel.OpenPrinter("Spirit Image");
		p.size(1200, 800);
        p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Close();
	}
	
	private void Button2_Click(object sender, RoutedEventArgs e)
	{
		var file="C:\\打印精灵\\xxx.psl";
		// 以/tmp/xxxx1.psl为模板创建新的标签，并编辑
		// 标签大小为 120mm X 80mm, 打印机最佳分辨率为300DPI 
		SpiritLabel.NewLabel(file, "名称", "说明", 1200, 800, 300, "") 
		SpiritLabel.Design(file);
	}
}