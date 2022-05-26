using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace ILShell.Editor.Util
{
    [InitializeOnLoad]
    public class ProjectConvertor
    {
        static ProjectConvertor()
        {
            ObjectFactory.componentWasAdded -= componentWasAdded;
            ObjectFactory.componentWasAdded += componentWasAdded;
        }


        private static void componentWasAdded(Component component)
        {
            if (component is MonoBehaviour behaviour)
                if (ConvertAction.FromMonoBehaviour(behaviour) != null)
                    EditorApplication.delayCall += () => Component.DestroyImmediate(behaviour);
        }
    }
}
