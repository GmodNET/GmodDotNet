AddCSLuaFile()

function dotnet_load (module_name)
  require("dotnet")
  dotnet_internal_load(module_name)
end
