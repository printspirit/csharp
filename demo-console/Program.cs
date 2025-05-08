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
        SpiritLabel.Print("xxx", "Spirit Image", 1200, 800, vars);

        var file="C:\\Users\\刘刚\\Documents\\打印精灵\\labels\\xxx.psl";
		SpiritLabel.Design(file);
    }
}