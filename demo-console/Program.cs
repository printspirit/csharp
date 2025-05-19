using SpiritLabelLibrary;

class Program
{
    static void Main()
    {
        var vars = new Dictionary<string, object>
        {
            { "co_name", "打印精灵" },
            { "name", "标签打印机" }
        };
        
        // 调用打印接口
        var p = SpiritLabel.OpenPrinter("LPT1");
        p.size(500, 300).quality(Qty.Hight);
        p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Close();
        
		// 获取打印机列表和详细信息
        foreach (var prn in SpiritLabel.PrnLst()) {
            Console.WriteLine(prn);
            Console.WriteLine(SpiritLabel.PrnInfo(prn.name));
        }

		// 安装许可证。许可证和桌面版共享，可以使用桌面版界面安装
        //var err = SpiritLabel.AddLicense("0-HCAEO-2AAAF-GFWSA-5AAAA");
        //Console.WriteLine(err);
		
		var file="C:\\打印精灵\\xxx1.psl";
		
		// 以/tmp/xxxx1.psl为模板创建新的标签，并编辑
		// 标签大小为 120mm X 80mm, 打印机最佳分辨率为300DPI 
		SpiritLabel.NewLabel(file, "名称", "说明", 1200, 800, 300, "C:\\打印精灵\\xxx.psl");
		Console.WriteLine(file);
		SpiritLabel.Design(file);
    }
}
