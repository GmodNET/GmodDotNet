using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;

namespace GmodNET
{
    internal static class RuntimeServices
    {
        internal static T CreateNativeCaller<T>(IntPtr native_pointer) where T : Delegate
        {
            MethodInfo invoke_info = typeof(T).GetMethod("Invoke")!; // T is constrained to Delegates only, so Invoke method is always present.
            var return_type = invoke_info.ReturnType;
            var parameters_types = invoke_info.GetParameters().Select(param => param.ParameterType).Prepend<Type>(typeof(object)).ToArray();

            var dynamic_method = new DynamicMethod(string.Empty, return_type, parameters_types, true);

            var il = dynamic_method.GetILGenerator();

            for(byte i = 1; i < parameters_types.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_S, i);
            }

            if(Environment.Is64BitProcess)
            {
                il.Emit(OpCodes.Ldc_I8, native_pointer.ToInt64());
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, native_pointer.ToInt32());
            }

            il.EmitCalli(OpCodes.Calli, CallingConvention.Cdecl, return_type, parameters_types.Skip(1).ToArray());

            il.Emit(OpCodes.Ret);

            return (T)dynamic_method.CreateDelegate(typeof(T), null);
        }
    }
}
