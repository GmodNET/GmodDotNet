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
        /// Pushes table[key] onto the stack.
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
        /// Sets table[key] to the element on the top of the stack. Pops the original element from the stack.
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack.</param>
        /// <param name="key">Key in the table to set value for.</param>
        /// <remarks>
        /// This method can be used to fill Lua tables with values or update them. 
        /// In the example below we will add a Lua funtion <c>SayHello</c> to the Lua global table <see cref="SPECIAL_TABLES.SPECIAL_GLOB"/>.
        /// This function then can be called from any Lua as <c>SayHello()</c>.
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
        /// Moves the value at the top of the stack in to iStackPos. Any elements above iStackPos are shifted upwards.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        public void Insert(int iStackPos);
        /// <summary>
        /// Removes the value at iStackPos from the stack. Any elements above iStackPos are shifted downwards.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        public void Remove(int iStackPos);
        /// <summary>
        /// Allows you to iterate tables similar to pairs(...). 
        /// Pops a key from the stack, and pushes a key-value pair from the table at the given index (the "next" pair after the given key).
        /// If there are no more elements in the table, then lua_next returns 0 (and pushes nothing).
        /// </summary>
        /// <param name="iStackPos">Position of the table</param>
        /// <returns></returns>
        public int Next(int iStackPos);
        /// <summary>
        /// Returns the string at iStackPos. iOutLen is set to the length of the string if it is not NULL. 
        /// If the value at iStackPos is a number, it will be converted in to a string.
        /// Returns empty string upon failure.
        /// </summary>
        /// <param name="iStackPos"></param>
        /// <returns></returns>
        public string GetString(int iStackPos);
        /// <summary>
        /// Returns the number at iStackPos. Returns 0 upon failure.
        /// </summary>
        /// <param name="iStackPos">Position of number of the stack</param>
        /// <returns></returns>
        public double GetNumber(int iStackPos);
        /// <summary>
        /// Returns the boolean at iStackPos (as int). Returns false upon failure.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        /// <returns></returns>
        public bool GetBool(int iStackPos);
        /// <summary>
        /// Returns the C-Function at iStackPos (native pointer). Returns NULL upon failure.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        /// <returns></returns>
        public IntPtr GetCFunction(int iStackPos);
        /// <summary>
        /// Pushes a nil value on to the stack
        /// </summary>
        public void PushNil();
        /// <summary>
        /// Pushes the given string on to the stack.
        /// </summary>
        /// <param name="str">String to push</param>
        public void PushString(in string str);
        /// <summary>
        /// Pushes the given double on to the stack.
        /// </summary>
        /// <param name="val">Number to push</param>
        public void PushNumber(double val);
        /// <summary>
        /// Pushes the given boolean on to the stack.
        /// </summary>
        /// <param name="val">Bool value to push</param>
        public void PushBool(bool val);
        /// <summary>
        /// Pushes the given C-Function on to the stack. Native C function must be of signature "int Func(void*)".
        /// </summary>
        /// <param name="native_func_ptr">Native C function pointer to push</param>
        public unsafe void PushCFunction(IntPtr native_func_ptr);
        /// <summary>
        /// Pushes a given C# function pointer with native Cdecl calling convention onto the Lua stack.
        /// </summary>
        /// <remarks>
        /// Function which pointer is pushed onto the Lua stack must conform lua_CFunction specification as described here: https://www.lua.org/pil/26.1.html
        /// </remarks>
        /// <param name="function_pointer">C# function pointer with native calling convention Cdecl.</param>
        public unsafe void PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int> function_pointer);
        /// <summary>
        /// Pushes the given C-Function on to the stack with upvalues.
        /// </summary>
        /// <param name="native_func_ptr"></param>
        /// <param name="iVars"></param>
        public void PushCClosure(IntPtr native_func_ptr, int iVars);
        /// <summary>
        /// Allows for values to be stored by reference for later use. Make sure you call ReferenceFree when you are done with a reference. 
        /// Pops the value to reference from the stack.
        /// </summary>
        /// <returns></returns>
        public int ReferenceCreate();
        /// <summary>
        /// Free reference
        /// </summary>
        /// <param name="reference">Reference to free</param>
        public void ReferenceFree(int reference);
        /// <summary>
        /// Push reference on to the stack
        /// </summary>
        /// <param name="reference">Reference to push</param>
        public void ReferencePush(int reference);
        /// <summary>
        /// Push a special value onto the top of the stack.
        /// </summary>
        /// <param name="table">Table to push</param>
        public void PushSpecial(SPECIAL_TABLES table);
        /// <summary>
        /// Returns true if the value at iStackPos is of type iType.
        /// </summary>
        /// <param name="iStackPos">Position of value to check type of</param>
        /// <param name="iType">Type index</param>
        /// <returns></returns>
        public bool IsType(int iStackPos, int iType);
        /// <summary>
        /// Returns true if the value at iStackPos is of the given type.
        /// </summary>
        /// <param name="iStackPos">Position of value to check type of</param>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public bool IsType(int iStackPos, TYPES type);
        /// <summary>
        /// Returns the type of the value at iStackPos.
        /// </summary>
        /// <param name="iStackPos"></param>
        /// <returns></returns>
        public int GetType(int iStackPos);
        /// <summary>
        /// Returns the name associated with the given type ID. Doesn't work with user-defined types.
        /// </summary>
        /// <param name="iType">Type index</param>
        /// <returns></returns>
        public string GetTypeName(int iType);
        /// <summary>
        /// Returns the name associated with the given type.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns></returns>
        public string GetTypeName(TYPES type);
        /// <summary>
        /// Returns the length of the object at iStackPos.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        /// <returns></returns>
        public int ObjLen(int iStackPos);
        /// <summary>
        /// Returns the angle at iStackPos as C# Vector3.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        /// <returns></returns>
        public Vector3 GetAngle(int iStackPos);
        /// <summary>
        /// Returns the vector at iStackPos.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        /// <returns></returns>
        public Vector3 GetVector(int iStackPos);
        /// <summary>
        /// Pushes the given angle to the top of the stack.
        /// </summary>
        /// <param name="ang">Angle (Vector3 represented) to push</param>
        public void PushAngle(Vector3 ang);
        /// <summary>
        /// Pushes the given vector to the top of the stack.
        /// </summary>
        /// <param name="vec">Vector to push</param>
        public void PushVector(Vector3 vec);
        /// <summary>
        /// Sets the lua_State to be used by the ILuaBase implementation.
        /// </summary>
        /// <param name="lua_state">Pointer to the lua_state</param>
        public void SetState(IntPtr lua_state);
        /// <summary>
        /// Pushes the metatable associated with the given type name.
        /// Returns the type ID to use for this type.
        /// </summary>
        /// <param name="name">Name of the metatable</param>
        /// <returns>ID (type index) of the metatable</returns>
        public int CreateMetaTable(in string name);
        /// <summary>
        /// Pushes the metatable associated with the given type.
        /// </summary>
        /// <param name="iType">Index of the type which metatable to push</param>
        /// <returns>Success indicator</returns>
        public bool PushMetaTable(int iType);
        /// <summary>
        /// Pushes the metatable associated with the given type.
        /// </summary>
        /// <param name="type">Type which metatable to push</param>
        /// <returns>Success indicator</returns>
        public bool PushMetaTable(TYPES type);
        /// <summary>
        /// Creates a new UserData of type iType that references the given data.
        /// </summary>
        /// <param name="data_pointer">Pointer to data to reference as user data</param>
        /// <param name="iType">Type index</param>
        public void PushUserType(IntPtr data_pointer, int iType);
        /// <summary>
        /// Sets the data pointer of the UserType at iStackPos. You can use this to invalidate a UserType by passing NULL.
        /// </summary>
        /// <param name="iStackPos">Position of object on the stack</param>
        /// <param name="data_pointer">User data pointer</param>
        public void SetUserType(int iStackPos, IntPtr data_pointer);
        /// <summary>
        /// Returns the data of the UserType at iStackPos if it is of the given type.
        /// </summary>
        /// <param name="iStackPos">Position on the stack</param>
        /// <param name="iType">Type index</param>
        /// <returns>pointer to the user type</returns>
        public IntPtr GetUserType(int iStackPos, int iType);
        /// <summary>
        /// Pushes table[key] on to the stack. Table = value at iStackPos. Key = value at top of the stack.
        /// Pops the key from the stack
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack</param>
        public void GetTable(int iStackPos);
        /// <summary>
        /// Sets table[key] to the value at the top of the stack. Table = value at iStackPos. Key = value 2nd to the top of the stack.
        /// Pops the key and the value from the stack.
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack</param>
        public void SetTable(int iStackPos);
        /// <summary>
        /// Pushes table[key] on to the stack. Table = value at iStackPos. Key = value at top of the stack. Does not invoke metamethods.
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack</param>
        public void RawGet(int iStackPos);
        /// <summary>
        /// Sets table[key] to the value at the top of the stack. Table = value at iStackPos. Key = value 2nd to the top of the stack.
        /// Pops the key and the value from the stack. Does not invoke metamethods.
        /// </summary>
        /// <param name="iStackPos">Position of the table on the stack</param>
        public void RawSet(int iStackPos);
        /// <summary>
        /// Pushes the given pointer on to the stack as light-userdata.
        /// </summary>
        /// <param name="data">Pointer to the user data</param>
        [Obsolete("This method is unsafe and obsolete. Use ILua.PushUserType instead")]
        public void PushUserData(IntPtr data);
        /// <summary>
        /// Get ILuaBase native pointer from Garry's Mod.
        /// </summary>
        /// <returns></returns>
        public IntPtr GetInternalPointer();
        /// <summary>
        /// High level wrapper around PCall. If call is successfull, MCall will behave just like Call. 
        /// But if Lua exception is thrown while call, GmodLuaException managed exception will be thrown.
        /// </summary>
        /// <param name="iArgs">Number of arguments of the function to call</param>
        /// <param name="iResults">Number of returns of the function to call</param>
        public void MCall(int iArgs, int iResults);
        /// <summary>
        /// Push a managed function or delegate to the lua stack.
        /// </summary>
        /// <param name="function">A managed function or delegate to push.</param>
        /// <returns>A GCHandle instance, allocated for managed delegate. For advanced scenarios.</returns>
        public GCHandle PushManagedFunction(Func<ILua, int> function);
        /// <summary>
        /// Push managed function or delegate together with upvalues as Lua closure. Upvalues must be pushed first. Pops upvalues from the stack.
        /// </summary>
        /// <param name="function">Managed function or delegate to form closure from.</param>
        /// <param name="number_of_upvalues">Number of upvalues.</param>
        /// <returns>A GCHandle instance, allocated for managed delegate. For advanced scenarios.</returns>
        public GCHandle PushManagedClosure(Func<ILua, int> function, byte number_of_upvalues);
    }

    /// <summary>
    /// Managed exception which incapsulates information about Lua exception
    /// </summary>
    public class GmodLuaException : Exception
    {
        int error_code;

        /// <summary>
        /// Create new GmodLuaException
        /// </summary>
        /// <param name="lua_error_code">Lua exception code</param>
        /// <param name="lua_error_message">Lua exception message</param>
        public GmodLuaException(int lua_error_code, string lua_error_message) : base(lua_error_message)
        {
            this.error_code = lua_error_code;
        }

        /// <summary>
        /// Error code of the lua exception
        /// </summary>
        public int ErrorCode => error_code;
        /// <summary>
        /// Lua exception message
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
