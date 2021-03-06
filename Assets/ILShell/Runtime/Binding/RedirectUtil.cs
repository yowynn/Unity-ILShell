using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Utils;
using ILRuntime.Reflection;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace ILShell.Runtime.Binding
{
    public static class RedirectUtil
    {
        public delegate object RedirectHandler(object instance);

        public delegate void RedirectHandlerNoReturn(object instance);

        public delegate object RedirectHandlerWithArgs(object instance, params object[] args);

        public delegate void RedirectHandlerNoReturnWithArgs(object instance, params object[] args);

        public delegate object RedirectHandlerGeneric(object instance, IType T);

        public delegate void RedirectHandlerNoReturnGeneric(object instance, IType T);

        public delegate object RedirectHandlerWithArgsGeneric(object instance, IType T, params object[] args);

        public delegate void RedirectHandlerNoReturnWithArgsGeneric(object instance, IType T, params object[] args);

        public delegate object RedirectHandlerFull(object instance, AppDomain domain, IType[] Ts, params object[] args);

        public delegate void RedirectHandlerFullNoReturn(object instance, AppDomain domain, IType[] Ts, params object[] args);

        public const BindingFlags BindingFlag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        public const string Constructor = "#ctor";

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandler handler)
        {
            var methodInfo = GetMethod(type, methodName, null, 0);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var ret = handler(instance);
                esp = ILIntepreter.PushObject(esp, mStack, ret);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerNoReturn handler)
        {
            var methodInfo = GetMethod(type, methodName, null, 0);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                handler(instance);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerWithArgs handler, params Type[] paramTypes)
        {
            paramTypes = paramTypes ?? new Type[] { };
            var paramLength = paramTypes.Length;
            var methodInfo = GetMethod(type, methodName, paramTypes, 0);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                var args = new object[paramLength];
                for (int i = paramLength - 1; i >= 0; --i)
                {
                    var paramType = paramTypes[i];
                    var arg = paramType.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    args[i] = WrapObject(appdomain, arg);
                    if (!paramType.IsPrimitive)
                        intp.Free(esp);
                }
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var ret = handler(instance, args);
                esp = ILIntepreter.PushObject(esp, mStack, ret);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerNoReturnWithArgs handler, params Type[] paramTypes)
        {
            paramTypes = paramTypes ?? new Type[] { };
            var paramLength = paramTypes.Length;
            var methodInfo = GetMethod(type, methodName, paramTypes, 0);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                var args = new object[paramLength];
                for (int i = paramLength - 1; i >= 0; --i)
                {
                    var paramType = paramTypes[i];
                    var arg = paramType.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    args[i] = WrapObject(appdomain, arg);
                    if (!paramType.IsPrimitive)
                        intp.Free(esp);
                }
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                handler(instance, args);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerGeneric handler)
        {
            var methodInfo = GetMethod(type, methodName, null, 1);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var genericType = method.GenericArguments?[0];
                var ret = handler(instance, genericType);
                esp = ILIntepreter.PushObject(esp, mStack, ret);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerNoReturnGeneric handler)
        {
            var methodInfo = GetMethod(type, methodName, null, 1);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var genericType = method.GenericArguments?[0];
                handler(instance, genericType);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerWithArgsGeneric handler, params Type[] paramTypes)
        {
            paramTypes = paramTypes ?? new Type[] { };
            var paramLength = paramTypes.Length;
            var methodInfo = GetMethod(type, methodName, paramTypes, 1);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                var args = new object[paramLength];
                for (int i = paramLength - 1; i >= 0; --i)
                {
                    var paramType = paramTypes[i];
                    var arg = paramType.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    args[i] = WrapObject(appdomain, arg);
                    if (!paramType.IsPrimitive)
                        intp.Free(esp);
                }
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var genericType = method.GenericArguments?[0];
                var ret = handler(instance, genericType, args);
                esp = ILIntepreter.PushObject(esp, mStack, ret);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerNoReturnWithArgsGeneric handler, params Type[] paramTypes)
        {
            paramTypes = paramTypes ?? new Type[] { };
            var paramLength = paramTypes.Length;
            var methodInfo = GetMethod(type, methodName, paramTypes, 1);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                var args = new object[paramLength];
                for (int i = paramLength - 1; i >= 0; --i)
                {
                    var paramType = paramTypes[i];
                    var arg = paramType.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    args[i] = WrapObject(appdomain, arg);
                    if (!paramType.IsPrimitive)
                        intp.Free(esp);
                }
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var genericType = method.GenericArguments?[0];
                handler(instance, genericType, args);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerFull handler, params Type[] paramTypes)
        {
            paramTypes = paramTypes ?? new Type[] { };
            var paramLength = paramTypes.Length;
            var methodInfo = GetMethod(type, methodName, paramTypes);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                var args = new object[paramLength];
                for (int i = paramLength - 1; i >= 0; --i)
                {
                    var paramType = paramTypes[i];
                    var arg = paramType.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    args[i] = WrapObject(appdomain, arg);
                    if (!paramType.IsPrimitive)
                        intp.Free(esp);
                }
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var genericTypes = method.GenericArguments;
                var ret = handler(instance, appdomain, genericTypes, args);
                esp = ILIntepreter.PushObject(esp, mStack, ret);
                return esp;
            });
        }

        public static unsafe void RedirectMethod(AppDomain domain, Type type, string methodName, RedirectHandlerFullNoReturn handler, params Type[] paramTypes)
        {
            paramTypes = paramTypes ?? new Type[] { };
            var paramLength = paramTypes.Length;
            var methodInfo = GetMethod(type, methodName, paramTypes);
            domain.RegisterCLRMethodRedirection(methodInfo, delegate (ILIntepreter intp, StackObject* esp, IList<object> mStack, CLRMethod method, bool isNewObj)
            {
                var appdomain = intp.AppDomain;
                var args = new object[paramLength];
                for (int i = paramLength - 1; i >= 0; --i)
                {
                    var paramType = paramTypes[i];
                    var arg = paramType.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    args[i] = WrapObject(appdomain, arg);
                    if (!paramType.IsPrimitive)
                        intp.Free(esp);
                }
                object instance = null;
                if (method.HasThis)
                {
                    instance = type.CheckCLRTypes(StackObject.ToObject(--esp, appdomain, mStack));
                    intp.Free(esp);
                }
                var genericTypes = method.GenericArguments;
                handler(instance, appdomain, genericTypes, args);
                return esp;
            });
        }

        private static object WrapObject(AppDomain domain, object obj)
        {
            if (obj is Type c)
            {
                IType type;
                if (c is ILRuntimeWrapperType)
                {
                    type = ((ILRuntimeWrapperType)c).CLRType;
                }
                else if (c is ILRuntimeType)
                {
                    type = ((ILRuntimeType)c).ILType;
                }
                else
                    type = domain.GetType(c);
                return type;
            }
            return obj;
        }

        private static MethodBase GetMethod(Type type, string name, Type[] types = null, int genericParameterCount = -1, BindingFlags bindingAttr = BindingFlag)
        {
            types = types ?? new Type[] { };
            MethodBase method;
            if (name == Constructor)
            {
                method = type.GetConstructor(bindingAttr, null, types, null);
            }
            else
            {
#if UNITY_2021_2_OR_NEWER
                if (genericParameterCount < 0)
                    method = type.GetMethod(name, bindingAttr, null, types, null);
                else
                    method = type.GetMethod(name, genericParameterCount, bindingAttr, null, types, null);
#else
                method = type.GetMethod(name, bindingAttr, null, types, null);
                if (method != null)
                    if (genericParameterCount >= 0 && method.GetGenericArguments().Length != genericParameterCount)
                        foreach (var m in type.GetMethods(bindingAttr))
                            if (m.Name == name && m.GetGenericArguments().Length == genericParameterCount && m.GetParameters().Select(p => p.ParameterType).SequenceEqual(types))
                            {
                                method = m;
                                break;
                            }

#endif
            }
            if (method == null)
                throw new Exception("Cannot Find Method!!!");
            return method;
        }
    }
}
