using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILShell.Agent;
using System;

namespace ILShell.Adapter
{
    public class MonoBehaviourAdapter : CrossBindingAdaptor
    {
        public override Type BaseCLRType => typeof(UnityEngine.MonoBehaviour);

        public override Type AdaptorType => typeof(MonoBehaviourAgent);

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            // ! you cannot new a MonoBehaviourAgent for it, because call MonoBehaviour's Constuctor is not allowed in Unity
            return null;
        }
    }
}
