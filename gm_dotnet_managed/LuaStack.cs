using System;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace Gmod.NET
{
    /// <summary>
    /// Static class which implements methods to interact with Garry's mod Lua engine stack. This is very low-level api and should 
    /// be used with caution. Keep in mind that any pure interactions with Lua stack are prone to race condition.
    /// </summary>
    public static class LuaStack
    {
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_special(int index);
        /// <summary>
        /// Push special table on stack
        /// </summary>
        /// <param name="table">Table to push</param>
        public static void PushSpecial(SpecialTables table)
        {
            push_special((int)table);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_number(double number);
        /// <summary>
        /// Push double number on the stack
        /// </summary>
        /// <param name="number">Number to push</param>
        public static void PushNumber(double number)
        {
            push_number(number);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_string(string str);
        /// <summary>
        /// Push string on stack. String will be encoded with UTF-8.
        /// </summary>
        /// <param name="str">String to push</param>
        public static void PushString(string str)
        {
            push_string(str);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_bool(byte b);
        /// <summary>
        /// Push bool value on stack
        /// </summary>
        /// <param name="b">Bool value to push</param>
        public static void PushBool(bool b)
        {
            if (!b)
            {
                push_bool(0x0);
            }
            else
            {
                push_bool(0x1);
            }
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void get_field(int pos, string key);
        /// <summary>
        /// Push iStackPos[Key] value on top of stack
        /// </summary>
        /// <param name="iStackPos">Position of the table on stack</param>
        /// <param name="Key">Key of the value in table</param>
        public static void GetField(int iStackPos, string Key)
        {
            if(iStackPos == 0)
            {
                throw new ArgumentException("iStackPos should be either positive or negative!");
            }
            get_field(iStackPos, Key);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void set_field(int pos, string key);
        /// <summary>
        /// Set the iStackPos[Key] value to the item on top of the stack
        /// </summary>
        /// <param name="iStackPos">Position of the table</param>
        /// <param name="Key">Key of the value in table</param>
        public static void SetField(int iStackPos, string Key)
        {
            if(iStackPos == 0)
            {
                throw new ArgumentException("Stack pos must be either positive or negative");
            }
            set_field(iStackPos, Key);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_function(IntPtr ptr);
        /// <summary>
        /// Push delegate on stack
        /// </summary>
        /// <param name="func">Delegate to push. You should prevent this delegate from garbage collection</param>
        public static void PushFunction(GarrysModFunc func)
        {
            IntPtr func_ptr = Marshal.GetFunctionPointerForDelegate<GarrysModFunc>(func);
            push_function(func_ptr);
        }
        /// <summary>
        /// Push function pointer on stack. USE WITH CAUTION. USE LuaStack.PushFunction INSTEAD.
        /// </summary>
        /// <param name="pointer">Pointer to push</param>
        public static void PushFunctionPointer(IntPtr pointer)
        {
            push_function(pointer);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void pop(int num);
        /// <summary>
        /// Pop values from the stack
        /// </summary>
        /// <param name="number">Number of values to pop</param>
        public static void Pop(int number = 1)
        {
            if (number <= 0)
            {
                throw new ArgumentException("You can pop only positive number of values from the stack!");
            }
            pop(number);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push(int pos);
        /// <summary>
        /// Push a copy of value at iStackPos to the top of the stack
        /// </summary>
        /// <param name="iStackPos">Position of value to copy</param>
        public static void Push(int iStackPos = -1)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos should be positive or negative!");
            }
            push(iStackPos);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void call(int num_of_args, int num_of_returns);
        /// <summary>
        /// Call a function from a stack
        /// </summary>
        /// <param name="Number_of_args">Number of arguments</param>
        /// <param name="Number_of_returns">Number of return values</param>
        public static void Call(int Number_of_args, int Number_of_returns)
        {
            if (Number_of_args < 0 || Number_of_returns < 0)
            {
                throw new ArgumentException("Negative arguments!");
            }
            call(Number_of_args, Number_of_returns);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static int pcall(int num_of_args, int num_of_returns, int error_func);
        /// <summary>
        /// Safe version of LuaStack.Call. Call function from the stack
        /// </summary>
        /// <param name="Number_of_args">Number of arguments</param>
        /// <param name="Number_of_returns">Number of return values</param>
        /// <param name="Error_func">Error function id (see lua documentation)</param>
        /// <returns>Error code</returns>
        public static int PCall(int Number_of_args, int Number_of_returns, int Error_func = 0)
        {
            if (Number_of_args < 0 || Number_of_returns < 0)
            {
                throw new ArgumentException("Negative arguments!");
            }

            return pcall(Number_of_args, Number_of_returns, Error_func);
        }
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static double get_number(int pos);
        /// <summary>
        /// Get number from the stack
        /// </summary>
        /// <param name="iStackPos">Position on stack to get number from</param>
        /// <returns>Number from stack (0 upon failure)</returns>
        public static double GetNumber(int iStackPos = -1)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be zero!");
            }
            return get_number(iStackPos);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static byte get_bool(int pos);
        /// <summary>
        /// Get bool from the stack
        /// </summary>
        /// <param name="iStackPos">Position on stack to get value from</param>
        /// <returns>Bool from the stack (false upon failure)</returns>
        public static bool GetBool(int iStackPos = -1)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be 0!");
            }
            byte tmp = get_bool(iStackPos);
            if (tmp == 0x0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        struct get_string_struct
        {
            public int length;
            public IntPtr ptr;
        }
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static get_string_struct get_string(int pos);
        /// <summary>
        /// Get string from stack as byte array.
        /// </summary>
        /// <param name="iStackPos">Position in stack to get value from</param>
        /// <returns>String as byte array. Returns zero length array upon failure</returns>
        public static byte[] GetStringBytes(int iStackPos = -1)
        {
            get_string_struct tmp = get_string(iStackPos);
            if(tmp.length == 0)
            {
                return new byte[0];
            }
            else
            {
                byte[] buffer = new byte[tmp.length];
                Marshal.Copy(tmp.ptr, buffer, 0, tmp.length);
                return buffer;
            }
        }
        /// <summary>
        /// Get string from stack
        /// </summary>
        /// <param name="iStackPos">Position in stack to get value from</param>
        /// <returns>String from the stack. Returns empty string upon failure</returns>
        public static string GetString(int iStackPos = -1)
        {
            return Encoding.UTF8.GetString(GetStringBytes(iStackPos));
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static IntPtr get_function(int pos);
        /// <summary>
        /// Get function pointer from the stack.
        /// </summary>
        /// <param name="iStackPos">Position in the stack to get value from</param>
        /// <returns>Function pointer which is specified as CFunc. Can be converted to Gmod.NET.GarrysModFunc.
        /// Return NULL upon failure</returns>
        public static IntPtr GetFunction(int iStackPos = -1)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be 0!");
            }
            return get_function(iStackPos);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_nil();
        /// <summary>
        /// Push nil value on the stack
        /// </summary>
        public static void PushNil()
        {
            push_nil();
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_vector(float x, float y, float z);
        /// <summary>
        /// Push vector on stack
        /// </summary>
        /// <param name="vector">Vector to push</param>
        public static void PushVector(in Vector3 vector)
        {
            push_vector(vector.X, vector.Y, vector.Z);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void push_angle(float pitch, float yaw, float roll);
        /// <summary>
        /// Push angle on stack
        /// </summary>
        /// <param name="angle">Angle to push</param>
        public static void PushAngle(in Vector3 angle)
        {
            push_angle(angle.X, angle.Y, angle.Z);
        }

        struct get_vector_struct
        {
            public float x;
            public float y;
            public float z;
        }
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static get_vector_struct get_vector(int pos);
        /// <summary>
        /// Get vector from the stack
        /// </summary>
        /// <param name="iStackPos">Position in stack to get value from</param>
        /// <returns>Vector from stack</returns>
        public static Vector3 GetVector(int iStackPos = -1)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be 0");
            }
            get_vector_struct tmp = get_vector(iStackPos);

            return new Vector3(tmp.x, tmp.y, tmp.z);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static get_vector_struct get_angle(int pos);
        /// <summary>
        /// Get angle (as Vector3) from the stack
        /// </summary>
        /// <param name="iStackPos">Position is the stack to get value from</param>
        /// <returns>Vector3 representation of angle from the stack</returns>
        public static Vector3 GetAngle(int iStackPos = -1)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be 0!");
            }
            get_vector_struct tmp = get_angle(iStackPos);

            return new Vector3(tmp.x, tmp.y, tmp.z);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void create_table();
        /// <summary>
        /// Create new table and push it on the stack
        /// </summary>
        public static void CreateTable()
        {
            create_table();
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static int create_meta_table(string name);
        /// <summary>
        /// Creates new meta table and pushes it on the stack
        /// </summary>
        /// <param name="TableName">Meta Table name</param>
        /// <returns>ID of the table</returns>
        public static int CreateMetaTable(string TableName)
        {
            return create_meta_table(TableName);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static int get_type(int pos);
        /// <summary>
        /// Get ID of the type from the stack
        /// </summary>
        /// <param name="iStackPos">Position in stack to check type</param>
        /// <returns>ID of the type</returns>
        public static int GetType(int iStackPos)
        {
            if (iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be 0!");
            }
            return get_type(iStackPos);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static get_string_struct get_type_name(int type_id);
        /// <summary>
        /// Get type name as byte array (C-string)
        /// </summary>
        /// <param name="Type_ID">ID of the type</param>
        /// <returns>Type name as byte array</returns>
        public static byte[] GetTypeNameBytes(int Type_ID)
        {
            get_string_struct tmp = get_type_name(Type_ID);
            if(tmp.length == 0)
            {
                return new byte[0];
            }
            else
            {
                byte[] buffer = new byte[tmp.length];
                Marshal.Copy(tmp.ptr, buffer, 0, tmp.length);
                return buffer;
            }
        }
        /// <summary>
        /// Get type name
        /// </summary>
        /// <param name="Type_ID">Type ID</param>
        /// <returns>Type name</returns>
        public static string GetTypeName(int Type_ID)
        {
            byte[] tmp = GetTypeNameBytes(Type_ID);
            return Encoding.UTF8.GetString(tmp);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static byte get_meta_table(int pos);
        /// <summary>
        /// Push Meta Table of the value in iStackPos on top of the stack
        /// </summary>
        /// <param name="iStackPos">Position of valie in stack to get meta table</param>
        /// <returns>False upon failure</returns>
        public static bool GetMetaTable(int iStackPos = -1)
        {
            if(iStackPos == 0)
            {
                throw new ArgumentException("iStackPos can't be 0!");
            }
            byte tmp = get_meta_table(iStackPos);
            if (tmp == 0x0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static byte push_meta_table(int type);
        /// <summary>
        /// Push meta table of the given type on top of the stack
        /// </summary>
        /// <param name="Type_ID">ID of the type to push meta table</param>
        /// <returns>Flase upon failure</returns>
        public static bool PushMetaTable(int Type_ID)
        {
            byte tmp = push_meta_table(Type_ID);
            if(tmp == 0x0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
