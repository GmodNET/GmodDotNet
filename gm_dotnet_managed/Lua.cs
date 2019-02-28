using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Gmod.NET
{
    public class Lua
    {
        static Lua engine;

        /// <summary>
        /// Object to interact with Garry's mod Lua api. Same as LuaStack, but singleton and thus can be locked for archiving 
        /// thread safety.
        /// </summary> 
        public static Lua Engine
        {
            get
            {
                return engine;
            }
        }

        private Lua()
        {

        }

        static Lua()
        {
            engine = new Lua();
        }

        /// <summary>
        /// Push special table on stack
        /// </summary>
        /// <param name="table">Table to push</param>
        public void PushSpecial(SpecialTables table)
        {
            LuaStack.PushSpecial(table);
        }

        /// <summary>
        /// Push double number on the stack
        /// </summary>
        /// <param name="number">Number to push</param>
        public void PushNumber(double number)
        {
            LuaStack.PushNumber(number);
        }

        /// <summary>
        /// Push string on stack. String will be encoded with UTF-8.
        /// </summary>
        /// <param name="str">String to push</param>
        public void PushString(string str)
        {
            LuaStack.PushString(str);
        }

        /// <summary>
        /// Push bool value on stack
        /// </summary>
        /// <param name="b">Bool value to push</param>
        public void PushBool(bool b)
        {
            LuaStack.PushBool(b);
        }

        /// <summary>
        /// Push iStackPos[Key] value on top of stack
        /// </summary>
        /// <param name="iStackPos">Position of the table on stack</param>
        /// <param name="Key">Key of the value in table</param>
        public void GetField(int iStackPos, string Key)
        {
            LuaStack.GetField(iStackPos, Key);
        }

        /// <summary>
        /// Set the iStackPos[Key] value to the item on top of the stack
        /// </summary>
        /// <param name="iStackPos">Position of the table</param>
        /// <param name="Key">Key of the value in table</param>
        public void SetField(int iStackPos, string Key)
        {
            LuaStack.SetField(iStackPos, Key);
        }

        /// <summary>
        /// Push delegate on stack
        /// </summary>
        /// <param name="func">Delegate to push. You should prevent this delegate from garbage collection</param>
        public void PushFunction(GarrysModFunc func)
        {
            LuaStack.PushFunction(func);
        }

        /// <summary>
        /// Push function pointer on stack. USE WITH CAUTION. USE LuaStack.PushFunction INSTEAD.
        /// </summary>
        /// <param name="pointer">Pointer to push</param>
        public void PushFunctionPointer(IntPtr pointer)
        {
            LuaStack.PushFunctionPointer(pointer);
        }

        /// <summary>
        /// Pop values from the stack
        /// </summary>
        /// <param name="number">Number of values to pop</param>
        public void Pop(int number = 1)
        {
            LuaStack.Pop(number);
        }

        /// <summary>
        /// Push a copy of value at iStackPos to the top of the stack
        /// </summary>
        /// <param name="iStackPos">Position of value to copy</param>
        public void Push(int iStackPos = -1)
        {
            LuaStack.Push(iStackPos);
        }

        /// <summary>
        /// Call a function from a stack
        /// </summary>
        /// <param name="Number_of_args">Number of arguments</param>
        /// <param name="Number_of_returns">Number of return values</param>
        public void Call(int Number_of_args, int Number_of_returns)
        {
            LuaStack.Call(Number_of_args, Number_of_returns);
        }

        /// <summary>
        /// Safe version of LuaStack.Call. Call function from the stack
        /// </summary>
        /// <param name="Number_of_args">Number of arguments</param>
        /// <param name="Number_of_returns">Number of return values</param>
        /// <param name="Error_func">Error function id (see lua documentation)</param>
        /// <returns>Error code</returns>
        public int PCall(int Number_of_args, int Number_of_returns, int Error_func = 0)
        {
            return LuaStack.PCall(Number_of_args, Number_of_returns, Error_func);
        }

        /// <summary>
        /// Get number from the stack
        /// </summary>
        /// <param name="iStackPos">Position on stack to get number from</param>
        /// <returns>Number from stack (0 upon failure)</returns>
        public double GetNumber(int iStackPos = -1)
        {
            return LuaStack.GetNumber(iStackPos);
        }

        /// <summary>
        /// Get bool from the stack
        /// </summary>
        /// <param name="iStackPos">Position on stack to get value from</param>
        /// <returns>Bool from the stack (false upon failure)</returns>
        public bool GetBool(int iStackPos = -1)
        {
            return LuaStack.GetBool(iStackPos);
        }

        /// <summary>
        /// Get string from stack as byte array.
        /// </summary>
        /// <param name="iStackPos">Position in stack to get value from</param>
        /// <returns>String as byte array. Returns zero length array upon failure</returns>
        public byte[] GetStringBytes(int iStackPos = -1)
        {
            return LuaStack.GetStringBytes(iStackPos);
        }

        /// <summary>
        /// Get string from stack
        /// </summary>
        /// <param name="iStackPos">Position in stack to get value from</param>
        /// <returns>String from the stack. Returns empty string upon failure</returns>
        public string GetString(int iStackPos = -1)
        {
            return LuaStack.GetString(iStackPos);
        }

        /// <summary>
        /// Get function pointer from the stack.
        /// </summary>
        /// <param name="iStackPos">Position in the stack to get value from</param>
        /// <returns>Function pointer which is specified as CFunc. Can be converted to Gmod.NET.GarrysModFunc.
        /// Return NULL upon failure</returns>
        public IntPtr GetFunction(int iStackPos = -1)
        {
            return LuaStack.GetFunction(iStackPos);
        }

        /// <summary>
        /// Push nil value on the stack
        /// </summary>
        public void PushNil()
        {
            LuaStack.PushNil();
        }

        /// <summary>
        /// Push vector on stack
        /// </summary>
        /// <param name="vector">Vector to push</param>
        public void PushVector(in Vector3 vector)
        {
            LuaStack.PushVector(vector);
        }

        /// <summary>
        /// Push angle on stack
        /// </summary>
        /// <param name="angle">Angle to push</param>
        public void PushAngle(in Vector3 angle)
        {
            LuaStack.PushAngle(angle);
        }

        /// <summary>
        /// Get vector from the stack
        /// </summary>
        /// <param name="iStackPos">Position in stack to get value from</param>
        /// <returns>Vector from stack</returns>
        public Vector3 GetVector(int iStackPos = -1)
        {
            return LuaStack.GetVector(iStackPos);
        }

        /// <summary>
        /// Get angle (as Vector3) from the stack
        /// </summary>
        /// <param name="iStackPos">Position is the stack to get value from</param>
        /// <returns>Vector3 representation of angle from the stack</returns>
        public Vector3 GetAngle(int iStackPos = -1)
        {
            return LuaStack.GetAngle(iStackPos);
        }

        /// <summary>
        /// Create new table and push it on the stack
        /// </summary>
        public void CreateTable()
        {
            LuaStack.CreateTable();
        }

        /// <summary>
        /// Creates new meta table and pushes it on the stack
        /// </summary>
        /// <param name="TableName">Meta Table name</param>
        /// <returns>ID of the table</returns>
        public int CreateMetaTable(string TableName)
        {
            return LuaStack.CreateMetaTable(TableName);
        }

        /// <summary>
        /// Get ID of the type from the stack
        /// </summary>
        /// <param name="iStackPos">Position in stack to check type</param>
        /// <returns>ID of the type</returns>
        public int GetType(int iStackPos)
        {
            return LuaStack.GetType(iStackPos);
        }

        /// <summary>
        /// Get type name as byte array (C-string)
        /// </summary>
        /// <param name="Type_ID">ID of the type</param>
        /// <returns>Type name as byte array</returns>
        public byte[] GetTypeNameBytes(int Type_ID)
        {
            return LuaStack.GetTypeNameBytes(Type_ID);
        }

        /// <summary>
        /// Get type name
        /// </summary>
        /// <param name="Type_ID">Type ID</param>
        /// <returns>Type name</returns>
        public string GetTypeName(int Type_ID)
        {
            return LuaStack.GetTypeName(Type_ID);
        }

        /// <summary>
        /// Push Meta Table of the value in iStackPos on top of the stack
        /// </summary>
        /// <param name="iStackPos">Position of valie in stack to get meta table</param>
        /// <returns>False upon failure</returns>
        public bool GetMetaTable(int iStackPos = -1)
        {
            return LuaStack.GetMetaTable(iStackPos);
        }

        /// <summary>
        /// Push meta table of the given type on top of the stack
        /// </summary>
        /// <param name="Type_ID">ID of the type to push meta table</param>
        /// <returns>Flase upon failure</returns>
        public bool PushMetaTable(int Type_ID)
        {
            return LuaStack.PushMetaTable(Type_ID);
        }
    }
}
