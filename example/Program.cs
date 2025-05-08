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

        // 创建按钮并设置其文本
        Button button = new Button("编辑标签");
        button.Clicked += (sender, e) =>
        {
            // 按钮点击时弹出消息
            SpiritLabel.Design("/home/lg/文档/打印精灵/labels/xxx.psl");
        };

        // 将按钮添加到窗口
        Add(button);

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

