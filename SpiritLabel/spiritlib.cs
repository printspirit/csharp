using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpiritLabelLibrary
{
    public class SpiritLabel
    {
		
        // 统一使用通用库名称（实际加载时会动态替换）
        private const string SpiritLib = "spirit";
        
        private string printer="";
        private int width, height;

        // 声明动态库函数（统一使用 spirit 作为库名）
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritInit();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritDeInit();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Print(ref Cmd cmd);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritEdit(IntPtr file);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritNewLabel(string file, string name, string memo, int width, int height, int dpi, string ref_label );
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ResetPrinter(string p);

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

		private static void OnProcessExit(object sender, EventArgs e)
		{
			// 进程退出时执行的清理工作
			Console.WriteLine("Program is exiting. Performing cleanup...");
			SpiritDeInit();
		}

        // 定义 Cmd 结构体
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct Cmd
        {
            public string tpid;
            public string printer;
            public int width;
            public int height;
            public IntPtr vars; // 指向 KV 参数的指针
        }
        
        public static void NewLabel(string file, string name, string memo, int width, int height, int dpi, string ref_label) {
            SpiritNewLabel(file, name, memo, width, height, dpi, ref_label);
        }
        
        public static SpiritLabel OpenPrinter(string prn, int width, int height) {
            var p = new SpiritLabel();
            p.printer = prn;
            p.width = width;
            p.height = height;
            return p;
        }

        // 创建并打印
        public void Print(string tpid, Dictionary<string, object> vars)
        {
			Console.WriteLine("print");
            // 创建 Cmd 对象并设置参数
            Cmd c = new Cmd
            {
                tpid = tpid,
                printer = printer,
                width = width,
                height = height,
            };
            
            // 调用 C 函数打印
            Print(ref c);
        }
        
         // 创建并打印
        public void Close()
        {
            ResetPrinter(printer);
        }
        
        public static void Design(string file) {
			byte[] utf8Bytes = Encoding.UTF8.GetBytes(file);

			byte[] utf8BytesWithNull = new byte[utf8Bytes.Length + 1];
			Array.Copy(utf8Bytes, utf8BytesWithNull, utf8Bytes.Length);
			
			// 分配非托管内存，并将字节数组复制到该内存中
			IntPtr ptr= Marshal.AllocHGlobal(utf8BytesWithNull.Length);
			Marshal.Copy(utf8BytesWithNull, 0, ptr, utf8BytesWithNull.Length);

			// 调用 C 函数，传递非托管字符串
			SpiritEdit(ptr);

			// 释放非托管内存
			Marshal.FreeHGlobal(ptr);
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
		
    }
}
