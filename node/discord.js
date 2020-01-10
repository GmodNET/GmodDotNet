let webhook = process.argv[2];
let branch = process.argv[3];
let commit = process.argv[4];
let commit_message = process.argv[5];

const whook = require("webhook-discord");

const hook = new whook.Webhook(webhook);

hook.info("Travis CI", "**Successful build**: " + branch + " " + commit + " " + "\"" + commit_message + "\""
            + "\n\n Windows: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/Windows/" + branch + "/" + branch + "-" + commit + ".zip"
            + "\n\n"
            + "Linux: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/Linux/" + branch + "/" + branch + "-" + commit + ".tar.gz"
            + "\n\n"
            + "Osx: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/Osx/" + branch + "/" + branch + "-" + commit + ".tar.gz"
            + "\n\n"
            + "Lua client: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/Lua/" + branch + "/" + commit + "/" + "gm_dotnet_client.lua"
            + "\n\n"
            + "Lua server: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/Lua/" + branch + "/" + commit + "/" + "gm_dotnet_server.lua");
