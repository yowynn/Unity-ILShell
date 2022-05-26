using ILRuntime.CLR.TypeSystem;
using ILShell.Agent;
using ILShell.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ILShell.Editor.Util
{
    public static class ConvertAction
    {
        public static MonoBehaviourAgent FromMonoBehaviour(MonoBehaviour behaviour)
        {

            var typeName = behaviour.GetType().FullName;
            var type = ILAPP.CurrentDomain.GetType(typeName) as ILType;
            if (type != null)
            {
                var agent = behaviour.gameObject.AddComponent<MonoBehaviourAgent>();
                var instance = type.Instantiate();
                instance.CLRInstance = agent;
                agent.ILInstance = instance;
                agent.ILType = typeName;
                //ReadFromMonoBehaviour(agent, behaviour);
                return agent;
            }
            return null;
        }
    }
}
