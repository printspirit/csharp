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
        p.Print("/tmp/labels/xxxx2.psl", vars);
        p.Close();

        // 调用打印接口
        //SpiritLabel.Print("xxx", "Spirit Image", 1200, 800, vars);
        //var file="C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl";
        //var file="/tmp/xxxx1.psl";
        //SpiritLabel.Design(file);
    }
}
