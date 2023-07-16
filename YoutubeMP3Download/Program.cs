using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

Console.CursorVisible = false;

Console.Write("Enter youtube url: ");
string? url = Console.ReadLine();

if (url == null)
    return;

var vId = VideoId.TryParse(url);

int number = 1;
int totalNumber = 1;

if (vId.HasValue)
{
    string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string dir = Path.Combine(desktop, $"{DateTime.Now:dd.MM.yyyyy HH.mm.ss.fff}");
    Directory.CreateDirectory(dir);

    YoutubeClient cl = new();
    Video vid = await cl.Videos.GetAsync(vId.Value);
    await DownloadSingleVideo(dir, cl, vid);
}
else
{
    var pId = PlaylistId.TryParse(url);
    if (pId.HasValue)
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string dir = Path.Combine(desktop, $"{DateTime.Now:dd.MM.yyyyy HH.mm.ss.fff}");
        Directory.CreateDirectory(dir);

        YoutubeClient cl = new();
        var list = await cl.Playlists.GetVideosAsync(pId.Value);
        totalNumber = list.Count;
        foreach (var item in list)
        {
            await DownloadSingleVideo(dir, cl, item, true);
        }
    }
    else
    {
        Console.WriteLine("Invalid url.");
    }
}

async Task DownloadSingleVideo(string dir, YoutubeClient cl, IVideo vid, bool numbered = false)
{
    Console.WriteLine($"{vid.Title} - {vid.Author.ChannelTitle} ({vid.Duration})");
    Console.Write("Downloading");
    int cursorLeft = Console.CursorLeft;
    int cursorTop = Console.CursorTop;

    var manifest = await cl.Videos.Streams.GetManifestAsync(vid.Id);
    IStreamInfo bestAudio = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();
    string filePath = Path.Combine(dir, (numbered ? "" : number + "- ") + string.Concat(vid.Title.Split(Path.GetInvalidFileNameChars())) + ".mp3");
    await cl.Videos.Streams.DownloadAsync(bestAudio, filePath);
    Console.WriteLine("\rDownload Completed" + (numbered ? $" ({number++}/{totalNumber})" : ""));
}