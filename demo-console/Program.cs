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
        
        var p = SpiritLabel.OpenPrinter("Spirit Image");
        p.size(1200, 800).quality(Qty.Hight);
        //p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Print("/home/lg/文档/打印精灵/labels/xxx1.psl", vars);
        p.Close();
        
        foreach (var prn in SpiritLabel.PrnLst()) {
            Console.WriteLine(prn);
            SpiritLabel.PrnInfo(prn.name);
        }

        var err = SpiritLabel.AddLicense("0-HCAEO-2AAAF-GFWSA-5AAAA");
        Console.WriteLine(err);
       
        // 调用打印接口
        //SpiritLabel.Print("xxx", "Spirit Image", 1200, 800, vars);
        //var file="C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl";
        //var file="/tmp/xxxx1.psl";
        //SpiritLabel.Design(file);
    }
}
