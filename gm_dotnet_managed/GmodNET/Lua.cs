﻿using System;
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
            return top(ptr);
        }

        public void Push(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            push(ptr, iStackPos);
        }

        public void Pop(int IAmt = 1)
        {
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
            create_table(ptr);
        }

        public void SetMetaTable(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            set_metatable(ptr, iStackPos);
        }

        public bool GetMetaTable(int iStackPos)
        {
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
            call(ptr, iArgs, iResults);
        }

        public int PCall(int IArgs, int IResults, int ErrorFunc)
        {
            return pcall(ptr, IArgs, IResults, ErrorFunc);
        }

        public bool Equal(int iA, int iB)
        {
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
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            insert(ptr, iStackPos);
        }

        public void Remove(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            remove(ptr, iStackPos);
        }

        public int Next(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return next(ptr, iStackPos);
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public void ThrowError(in string error_message)
        {
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
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            check_type(ptr, iStackPos, IType);
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public void ArgError(int iArgNum, in string error_message)
        {
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
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_number(ptr, iStackPos);
        }

        public bool GetBool(int iStackPos)
        {
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
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_c_function(ptr, iStackPos);
        }

        public void PushNil()
        {
            push_nil(ptr);
        }

        public void PushString(string str)
        {
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
            push_number(ptr, val);
        }

        public void PushBool(bool val)
        {
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
            if(native_func_ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException("native_func_ptr", "Parameter can't be nullptr.");
            }

            push_c_function(ptr, native_func_ptr);
        }

        public unsafe void PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int> function_pointer)
        {
            IntPtr int_ptr = (IntPtr)function_pointer;

            if (int_ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException("function_pointer", "Parameter can't be nullptr.");
            }

            push_c_function(ptr, int_ptr);
        }

        public void PushCClosure(IntPtr native_func_ptr, int iVars)
        {
            push_c_closure(ptr, native_func_ptr, iVars);
        }

        public int ReferenceCreate()
        {
            return reference_create(ptr);
        }

        public void ReferenceFree(int reference)
        {
            reference_free(ptr, reference);
        }

        public void ReferencePush(int reference)
        {
            reference_push(ptr, reference);
        }

        public void PushSpecial(SPECIAL_TABLES table)
        {
            push_special(ptr, (int)table);
        }

        public bool IsType(int iStackPos, int iType)
        {
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
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_type(ptr, iStackPos);
        }

        public string GetTypeName(int iType)
        {
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
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return obj_len(ptr, iStackPos);
        }

        public Vector3 GetAngle(int iStackPos)
        {
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
            push_angle(ptr, ang.X, ang.Y, ang.Z);
        }

        public void PushVector(Vector3 vec)
        {
            push_vector(ptr, vec.X, vec.Y, vec.Z);
        }

        public void SetState(IntPtr lua_state)
        {
            set_state(ptr, lua_state);
        }

        public int CreateMetaTable(string name)
        {
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
            push_user_type(ptr, data_pointer, iType);
        }

        public void SetUserType(int iStackPos, IntPtr data_pointer)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            set_user_type(ptr, iStackPos, data_pointer);
        }

        public IntPtr GetUserType(int iStackPos, int iType)
        { 
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            return get_user_type(ptr, iStackPos, iType);
        }

        public void GetTable(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            get_table(ptr, iStackPos);
        }

        public void SetTable(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            set_table(ptr, iStackPos);
        }

        public void RawGet(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            raw_get(ptr, iStackPos);
        }

        public void RawSet(int iStackPos)
        {
            if (iStackPos == 0)
            { 
                throw new ArgumentOutOfRangeException("iStackPos", "iStackPos can't be zero!");
            }
            raw_set(ptr, iStackPos);
        }

        public void PushUserData(IntPtr data)
        {
            push_user_data(ptr, data);
        }

        [Obsolete("BUG: LuaJIT exception mechanism is incompatible with CoreCLR.", true)]
        public string CheckString(int iStackPos)
        {
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
