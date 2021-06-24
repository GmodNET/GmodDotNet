//
// Created by Gleb Krasilich on 06.10.2019.
//
#include "LuaAPIExposure.h"
#include <GarrysMod/Lua/Interface.h>
#include <GarrysMod/Lua/LuaBase.h>
#include <string>
#include <cstring>

using namespace std;

using namespace GarrysMod;

using namespace GarrysMod::Lua;

int export_top(ILuaBase * lua)
{
    return lua->Top();
}

void export_push(ILuaBase * lua, int iStackPos)
{
    lua->Push(iStackPos);
}

void export_pop(ILuaBase * lua, int IAmt)
{
    lua->Pop(IAmt);
}

void export_get_field(ILuaBase * lua, int iStackPos, const char * key)
{
    lua->GetField(iStackPos, key);
}

void export_set_field(ILuaBase * lua, int iStackPos, const char * key)
{
    lua->SetField(iStackPos, key);
}

void export_create_table(ILuaBase * lua)
{
    lua->CreateTable();
}

void export_set_metatable(ILuaBase * lua, int iStackPos)
{
    lua->SetMetaTable(iStackPos);
}

int export_get_metatable(ILuaBase * lua, int iStackPos)
{
    return static_cast<int>(lua->GetMetaTable(iStackPos));
}

void export_call(ILuaBase * lua, int IArgs, int iResults)
{
    lua->Call(IArgs, iResults);
}

int export_p_call(ILuaBase * lua, int IArgs, int IResults, int ErrorFunc)
{
    return lua->PCall(IArgs, IResults, ErrorFunc);
}

int exports_equal(ILuaBase * lua, int iA, int iB)
{
    return lua->Equal(iA, iB);
}

int export_raw_equal(ILuaBase * lua, int iA, int iB)
{
    return lua->RawEqual(iA, iB);
}

void export_insert(ILuaBase * lua, int iStackPos)
{
    lua->Insert(iStackPos);
}

void export_remove(ILuaBase * lua, int iStackPos)
{
    lua->Remove(iStackPos);
}

int export_next(ILuaBase * lua, int iStackPos)
{
    return lua->Next(iStackPos);
}

void export_throw_error(ILuaBase * lua, const char * error_msg)
{
    lua->ThrowError(error_msg);
}

void export_check_type(ILuaBase * lua, int iStackPos, int IType)
{
    lua->CheckType(iStackPos, IType);
}

void export_arg_error(ILuaBase * lua, int iArgNum, const char * error_msg)
{
    lua->ArgError(iArgNum, error_msg);
}

const char * export_get_string(ILuaBase * lua, int iStackPos, unsigned int * iOutLength)
{
    return lua->GetString(iStackPos, iOutLength);
}

double export_get_number(ILuaBase * lua, int iStackPos)
{
    return lua->GetNumber(iStackPos);
}

int export_get_bool (ILuaBase * lua, int iStackPos)
{
    return static_cast<int>(lua->GetBool(iStackPos));
}

CFunc export_get_c_function(ILuaBase * lua, int iStackPos)
{
    return lua->GetCFunction(iStackPos);
}

void export_push_nil(ILuaBase * lua)
{
    lua->PushNil();
}

void export_push_string(ILuaBase * lua, const char * string, unsigned int len)
{
    lua->PushString(string, len);
}

void export_push_number(ILuaBase * lua, double val)
{
    lua->PushNumber(val);
}

void export_push_bool(ILuaBase * lua, int val)
{
    lua->PushBool(static_cast<bool>(val));
}

void export_push_c_function(ILuaBase * lua, CFunc val)
{
    lua->PushCFunction(val);
}

void export_push_c_closure(ILuaBase * lua, CFunc val, int iVars)
{
    lua->PushCClosure(val, iVars);
}

int export_reference_create(ILuaBase * lua)
{
    return lua->ReferenceCreate();
}

void export_reference_free(ILuaBase * lua, int reference)
{
    lua->ReferenceFree(reference);
}

void export_reference_push(ILuaBase * lua, int reference)
{
    lua->ReferencePush(reference);
}

void export_push_special(ILuaBase * lua, int table_type_number)
{
    lua->PushSpecial(table_type_number);
}

int export_is_type(ILuaBase * lua, int iStackPos, int iType)
{
    return static_cast<int>(lua->IsType(iStackPos, iType));
}

int export_get_type(ILuaBase * lua, int iStackPos)
{
    return lua->GetType(iStackPos);
}

const char * export_get_type_name(ILuaBase * lua, int iType, int * out_name_len)
{
    const char * name =  lua->GetTypeName(iType);
    *out_name_len = static_cast<int>(strlen(name));
    return name;
}

int export_obj_len(ILuaBase * lua, int iStackPos)
{
    return lua->ObjLen(iStackPos);
}

void export_get_angle(ILuaBase * lua, float * out_angle_components, int iStackPos)
{
    const QAngle& angle = lua->GetAngle(iStackPos);

    out_angle_components[0] = angle.x;
    out_angle_components[1] = angle.y;
    out_angle_components[2] = angle.z;
}

void export_get_vector(ILuaBase * lua, float * out_vector_components, int iStackPos)
{
    const Vector& vector = lua->GetVector(iStackPos);

    out_vector_components[0] = vector.x;
    out_vector_components[1] = vector.y;
    out_vector_components[2] = vector.z;
}

void export_push_angle(ILuaBase * lua, float x, float y, float z)
{
    QAngle angle;

    angle.x = x;
    angle.y = y;
    angle.z = z;

    lua->PushAngle(angle);
}

void export_push_vector(ILuaBase * lua, float x, float y, float z)
{
    Vector vector;

    vector.x = x;
    vector.y = y;
    vector.z = z;

    lua->PushVector(vector);
}

void export_set_state(ILuaBase * lua, lua_State * state)
{
    lua->SetState(state);
}

int export_create_metatable(ILuaBase * lua, const char * name)
{
    return lua->CreateMetaTable(name);
}

int export_push_metatable(ILuaBase * lua, int iType)
{
    return static_cast<int>(lua->PushMetaTable(iType));
}

void export_push_user_type(ILuaBase * lua, void * data, int iType)
{
    lua->PushUserType(data, iType);
}

void export_set_user_type(ILuaBase * lua, int iStackPos, void * data)
{
    lua->SetUserType(iStackPos, data);
}

void * export_get_user_type(ILuaBase * lua, int iStackPos, int iType)
{
    return lua->GetUserType<void*>(iStackPos, iType);
}

ILuaBase * export_get_iluabase_from_the_lua_state(lua_State * state)
{
    return state->luabase;
}

void export_get_table(ILuaBase * lua, int iStackPos)
{
    lua->GetTable(iStackPos);
}

void export_set_table(ILuaBase * lua, int iStackPos)
{
    lua->SetTable(iStackPos);
}

void export_raw_get(ILuaBase * lua, int iStackPos)
{
    lua->RawGet(iStackPos);
}

void export_raw_set(ILuaBase * lua, int iStackPos)
{
    lua->RawSet(iStackPos);
}

void export_push_user_data(ILuaBase * lua, void * data)
{
    lua->PushUserdata(data);
}

const char * export_check_string(ILuaBase * lua, int iStackPos, int * output_string_length)
{
    const char * str = lua->CheckString(iStackPos);

    if(str != nullptr)
    {
        *output_string_length = static_cast<int>(strlen(str));
    }

    return str;
}

double export_check_number(ILuaBase * lua, int iStackPos)
{
    return lua->CheckNumber(iStackPos);
}

// ClosureSafeWrapper is used internally in export_push_c_function_safe
int ClosureSafeWrapper(lua_State * luaState)
{
    ILuaBase * lua = luaState->luabase;
    //Get actual function pointer form upvalue pseudoindex
    CFunc func_ptr = lua->GetCFunction(-10003);

    //Call func_ptr
    int arg_ret_num = func_ptr(luaState);

    if(arg_ret_num < 0)
    {
        const char * error_msg = lua->GetString(-1);
        lua->ThrowError(error_msg);
        return 0;
    }
    else
    {
        return arg_ret_num;
    }
}
void export_push_c_function_safe(GarrysMod::Lua::ILuaBase * lua, GarrysMod::Lua::CFunc safe_wrapper, GarrysMod::Lua::CFunc val)
{
    lua->PushCFunction(safe_wrapper);
    lua->PushCFunction(val);
    lua->PushCClosure(ClosureSafeWrapper, 2);
}


