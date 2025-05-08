using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SpiritLabelLibrary
{
    public class SpiritLabel
    {
		
        // 统一使用通用库名称（实际加载时会动态替换）
        private const string SpiritLib = "spirit";

        // 声明动态库函数（统一使用 spirit 作为库名）
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritInit();
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Print(ref Cmd cmd);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr spirit_vars_init(int size);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void spirit_vars_add(IntPtr vars, string key, string value);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void free(IntPtr ptr);
        
        [DllImport(SpiritLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpiritEdit(string file);

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
                        ? "spirit.dll" 
                        : "spirit.so";
                    
                    // 加载实际库文件（会自动搜索程序目录等路径）
                    return NativeLibrary.Load(actualLibName, assembly, searchPath);
                }
                return IntPtr.Zero; // 其他库保持默认行为
            });
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

        // SpiritVars 类，用于简化 key-value 管理
        public class SpiritVars
        {
            private IntPtr _varsPointer;

            public SpiritVars(Dictionary<string, object> keyValues)
            {
                _varsPointer = ToSpiritVar(keyValues);
            }

            // 将字典转换为 C 所需的格式
            private IntPtr ToSpiritVar(Dictionary<string, object> keyValues)
            {
                int size = keyValues.Count;
                IntPtr varsPointer = spirit_vars_init(size);

                foreach (var kvp in keyValues)
                {
                    // 对于不同类型的值，我们将其转换为字符串
                    string value = Convert.ToString(kvp.Value);  // 转换为字符串
                    spirit_vars_add(varsPointer, kvp.Key, value);
                }

                return varsPointer;
            }

            // 获取原始指针，以便传递给 C 函数
            internal IntPtr GetPointer()
            {
                return _varsPointer;
            }

            // 析构函数，释放内存
            ~SpiritVars()
            {
                if (_varsPointer != IntPtr.Zero)
                {
                    free(_varsPointer);
                    _varsPointer = IntPtr.Zero;
                }
            }
        }

        // 创建并打印
        public static void Print(string tpid, string printer, int width, int height, Dictionary<string, object> vars)
        {
            // 初始化 Spirit
            SpiritInit();

            // 创建 SpiritVars 对象并将字典传递给它
            SpiritVars spiritVars = new SpiritVars(vars);

            // 创建 Cmd 对象并设置参数
            Cmd c = new Cmd
            {
                tpid = tpid,
                printer = printer,
                width = width,
                height = height,
                vars = spiritVars.GetPointer()
            };

            // 调用 C 函数打印
            Print(ref c);
        }
        
        public static void Design(string file) {
            SpiritEdit(file);
        }
    }
}
