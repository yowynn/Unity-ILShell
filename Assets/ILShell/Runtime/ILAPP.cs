using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace ILShell
{
    public class ILAPP : MonoBehaviour
    {
        #region Static

        private static ILAPP currentInstance;

        private static AppDomain domainForEditorMode;

        public static ILAPP CurrentInstance
        {
            get
            {
                if (currentInstance != null)
                    return currentInstance;
                else
                {
                    throw new System.Exception("[ILAPP]Current APP not found");
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
#if UNITY_EDITOR
                    domainForEditorMode = domainForEditorMode ?? CreateEditorDomain();
                    return domainForEditorMode;
#endif
                    throw new System.Exception("[ILAPP]Current Domain not found");
                }
            }
        }

        private static AppDomain CreateEditorDomain()
        {
            throw new System.NotImplementedException();
        }

        #endregion Static

        #region Instance

        public AppDomain AppDomain { get; private set; }

        #endregion Instance
    }
}
