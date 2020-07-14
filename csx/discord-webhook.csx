#! "netcoreapp3.1"

using System.Net.Http;
using System.Text.Json;

// Params order: Webhook URL, branch, commit, version
string webhook = Args[0];
string branch = Args[1];
string commit = Args[2];
string version = Args[3];

struct DiscordEmbedded
{
    public string type {get; set;}
    public string title {get; set;}
    public string url {get; set;}
    public string description {get; set;}
    public int color {get; set;}
}

struct DiscordMessage
{
    public string content {get; set;}
    public DiscordEmbedded[] embeds {get; set;}
}

string discord_text = "Signed as version " + version
                    + "\n\n Windows: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/windows" + "/" + "windows-" + commit + ".zip"
                    + "\n\n"
                    + "Linux: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/linux" + "/" + "linux-" + commit + ".tar.gz"
                    + "\n\n"
                    + "macOS: https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/osx" + "/" + "osx-" + commit + ".tar.gz"
                    + "\n\n"
                    + "Lua client: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/Lua/" + commit + "/" + "gm_dotnet_client.lua"
                    + "\n\n"
                    + "Lua server: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/Lua/" + commit + "/" + "gm_dotnet_server.lua"
                    + "\n\n"
                    + "Debug assemblies: " + "https://gleb-krasilich.fra1.digitaloceanspaces.com/GmodDotNetBuilds/GitHubWorkflow/debug/debug-" + commit + ".tar.gz";

DiscordMessage msg = new DiscordMessage
{
    embeds = new DiscordEmbedded[] { new DiscordEmbedded
        {
            type = "rich",
            title = "Nightly build for `" + branch + "` commit `" + commit.Substring(0, 7) + "`",
            description = discord_text,
            url = "https://github.com/GlebChili/GmodDotNet/commit/" + commit,
            color = 65530
        }
    }
};

var http_client = new HttpClient();

var msg_content = new ReadOnlyMemoryContent(JsonSerializer.SerializeToUtf8Bytes<DiscordMessage>(msg));
msg_content.Headers.Add("Content-Type", "application/json");

var response = http_client.PostAsync(Args[0], msg_content).Result;

if(response.IsSuccessStatusCode)
{
    Console.WriteLine("Discord webhook was executed");
    return 0;
}
else
{
    Console.WriteLine("Unsuccessful Discord webhook execution. Response code " + (int)response.StatusCode);
    return 1;
}
