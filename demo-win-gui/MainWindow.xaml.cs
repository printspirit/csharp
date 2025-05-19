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
        InitializePrinterListComboBox();
    }
    
    private void InitializePrinterListComboBox()
    {
        foreach (var prn in SpiritLabel.PrnLst()) {
            cmbOptions.Items.Add(prn.name);
        }
        cmbOptions.SelectedIndex = 0; // 设置默认选中项
    }
	
	// 事件处理程序
	private void BtnPrint_Click(object sender, RoutedEventArgs e)
	{
	    var selectedItem = cmbOptions.SelectedItem as ComboBoxItem;
	    printer = selectedItem.Content
	
		var vars = new Dictionary<string, object>
        {
            { "co_name", "打印精灵" },
            { "name", "标签打印机" }
        };

        var p = SpiritLabel.OpenPrinter(printer);
		p.size(500, 300);
        p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Close();
        
        MessageBox.Show($"打印完成", "提示");
	}
	
	private void BtnEdit_Click(object sender, RoutedEventArgs e)
	{
		//var file="C:\\打印精灵\\xxx1.psl";
		var file="c:\\tmp\\xxx.psl";
		
		// 以/tmp/xxxx1.psl为模板创建新的标签，并编辑
		// 标签大小为 120mm X 80mm, 打印机最佳分辨率为300DPI 
		SpiritLabel.NewLabel(file, "名称", "说明", 1200, 800, 300, "C:\\打印精灵\\xxx.psl");
		SpiritLabel.Design(file);
	}
}
