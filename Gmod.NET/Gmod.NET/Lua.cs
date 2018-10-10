/* Lua is a singleton class, which provides a way to interact with Garry's Mod Lua api partially described here:
 * https://wiki.garrysmod.com/page/C_Lua:_Functions
 * 
 * To call Garry's Mod lua api use property Lua.Api. Generally, methods in "GarrysMod/Lua/Interface.h" and in managed Lua class
 * are named accordingly. For example, managed counterpart of LUA->PushString(char*) is Lua.Api.PushString(string).
 * 
 * Keep in mind that that Garry's Mod's Lua engine uses generalized stack and therefore not thread safe. It means you should use
 * lock(Lua.Api) {...} statement wherever you want to perform strict sequence of Lua api calls, which is almost always the case.
 * 
 * Authors: Gleb Krasilich
 * 
 * 2018*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GmodNET
{
    /// <summary>
    /// Singleton to make calls to Lua engine
    /// </summary>
    sealed class Lua
    {
        //Constructor
        private Lua()
        {

        }

        //Single instance of the Lua class
        private static readonly Lua API;

        /// <summary>
        /// Delegate to represent C functions, returned from Lua stack
        /// </summary>
        /// <param name="ptr">Pointer to the "lua state". Should be provided by Lua specification.</param>
        /// <returns>Number of returns of the function</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int IntPtrCFunc(IntPtr ptr);

        //Internal list of pushed functions
        static List<IntPtrCFunc> ListOfPushedDelegates;

        //Static constructor
        static Lua()
        {
            API = new Lua();
            ListOfPushedDelegates = new List<IntPtrCFunc>();
        }

        /// <summary>
        /// Delegate which is designed to push C like functions to Lua engine
        /// </summary>
        /// <returns>Number of values function pushed on stack as results</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int CFunc();


        /// <summary>
        /// Instance to make calls to Lua api.
        /// </summary>
        public static Lua Api
        {
            get
            {
                return API;
            }
        }

        /// <summary>
        /// This enum consists of Global, Environment and Registry tables of Garry's Mod Lua engine
        /// </summary>
        public enum SpecialTables
        {
            Global = 0,
            Environment = 1,
            Registry = 2
        }

        //Managed interface of the LuaPushSpecial from native part
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPushSpecial(int i);

        /// <summary>
        /// Pushs special table on Lua stack
        /// </summary>
        /// <param name="specialTable">One of the three types of special tables</param>
        public void PushSpecial(SpecialTables specialTable)
        {
            IntPushSpecial((int)specialTable);
        }

        //Bridge to LuaPush
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPush(int StackPos);

        /// <summary>
        /// Pushs a value form the StackPos of the stack on top
        /// </summary>
        /// <param name="StackPos">Position in the stack</param>
        public void Push(int StackPos)
        {
            IntPush(StackPos);
        }

        //Bridge to LuaPushString
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPushString(string str);

        /// <summary>
        /// Pushs a string on the stack
        /// </summary>
        /// <param name="str">String to push</param>
        public void PushString(string str)
        {
            IntPushString(str);
        }

        //Bridge to LuaPushNumber
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPushNumber(double d);

        /// <summary>
        /// Pushs double number on stack
        /// </summary>
        /// <param name="d">Number to push</param>
        public void PushNumber(double d)
        {
            IntPushNumber(d);
        }

        //Bridge to LuaPushBool
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPushBool(bool b);


        /// <summary>
        /// Pushs bool on the stack
        /// </summary>
        /// <param name="b">Boolean to push</param>
        public void PushBool(bool b)
        {
            IntPushBool(b);
        }

        //Bridge for LuaGetField
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntGetField(int i, string s);

        /// <summary>
        /// Pushs table[FieldName] on stack.
        /// </summary>
        /// <param name="StackPos">Position of the table in the stack</param>
        /// <param name="FieldName">Field name in the table</param>
        public void GetField(int StackPos, string FieldName)
        {
            IntGetField(StackPos, FieldName);
        }

        //Bridge to LuaCall
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntCall(int num_of_args, int num_of_returns);

        /// <summary>
        /// Calls a function on the stack
        /// </summary>
        /// <param name="num_of_args">Number of arguments of function</param>
        /// <param name="num_of_returns">Number of values returned by function</param>
        public void Call(int num_of_args, int num_of_returns)
        {
            IntCall(num_of_args, num_of_returns);
        }

        //Bridge to the LuaPCall
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static int IntPCall(int args, int results, int error_func);

        /// <summary>
        /// Safe version of Lua.Call function. Call a function with a given number of argumnts,
        /// results and given error handler
        /// </summary>
        /// <param name="num_of_args">Number of arguments for the function</param>
        /// <param name="num_of_results">Number of values returned by function</param>
        /// <param name="error_func">Index of the error handle function in the stack (leave 0 to not catch function)</param>
        /// <returns>Error code</returns>
        public int PCall(int num_of_args, int num_of_results, int error_func)
        {
            return IntPCall(num_of_args, num_of_results, error_func);
        }

        //Method which Pops given number of elements from the stack
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPop(int n);

        /// <summary>
        /// Pops one element from the stack
        /// </summary>
        public void Pop()
        {
            IntPop(1);
        }

        /// <summary>
        /// Pops n elements from the stack
        /// </summary>
        /// <param name="num_to_pop">Number of elements to pop</param>
        public void Pop(int num_to_pop)
        {
            IntPop(num_to_pop);
        }

        //Method to get double from the stack in given position
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static double IntGetNumber(int stack_pos);

        /// <summary>
        /// Returns double number form position -1 in the stack
        /// </summary>
        /// <returns>Number from the stack</returns>
        public double GetNumber()
        {
            return IntGetNumber(-1);
        }

        /// <summary>
        /// Returns double number from the given position in the stack
        /// </summary>
        /// <param name="stack_pos">Position in the stack to extract number</param>
        /// <returns>Number from the stack</returns>
        public double GetNumber(int stack_pos)
        {
            return IntGetNumber(stack_pos);
        }

        //Get string from the given position in the stack
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static string IntGetString(int stack_pos);

        /// <summary>
        /// Returns string from the first position in the stack
        /// </summary>
        /// <returns>String from the stack</returns>
        public string GetString()
        {
            return IntGetString(-1);
        }

        /// <summary>
        /// Returns string from the given position in the stack
        /// </summary>
        /// <param name="position_in_stack">Position in the stack</param>
        /// <returns>String from the stack</returns>
        public string GetString(int position_in_stack)
        {
            return IntGetString(position_in_stack);
        }

        //Returns bool form the stack in given pos
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static bool IntGetBool(int pos_in_stack);

        /// <summary>
        /// Returns a bool value from the -1 position in the stack
        /// </summary>
        /// <returns>Bool value in the stack</returns>
        public bool GetBool()
        {
            return IntGetBool(-1);
        }

        /// <summary>
        /// Returns a bool value from the given position in the stack
        /// </summary>
        /// <param name="pos_in_stack">Position in the stack</param>
        /// <returns>Bool value from the stack</returns>
        public bool GetBool(int pos_in_stack)
        {
            return IntGetBool(pos_in_stack);
        }

        //Bridge to LuaPushCFunction
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntPushCFunction(IntPtr d);

        /// <summary>
        /// Push a function with delegate func on Lua stack
        /// </summary>
        /// <param name="func">Delegate of the function to push on stack</param>
        public void PushCFunction(CFunc func)
        {
            //Init function to be wrapped by delegate
            int PassFunc(IntPtr ptr)
            {
                return func();
            }
            //Init delegate of this function
            IntPtrCFunc del_to_pass = new IntPtrCFunc(PassFunc);
            //Save the reference to the delegate
            ListOfPushedDelegates.Add(del_to_pass);
            //Get pointer on delegate
            IntPtr pointer = Marshal.GetFunctionPointerForDelegate<IntPtrCFunc>(del_to_pass);
            //Call bridge
            IntPushCFunction(pointer);
        }

        //Bridge to LuaGetCFunction
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static IntPtr IntGetCFunction(int pos);

        /// <summary>
        /// Function to get C function pointer from the Lua stack
        /// </summary>
        /// <param name="stack_pos">Position of the C function on the stack</param>
        /// <returns>Delegate on C function</returns>
        public IntPtr GetCFunction(int stack_pos = -1)
        {
            //Get function pointer
            IntPtr func_pointer = IntGetCFunction(stack_pos);
            //Returned delegate
            return func_pointer; ;
        }

        //Bridge to LuaSetField
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        extern static void IntSetField(int pos, string key);

        /// <summary>
        /// Set a field t[k] = v, where t is a table at stack_pos, k is a key and v is a value on top of the lua stack
        /// </summary>
        /// <param name="stack_pos">Position of the table to set new field on lua stack</param>
        /// <param name="key">Key of the value to set in the table</param>
        public void SetField(int stack_pos, string key)
        {
            IntSetField(stack_pos, key);
        }
    }
}
