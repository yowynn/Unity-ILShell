using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILShell.Adapter;
using ILShell.Runtime.Binding;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ILShell.Runtime
{
    public class ILAPP : MonoBehaviour
    {
        #region Static

        private static ILAPP currentInstance;

        private static AppDomain domainForEditorMode;

        public static ILAPP Current
        {
            get
            {
                if (currentInstance != null)
                    return currentInstance;
                else
                {
                    currentInstance = CreateInstance("Global");
                    return currentInstance;
                }
            }
        }

        public static AppDomain CurrentDomain
        {
            get
            {
                var currentDomain = currentInstance?.AppDomain;
                if (currentDomain != null)
                {
                    return currentDomain;
                }
                else
                {
                    if (!Application.isPlaying)
                    {
                        domainForEditorMode = domainForEditorMode ?? CreateEditorDomain();
                        return domainForEditorMode;
                    }
                    else
                    {
                        currentInstance = CreateInstance("Global");
                        return currentInstance?.AppDomain;
                    }
                }
            }
        }

        public static ILAPP CreateInstance(string name)
        {
            GameObject go = new GameObject();
            go.hideFlags = HideFlags.DontSave;
            go.name = $"{typeof(ILAPP).Name}({name})";
            var instance = go.AddComponent<ILAPP>();
            instance.Name = name;
            instance.Initialize();
            return instance;
        }

        private static AppDomain CreateEditorDomain()
        {
#if UNITY_EDITOR
            var AppDomain = new AppDomain();
            var findPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/ScriptAssemblies/");
            string[] files = Directory.GetFiles(findPath, "Product.*.dll", SearchOption.TopDirectoryOnly);
            foreach (string path in files)
            {
                var dllpath = path;
                var pdbpath = Path.ChangeExtension(path, ".pdb");
                LoadAssemblyLocal(AppDomain, dllpath, pdbpath);
            }
            Debug.LogWarning($"[ILAPP]CreateDefaultDomain in DEBUG!!!");

            AppDomain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
            CLRBindingsFixed.Initialize(AppDomain);

            return AppDomain;
#endif
            throw new System.Exception("Debug Method");
        }

        public static IType GetType(string typeName)
        {
            return CurrentDomain.GetType(typeName);
        }

        #endregion Static

        #region Instance

        public string Name { get; private set; }

        public AppDomain AppDomain { get; private set; }

        public ILInvoker ILInvoker { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            ILInvoker.Update();
        }


        public void Initialize()
        {
            AppDomain = CreateEditorDomain();
            ILInvoker = ILInvoker.CreateInstance();
        }

        public static void LoadAssemblyLocal(AppDomain domain, string dllpath, string pdbpath = null)
        {
            byte[] dlldata = null, pdbdata = null;
            dlldata = File.ReadAllBytes(dllpath);
            if (pdbpath != null)
            {
                pdbdata = File.ReadAllBytes(pdbpath);
                domain.LoadAssembly(new MemoryStream(dlldata), new MemoryStream(pdbdata), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            else
            {
                domain.LoadAssembly(new MemoryStream(dlldata));
            }
            Debug.Log($"LoadAssemblyLocalFinish: {dllpath}: {dlldata?.Length} & {pdbpath}: {pdbdata?.Length}");
        }

        public static IEnumerator LoadAssemblyRemote(AppDomain domain, string dllurl, string pdburl = null)
        {
            byte[] dlldata = null, pdbdata = null;
            using (UnityWebRequest uwr = new UnityWebRequest(dllurl))
            {
                uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                yield return uwr.SendWebRequest();
                if (uwr.isDone && uwr.result == UnityWebRequest.Result.Success)
                {
                    dlldata = uwr.downloadHandler.data;
                }
            }
            if (dlldata == null)
                throw new System.Exception($"Cannot load DLL: {dllurl}");
            if (pdburl != null)
            {
                using (UnityWebRequest uwr = new UnityWebRequest(pdburl))
                {
                    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                    yield return uwr.SendWebRequest();
                    if (uwr.isDone && uwr.result == UnityWebRequest.Result.Success)
                    {
                        pdbdata = uwr.downloadHandler.data;
                    }
                }
                domain.LoadAssembly(new MemoryStream(dlldata), new MemoryStream(pdbdata), new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            else
            {
                domain.LoadAssembly(new MemoryStream(dlldata));
            }
            Debug.Log($"LoadAssemblyRemoteFinish: {dllurl} : {pdburl}");
        }

        #endregion Instance
    }
}
