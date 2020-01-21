using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.InteropServices;

namespace GmodNET.API
{
    /// <summary>
    /// Delegate which can be converted to native CFunc pointer, pushed on Garry's Mod Lua stack, and called be Garry's Mod.
    /// When pushed to Garry's Mod, make sure that delegate instance will not be garbage collected.
    /// </summary>
    /// <param name="lua_state_pointer">lua_state pointer. Use IModule.GetILuaFromLuaStatePointerMethod to get ILua interface from it</param>
    /// <returns>Number of return values function pushes on the stack</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int CFuncManagedDelegate(IntPtr lua_state_pointer);
    /// <summary>
    /// Managed wrapper around Garry's Mod native ILuaBase.
    /// </summary>
    public interface ILua
    {
        /// <summary>
        /// Returns the amount of values on the stack.
        /// </summary>
        /// <returns></returns>
        public int Top();
        /// <summary>
        /// Pushes a copy of the value at iStackPos to the top of the stack.
        /// </summary>
        /// <param name="iStackPos">position of the value on the stack</param>
        public void Push(int iStackPos);
        /// <summary>
        /// Pops iAmt values from the top of the stack.
        /// </summary>
        /// <param name="IAmt">amount of values to pop</param>
        public void Pop(int IAmt);
        /// <summary>
        /// Pushes table[key] on to the stack.
        /// </summary>
        /// <param name="iStackPos">position of the table on the stack</param>
        /// <param name="key">Key in the table</param>
        public void GetField(int iStackPos, in string key);
        /// <summary>
        /// Sets table[key] to the value at the top of the stack.
        /// </summary>
        /// <param name="iStackPos">position of the table on the stack</param>
        /// <param name="key">Key in the table</param>
        public void SetField(int iStackPos, in string key);
        /// <summary>
        /// Creates a new table and pushes it to the top of the stack.
        /// </summary>
        public void CreateTable();
        /// <summary>
        /// Sets the metatable for the value at iStackPos to the value at the top of the stack. Pops the value off of the top of the stack.
        /// </summary>
        /// <param name="iStackPos">Position of object ot set metatable to</param>
        public void SetMetaTable(int iStackPos);
        /// <summary>
        /// Pushes the metatable of the value at iStackPos on to the top of the stack. Upon failure, returns false and does not push anything.
        /// </summary>
        /// <param name="iStackPos">Position of the object to get metatable from</param>
        /// <returns>Success indicator</returns>
        public bool GetMetaTable(int iStackPos);
        /// <summary>
        /// Calls a function. To use it: Push the function on to the stack followed by each argument. 
        /// Pops the function and arguments from the stack, leaves iResults values on the stack.
        /// </summary>
        /// <param name="iArgs">number of arguments of the function</param>
        /// <param name="iResults">number of return values of the function</param>
        public void Call(int iArgs, int iResults);
        /// <summary>
        /// Similar to Call. Calls a function in protected mode. Both nargs and nresults have the same meaning as in lua_call.
        /// If there are no errors during the call, lua_pcall behaves exactly like lua_call.
        /// However, if there is any error, lua_pcall catches it, pushes a single value on the stack (the error message), and returns an error code.
        /// Like lua_call, lua_pcall always removes the function and its arguments from the stack.
        /// If errfunc is 0, then the error message returned on the stack is exactly the original error message.
        /// Otherwise, errfunc is the stack index of an error handler function.
        /// (In the current implementation, this index cannot be a pseudo-index.)
        /// In case of runtime errors, this function will be called with the error message and its return value will be the message returned on the stack by lua_pcall.
        /// </summary>
        /// <param name="IArgs">number of arguments of the function</param>
        /// <param name="IResults">number of return values of the function</param>
        /// <param name="ErrorFunc">the stack index of an error handler function</param>
        /// <returns>0 in case of success or one of the error codes (defined by lua engine)</returns>
        public int PCall(int IArgs, int IResults, int ErrorFunc);
        /// <summary>
        /// Returns true if the values at iA and iB are equal.
        /// </summary>
        /// <param name="iA">position of the first value to compare</param>
        /// <param name="iB">position of the second value</param>
        /// <returns></returns>
        public bool Equal(int iA, int iB);
        /// <summary>
        /// Returns true if the value at iA and iB are equal. Does not invoke metamethods.
        /// </summary>
        /// <param name="iA">position of the first value to compare</param>
        /// <param name="iB">position of the second value</param>
        /// <returns></returns>
        public bool RawEqual(int iA, int iB);
        /// <summary>
        /// Moves the value at the top of the stack in to iStackPos. Any elements above iStackPos are shifted upwards.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        public void Insert(int iStackPos);
        /// <summary>
        /// Removes the value at iStackPos from the stack. Any elements above iStackPos are shifted downwards.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        public void Remove(int iStackPos);
        /// <summary>
        /// Allows you to iterate tables similar to pairs(...). 
        /// Pops a key from the stack, and pushes a key-value pair from the table at the given index (the "next" pair after the given key).
        /// If there are no more elements in the table, then lua_next returns 0 (and pushes nothing).
        /// </summary>
        /// <param name="iStackPos">position of the table</param>
        /// <returns></returns>
        public int Next(int iStackPos);
        /// <summary>
        /// Throws an error and ceases execution of the function.
        /// </summary>
        /// <param name="error_message">error message</param>
        public void ThrowError(in string error_message);
        /// <summary>
        /// Checks that the type of the value at iStackPos is iType. Throws and error and ceases execution of the function otherwise.
        /// </summary>
        /// <param name="iStackPos">position on the stack of the value to check type of</param>
        /// <param name="IType">type index</param>
        public void CheckType(int iStackPos, int IType);
        /// <summary>
        /// Throws a pretty error message about the given argument.
        /// </summary>
        /// <param name="iArgNum">index of the problematic argument</param>
        /// <param name="error_message">error message</param>
        public void ArgError(int iArgNum, in string error_message);
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
        /// <param name="iStackPos">position of number of the stack</param>
        /// <returns></returns>
        public double GetNumber(int iStackPos);
        /// <summary>
        /// Returns the boolean at iStackPos (as int). Returns false upon failure.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <returns></returns>
        public bool GetBool(int iStackPos);
        /// <summary>
        /// Returns the C-Function at iStackPos (native pointer). Returns NULL upon failure.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <returns></returns>
        public IntPtr GetCFunction(int iStackPos);
        /// <summary>
        /// Pushes a nil value on to the stack
        /// </summary>
        public void PushNil();
        /// <summary>
        /// Pushes the given string on to the stack.
        /// </summary>
        /// <param name="str">string to push</param>
        public void PushString(in string str);
        /// <summary>
        /// Pushes the given double on to the stack.
        /// </summary>
        /// <param name="val">number to push</param>
        public void PushNumber(double val);
        /// <summary>
        /// Pushes the given boolean on to the stack.
        /// </summary>
        /// <param name="val">bool value to push</param>
        public void PushBool(bool val);
        /// <summary>
        /// Pushes the given managed function on to the stack. The managed function will be converted to the C function.
        /// </summary>
        /// <param name="managed_function">function to push</param>
        public void PushCFunction(CFuncManagedDelegate managed_function);
        /// <summary>
        /// Pushes the given C-Function on to the stack. Native C function must be of signature "int Func(void*)".
        /// </summary>
        /// <param name="native_func_ptr">Native C function pointer to push</param>
        public void PushCFunction(IntPtr native_func_ptr);
        /// <summary>
        /// Pushes the given C-Function on to the stack with upvalues.
        /// </summary>
        /// <param name="native_func_ptr"></param>
        /// <param name="iVars"></param>
        public void PushCClosure(IntPtr native_func_ptr, int iVars);
        /// <summary>
        /// Allows for values to be stored by reference for later use. Make sure you call ReferenceFree when you are done with a reference.
        /// </summary>
        /// <returns></returns>
        public int ReferenceCreate();
        /// <summary>
        /// Free reference
        /// </summary>
        /// <param name="reference">reference to free</param>
        public void ReferenceFree(int reference);
        /// <summary>
        /// Push reference on to the stack
        /// </summary>
        /// <param name="reference">reference to push</param>
        public void ReferencePush(int reference);
        /// <summary>
        /// Push a special value onto the top of the stack.
        /// </summary>
        /// <param name="table">table to push</param>
        public void PushSpecial(SPECIAL_TABLES table);
        /// <summary>
        /// Returns true if the value at iStackPos is of type iType.
        /// </summary>
        /// <param name="iStackPos">position of value to check type of</param>
        /// <param name="iType">type index</param>
        /// <returns></returns>
        public bool IsType(int iStackPos, int iType);
        /// <summary>
        /// Returns the type of the value at iStackPos.
        /// </summary>
        /// <param name="iStackPos"></param>
        /// <returns></returns>
        public int GetType(int iStackPos);
        /// <summary>
        /// Returns the name associated with the given type ID.
        /// </summary>
        /// <param name="iType">type index</param>
        /// <returns></returns>
        public string GetTypeName(int iType);
        /// <summary>
        /// Returns the length of the object at iStackPos.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <returns></returns>
        public int ObjLen(int iStackPos);
        /// <summary>
        /// Returns the angle at iStackPos as C# Vector3.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <returns></returns>
        public Vector3 GetAngle(int iStackPos);
        /// <summary>
        /// Returns the vector at iStackPos.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <returns></returns>
        public Vector3 GetVector(int iStackPos);
        /// <summary>
        /// Pushes the given angle to the top of the stack.
        /// </summary>
        /// <param name="ang">angle (Vector3 represented) to push</param>
        public void PushAngle(Vector3 ang);
        /// <summary>
        /// Pushes the given vector to the top of the stack.
        /// </summary>
        /// <param name="vec">vector to push</param>
        public void PushVector(Vector3 vec);
        /// <summary>
        /// Sets the lua_State to be used by the ILuaBase implementation.
        /// </summary>
        /// <param name="lua_state">pointer to the lua_state</param>
        public void SetState(IntPtr lua_state);
        /// <summary>
        /// Pushes the metatable associated with the given type name.
        /// </summary>
        /// <param name="name">name of the metatable</param>
        /// <returns>ID (type index) of the metatable</returns>
        public int CreateMetaTable(in string name);
        /// <summary>
        /// ushes the metatable associated with the given type.
        /// </summary>
        /// <param name="iType">type which contains metatable</param>
        /// <returns>Success indicator</returns>
        public bool PushMetaTable(int iType);
        /// <summary>
        /// Creates a new UserData of type iType that references the given data.
        /// </summary>
        /// <param name="data_pointer">pointer to data to reference as user data</param>
        /// <param name="iType">type index</param>
        public void PushUserType(IntPtr data_pointer, int iType);
        /// <summary>
        /// Sets the data pointer of the UserType at iStackPos. You can use this to invalidate a UserType by passing NULL.
        /// </summary>
        /// <param name="iStackPos">position of object on the stack</param>
        /// <param name="data_pointer">user data pointer</param>
        public void SetUserType(int iStackPos, IntPtr data_pointer);
        /// <summary>
        /// Returns the data of the UserType at iStackPos if it is of the given type.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <param name="iType">type index</param>
        /// <returns>pointer to the user type</returns>
        public IntPtr GetUserType(int iStackPos, int iType);
        /// <summary>
        /// Pushes table[key] on to the stack. Table = value at iStackPos. Key = value at top of the stack.
        /// Pops the key from the stack
        /// </summary>
        /// <param name="iStackPos">position of the table on the stack</param>
        public void GetTable(int iStackPos);
        /// <summary>
        /// Sets table[key] to the value at the top of the stack. Table = value at iStackPos. Key = value 2nd to the top of the stack.
        /// Pops the key and the value from the stack.
        /// </summary>
        /// <param name="iStackPos">position of the table on the stack</param>
        public void SetTable(int iStackPos);
        /// <summary>
        /// Pushes table[key] on to the stack. Table = value at iStackPos. Key = value at top of the stack. Does not invoke metamethods.
        /// </summary>
        /// <param name="iStackPos">position of the table on the stack</param>
        public void RawGet(int iStackPos);
        /// <summary>
        /// Sets table[key] to the value at the top of the stack. Table = value at iStackPos. Key = value 2nd to the top of the stack.
        /// Pops the key and the value from the stack. Does not invoke metamethods.
        /// </summary>
        /// <param name="iStackPos">position of the table on the stack</param>
        public void RawSet(int iStackPos);
        /// <summary>
        /// Pushes the given pointer on to the stack as light-userdata.
        /// </summary>
        /// <param name="data">pointer to the user data</param>
        public void PushUserData(IntPtr data);
        /// <summary>
        /// Returns string from the stack, but throws errors and returns empty string if not of the expected type.
        /// </summary>
        /// <param name="iStackPos">position of the "string" on the stack</param>
        /// <returns>String on success and empty string otherwise</returns>
        public string CheckString(int iStackPos);
        /// <summary>
        /// Returns number from the stack, but throws errors and returns if not of the expected type.
        /// </summary>
        /// <param name="iStackPos">position on the stack</param>
        /// <returns>number from the stack</returns>
        public double CheckNumber(int iStackPos);
        /// <summary>
        /// Get ILuaBase native pointer from Garry's Mod.
        /// </summary>
        /// <returns></returns>
        public IntPtr GetInternalPointer();
    }

    /// <summary>
    /// Indeces of the Lua special tables.
    /// </summary>
    public enum SPECIAL_TABLES
    {
        /// <summary>
        /// Global table.
        /// </summary>
        SPECIAL_GLOB,
        /// <summary>
        /// Environment table.
        /// </summary>
        SPECIAL_ENV,
        /// <summary>
        /// Unknown. TODO.
        /// </summary>
        SPECIAL_REG
    }

    /// <summary>
    /// Indeces of common lua and gmod types.
    /// </summary>
    public enum TYPES
    {
        NONE = -1,
        NIL,
        BOOL,
        LIGHTUSERDATA,
        NUMBER,
        STRING,
        TABLE,
        FUNCTION,
        USERDATA,
        THREAD,

        // GMod Types
        ENTITY,
        Vector, // GMOD: GO TODO - This was renamed... I'll probably forget to fix it before this ends up public
        ANGLE,
        PHYSOBJ,
        SAVE,
        RESTORE,
        DAMAGEINFO,
        EFFECTDATA,
        MOVEDATA,
        RECIPIENTFILTER,
        USERCMD,
        SCRIPTEDVEHICLE,
        MATERIAL,
        PANEL,
        PARTICLE,
        PARTICLEEMITTER,
        TEXTURE,
        USERMSG,
        CONVAR,
        IMESH,
        MATRIX,
        SOUND,
        PIXELVISHANDLE,
        DLIGHT,
        VIDEO,
        FILE,
        LOCOMOTION,
        PATH,
        NAVAREA,
        SOUNDHANDLE,
        NAVLADDER,
        PARTICLESYSTEM,
        PROJECTEDTEXTURE,
        PHYSCOLLIDE,

        COUNT
    }
}
