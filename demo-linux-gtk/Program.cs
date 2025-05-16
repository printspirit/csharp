using System;
using Gtk;
using SpiritLabelLibrary;


public class MainWindow : Window
{
    public MainWindow() : base("GtkSharp 示例")
    {
        // 设置窗口关闭行为
        SetDefaultSize(300, 200);
        SetPosition(WindowPosition.Center);

        Button buttonEdit = new Button("编辑标签");
        buttonEdit.Clicked += (sender, e) =>
        {
            SpiritLabel.Design("/tmp/xxxx1.psl");
        };
        
        Button buttonNewLabel = new Button("新建并编辑标签");
        buttonNewLabel.Clicked += (sender, e) =>
        {
            // 以/tmp/xxxx1.psl为模板创建新的标签，并编辑
			// 标签大小为 120mm X 80mm, 打印机最佳分辨率为300DPI 
			SpiritLabel.NewLabel("/tmp/labels/xxxx2.psl", "名称", "说明", 1200, 800, 300, "/tmp/xxxx1.psl");
            SpiritLabel.Design("/tmp/labels/xxxx2.psl");
        };
        
        Button buttonPrint = new Button("打印");
        buttonPrint.Clicked += (sender, e) =>
        {
            var vars = new Dictionary<string, object>{
                { "AA", "BB" },
                { "CC", "DD" }
            };
            var p = SpiritLabel.OpenPrinter("Spirit Image", 1200, 800);
			p.size(1200, 800);
			p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
			p.Close();
        };
                
        VBox vbox = new VBox();
        
        vbox.PackStart(buttonEdit, true, true, 0);
        vbox.PackStart(buttonNewLabel, true, true, 0);
        vbox.PackStart(buttonPrint, true, true, 0);

        Add(vbox);

        // 显示所有控件
        ShowAll();
    }

    public static void Main()
    {
        Application.Init();
        new MainWindow();
        Application.Run();
    }
}

