#! "net5.0"
#r "nuget: GmodNET.VersionTool.Core, 2.0.0"

using System.Net.Http;
using System.Text.Json;

// Params order: Webhook URL, branch, commit, version
string webhook = Args[0];
string commit = Args[1];

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

GmodNET.VersionTool.Core.VersionGenerator ver_gen = new GmodNET.VersionTool.Core.VersionGenerator("../version.json");

string discord_text = "Version: `" + ver_gen.VersionWithoutBuildData + "`\n\n"
                        + @"Get last nightly builds at http://nightly.gmodnet.xyz";

string branch = ver_gen.BranchName;

DiscordMessage msg = new DiscordMessage
{
    embeds = new DiscordEmbedded[] { new DiscordEmbedded
        {
            type = "rich",
            title = "Nightly build of `GmodDotNet` for branch `" + branch + "` commit `" + commit.Substring(0, 7) + "`",
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
