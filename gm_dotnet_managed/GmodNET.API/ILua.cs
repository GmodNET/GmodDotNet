using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GmodNET.API
{
    /// <summary>
    /// A .NET interface to interact with Garry's Mod Lua engine.
    /// </summary>
    public interface ILua
    {
        /// <summary>
        /// Returns the number of elements in the current lua stack.
        /// </summary>
        /// <returns>The number of elements in the current lua stack.</returns>
        /// <remarks>
        /// See <c>lua_gettop</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        public int Top();

        /// <summary>
        /// Pushes a copy of the element at iStackPos to the top of the stack.
        /// </summary>
        /// <param name="iStackPos">Position of the element on the stack.</param>
        public void Push(int iStackPos);

        /// <summary>
        /// Pops iAmt elements from the top of the stack.
        /// </summary>
        /// <param name="IAmt">Number of elements to pop from the stack.</param>
        /// <remarks>
        /// See <c>lua_pop</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        public void Pop(int IAmt = 1);

        /// <summary>
        /// Pushes a value from the table at <paramref name="iStackPos"/> by a given string <paramref name="key"/>.
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack.</param>
        /// <param name="key">Key for the value in the table.</param>
        /// <remarks>
        /// You can use this function to get values from Lua tables. 
        /// In the example below we get a global Garry's Mod Lua function <c>print</c> (https://wiki.facepunch.com/gmod/Global.print) 
        /// from the global table <see cref="SPECIAL_TABLES.SPECIAL_GLOB"/> to print a message to the game console.
        /// 
        /// See <c>lua_getfield</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <example>
        /// <code>
        /// public int LuaFunc(ILua lua)
        /// {
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.GetField(-1, "print");
        ///     lua.PushString("Hello");
        ///     lua.MCall(1, 0);
        ///     lua.Pop(1);
        ///     
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ILua.SetField(int, in string)"/>
        public void GetField(int iStackPos, in string key);

        /// <summary>
        /// Does a table key-value assignment <c>t[k] = v</c>,
        /// where <c>t</c> is the table at <paramref name="iStackPos"/>,
        /// <c>k</c> is a string <paramref name="key"/>,
        /// and <c>v</c> is a value on top of the stack.
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack.</param>
        /// <param name="key">Key in the table to set value for.</param>
        /// <remarks>
        /// This method can be used to fill Lua tables with values or update them. 
        /// In the example below we will add a Lua funtion <c>SayHello</c> to the Lua global table <see cref="SPECIAL_TABLES.SPECIAL_GLOB"/>.
        /// This function then can be called from any Lua script as <c>SayHello()</c>.
        /// 
        /// See <c>lua_setfield</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <example>
        /// <code>
        /// public int LuaFunc(ILua lua)
        /// {
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.PushManagedFunction(lua =>
        ///     {
        ///         lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///         lua.GetField(-1, "print");
        ///         lua.PushString("Hello!");
        ///         lua.MCall(1, 0);
        ///         lua.Pop(1);
        /// 
        ///         return 0;
        ///     });
        ///     lua.SetField(-2, "SayHello");
        ///     lua.Pop(1); // As you can see we pop only one value from the stack (remaining global table) since the function itself was poped by SetField already
        /// 
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ILua.GetField(int, in string)"/>
        public void SetField(int iStackPos, in string key);

        /// <summary>
        /// Creates a new table and pushes it to the top of the stack.
        /// </summary>
        /// <remarks>
        /// See <c>lua_createtable</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        public void CreateTable();

        /// <summary>
        /// Sets the metatable for the element at <paramref name="iStackPos"/> to the table on the top of the stack. Pops the table from the top of the stack.
        /// </summary>
        /// <param name="iStackPos">Stack position of the element to set metatable for.</param>
        /// <remarks>
        /// See <c>lua_setmetatable</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// To learn more about Lua metatables see https://www.lua.org/pil/13.html
        /// </remarks>
        /// <seealso cref="ILua.GetMetaTable(int)"/>
        public void SetMetaTable(int iStackPos);

        /// <summary>
        /// Pushes the metatable of the element at <paramref name="iStackPos"/> to the top of the stack. Upon failure, returns false and does not push anything.
        /// </summary>
        /// <param name="iStackPos">Stack position of the element to get metatable for.</param>
        /// <returns><c>True</c> if metatable was pushed to the stack, <c>False</c> otherwise.</returns>
        /// <remarks>
        /// See <c>lua_getmetatable</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// To learn more about Lua metatables see https://www.lua.org/pil/13.html
        /// </remarks>
        /// <seealso cref="ILua.SetMetaTable(int)"/>
        public bool GetMetaTable(int iStackPos);

        /// <summary>
        /// Calls a function (or any other callable object) followed by its arguments from the stack. 
        /// Pops the function and arguments and push function’s return values onto the stack. 
        /// WARNING: this method is unsafe and should not be used in the most cases. 
        /// Use <see cref="ILua.MCall(int, int)"/> and <see cref="ILua.PCall(int, int, int)"/> instead.
        /// </summary>
        /// <param name="iArgs">Number of arguments to pass to the function.</param>
        /// <param name="iResults">Number of the function’s return values to push back onto the stack.</param>
        /// <remarks>
        /// <see cref="ILua.Call(int, int)"/> is the most basic and unsafe way to call a Lua function. 
        /// Unlike <see cref="ILua.MCall(int, int)"/> and <see cref="ILua.PCall(int, int, int)"/>, 
        /// <see cref="ILua.Call(int, int)"/> does not catch any errors or exceptions thrown by a callee. 
        /// Thus, any thrown error can cause a complete termination of the game or server process, especially on Linux and macOS platforms.
        /// 
        /// See <c>lua_call</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <seealso cref="ILua.MCall(int, int)"/>
        /// <seealso cref="ILua.PCall(int, int, int)"/>
        /// <example>
        /// The following example function prints a message to the game console by calling Garry’s Mod native function <c>print</c> with <see cref="ILua.Call(int, int)"/>.
        /// <code>
        /// static int HelloWorld(ILua lua)
        /// {
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.GetField(-1, "print"); // Push Lua function to the stack
        ///     lua.PushString("Hello World!"); // Push an argument for print function
        ///     lua.Call(1, 0); // Call print function with one argument and zero return values
        ///     lua.Pop(1); // ILua.Call has poped function and string from the stack, so there is only global table left
        /// 
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        [Obsolete("This method is unsafe. Use ILua.MCall or ILua.PCall instead.", false)]
        public void Call(int iArgs, int iResults);

        /// <summary>
        /// Calls a function (or any other callable object) followed by its arguments from the stack in the protected mode (will catch any exception).
        /// Pops the function and arguments from the stack.
        /// </summary>
        /// <remarks>
        /// If execution is successful, pushes function’s return values back to the stack.
        /// Otherwise, if exception was thrown while function call, pushes error message to the stack and returns exception’s code.
        /// If <paramref name="ErrorFunc"/> is 0, then the error message returned on the stack is exactly the original error message.
        /// Otherwise, <paramref name="ErrorFunc"/> is the stack index of an error handler function (must be a real index, pseudo-indexes are not supported).
        /// In case of runtime errors, 
        /// this handler function will be called with the error message and its return 
        /// value will be the message returned on the stack by <see cref="ILua.PCall(int, int, int)"/>.
        /// 
        /// See <c>lua_pcall</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="IArgs">Number of arguments to pass to the function.</param>
        /// <param name="IResults">Number of the function’s return values to push back onto the stack.</param>
        /// <param name="ErrorFunc">The stack index of an error handler function.</param>
        /// <returns><c>0</c> in case of success or one of the error codes (defined by Lua specification).</returns>
        /// <seealso cref="ILua.MCall(int, int)"/>
        /// <seealso cref="ILua.Call(int, int)"/>
        public int PCall(int IArgs, int IResults, int ErrorFunc);

        /// <summary>
        /// Checks if two objects on the stack are equal. Invokes types’ metamethods if applicable.
        /// </summary>
        /// <remarks>
        /// <see cref="ILua.Equal(int, int)"/> follows semantics of Lua <c>==</c> operator.
        /// In particular, if two objects being compered have the same type with <c>__eq</c> metamethod defined, 
        /// then such metamethod will be implicitly invoked to determine equality.
        /// 
        /// See <c>lua_equal</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// 
        /// See "Relational Metamethods" for more information about equality and <c>__eq</c> metamethod: https://www.lua.org/pil/13.2.html
        /// </remarks>
        /// <param name="iA">Stack index of the first object to compare.</param>
        /// <param name="iB">Stack index of the second object to compare.</param>
        /// <returns><c>True</c> if objects being compared are equal, <c>False</c> otherwise.</returns>
        /// <seealso cref="ILua.RawEqual(int, int)"/>
        public bool Equal(int iA, int iB);

        /// <summary>
        /// Checks if two objects on the stack are equal. Does not invoke metamethods.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="ILua.Equal(int, int)"/>, performs a “raw” comparison of two objects and does not relies on objects’ metamethods. 
        /// In particular, the method will return <c>True</c> for two tables if and only if they contain same key-value pairs, 
        /// any custom <c>__eq</c> metamethod will be ignored.
        /// 
        /// See <c>lua_rawequal</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iA">Stack index of the first object to compare.</param>
        /// <param name="iB">Stack index of the second object to compare.</param>
        /// <returns><c>True</c> if objects being compared are equal, <c>False</c> otherwise.</returns>
        /// <seealso cref="ILua.Equal(int, int)"/>
        public bool RawEqual(int iA, int iB);

        /// <summary>
        /// Moves an element at the top of the stack to the index passed as <paramref name="iStackPos"/> parameter.
        /// Any elements above (with greater positive index) <paramref name="iStackPos"/> are shifted upwards.
        /// </summary>
        /// <remarks>
        /// Cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
        /// 
        /// See <c>lua_insert</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">An index to insert an element from the top of the stack into.</param>
        public void Insert(int iStackPos);

        /// <summary>
        /// Removes an object at <paramref name="iStackPos"/> index from the stack.
        /// Any elements above (with greater positive index) <paramref name="iStackPos"/> are shifted downwards.
        /// </summary>
        /// <remarks>
        /// Cannot be called with a pseudo-index, because a pseudo-index is not an actual stack position.
        /// 
        /// See <c>lua_remove</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">Stack position of the object to remove.</param>
        public void Remove(int iStackPos);

        /// <summary>
        /// Pops a key from the top of the stack, and pushes a key-value pair from the table at the given index (the "next" pair after the given key). 
        /// If there are no more elements in the table, then lua_next returns <c>0</c> (and pushes nothing).
        /// </summary>
        /// <remarks>
        /// Allows you to iterate tables similar to Lua <c>pairs(...)</c>.
        /// If the value on the top of the stack is <c>nil</c>, then the first key-value pair will be pushed onto the stack.
        /// After successful execution, the key will have stack index <c>-2</c> and the value will have index <c>-1</c>.
        /// 
        /// See <c>lua_next</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">The stack index of the table to get a key-value from.</param>
        /// <returns><c>0</c> if there are no more key-value pairs to push, and non-zero integer otherwise.</returns>
        /// <example>
        /// The following example demonstrates usage of <see cref="ILua.Next(int)"/> to print server players’ nicknames to the game console.
        /// <code>
        /// public static int TableIter(ILua lua)
        /// {
        ///     List<![CDATA[<string>]]> players = new List<![CDATA[<string>]]>();
        ///         
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.GetField(-1, "player");
        ///     lua.GetField(-1, "GetAll");
        ///     lua.MCall(0, 1); // Will push a table of all players on server
        ///     lua.PushNil(); // Calls ILua.Next on nil value to start iterating over table
        ///     while(lua.Next(-2) != 0) // Iterating over players table
        ///     {
        ///         lua.GetField(-1, "Nick"); // Extracts a method which returns a player's nick
        ///         lua.Push(-2); // Pushes a copy of player object to pass as a param to Nick()
        ///         lua.MCall(1, 1);
        ///         players.Add(lua.GetString(-1));
        ///         lua.Pop(2); // Pops player object and nickname string from the stack, leaves corresponding key to continue iteration
        ///     }
        ///     lua.Pop(3);
        /// 
        ///     if(players.Count != 0)
        ///     {
        ///         Console.WriteLine("Players on server:");
        ///         foreach(string n in players)
        ///         {
        ///             Console.WriteLine(n);
        ///         }
        ///     }
        /// 
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        public int Next(int iStackPos);

        /// <summary>
        /// Returns a string at <paramref name="iStackPos"/>.
        /// If the value at <paramref name="iStackPos"/> is not a string, then the Lua engine will try to convert the object to string.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="String.Empty"/> upon failure.
        /// 
        /// See <c>lua_tolstring</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">The stack index of the object to get as string.</param>
        /// <returns>The string from the stack. <see cref="String.Empty"/> upon failure.</returns>
        public string GetString(int iStackPos);

        /// <summary>
        /// Returns the number at <paramref name="iStackPos"/>.
        /// If the value at <paramref name="iStackPos"/> is not a number, then the Lua engine will try to convert the object to number.
        /// </summary>
        /// <remarks>
        /// Returns <c>0.0</c> upon failure.
        /// 
        /// See <c>lua_tonumber</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">The stack position of the object to get as number.</param>
        /// <returns>The number from the stack. <c>0.0</c> upon failure.</returns>
        public double GetNumber(int iStackPos);

        /// <summary>
        /// Returns a boolean value from the <paramref name="iStackPos"/>.
        /// </summary>
        /// <remarks>
        /// Returns <c>True</c> for any value different from <c>False</c> and <c>nil</c>.
        /// If you want to accept only actual boolean values, 
        /// use <see cref="ILua.IsType(int, TYPES)"/> to test the value's type before calling <see cref="ILua.GetBool(int)"/>.
        /// Returns <c>False</c> upon failure.
        /// 
        /// See <c>lua_toboolean</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">The stack position of the object to get as bool.</param>
        /// <returns>A boolean value from the stack. <c>False</c> upon failure.</returns>
        public bool GetBool(int iStackPos);

        /// <summary>
        /// Returns a C Function pointer (as <see cref="IntPtr"/>) from the <paramref name="iStackPos"/>.
        /// </summary>
        /// <remarks>
        /// If the operation fails or the object at <paramref name="iStackPos"/> is not a C Function, then <see cref="IntPtr.Zero"/> will be returned.
        /// 
        /// See <c>lua_tocfunction</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// 
        /// See “C Functions” for more info on C functions in Lua: https://www.lua.org/pil/26.1.html
        /// </remarks>
        /// <param name="iStackPos">The stack position of the C Function.</param>
        /// <returns>C Function pointer from the stack.</returns>
        public IntPtr GetCFunction(int iStackPos);

        /// <summary>
        /// Pushes a <c>nil</c> value onto the stack.
        /// </summary>
        /// <remarks>
        /// See <c>lua_pushnil</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        public void PushNil();

        /// <summary>
        /// Pushes a given string to the Lua stack.
        /// </summary>
        /// <remarks>
        /// The string will be converted to UTF-8 encoded byte array. 
        /// 
        /// See <c>lua_pushstring</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="str">A string to push.</param>
        public void PushString(in string str);

        /// <summary>
        /// Pushes a given double-precision number to the Lua stack.
        /// </summary>
        /// <remarks>
        /// See <c>lua_pushnumber</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="val">A number to push.</param>
        public void PushNumber(double val);

        /// <summary>
        /// Pushes a given boolean value onto the stack.
        /// </summary>
        /// <remarks>
        /// See <c>lua_pushboolean</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="val">A bool value to push.</param>
        public void PushBool(bool val);

        /// <summary>
        /// Pushes a given C Function pointer (as <see cref="IntPtr"/>) onto the stack.
        /// </summary>
        /// <remarks>
        /// A native C function, whose pointer is being pushed, must be of signature <c>int Func(void* lua_state)</c>.
        /// 
        /// See “C Functions” for more info on C functions in Lua: https://www.lua.org/pil/26.1.html
        /// 
        /// See <c>lua_pushcfunction</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="native_func_ptr">A native C function pointer to push.</param>
        public unsafe void PushCFunction(IntPtr native_func_ptr);

        /// <summary>
        /// Pushes a given C Function pointer (as C# 9.0 function pointer) onto the stack.
        /// </summary>
        /// <remarks>
        /// A natively callable function, whose pointer is being pushed, must be ABI-compatible with a C function of signature <c>int Func(void* lua_state)</c>.
        /// 
        /// See “C Functions” for more info on C functions in Lua: https://www.lua.org/pil/26.1.html
        /// 
        /// See <c>lua_pushcfunction</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="function_pointer">A C# 9.0 function pointer to push.</param>
        public unsafe void PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int> function_pointer);

        /// <summary>
        /// Pushes a given C Function pointer (as <see cref="IntPtr"/>) onto the stack, 
        /// associates it with <paramref name="iVars"/> objects on top of the stack (such objects are called upvalues), 
        /// and creates a Lua function closure.
        /// </summary>
        /// <remarks>
        /// Pops upvalues from the stack.
        /// 
        /// See “Upvalues” for more information about Lua closures: https://www.lua.org/pil/27.3.3.html
        /// 
        /// See <c>lua_pushcclosure</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="native_func_ptr">A C function pointer to push onto the stack and create closure from.</param>
        /// <param name="iVars">A number of objects on top of the stack to associate with the closure as upvalues.</param>
        public void PushCClosure(IntPtr native_func_ptr, int iVars);

        /// <summary>
        /// Creates a unique integer identifier (reference) for the object on top of the stack and saves it.
        /// </summary>
        /// <remarks>
        /// Pops object from the stack.
        /// 
        /// Returned reference can be used to push saved object onto any Lua execution stack with <see cref="ILua.ReferencePush(int)"/>.
        /// If the saved object is no longer needed, the reference must be freed with <see cref="ILua.ReferenceFree(int)"/> to prevent memory leaks.
        /// 
        /// See <c>luaL_ref</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <returns>A unique integer identifier for saved Lua object.</returns>
        /// <seealso cref="ILua.ReferencePush(int)"/>
        /// <seealso cref="ILua.ReferenceFree(int)"/>
        public int ReferenceCreate();

        /// <summary>
        /// Frees a Lua reference, previously created by <see cref="ILua.ReferenceCreate"/>.
        /// </summary>
        /// <remarks>
        /// See <c>luaL_ref</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="reference">A reference to free.</param>
        /// <seealso cref="ILua.ReferenceCreate"/>
        /// <seealso cref="ILua.ReferencePush(int)"/>
        public void ReferenceFree(int reference);

        /// <summary>
        /// Pushes a previously saved (with <see cref="ILua.ReferenceCreate"/>) object by its reference onto the stack.
        /// </summary>
        /// <remarks>
        /// See <c>luaL_ref</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="reference">A reference to object to push onto the stack.</param>
        /// <seealso cref="ILua.ReferenceCreate"/>
        /// <seealso cref="ILua.ReferenceFree(int)"/>
        public void ReferencePush(int reference);

        /// <summary>
        /// Pushes a special Lua table (like <see cref="SPECIAL_TABLES.SPECIAL_GLOB"/>) onto the stack.
        /// </summary>
        /// <remarks>
        /// See <see cref="SPECIAL_TABLES"/> docs for more information about special tables.
        /// </remarks>
        /// <param name="table">A special table to push (enumerated by <see cref="SPECIAL_TABLES"/>).</param>
        public void PushSpecial(SPECIAL_TABLES table);

        /// <summary>
        /// Checks if the type id of the object at <paramref name="iStackPos"/> is equal to <paramref name="iType"/>.
        /// </summary>
        /// <remarks>
        /// See <c>lua_type</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the object whose type must be checked.</param>
        /// <param name="iType">A type id to compare with object’s one.</param>
        /// <returns><c>True</c> if object’s type id is equal to <paramref name="iType"/>, <c>False</c> otherwise.</returns>
        /// <seealso cref="ILua.GetType(int)"/>
        public bool IsType(int iStackPos, int iType);

        /// <summary>
        /// Checks if the type of the object at <paramref name="iStackPos"/> is equal to Lua or Garry’s Mod built-in one 
        /// (built-in types are enumerated by <see cref="TYPES"/>).
        /// </summary>
        /// <remarks>
        /// See <c>lua_type</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the object whose type must be checked.</param>
        /// <param name="type">One of Lua and Garry’s Mod built-in types to compare with.</param>
        /// <returns><c>True</c> if the object’s type is equal to <paramref name="type"/>, <c>False</c> otherwise.</returns>
        /// <seealso cref="ILua.GetType(int)"/>
        public bool IsType(int iStackPos, TYPES type);

        /// <summary>
        /// Returns a type id of the object at <paramref name="iStackPos"/>.
        /// </summary>
        /// <remarks>
        /// See <c>lua_type</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the object whose type id must be returned.</param>
        /// <returns>A type id of the object at <paramref name="iStackPos"/>.</returns>
        /// <seealso cref="ILua.IsType(int, int)"/>
        /// <seealso cref="ILua.IsType(int, TYPES)"/>
        public int GetType(int iStackPos);

        /// <summary>
        /// Returns a type name by its type id.
        /// </summary>
        /// <remarks>
        /// Works only for built-in types. See <see cref="TYPES"/>.
        /// </remarks>
        /// <param name="iType">An id of the type whose name to return.</param>
        /// <returns>A name of the type.</returns>
        public string GetTypeName(int iType);

        /// <summary>
        /// Returns a name of Lua or Garry’s Mod built-in type.
        /// </summary>
        /// <param name="type">One of Lua and Garry’s Mod built-in types.</param>
        /// <returns>A type’s name.</returns>
        public string GetTypeName(TYPES type);

        /// <summary>
        /// Returns a “length” of the object at the given index <paramref name="iStackPos"/>.
        /// </summary>
        /// <remarks>
        /// A “length” has different meanings for different types: 
        /// for strings, this is a string length; 
        /// for tables, this is the result of the Lua length operator <c>#</c>; 
        /// for userdata, this is the size of the memory block allocated for the userdata; 
        /// for all other objects, it is 0.
        /// 
        /// See <c>lua_objlen</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the object whose “length” to return.</param>
        /// <returns>A “length” of the object at <paramref name="iStackPos"/>.</returns>
        public int ObjLen(int iStackPos);

        /// <summary>
        /// Returns a Garry’s Mod angle at <paramref name="iStackPos"/> as <see cref="Vector3"/>.
        /// </summary>
        /// <param name="iStackPos">A stack position of a Garry’s Mod angle.</param>
        /// <returns>A Garry’s Mod angle at <paramref name="iStackPos"/>.</returns>
        public Vector3 GetAngle(int iStackPos);

        /// <summary>
        /// Returns a Garry’s Mod vector at <paramref name="iStackPos"/> as <see cref="Vector3"/>.
        /// </summary>
        /// <param name="iStackPos">A stack position of Garry’s Mod vector.</param>
        /// <returns>A Garry’s Mod vector at <paramref name="iStackPos"/>.</returns>
        public Vector3 GetVector(int iStackPos);

        /// <summary>
        /// Pushes a given angle, represented as <see cref="Vector3"/>, as Garry’s Mod angle onto the stack.
        /// </summary>
        /// <param name="ang">An angle to push onto the stack.</param>
        public void PushAngle(Vector3 ang);

        /// <summary>
        /// Pushes a given <see cref="Vector3"/> as Garry’s Mod vector onto the stack.
        /// </summary>
        /// <param name="vec">A vector to push.</param>
        public void PushVector(Vector3 vec);

        /// <summary>
        /// Sets a new internal pointer to Garry’s Mod native <c>lua_state</c> structure of the <see cref="ILua"/> implementation.
        /// </summary>
        /// <remarks>
        /// For advanced use cases only.
        /// </remarks>
        /// <param name="lua_state">A pointer to <c>lua_state</c> structure.</param>
        public void SetState(IntPtr lua_state);

        /// <summary>
        /// Creates a new Lua type, pushes its metatable onto the stack, and returns new type’s id.
        /// </summary>
        /// <remarks>
        /// <see cref="ILua.CreateMetaTable(in string)"/> allows you to extend Lua and Garry’s Mod type system with custom types.
        /// Returned type id can be used with <see cref="ILua.PushUserType(IntPtr, int)"/>.
        /// 
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="name">A name for the new type.</param>
        /// <returns>A type id for newly created type.</returns>
        /// <seealso cref="ILua.PushUserType(IntPtr, int)"/>
        public int CreateMetaTable(in string name);

        /// <summary>
        /// Pushes a metatable of the given type onto the stack.
        /// </summary>
        /// <remarks>
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iType">An id of the type whose metatable must be pushed.</param>
        /// <returns><c>True</c> if metatable was successfully pushed, <c>False</c> otherwise.</returns>
        public bool PushMetaTable(int iType);

        /// <summary>
        /// Pushes a metatable of one of the Lua or Garry’s Mod built-in types.
        /// </summary>
        /// <remarks>
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="type">A type whose metatable must be pushed.</param>
        /// <returns><c>True</c> if metatable was successfully pushed, <c>False</c> otherwise.</returns>
        public bool PushMetaTable(TYPES type);

        /// <summary>
        /// Pushes a userdata onto the stack and assigns a given type to it.
        /// </summary>
        /// <remarks>
        /// See “Userdata” for more information on userdata: https://www.lua.org/pil/28.1.html
        /// 
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="data_pointer">A userdata pointer to push.</param>
        /// <param name="iType">An id of the type which must be assigned to pushed userdata.</param>
        public void PushUserType(IntPtr data_pointer, int iType);

        /// <summary>
        /// Sets a new pointer for userdata at iStackPos without changing its type.
        /// </summary>
        /// <remarks>
        /// Can be used for invalidation of userdata by passing <see cref="IntPtr.Zero"/>.
        /// 
        /// See “Userdata” for more information on userdata: https://www.lua.org/pil/28.1.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of userdata.</param>
        /// <param name="data_pointer">New userdata pointer.</param>
        public void SetUserType(int iStackPos, IntPtr data_pointer);

        /// <summary>
        /// Returns a pointer from userdata at <paramref name="iStackPos"/> if it has given type.
        /// </summary>
        /// <remarks>
        /// If type id of userdata is not equal to <paramref name="iType"/>, then <see cref="IntPtr.Zero"/> will be returned.
        /// 
        /// See “Userdata” for more information on userdata: https://www.lua.org/pil/28.1.html
        /// 
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the userdata to get a pointer from.</param>
        /// <param name="iType">An id of the type which userdata must have.</param>
        /// <returns>A userdata pointer, if userdata’s type id is equal to <paramref name="iType"/>, <see cref="IntPtr.Zero"/> otherwise.</returns>
        public IntPtr GetUserType(int iStackPos, int iType);

        /// <summary>
        /// Pushes the value <c>t[k]</c> onto of the stack, 
        /// where <c>t</c> is a table-like object at <paramref name="iStackPos"/>,
        /// and <c>k</c> is an object on top of the stack.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="ILua.GetField(int, in string)"/>, allows to get a value from the table when the key in the key-value pair is not a string.
        /// 
        /// Pops a key object from the stack.
        /// 
        /// See <c>lua_gettable</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the table to get a value from.</param>
        /// <example>
        /// The following example shows how <see cref="ILua.GetTable(int)"/> can be used to get a value from the table instead of <see cref="ILua.GetField(int, in string)"/>.
        /// <code>
        /// public static int GetTableExample(ILua lua)
        /// {
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.PushString("print");
        ///     lua.GetTable(-2); // Getting print function from the Lua Global table
        ///     lua.PushString("GetTable works!");
        ///     lua.MCall(1, 0);
        ///     lua.Pop(1);
        /// 
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ILua.GetField(int, in string)"/>
        public void GetTable(int iStackPos);

        /// <summary>
        /// Does a table value assignment equivalent to <c>t[k]=v</c>, 
        /// where <c>t</c> is a table-like object at <paramref name="iStackPos"/>,
        /// <c>v</c> is a value on top of the stack,
        /// and <c>k</c> is a key at stack index <c>-2</c>.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="ILua.SetField(int, in string)"/>, allows to add a key-value pair to a table with the key not being a string.
        /// 
        /// Pops both the key and the value from the stack.
        /// 
        /// See <c>lua_settable</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the table to add a key-value pair to.</param>
        /// <example>
        /// The following example shows how <see cref="ILua.SetTable(int)"/> can be used instead of <see cref="ILua.SetField(int, in string)"/>.
        /// <code>
        /// public static int SetTableExample(ILua lua)
        /// {
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.PushString("SetTableWorks");
        ///     lua.PushString("Yes, SetTable works!");
        ///     lua.SetTable(-3); // Adds a key-value pair "SetTableWorks":"Yes, SetTable works!" to the Global table
        ///     lua.Pop(1);
        /// 
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ILua.SetField(int, in string)"/>
        public void SetTable(int iStackPos);

        /// <summary>
        /// Pushes the value <c>t[k]</c> onto of the stack, 
        /// where <c>t</c> is a table at <paramref name="iStackPos"/>,
        /// and <c>k</c> is an object on top of the stack.
        /// Ignores redefined metamethods.
        /// </summary>
        /// <remarks>
        /// Works as <see cref="ILua.GetTable(int)"/>, but if the table has a custom redefined metamethod <c>__index</c>,
        /// it will be ignored.
        /// 
        /// Pops a key object from the stack.
        /// 
        /// See <c>lua_rawget</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// 
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the table to get value from.</param>
        /// <seealso cref="ILua.GetTable(int)"/>
        public void RawGet(int iStackPos);

        /// <summary>
        /// Does a table value assignment equivalent to <c>t[k]=v</c>, 
        /// where <c>t</c> is a table at <paramref name="iStackPos"/>,
        /// <c>v</c> is a value on top of the stack,
        /// and <c>k</c> is a key at stack index <c>-2</c>.
        /// Ignores redefined metamethods.
        /// </summary>
        /// <remarks>
        /// Works as <see cref="ILua.SetTable(int)"/>, but if the table has a custom redefined metamethod <c> __newindex</c>,
        /// it will be ignored.
        /// 
        /// Pops both the key and the value from the stack.
        /// 
        /// See <c>lua_rawset</c> function in the Lua manual: https://www.lua.org/manual/5.1/manual.html
        /// 
        /// See section "Metatables" in the Lua manual for more information about types and metatables: https://www.lua.org/manual/5.1/manual.html
        /// </remarks>
        /// <param name="iStackPos">A stack position of the table to add a key-value pair to.</param>
        public void RawSet(int iStackPos);

        /// <summary>
        /// Pushes a given pointer as light-userdata onto the stack.
        /// OBSOLETE! This method is deprecated.
        /// Use <see cref="ILua.PushUserType(IntPtr, int)"/> instead.
        /// </summary>
        /// <param name="data">A pointer to push.</param>
        /// <seealso cref="ILua.PushUserType(IntPtr, int)"/>
        [Obsolete("This method is deprecated. Use ILua.PushUserType instead")]
        public void PushUserData(IntPtr data);

        /// <summary>
        /// Returns an internal pointer to Garry’s Mod native <c>ILuaBase</c> structure used by interface implementation.
        /// </summary>
        /// <remarks>
        /// For advanced use cases only.
        /// </remarks>
        /// <returns>An internal pointer to Garry’s Mod native <c>ILuaBase</c> structure used by interface implementation.</returns>
        public IntPtr GetInternalPointer();

        /// <summary>
        /// Calls a function (or any other callable object) followed by its arguments from the stack in the protected mode (will catch any exception).
        /// All caught exceptions are rethrown as <see cref="GmodLuaException"/>.
        /// Pops the function and arguments from the stack.
        /// </summary>
        /// <remarks>
        /// If execution is successful, pushes function’s return values back to the stack.
        /// Otherwise, nothing will be pushed, and <see cref="GmodLuaException"/> will be thrown.
        /// 
        /// This makes <see cref="ILua.MCall(int, int)"/> a better alternative to <see cref="ILua.PCall(int, int, int)"/>, 
        /// since any thrown exception then can be caught with usual C# <c>try/catch</c> block.
        /// </remarks>
        /// <param name="iArgs">Number of arguments to pass to the function.</param>
        /// <param name="iResults">Number of the function’s return values to push back onto the stack.</param>
        /// <exception cref="GmodLuaException">Lua engine exception was thrown while function execution.</exception>
        /// <example>
        /// The following example shows, how to call a Lua function with <see cref="ILua.MCall(int, int)"/> and catch any possible exceptions.
        /// <code>
        /// public static int CallAndCatch(ILua lua)
        /// {
        ///     lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
        ///     lua.GetField(-1, "print");
        ///     lua.PushString("Lua function was called");
        ///     try
        ///     {
        ///         lua.MCall(1, 0); // Calling Lua print function
        ///     }
        ///     catch(GmodLuaException e) // Catchs a Lua exception, if thrown
        ///     {
        ///         Console.WriteLine("Print function threw an exception: " + e.Message);
        ///     }
        ///     finally
        ///     {
        ///         lua.Pop(1);
        ///     }
        /// 
        ///     return 0;
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="ILua.PCall(int, int, int)"/>
        /// <seealso cref="ILua.Call(int, int)"/>
        public void MCall(int iArgs, int iResults);

        /// <summary>
        /// Pushes a given .NET delegate as a Lua function onto the stack.
        /// </summary>
        /// <remarks>
        /// Pushed functions can throw .NET exceptions. 
        /// If pushed function throws a .NET exception, it will be converted to a native Lua exception and rethrown.
        /// </remarks>
        /// <param name="function">A delegate to push.</param>
        /// <returns>An internal <see cref="GCHandle"/> instance allocated for the pushed delegate. For advanced use cases only. Can be ignored most of the time.</returns>
        public GCHandle PushManagedFunction(Func<ILua, int> function);

        /// <summary>
        /// Pushes a given .NET delegate onto the stack, 
        /// associates it with <paramref name="number_of_upvalues"/> objects on top of the stack (such objects are called upvalues), 
        /// and creates a Lua function closure.
        /// </summary>
        /// <remarks>
        /// Pops upvalues from the stack.
        /// 
        /// Pushed closures can throw .NET exceptions.
        /// If pushed closure throws a .NET exception, it will be converted to a native Lua exception and rethrown.
        /// 
        /// See “Upvalues” for more information about Lua closures: https://www.lua.org/pil/27.3.3.html
        /// </remarks>
        /// <param name="function">A .NET delegate to create a Lua closure from.</param>
        /// <param name="number_of_upvalues">A number of objects on top of the stack to associate with the closure as upvalues.</param>
        /// <returns>An internal <see cref="GCHandle"/> instance allocated for the pushed delegate. For advanced use cases only. Can be ignored most of the time.</returns>
        public GCHandle PushManagedClosure(Func<ILua, int> function, byte number_of_upvalues);
    }

    /// <summary>
    /// A .NET representation of Lua engine exception.
    /// </summary>
    public class GmodLuaException : Exception
    {
        int error_code;

        /// <summary>
        /// Initializes a new instance of the <see cref="GmodLuaException"/> class.
        /// </summary>
        /// <param name="lua_error_code">A Lua exception code. Must be one of the values defined by Lua specification.</param>
        /// <param name="lua_error_message">A Lua exception message.</param>
        public GmodLuaException(int lua_error_code, string lua_error_message) : base(lua_error_message)
        {
            this.error_code = lua_error_code;
        }

        /// <summary>
        /// A Lua exception error code (the value defined by Lua specification).
        /// </summary>
        public int ErrorCode => error_code;

        /// <summary>
        /// A Lua exception message.
        /// </summary>
        public override string Message => base.Message;
    }

    /// <summary>
    /// Enumeration of the indices for the Lua special tables. Designed to be used with <see cref="ILua.PushSpecial(SPECIAL_TABLES)"/>.
    /// </summary>
    public enum SPECIAL_TABLES
    {
        /// <summary>
        /// Lua Global table. Can be used to access Garry's Mod Lua API. 
        /// For more information see an official Lua documentation on usage of the Global Table (https://www.lua.org/pil/15.4.html).
        /// </summary>
        SPECIAL_GLOB,

        /// <summary>
        /// Lua Environment table.
        /// For more information see an official Lua documentation on The Environment (https://www.lua.org/pil/14.html).
        /// </summary>
        SPECIAL_ENV,

        /// <summary>
        /// Lua Registry table. For more information see an official Lua documentation on The Registry (https://www.lua.org/pil/27.3.1.html).
        /// </summary>
        SPECIAL_REG
    }

    /// <summary>
    /// Enumeration of type ids of common Lua and Garry's Mod built-in types. 
    /// Designed to be used with <see cref="ILua.GetTypeName(TYPES)"/> and <see cref="ILua.IsType(int, TYPES)"/>.
    /// </summary>
    public enum TYPES
    {
        /// <summary>
        /// NONE
        /// </summary>
        NONE = -1,

        /// <summary>
        /// An id for Lua type NIL.
        /// </summary>
        NIL,

        /// <summary>
        /// An id for Lua type BOOL.
        /// </summary>
        BOOL,

        /// <summary>
        /// An id for Lua type LIGHTUSERDATA.
        /// </summary>
        LIGHTUSERDATA,

        /// <summary>
        /// An id for Lua type NUMBER.
        /// </summary>
        NUMBER,

        /// <summary>
        /// An id for Lua type STRING.
        /// </summary>
        STRING,

        /// <summary>
        /// An id for Lua type TABLE.
        /// </summary>
        TABLE,

        /// <summary>
        /// An id for Lua type FUNCTION.
        /// </summary>
        FUNCTION,

        /// <summary>
        /// An id for Lua type USERDATA.
        /// </summary>
        USERDATA,

        /// <summary>
        /// An id for Lua type THREAD.
        /// </summary>
        THREAD,

        // GMod Types

        /// <summary>
        /// An id for Garry's Mod type ENTITY.
        /// See https://wiki.facepunch.com/gmod/Entity.
        /// </summary>
        ENTITY,

        /// <summary>
        /// An id for Garry's Mod type Vector.
        /// See https://wiki.facepunch.com/gmod/Vector.
        /// </summary>
        Vector,

        /// <summary>
        /// An id for Garry's Mod type Angle.
        /// See https://wiki.facepunch.com/gmod/Angle.
        /// </summary>
        ANGLE,

        /// <summary>
        /// An id for Garry's Mod type PHYSOBJ.
        /// See https://wiki.facepunch.com/gmod/PhysObj.
        /// </summary>
        PHYSOBJ,

        /// <summary>
        /// An id for Garry's Mod type SAVE.
        /// See https://wiki.facepunch.com/gmod/ISave.
        /// </summary>
        SAVE,

        /// <summary>
        /// An id for Garry's Mod type RESTORE.
        /// See https://wiki.facepunch.com/gmod/IRestore.
        /// </summary>
        RESTORE,

        /// <summary>
        /// An id for Garry's Mod type DAMAGEINFO.
        /// See https://wiki.facepunch.com/gmod/CTakeDamageInfo.
        /// </summary>
        DAMAGEINFO,

        /// <summary>
        /// An id for Garry's Mod type EFFECTDATA.
        /// See https://wiki.facepunch.com/gmod/CEffectData.
        /// </summary>
        EFFECTDATA,

        /// <summary>
        /// An id for Garry's Mod type MOVEDATA.
        /// See https://wiki.facepunch.com/gmod/CMoveData.
        /// </summary>
        MOVEDATA,

        /// <summary>
        /// An id for Garry's Mod type RECIPIENTFILTER.
        /// See https://wiki.facepunch.com/gmod/CRecipientFilter.
        /// </summary>
        RECIPIENTFILTER,

        /// <summary>
        /// An id for Garry's Mod type USERCMD.
        /// See https://wiki.facepunch.com/gmod/CUserCmd.
        /// </summary>
        USERCMD,

        /// <summary>
        /// An id for Garry's Mod type SCRIPTEDVEHICLE.
        /// See https://wiki.facepunch.com/gmod/Vehicle.
        /// </summary>
        SCRIPTEDVEHICLE,

        /// <summary>
        /// An id for Garry's Mod type MATERIAL.
        /// See https://wiki.facepunch.com/gmod/IMaterial.
        /// </summary>
        MATERIAL,

        /// <summary>
        /// An id for Garry's Mod type PANEL.
        /// See https://wiki.facepunch.com/gmod/Panel.
        /// </summary>
        PANEL,

        /// <summary>
        /// An id for Garry's Mod type PARTICLE.
        /// See https://wiki.facepunch.com/gmod/CLuaParticle.
        /// </summary>
        PARTICLE,

        /// <summary>
        /// An id for Garry's Mod type PARTICLEEMITTER.
        /// See https://wiki.facepunch.com/gmod/CLuaEmitter.
        /// </summary>
        PARTICLEEMITTER,

        /// <summary>
        /// An id for Garry's Mod type TEXTURE.
        /// See https://wiki.facepunch.com/gmod/ITexture.
        /// </summary>
        TEXTURE,

        /// <summary>
        /// An id for Garry's Mod type USERMSG.
        /// </summary>
        USERMSG,

        /// <summary>
        /// An id for Garry's Mod type CONVAR.
        /// See https://wiki.facepunch.com/gmod/ConVar.
        /// </summary>
        CONVAR,

        /// <summary>
        /// An id for Garry's Mod type IMESH.
        /// See https://wiki.facepunch.com/gmod/IMesh.
        /// </summary>
        IMESH,

        /// <summary>
        /// An id for Garry's Mod type MATRIX.
        /// See https://wiki.facepunch.com/gmod/VMatrix.
        /// </summary>
        MATRIX,

        /// <summary>
        /// An id for Garry's Mod type SOUND.
        /// See https://wiki.facepunch.com/gmod/CSoundPatch.
        /// </summary>
        SOUND,

        /// <summary>
        /// An id for Garry's Mod type PIXELVISHANDLE.
        /// See https://wiki.facepunch.com/gmod/pixelvis_handle_t.
        /// </summary>
        PIXELVISHANDLE,

        /// <summary>
        /// An id for Garry's Mod type DLIGHT.
        /// See https://wiki.facepunch.com/gmod/Structures/DynamicLight.
        /// </summary>
        DLIGHT,

        /// <summary>
        /// An id for Garry's Mod type VIDEO.
        /// See https://wiki.facepunch.com/gmod/IVideoWriter.
        /// </summary>
        VIDEO,

        /// <summary>
        /// An id for Garry's Mod type FILE.
        /// See https://wiki.facepunch.com/gmod/file_class.
        /// </summary>
        FILE,

        /// <summary>
        /// An id for Garry's Mod type LOCOMOTION.
        /// See https://wiki.facepunch.com/gmod/CLuaLocomotion.
        /// </summary>
        LOCOMOTION,

        /// <summary>
        /// An id for Garry's Mod type PATH.
        /// See https://wiki.facepunch.com/gmod/PathFollower.
        /// </summary>
        PATH,

        /// <summary>
        /// An id for Garry's Mod type NAVAREA.
        /// See https://wiki.facepunch.com/gmod/CNavArea.
        /// </summary>
        NAVAREA,

        /// <summary>
        /// An id for Garry's Mod type SOUNDHANDLE.
        /// </summary>
        SOUNDHANDLE,

        /// <summary>
        /// An id for Garry's Mod type NAVLADDER.
        /// See https://wiki.facepunch.com/gmod/CNavLadder.
        /// </summary>
        NAVLADDER,

        /// <summary>
        /// An id for Garry's Mod type PARTICLESYSTEM.
        /// See https://wiki.facepunch.com/gmod/CNewParticleEffect.
        /// </summary>
        PARTICLESYSTEM,

        /// <summary>
        /// An id for Garry's Mod type PROJECTEDTEXTURE.
        /// See https://wiki.facepunch.com/gmod/ProjectedTexture.
        /// </summary>
        PROJECTEDTEXTURE,

        /// <summary>
        /// An id for Garry's Mod type PHYSCOLLIDE.
        /// See https://wiki.facepunch.com/gmod/PhysCollide.
        /// </summary>
        PHYSCOLLIDE,

        /// <summary>
        /// An id for Garry's Mod type SURFACEINFO.
        /// See https://wiki.facepunch.com/gmod/SurfaceInfo.
        /// </summary>
        SURFACEINFO,

        /// <summary>
        /// Presented for compatibility.
        /// </summary>
        COUNT
    }
}
