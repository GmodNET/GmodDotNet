//
// Created by Gleb Krasilich on 06.10.2019.
//
#include "LuaAPIExposure.h"
#include <GarrysMod/Lua/Interface.h>
#include <GarrysMod/Lua/LuaBase.h>
#include <string>

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
    bool tmp_ret = lua->GetMetaTable(iStackPos);

    if(tmp_ret)
    {
        return 1;
    }
    else
    {
        return 0;
    }
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
    bool tmp_ret = lua->GetBool(iStackPos);

    if(tmp_ret)
    {
        return 1;
    }
    else
    {
        return 0;
    }
}

CFunc export_get_c_function(ILuaBase * lua, int iStackPos)
{
    return lua->GetCFunction(iStackPos);
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
    bool tmp;
    if(val == 0)
    {
        tmp = false;
    }
    else
    {
        tmp = true;
    }

    lua->PushBool(tmp);
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
    bool tmp = lua->IsType(iStackPos, iType);

    if(tmp)
    {
        return 1;
    }
    else
    {
        return 0;
    }
}

int export_get_type(ILuaBase * lua, int iStackPos)
{
    return lua->GetType(iStackPos);
}

const char * export_get_type_name(ILuaBase * lua, int iType)
{
    return lua->GetTypeName(iType);
}

int export_obj_len(ILuaBase * lua, int iStackPos)
{
    return lua->ObjLen(iStackPos);
}

void export_get_angle(ILuaBase * lua, float * out_angle_components, int iStackPos)
{
    const QAngle& tmp = lua->GetAngle(iStackPos);

    out_angle_components[0] = tmp.x;
    out_angle_components[1] = tmp.y;
    out_angle_components[2] = tmp.z;
}

void export_get_vector(ILuaBase * lua, float * out_vector_components, int iStackPos)
{
    const Vector& tmp = lua->GetVector(iStackPos);

    out_vector_components[0] = tmp.x;
    out_vector_components[1] = tmp.y;
    out_vector_components[2] = tmp.z;
}

void export_push_angle(ILuaBase * lua, float x, float y, float z)
{
    QAngle tmp;

    tmp.x = x;
    tmp.y = y;
    tmp.z = z;

    lua->PushAngle(tmp);
}

void export_push_vector(ILuaBase * lua, float x, float y, float z)
{
    Vector tmp;

    tmp.x = x;
    tmp.y = y;
    tmp.z = z;

    lua->PushVector(tmp);
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
    bool tmp = lua->PushMetaTable(iType);

    if(tmp)
    {
        return 1;
    }
    else
    {
        return 0;
    }
}

void export_push_user_type(ILuaBase * lua, void * data, int iType)
{
    lua->PushUserType(data, iType);
}

void export_set_user_type(ILuaBase * lua, int iStackPos, void * data)
{
    lua->SetUserType(iStackPos, data);
}

ILuaBase * export_get_iluabase_from_the_lua_state(lua_State * state)
{
    return state->luabase;
}

