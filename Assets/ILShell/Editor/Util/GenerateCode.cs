using ILRuntime.Runtime.CLRBinding;
using ILRuntime.Runtime.Enviorment;
using ILShell.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.ILShell.Editor.Util
{
    public class GenerateCode
    {
        [MenuItem("ILShell/Generate/CrossBindingAdapters")]
        static void GenerateCrossBindingAdapters()
        {
            //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
            //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题

            var types = new Type[]
            {
                typeof(MonoBehaviour),
            };
            var path = "Assets/ILShell/Runtime/Adapter";
            var @namespace = "ILShell.Adapter";
            GenerateCrossBindingAdapters(path, @namespace, types);
        }


        public static void GenerateCrossBindingAdapters(string path, string @namespace, params Type[] types)
        {
            if (types == null || types.Length == 0)
                return;
            Directory.CreateDirectory(path);
            foreach (Type type in types)
            {
                var outPath = Path.Combine(path, type.Name + "Adapter.cs");
                using (var sw = new StreamWriter(outPath))
                {
                    sw.WriteLine(CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, @namespace));
                }
            }
            AssetDatabase.Refresh();
        }


        //[MenuItem("ILShell/通过自动分析热更DLL生成CLR绑定")]
        //private static void GenerateCLRBindings2()
        //{
        //    var domain = ILAPP.CurrentDomain;
        //    BindingCodeGenerator.GenerateBindingCode(domain, "Assets/ILShell/Runtime/Bindings/Generated");
        //    AssetDatabase.Refresh();
        //}

        [MenuItem("ILShell/Generate/CLRBindings")]
        public static void GenerateCLRBindings()
        {
            var types = new Type[]
            {
                typeof(MonoBehaviour),
            };
            var path = "Assets/ILShell/Runtime/Binding/Generated";
            GenerateCLRBindings(path, types);
        }

        public static void GenerateCLRBindings(string path, params Type[] types)
        {
            var domain = ILAPP.CurrentDomain;
            var typeList = new List<Type>(types);
            BindingCodeGenerator.GenerateBindingCode(typeList, path);
            AssetDatabase.Refresh();
        }
    }
}
