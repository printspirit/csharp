using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Newtonsoft.Json;

namespace SpiritLabelLibrary
{
    public class SpiritException : Exception
    {
        public SpiritException() { }

        public SpiritException(string message) 
            : base(message) { }

        public SpiritException(string message, Exception inner) 
            : base(message, inner) { }
    }
    
    public class Printer
    {
        public string name="" ;
        public string type="" ;
        public bool   act=false  ;
        
        // 重写 ToString 方法
        public override string ToString() {
            return $"Name: {name}, Type: {type}, Active:{act}";
        }
    }
    
    public class Paper {
        public string name="";
        public int    w=0;
        public int    h=0;
        public int    cols=0;
        public int    rows=0;
        public int    gapX=0;
        public int    gapY=0;
        public int    marginTop=0;
        public int    marginLeft=0;
        
        // 重写 ToString 方法
        public override string ToString() {
            return $"Name: {name}, {w}x{h} col={cols} rows={rows} gapX={gapX} gapY={gapY} marginTop={marginTop} marginLeft={marginLeft}";
        }
    }
            
    public class PrinterInfo
    {
        public string name="";
        public string output_dir="";
        public List<Paper> paper=new List<Paper>(0);
    }
    
    public enum Qty {
        Fast,
        Middle,
        Hight
    }    
    public enum PrinterType {ZPL, CPCL, TSPL, ESCPOS}
    
    public class SpiritLabel
    {
		
        // 统一使用通用库名称（实际加载时会动态替换）
        private const string SpiritLib = "spirit";
        
        // 声明动态库函数（统一使用 spirit 作为库名）
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritInit();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritDeInit();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Print(byte[] tpid, byte[] vars, byte[] opts);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SpiritEdit(byte[] file);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SpiritNewLabel(byte[] file, byte[] name, byte[] memo, int width, int height, int dpi, byte[] ref_label );
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ResetPrinter(string p);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SpiritInstallLicense(string key);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetSpiritError();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritFree(IntPtr p);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SpiritPrnLst();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SpiritPrnInfo(string prn);
        
        private Dictionary<string, object>? opts;

        // 静态构造函数：注册动态库解析器
        static SpiritLabel()
        {
            // 设置动态库解析逻辑
            NativeLibrary.SetDllImportResolver(typeof(SpiritLabel).Assembly, (libraryName, assembly, searchPath) =>
            {
                if (libraryName == SpiritLib)
                {
                    // 根据操作系统选择实际文件名
                    string actualLibName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                        ? "libspirit.dll" 
                        : "libspirit.so";
                    
                    // 加载实际库文件（会自动搜索程序目录等路径）
                    return NativeLibrary.Load(actualLibName, assembly, searchPath);
                }
                return IntPtr.Zero; // 其他库保持默认行为
            });
			
			// 初始化 Spirit
            SpiritInit();
			
			//
			// 静态构造函数，用于初始化静态类
			AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
			Console.CancelKeyPress += new ConsoleCancelEventHandler(OnProcessExit);
		}

        #nullable disable
		private static void OnProcessExit(object sender, EventArgs e)
		{
			// 进程退出时执行的清理工作
			Console.WriteLine("Program is exiting. Performing cleanup...");
			SpiritDeInit();
		}
		
		public static string last_error() {
		    IntPtr ptr = GetSpiritError();
            int length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
            {
                length++;
            }
            byte[] utf8Bytes = new byte[length];
            Marshal.Copy(ptr, utf8Bytes, 0, length);
		    SpiritFree(ptr);
		    return Encoding.UTF8.GetString(utf8Bytes);
		}
		        
        public static void NewLabel(string file, string name, string memo, int width, int height, int dpi, string ref_label) {
        
            if (SpiritNewLabel(
					Encoding.UTF8.GetBytes(file), 
					Encoding.UTF8.GetBytes(name), 
					Encoding.UTF8.GetBytes(memo), 
					width, height, dpi, 
					Encoding.UTF8.GetBytes(ref_label))<0) {
				var err =last_error();
    			throw new SpiritException(err);
			}
        }
        
        public static void NewLabel(string file, string name, string memo, int width, int height, int dpi) {
		    NewLabel(file, name, memo, width, height, dpi, "");
		}
        
        public static SpiritLabel OpenPrinter(string prn, PrinterType type) {
            var p = new SpiritLabel();
            p.opts=new Dictionary<string, object>();
            p.opts.Add("printer", prn);
            p.opts.Add("type", type);
			p.opts.Add("flush", true);
            return p;
        }
                
        public static SpiritLabel OpenPrinter(string prn) {
            var p = new SpiritLabel();
            p.opts=new Dictionary<string, object>();
            p.opts.Add("printer", prn);
			p.opts.Add("flush", true);
            return p;
        }
                
        public SpiritLabel row(int r) {
            opts.Add("row", r);
            return this;
        }

        public SpiritLabel col(int c) {
            opts.Add("col", c);
            return this;
        }

        public SpiritLabel fill(int c) {
            opts.Add("fill", c);
            return this;
        }

        public SpiritLabel size(int c) {
            opts.Add("size", c);
            return this;
        }

        public SpiritLabel size(int w, int h) {
            int[] size = { w, h };
            opts.Add("size", size);
            return this;
        }

        public SpiritLabel gap(int gapX, int gapY) {
            opts.Add("gapX", gapX);
            opts.Add("gapY", gapY);
            return this;
        }

        public SpiritLabel gap(int gapX) {
            opts.Add("gapX", gapX);
            opts.Add("gapY", gapX);
            return this;
        }

        public SpiritLabel gapX(int gapX) {
            opts.Add("gapX", gapX);
            return this;
        }

        public SpiritLabel gapY(int gapY) {
            opts.Add("gapY", gapY);
            return this;
        }

        public SpiritLabel flush(bool f) {
            opts.Add("flush", f);
            return this;
        }

        public SpiritLabel quality(Qty q) {
            switch (q) {
                case Qty.Fast:
                    opts.Add("quality", 0);
                    break;
                case Qty.Middle:
                    opts.Add("quality", 1);
                    break;
                case Qty.Hight:
                    opts.Add("quality", 2);
                    break;
            }
            return this;
        }

        public SpiritLabel add(string key, object val) {
            opts.Add(key, val);
            return this;
        }

        // 打印
        public void Print(string tpid, Dictionary<string, object> vars)
        {
            var json_vars = JsonConvert.SerializeObject(vars);
            var json_opts = JsonConvert.SerializeObject(opts);
            byte[] utf8_tpid = Encoding.UTF8.GetBytes(tpid);
			byte[] utf8_json_vars = Encoding.UTF8.GetBytes(json_vars);
			byte[] utf8_json_opts = Encoding.UTF8.GetBytes(json_opts);
			if (Print(utf8_tpid, utf8_json_vars, utf8_json_opts)<0) {
    			var err =last_error();
    			Console.WriteLine(err);
    			throw new SpiritException(err);
			}
        }
        
         // 创建并打印
        public void Close()
        {
            ResetPrinter(opts["printer"] as string);
        }
        
        public static void Design(string file) {
			if (SpiritEdit(Encoding.UTF8.GetBytes(file))<0) {
				throw new SpiritException(last_error());
			}
		}
	
		
		/*public static void Design(string file) {
		
			try
			{
				Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
				Console.WriteLine("文件已成功打开！");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"打开文件时出错: {ex.Message}");
			}
		}*/
		
		public static string AddLicense(string key) {
		    if (SpiritInstallLicense(key)<0) {
    		    var err =last_error();
    		    throw new SpiritException($"Add License KEY={key} Error: {err}");    
		    }
		    return "";
		}
		
		public static List<Printer> PrnLst() {
		    IntPtr ptr = SpiritPrnLst();
            int length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
            {
                length++;
            }
            byte[] utf8Bytes = new byte[length];
            Marshal.Copy(ptr, utf8Bytes, 0, length);
		    SpiritFree(ptr);
		    var js = Encoding.UTF8.GetString(utf8Bytes);
		    var l = JsonConvert.DeserializeObject<List<Printer>>(js);
		    return l;
		}
		
		public static Object PrnInfo(string prn) {
		    IntPtr ptr = SpiritPrnInfo(prn);
            int length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
            {
                length++;
            }
            byte[] utf8Bytes = new byte[length];
            Marshal.Copy(ptr, utf8Bytes, 0, length);
		    SpiritFree(ptr);
		    var js = Encoding.UTF8.GetString(utf8Bytes);
		    return JsonConvert.DeserializeObject<Object>(js);
		}
    }
}
