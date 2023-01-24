using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using GmodNET.API;
using System.Runtime.InteropServices;
using static GmodNET.LuaInterop;
using System.Runtime.CompilerServices;

namespace GmodNET
{
    internal class Lua : ILua
    {
        IntPtr ptr;

        internal Lua(IntPtr ptr)
        {
            this.ptr = ptr;
        }

        public int Top()
        {
            if (top is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(top));
            }

            return top(ptr);
        }

        public void Push(int iStackPos)
        {
            if (push is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            push(ptr, iStackPos);
        }

        public void Pop(int IAmt = 1)
        {
            if (pop is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(pop));
            }

            if (IAmt < 0)
            { 
                throw new ArgumentOutOfRangeException("iAmt", "Can't pop negative number of items from the stack");
            }
            if (IAmt == 0)
            {
                return;
            }

            pop(ptr, IAmt);
        }

        public void GetField(int iStackPos, string key)
        {
            if (get_field is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_field));
            }

            if (iStackPos == 0)
            { 
               throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            byte[] buff = Encoding.UTF8.GetBytes(key);
            unsafe
            {
                fixed(byte * tmp_ptr = &buff[0])
                {
                    get_field(ptr, iStackPos, (IntPtr)tmp_ptr);
                }
            }
        }

        public void SetField(int iStackPos, string key)
        {
            if (set_field is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(set_field));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            byte[] buff = Encoding.UTF8.GetBytes(key);
            unsafe
            {
                fixed(byte * tmp_ptr = &buff[0])
                {
                    set_field(ptr, iStackPos, (IntPtr)tmp_ptr);
                }
            }
        }

        public void CreateTable()
        {
            if (create_table is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(create_table));
            }

            create_table(ptr);
        }

        public void SetMetaTable(int iStackPos)
        {
            if (set_metatable is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(set_metatable));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            set_metatable(ptr, iStackPos);
        }

        public bool GetMetaTable(int iStackPos)
        {
            if (get_metatable is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_metatable));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            int tmp = get_metatable(ptr, iStackPos);

            if(tmp == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [Obsolete("Unsafe. Use Lua.PCall instead.", false)]
        public void Call(int iArgs, int iResults)
        {
            if (call is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(call));
            }

            call(ptr, iArgs, iResults);
        }

        public int PCall(int IArgs, int IResults, int ErrorFunc)
        {
            if (pcall is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(pcall));
            }

            return pcall(ptr, IArgs, IResults, ErrorFunc);
        }

        public bool Equal(int iA, int iB)
        {
            if (equal is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(equal));
            }

            if(iA == 0 || iB == 0)
            {
                throw new ArgumentException("Neither iA or iB can't be 0");
            }
            int tmp = equal(ptr, iA, iB);
            
            if(tmp == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool RawEqual(int iA, int iB)
        {
            if (raw_equal is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(raw_equal));
            }

            if(iA == 0 || iB == 0)
            {
                throw new ArgumentException("Neither iA or iB can't be 0");
            }
            int tmp = raw_equal(ptr, iA, iB);

            if(tmp == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Insert(int iStackPos)
        {
            if (insert is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(insert));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            insert(ptr, iStackPos);
        }

        public void Remove(int iStackPos)
        {
            if (remove is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(remove));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            remove(ptr, iStackPos);
        }

        public int Next(int iStackPos)
        {
            if (next is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(next));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return next(ptr, iStackPos);
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public void ThrowError(in string error_message)
        {
            if (throw_error is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(throw_error));
            }

            byte[] buff = Encoding.UTF8.GetBytes(error_message + "\0");
            unsafe
            {
                fixed(byte * tmp_ptr = &buff[0])
                {
                    throw_error(ptr, (IntPtr)tmp_ptr);
                }
            }
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public void CheckType(int iStackPos, int IType)
        {
            if (check_type is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(check_type));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            check_type(ptr, iStackPos, IType);
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public void ArgError(int iArgNum, in string error_message)
        {
            if (arg_error is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(arg_error));
            }

            byte[] buff = Encoding.UTF8.GetBytes(error_message);
            unsafe
            {
                fixed(byte * tmp_ptr = &buff[0])
                {
                    arg_error(ptr, iArgNum, (IntPtr)tmp_ptr);
                }
            }
        }

        public string GetString(int iStackPos)
        {
            if (get_string is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_string));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            uint len = 0;
            unsafe
            {
                uint * tmp_ptr = &len;
                IntPtr c_str = get_string(ptr, iStackPos, (IntPtr)tmp_ptr);
                if(c_str == IntPtr.Zero)
                {
                    return string.Empty;
                }
                return Encoding.UTF8.GetString((byte*)c_str, (int)len);
            }
        }

        public double GetNumber(int iStackPos)
        {
            if (get_number is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_number));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_number(ptr, iStackPos);
        }

        public bool GetBool(int iStackPos)
        {
            if (get_bool is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_bool));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            int tmp = get_bool(ptr, iStackPos);

            if(tmp == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IntPtr GetCFunction(int iStackPos)
        {
            if (get_c_function is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_c_function));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_c_function(ptr, iStackPos);
        }

        public void PushNil()
        {
            if (push_nil is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_nil));
            }

            push_nil(ptr);
        }

        public void PushString(string str)
        {
            if (push_string is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_string));
            }

            byte[] buff = Encoding.UTF8.GetBytes(str);

            unsafe
            {
                fixed(byte * tmp_ptr = &buff[0])
                {
                    push_string(ptr, (IntPtr)tmp_ptr, (uint)buff.Length);
                }
            }
        }

        public void PushNumber(double val)
        {
            if (push_number is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_number));
            }

            push_number(ptr, val);
        }

        public void PushBool(bool val)
        {
            if (push_bool is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_bool));
            }

            int tmp;

            if(val)
            {
                tmp = 1;
            }
            else
            {
                tmp = 0;
            }

            push_bool(ptr, tmp);
        }

        public unsafe void PushCFunction(IntPtr native_func_ptr)
        {
            if (push_c_function is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_c_function));
            }

            if(native_func_ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException("native_func_ptr", "Parameter can't be nullptr.");
            }

            push_c_function(ptr, native_func_ptr);
        }

        public unsafe void PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int> function_pointer)
        {
            if (push_c_function is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_c_function));
            }

            IntPtr int_ptr = (IntPtr)function_pointer;

            if (int_ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException("function_pointer", "Parameter can't be nullptr.");
            }

            push_c_function(ptr, int_ptr);
        }

        public void PushCClosure(IntPtr native_func_ptr, int iVars)
        {
            if (push_c_closure is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_c_closure));
            }

            push_c_closure(ptr, native_func_ptr, iVars);
        }

        public int ReferenceCreate()
        {
            if (reference_create is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(reference_create));
            }

            return reference_create(ptr);
        }

        public void ReferenceFree(int reference)
        {
            if (reference_free is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(reference_free));
            }

            reference_free(ptr, reference);
        }

        public void ReferencePush(int reference)
        {
            if (reference_push is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(reference_push));
            }

            reference_push(ptr, reference);
        }

        public void PushSpecial(SPECIAL_TABLES table)
        {
            if (push_special is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_special));
            }

            push_special(ptr, (int)table);
        }

        public bool IsType(int iStackPos, int iType)
        {
            if (is_type is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(is_type));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            int tmp = is_type(ptr, iStackPos, iType);

            if(tmp == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsType(int iStackPos, TYPES type)
        {
            return this.IsType(iStackPos, (int)type);
        }

        public int GetType(int iStackPos)
        {
            if (get_type is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_type));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_type(ptr, iStackPos);
        }

        public string GetTypeName(int iType)
        {
            if (get_type_name is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_type_name));
            }

            int len = 0;
            
            unsafe
            {
                int * len_ptr = &len;
                
                IntPtr c_str = get_type_name(ptr, iType, (IntPtr)len_ptr);

                return Encoding.UTF8.GetString((byte*)c_str, len);
            }
        }

        public string GetTypeName(TYPES type)
        {
            return this.GetTypeName((int)type);
        }

        public int ObjLen(int iStackPos)
        {
            if (obj_len is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(obj_len));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return obj_len(ptr, iStackPos);
        }

        public Vector3 GetAngle(int iStackPos)
        {
            if (get_angle is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_angle));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            Span<float> components = stackalloc float[3];
            unsafe
            {
                fixed(float * tmp_ptr = &components[0])
                {
                    get_angle(ptr, (IntPtr)tmp_ptr, iStackPos);
                }
            }

            return new Vector3(components[0], components[1], components[2]);
        }

        public Vector3 GetVector(int iStackPos)
        {
            if (get_vector is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_vector));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            Span<float> components = stackalloc float[3];
            unsafe
            {
                fixed(float * tmp_ptr = &components[0])
                {
                    get_vector(ptr, (IntPtr)tmp_ptr, iStackPos);
                }
            }

            return new Vector3(components[0], components[1], components[2]);
        }

        public void PushAngle(Vector3 ang)
        {
            if (push_angle is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_angle));
            }

            push_angle(ptr, ang.X, ang.Y, ang.Z);
        }

        public void PushVector(Vector3 vec)
        {
            if (push_vector is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_vector));
            }

            push_vector(ptr, vec.X, vec.Y, vec.Z);
        }

        public void SetState(IntPtr lua_state)
        {
            if (set_state is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(set_state));
            }

            set_state(ptr, lua_state);
        }

        public int CreateMetaTable(string name)
        {
            if (create_metatable is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(create_metatable));
            }

            byte[] buff = Encoding.UTF8.GetBytes(name);

            unsafe
            {
                fixed(byte * tmp_ptr = &buff[0])
                {
                    return create_metatable(ptr, (IntPtr)tmp_ptr);
                }
            }
        }

        public bool PushMetaTable(int iType)
        {
            if (push_metatable is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_metatable));
            }

            int tmp = push_metatable(ptr, iType);

            if(tmp == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool PushMetaTable(TYPES type)
        {
            return this.PushMetaTable((int)type);
        }

        public void PushUserType(IntPtr data_pointer, int iType)
        {
            if (push_user_type is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_user_type));
            }

            push_user_type(ptr, data_pointer, iType);
        }

        public void SetUserType(int iStackPos, IntPtr data_pointer)
        {
            if (set_user_type is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(set_user_type));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            set_user_type(ptr, iStackPos, data_pointer);
        }

        public IntPtr GetUserType(int iStackPos, int iType)
        {
            if (get_user_type is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_user_type));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_user_type(ptr, iStackPos, iType);
        }

        public void GetTable(int iStackPos)
        {
            if (get_table is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(get_table));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            get_table(ptr, iStackPos);
        }

        public void SetTable(int iStackPos)
        {
            if (set_table is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(set_table));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            set_table(ptr, iStackPos);
        }

        public void RawGet(int iStackPos)
        {
            if (raw_get is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(raw_get));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            raw_get(ptr, iStackPos);
        }

        public void RawSet(int iStackPos)
        {
            if (raw_set is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(raw_set));
            }

            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            raw_set(ptr, iStackPos);
        }

        public void PushUserData(IntPtr data)
        {
            if (push_user_data is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(push_user_data));
            }

            push_user_data(ptr, data);
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public string CheckString(int iStackPos)
        {
            if (check_string is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(check_string));
            }

            int str_len = 0;
            unsafe
            {
                int * str_len_ptr = &str_len;
                IntPtr c_str = check_string(ptr, iStackPos, (IntPtr)str_len_ptr);
                if (c_str == IntPtr.Zero)
                {
                    return string.Empty;
                }
                else
                { 
                    ReadOnlySpan<byte> c_str_wrapped = new ReadOnlySpan<byte>((void*)c_str, str_len);
                    return Encoding.UTF8.GetString(c_str_wrapped);
                }
            }
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public double CheckNumber(int iStackPos)
        {
            if (check_number is null)
            {
                throw new LuaInteropDelegateIsNullException(nameof(check_number));
            }

            return check_number(ptr, iStackPos);
        }

        public IntPtr GetInternalPointer()
        {
            return ptr;
        }

        public void MCall(int iArgs, int iResults)
        {
            int error_code = this.PCall(iArgs, iResults, 0);

            if(error_code != 0)
            {
                string error_message = this.GetString(-1);
                this.Pop(1);

                throw new GmodLuaException(error_code, error_message);
            }
        }

        public GCHandle PushManagedFunction(Func<ILua, int> function)
        {
            GCHandle delegate_handle = GCHandle.Alloc(function, GCHandleType.Normal);

            this.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            this.GetField(-1, ManagedFunctionMetaMethods.ManagedFunctionIdField);
            int managed_function_type_id = (int)this.GetNumber(-1);
            this.Pop(2);

            this.PushUserType((IntPtr)delegate_handle, managed_function_type_id);
            this.PushCClosure(ManagedFunctionMetaMethods.NativeDelegateExecutor, 1);

            return delegate_handle;
        }

        public GCHandle PushManagedClosure(Func<ILua, int> function, byte number_of_upvalues)
        {
            if(number_of_upvalues == byte.MaxValue)
            {
                throw new ArgumentOutOfRangeException("number_of_upvalues", "Number of upvalues must be less than 255");
            }

            GCHandle delegate_handle = GCHandle.Alloc(function, GCHandleType.Normal);

            this.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            this.GetField(-1, ManagedFunctionMetaMethods.ManagedFunctionIdField);
            int managed_function_type_id = (int)this.GetNumber(-1);
            this.Pop(2);

            this.PushUserType((IntPtr)delegate_handle, managed_function_type_id);
            this.Insert(-(number_of_upvalues + 1));
            this.PushCClosure(ManagedFunctionMetaMethods.NativeDelegateExecutor, number_of_upvalues + 1);

            return delegate_handle;
        }

        public void PushGlobalTable()
        {
            this.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        }
    }
}
