//
// Created by Gleb Krasilich on 06.10.2019.
//

#ifndef GM_DOTNET_NATIVE_LUAAPIEXPOSURE_H
#define GM_DOTNET_NATIVE_LUAAPIEXPOSURE_H
#include <GarrysMod/Lua/Interface.h>
#include <GarrysMod/Lua/LuaBase.h>

/// Returns the amount of values on the stack
/// \param lua ILuaBase pointer
/// \return
int export_top(GarrysMod::Lua::ILuaBase * lua);

/// Pushes a copy of the value at iStackPos to the top of the stack
/// \param lua ILuaBase pointer
/// \param iStackPos position of the value on the stack
void export_push(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Pops iAmt values from the top of the stack
/// \param lua ILuaBase pointer
/// \param IAmt amount of values to pop
void export_pop(GarrysMod::Lua::ILuaBase * lua, int IAmt);

/// Pushes table[key] on to the stack
/// \param lua ILuaBase pointer
/// \param iStackPos position of the table on the stack
/// \param key key to get from the table
void export_get_field(GarrysMod::Lua::ILuaBase * lua, int iStackPos, const char * key);

/// Sets table[key] to the value at the top of the stack
/// \param lua ILuaBase pointer
/// \param iStackPos position of the table on the stack
/// \param key key to write value to
void export_set_field(GarrysMod::Lua::ILuaBase * lua, int iStackPos, const char * key);

/// Creates a new table and pushes it to the top of the stack
/// \param lua ILuaBase pointer
void export_create_table(GarrysMod::Lua::ILuaBase * lua);

/// Sets the metatable for the value at iStackPos to the value at the top of the stack. Pops the value off of the top of the stack
/// \param lua ILuaBase pointer
/// \param iStackPos stack position of the value to set metatable
void export_set_metatable(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Pushes the metatable of the value at iStackPos on to the top of the stack. Upon failure, returns false and does not push anything.
/// \param lua ILuaBase pointer
/// \param iStackPos position of the value on the stack
/// \return 1 on success and 0 on failure
int export_get_metatable(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Calls a function. To use it: Push the function on to the stack followed by each argument. Pops the function and arguments from the stack, leaves iResults values on the stack.
/// \param lua ILuaBase pointer
/// \param IArgs number of arguments of the function
/// \param iResults number of return values of the function
void export_call(GarrysMod::Lua::ILuaBase * lua, int IArgs, int iResults);

/// Similar to Call. Calls a function in protected mode. Both nargs and nresults have the same meaning as in lua_call.
/// If there are no errors during the call, lua_pcall behaves exactly like lua_call.
/// However, if there is any error, lua_pcall catches it, pushes a single value on the stack (the error message), and returns an error code.
/// Like lua_call, lua_pcall always removes the function and its arguments from the stack.
/// If errfunc is 0, then the error message returned on the stack is exactly the original error message.
/// Otherwise, errfunc is the stack index of an error handler function.
/// (In the current implementation, this index cannot be a pseudo-index.)
/// In case of runtime errors, this function will be called with the error message and its return value will be the message returned on the stack by lua_pcall.
/// \param lua ILuaBase pointer
/// \param IArgs number of arguments of the function
/// \param IResults number of return values of the function
/// \param ErrorFunc is the stack index of an error handler function
/// \return returns 0 in case of success or one of the error codes (defined by lua engine)
int export_p_call(GarrysMod::Lua::ILuaBase * lua, int IArgs, int IResults, int ErrorFunc);

/// Returns true if the values at iA and iB are equal
/// \param lua ILuaBase pointer
/// \param iA position of the first value to compare
/// \param iB position of the second value
/// \return 1 if true, 0 otherwise
int exports_equal(GarrysMod::Lua::ILuaBase * lua, int iA, int iB);

/// Returns true if the value at iA and iB are equal. Does not invoke metamethods.
/// \param lua ILuaStack pointer
/// \param iA position of the first value to compare
/// \param iB position of the second value
/// \return 1 if true, 0 otherwise
int export_raw_equal(GarrysMod::Lua::ILuaBase * lua, int iA, int iB);

/// Moves the value at the top of the stack in to iStackPos. Any elements above iStackPos are shifted upwards.
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
void export_insert(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Removes the value at iStackPos from the stack. Any elements above iStackPos are shifted downwards.
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
void export_remove(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Allows you to iterate tables similar to pairs(...). Pops a key from the stack, and pushes a key-value pair from the table at the given index (the "next" pair after the given key).
/// If there are no more elements in the table, then lua_next returns 0 (and pushes nothing).
/// \param lua ILuaBase pointer
/// \param iStackPos position of the table
/// \return If there are no more elements in the table, then lua_next returns 0 (and pushes nothing).
int export_next(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Throws an error and ceases execution of the function.
/// \param lua ILuaBase pointer
/// \param error_msg error message
void export_throw_error(GarrysMod::Lua::ILuaBase * lua, const char * error_msg);

/// Checks that the type of the value at iStackPos is iType. Throws and error and ceases execution of the function otherwise.
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack of the value to check type of
/// \param IType type index
void export_check_type(GarrysMod::Lua::ILuaBase * lua, int iStackPos, int IType);

/// Throws a pretty error message about the given argument
/// \param lua ILuaBase pointer
/// \param iArgNum index of the problematic argument
/// \param error_msg error message
void export_arg_error(GarrysMod::Lua::ILuaBase * lua, int iArgNum, const char * error_msg);

/// Returns the string at iStackPos. iOutLen is set to the length of the string if it is not NULL. If the value at iStackPos is a number, it will be converted in to a string.
/// Returns NULL upon failure.
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
/// \param iOutLength output parametr - length of the string
/// \return pointer to the string
const char * export_get_string(GarrysMod::Lua::ILuaBase * lua, int iStackPos, unsigned int * iOutLength);

/// Returns the number at iStackPos. Returns 0 upon failure.
/// \param lua ILuaBase pointer
/// \param iStackPos position of number of the stack
/// \return
double export_get_number(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Returns the boolean at iStackPos (as int). Returns false upon failure.
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
/// \return bool value represented as int (1 = true and 0 = false)
int export_get_bool (GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Returns the C-Function at iStackPos. Returns NULL upon failure.
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
/// \return
GarrysMod::Lua::CFunc export_get_c_function(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Pushes the given string on to the stack. If len is 0, strlen will be used to determine the string's length.
/// \param lua ILuaBase pointer
/// \param string C-string to push
/// \param len Length of the string (0 to evaluate length with strlen)
void export_push_string(GarrysMod::Lua::ILuaBase * lua, const char * string, unsigned int len);

/// Pushes the given double on to the stack
/// \param lua ILuaBase pointer
/// \param val Number to push
void export_push_number(GarrysMod::Lua::ILuaBase * lua, double val);

/// Pushes the given boolean on to the stack
/// \param lua ILuaBase pointer
/// \param val bool value to push represented as integer (0 = false and anything else is true)
void export_push_bool(GarrysMod::Lua::ILuaBase * lua, int val);

/// Pushes the given C-Function on to the stack
/// \param lua ILuaBase pointer
/// \param val function to push on stack
void export_push_c_function(GarrysMod::Lua::ILuaBase * lua, GarrysMod::Lua::CFunc val);

/// Pushes the given C-Function on to the stack with upvalues
/// \param lua ILuaBase pointer
/// \param val
/// \param iVars
void export_push_c_closure(GarrysMod::Lua::ILuaBase * lua, GarrysMod::Lua::CFunc val, int iVars);

/// Allows for values to be stored by reference for later use. Make sure you call ReferenceFree when you are done with a reference.
/// \param lua ILuaBase pointer
/// \return reference
int export_reference_create(GarrysMod::Lua::ILuaBase * lua);

/// Free reference
/// \param lua ILuaBase pointer
/// \param reference reference to free
void export_reference_free(GarrysMod::Lua::ILuaBase * lua, int reference);

/// Push reference on to the stack
/// \param lua ILuaBase pointer
/// \param reference reference to push
void export_reference_push(GarrysMod::Lua::ILuaBase * lua, int reference);

/// Push a special value onto the top of the stack (see SPECIAL_* enums)
/// \param lua ILuaBase pointer
/// \param table_type_number Index of special table
void export_push_special(GarrysMod::Lua::ILuaBase * lua, int table_type_number);

/// Returns true (1) if the value at iStackPos is of type iType
/// \param lua ILuaBase pointer
/// \param iStackPos position of value to check type of
/// \param iType type index
/// \return Bool represented as integer
int export_is_type(GarrysMod::Lua::ILuaBase * lua, int iStackPos, int iType);

/// Returns the type of the value at iStackPos
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
/// \return
int export_get_type(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Returns the name associated with the given type ID
/// \param lua ILuaBase pointer
/// \param iType type index
/// \return
const char * export_get_type_name(GarrysMod::Lua::ILuaBase * lua, int iType);

/// Returns the length of the object at iStackPos
/// \param lua ILuaBase pointer
/// \param iStackPos position on the stack
/// \return length of the object
int export_obj_len(GarrysMod::Lua::ILuaBase * lua, int iStackPos);

/// Returns the angle at iStackPos as a float array of components
/// \param lua ILuaBase pointer
/// \param out_angle_components output param: pointer to float array of angle components
/// \param iStackPos position of the angle on the stack
void export_get_angle(GarrysMod::Lua::ILuaBase * lua, float * out_angle_components, int iStackPos);

/// Returns the vector at iStackPos as a float array of components
/// \param lua ILuaBase pointer
/// \param out_vector_components output_param: pointer to the array of vector components
/// \param iStackPos position of vector on the stack
void export_get_vector(GarrysMod::Lua::ILuaBase * lua, float * out_vector_components, int iStackPos);

/// Pushes the given angle to the top of the stack
/// \param lua ILuaBase pointer
/// \param x Pitch
/// \param y Yaw
/// \param z Roll
void export_push_angle(GarrysMod::Lua::ILuaBase * lua, float x, float y, float z);

/// Pushes the given vector to the top of the stack
/// \param lua ILuaBase pointer
/// \param x
/// \param y
/// \param z
void export_push_vector(GarrysMod::Lua::ILuaBase * lua, float x, float y, float z);

/// Sets the lua_State to be used by the ILuaBase implementation
/// \param lua ILuaState pointer
/// \param state lua_State pointer
void export_set_state(GarrysMod::Lua::ILuaBase * lua, lua_State * state);

/// Pushes the metatable associated with the given type name
/// \param lua ILuaBase pointer
/// \param name name of the metatable
/// \return the type ID to use for this type
int export_create_metatable(GarrysMod::Lua::ILuaBase * lua, const char * name);

/// Pushes the metatable associated with the given type
/// \param lua ILuaBase pointer
/// \param iType type index
/// \return Success indicator bool as integer
int export_push_metatable(GarrysMod::Lua::ILuaBase * lua, int iType);

/// Creates a new UserData of type iType that references the given data
/// \param lua ILuaBase pointer
/// \param data user data
/// \param iType type id to associate data with
void export_push_user_type(GarrysMod::Lua::ILuaBase * lua, void * data, int iType);

/// Sets the data pointer of the UserType at iStackPos. You can use this to invalidate a UserType by passing NULL.
/// \param lua ILuaBase pointer
/// \param iStackPos position of object on the stack
/// \param data user data
void export_set_user_type(GarrysMod::Lua::ILuaBase * lua, int iStackPos, void * data);

/// Get ILuaBase pointer from the lua_State.
/// \param state lua_State to extract pointer from
/// \return extracted ILuaBase pointer
GarrysMod::Lua::ILuaBase * export_get_iluabase_from_the_lua_state(lua_State * state);

#endif //GM_DOTNET_NATIVE_LUAAPIEXPOSURE_H
