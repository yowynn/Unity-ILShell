using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ILShell.Runtime
{
    public class ILInvoker
    {
        public class InvokeItem
        {
            public float Time;
            public object Instance;
            public string MethodName;
            public float LoopTime = -1;
        }

        public class TypeInvoker
        {
            private struct MethodCache
            {
                public string Name;
                public IMethod Method;
            }

            private ILType Type;
            private ILRuntime.Runtime.Enviorment.AppDomain AppDomain;
            private Dictionary<string, MethodCache> Cache;

            public TypeInvoker(ILType type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");
                Type = type;
                AppDomain = type.AppDomain;
                Cache = new Dictionary<string, MethodCache>();
            }

            public object Invoke(CrossBindingAdaptorType agent, string methodName, params object[] args)
            {
                MethodCache cache;
                IMethod method;
                if (Cache.TryGetValue(methodName, out cache))
                {
                    method = cache.Method;
                }
                else
                {
                    method = Type.GetMethod(methodName, args?.Length ?? 0);
                    cache = new MethodCache();
                    cache.Name = methodName;
                    cache.Method = method;
                    Cache.Add(methodName, cache);
                }
                if (method == null)
                {
                    //Debug.LogError($"[ILInvoker]can not find method \"{methodName}\" in type \"{Type.Name}\"");
                    return null;
                }
                return AppDomain.Invoke(method, agent.ILInstance, args);
            }
        }

        private static Comparison<InvokeItem> defaultComparison = delegate (InvokeItem a, InvokeItem b)
        {
            if (a == b)
                return 0;
            else if (a.Time > b.Time)
                return 1;
            else if (a.Time < b.Time)
                return -1;
            else
            {
                var ha = a.GetHashCode();
                var hb = b.GetHashCode();
                if (ha > hb)
                    return 1;
                else if (ha < hb)
                    return -1;
                else
                    return 0;
            }
        };

        public static ILInvoker CreateInstance()
        {
            return CreateInstance(defaultComparison);
        }

        public static ILInvoker CreateInstance(Comparison<InvokeItem> comparison)
        {
            var ins = new ILInvoker();
            ins.currentTime = 0;
            if (comparison == null)
                ins.invokeItems = new SortedSet<InvokeItem>();
            else
                ins.invokeItems = new SortedSet<InvokeItem>(Comparer<InvokeItem>.Create(comparison));
            ins.lastInvoked = new List<InvokeItem>();
            ins.typeInvokerMap = new Dictionary<ILType, TypeInvoker>();
            ins.InvokeHandler = ins.DefaultInvokeHandler;
            return ins;
        }

        private float currentTime;
        private List<InvokeItem> lastInvoked;
        private SortedSet<InvokeItem> invokeItems;
        private Dictionary<ILType, TypeInvoker> typeInvokerMap;

        public Action<object, string> InvokeHandler { private get; set; }

        private void DefaultInvokeHandler(object instance, string mathodName)
        {
            var agent = instance as CrossBindingAdaptorType;
            if (agent == null)
                return;
            GetTypeInvoker(agent).Invoke(agent, mathodName);
        }

        private ILInvoker()
        {
        }

        public void Update()
        {
            var deltaTime = Time.deltaTime;
            Update(deltaTime);
        }

        public void Update(float deltaTime)
        {
            lastInvoked.Clear();
            currentTime += deltaTime;
            foreach (var invokeItem in invokeItems)
                if (invokeItem.Time <= currentTime)
                    lastInvoked.Add(invokeItem);
                else break;
            foreach (var invokeItem in lastInvoked)
            {
                if (!invokeItems.Remove(invokeItem))
                    Debug.LogError($"[ILInvoker]\"{invokeItem.MethodName}\" of {invokeItem.Instance} cannot be removed");
                if (invokeItem.LoopTime > 0)
                {
                    invokeItem.Time += invokeItem.LoopTime;
                    invokeItems.Add(invokeItem);
                }
                InvokeInternal(invokeItem.Instance, invokeItem.MethodName);
            }
        }

        private void InvokeInternal(object instance, string methodName)
        {
            if (instance == null) return;
            //Debug.Log($"[ILInvoker]\"{methodName}\" of {instance} is invoked");
            if (InvokeHandler != null)
                try
                {
                    InvokeHandler(instance, methodName);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            else
                Debug.LogWarning($"[ILInvoker]InvokeHandler not found");
        }

        public void CancelInvoke(object instance)
        {
            lastInvoked.Clear();
            foreach (var invokeItem in invokeItems)
                if (invokeItem.Instance == instance)
                    lastInvoked.Add(invokeItem);
            foreach (var invokeItem in lastInvoked)
                invokeItems.Remove(invokeItem);
        }

        public void CancelInvoke(object instance, string methodName)
        {
            lastInvoked.Clear();
            foreach (var invokeItem in invokeItems)
                if (invokeItem.Instance == instance && invokeItem.MethodName == methodName)
                    lastInvoked.Add(invokeItem);
            foreach (var invokeItem in lastInvoked)
                invokeItems.Remove(invokeItem);
        }

        public void Invoke(object instance, string methodName, float time = 0f)
        {
            invokeItems.Add(new InvokeItem
            {
                Instance = instance,
                MethodName = methodName,
                Time = currentTime + time,
            });
        }

        public void InvokeRepeating(object instance, string methodName, float time, float repeatRate = -1)
        {
            invokeItems.Add(new InvokeItem
            {
                Instance = instance,
                MethodName = methodName,
                Time = currentTime + time,
                LoopTime = repeatRate,
            });
        }

        public bool IsInvoking(object instance, string methodName)
        {
            foreach (var invokeItem in invokeItems)
                if (invokeItem.Instance == instance && invokeItem.MethodName == methodName)
                    return true;
            return false;
        }

        public TypeInvoker GetTypeInvoker(CrossBindingAdaptorType agent)
        {
            var type = agent?.ILInstance?.Type;
            if (type == null)
                return null;
            TypeInvoker invoker;
            if (typeInvokerMap.TryGetValue(type, out invoker))
                return invoker;
            invoker = new TypeInvoker(type);
            typeInvokerMap.Add(type, invoker);
            return invoker;
        }
    }
}
