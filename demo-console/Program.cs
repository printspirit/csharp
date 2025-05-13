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
        
        var p = SpiritLabel.OpenPrinter("Spirit Image", 1200, 800);
        p.Print("acae8013-28db-4b77-a500-1a6052633a22", vars);
        p.Close();

        // 调用打印接口
        //SpiritLabel.Print("xxx", "Spirit Image", 1200, 800, vars);
        //var file="C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl";
        //var file="/tmp/xxxx1.psl";
        //SpiritLabel.Design(file);
    }
}
