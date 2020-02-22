let webhook = process.argv[2];
let branch = process.argv[3];
let commit = process.argv[4];
let signed_version = process.argv[5];

const whook = require("webhook-discord");

const hook = new whook.Webhook(webhook);

hook.info("GitHub Actions", "**Nightly build**: " + branch + " " + commit + " " + "signed as ver. " + signed_version
    + "\n\n Windows: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/windows" + "/" + "windows-" + commit + ".zip"
    + "\n\n"
    + "Linux: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/linux" + "/" + "linux-" + commit + ".tar.gz"
    + "\n\n"
    + "macOS: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/osx" + "/" + "osx-" + commit + ".tar.gz"
    + "\n\n"
    + "Lua client: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/Lua/" + commit + "/" + "gm_dotnet_client.lua"
    + "\n\n"
    + "Lua server: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/Lua/" + commit + "/" + "gm_dotnet_server.lua");