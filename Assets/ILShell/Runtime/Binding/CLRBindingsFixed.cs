using ILRuntime.Runtime.Generated;

namespace ILShell.Runtime.Binding
{
    public class CLRBindingsFixed
    {
        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_MonoBehaviour_Binding_Fixed.Register(app);

            // add default Initialize at last
            CLRBindings.Initialize(app);
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            CLRBindings.Shutdown(app);
        }
    }
}
