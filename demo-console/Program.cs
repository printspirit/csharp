using SpiritLabelLibrary;

class Program
{
    static void Main()
    {
        var vars = new Dictionary<string, object>
        {
            { "AA", "BB" },
            { "CC", "DD" }
        };
        
        // 调用打印接口
        var p = SpiritLabel.OpenPrinter("Spirit Image");
        p.size(1200, 800).quality(Qty.Hight);
        //p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Print("/home/lg/文档/打印精灵/labels/xxx1.psl", vars);
        p.Close();
        
	// 获取打印机列表和详细信息
        foreach (var prn in SpiritLabel.PrnLst()) {
            Console.WriteLine(prn);
            SpiritLabel.PrnInfo(prn.name);
        }

	// 安装许可证。许可证和桌面版共享，可以使用桌面版界面安装
        var err = SpiritLabel.AddLicense("0-HCAEO-2AAAF-GFWSA-5AAAA");
        Console.WriteLine(err);
    }
}
