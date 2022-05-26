using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;
using UnityEngine;

namespace ILShell.Runtime.Binding
{
    unsafe class UnityEngine_MonoBehaviour_Binding_Fixed
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            var type = typeof(MonoBehaviour);
            RedirectUtil.RedirectMethod(app, type, RedirectUtil.Constructor, o => o);
            RedirectUtil.RedirectMethod(app, typeof(MonoBehaviour), "CancelInvoke", o => ILAPP.Current.ILInvoker.CancelInvoke(o));
            RedirectUtil.RedirectMethod(app, typeof(MonoBehaviour), "CancelInvoke", (o, a) => ILAPP.Current.ILInvoker.CancelInvoke(o, (string)a[0]), typeof(string));
            RedirectUtil.RedirectMethod(app, typeof(MonoBehaviour), "Invoke", (o, a) => ILAPP.Current.ILInvoker.Invoke(o, (string)a[0], (float)a[1]), typeof(string), typeof(float));
            RedirectUtil.RedirectMethod(app, typeof(MonoBehaviour), "InvokeRepeating", (o, a) => ILAPP.Current.ILInvoker.InvokeRepeating(o, (string)a[0], (float)a[1], (float)a[2]), typeof(string), typeof(float), typeof(float));
            RedirectUtil.RedirectMethod(app, typeof(MonoBehaviour), "IsInvoking", (o, a) => ILAPP.Current.ILInvoker.IsInvoking(o, (string)a[0]), typeof(string));
        }
    }
}
