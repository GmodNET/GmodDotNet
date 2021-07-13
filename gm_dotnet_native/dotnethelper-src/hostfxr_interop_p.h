// hostfxr_interop's template method definitions

template<typename T>
T hostfxr_interop::validate_call(const std::function<int(T*)>& function_call, const std::string& result_name, const std::string& error_msg)
{
    static_assert(std::is_pointer_v<T>, "T must be a pointer type");

    T result = nullptr;
    int error_code = function_call(&result);
    if(error_code != 0)
    {
        throw std::runtime_error(error_msg + " (error code: " + std::to_string(error_code) + ")");
    }
    if(result == nullptr)
    {
        throw std::runtime_error(result_name + " is null");
    }

    return result;
}

template<typename T>
T hostfxr_interop::load_gmodnet_main()
{
    get_function_pointer_fn get_function_pointer = managed_function_pointer_getter();

    return validate_call<T>(
        [&](auto* result) {
            return get_function_pointer(_T("GmodNET.Startup, GmodNET"),
                                        _T("Main"),
                                        UNMANAGEDCALLERSONLY_METHOD,
                                        nullptr,
                                        nullptr,
                                        reinterpret_cast<void**>(result));
        },
        "managed_main",
        "Unable to load managed entry point");
}
