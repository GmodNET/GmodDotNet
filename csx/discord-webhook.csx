#! "net5.0"

using System.Net.Http;
using System.Text.Json;

// Params order: Webhook URL, branch, commit, version
string webhook = Args[0];
string commit = Args[1];
string branch = Args[2];
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

string discord_text = "Version: `" + version + "`\n\n"
                        + @"Get last nightly builds at http://nightly.gmodnet.xyz";

DiscordMessage msg = new DiscordMessage
{
    embeds = new DiscordEmbedded[] { new DiscordEmbedded
        {
            type = "rich",
            title = "Nightly build of `GmodDotNet` for `" + branch + "` commit `" + commit.Substring(0, 7) + "`",
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
