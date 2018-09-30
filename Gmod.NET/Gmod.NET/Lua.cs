using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

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
        private static readonly Lua API = new Lua();

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
    }
}
