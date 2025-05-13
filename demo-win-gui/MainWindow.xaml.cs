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
		//SpiritLabel.Init();
		//SpiritLabel.Design("C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl");
		//SpiritLabel.Design("C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl");
	
		var vars = new Dictionary<string, object>
        {
            { "AA", "BB" },
            { "CC", "DD" }
        };

        var p = SpiritLabel.OpenPrinter("Spirit Image", 1200, 800);
        p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Close();
	}
	
	private void Button2_Click(object sender, RoutedEventArgs e)
	{
		var file="C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl";
		SpiritLabel.Design(file);
		

	}
}