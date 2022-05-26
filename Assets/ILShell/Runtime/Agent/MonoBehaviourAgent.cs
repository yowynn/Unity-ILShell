using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILShell.Model;
using ILShell.Runtime;
using UnityEngine;

namespace ILShell.Agent
{
    public class MonoBehaviourAgent : MonoBehaviour, CrossBindingAdaptorType
    {
        private ILTypeInstance instance;
        private ILInvoker.TypeInvoker invoker;

        public string ILType;

        public ILData ILData;

        public ILTypeInstance ILInstance
        {
            get
            {
                if (instance == null)
                {
                    if (this.ILType == null)
                        return null;
                    var type = ILAPP.GetType(this.ILType) as ILType;
                    if (type != null)
                    {
                        instance = type.Instantiate();
                        instance.CLRInstance = this;
                        //MonoBehaviourAgentUtil.ILDeserialize(ILInstance, ILData);
                        invoker = ILAPP.Current.ILInvoker.GetTypeInvoker(this);
                    }
                    else
                        Debug.LogError($"[MonoBehaviourAgent]ILType not found: {ILType}");
                }
                return instance;
            }

            set
            {
                instance = value;
            }
        }

        public void Awake()
        {
            ILInstance = ILInstance;
            invoker?.Invoke(this, "Awake");
        }

        public void Start()
        {
            invoker?.Invoke(this, "Start");
        }

        public void OnEnable()
        {
            invoker?.Invoke(this, "OnEnable");
        }

        public void OnDisable()
        {
            invoker?.Invoke(this, "OnDisable");
        }
    }
}
