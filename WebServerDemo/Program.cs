using System.Net.Http;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

class Startup
{
    private const string HtmlForm = @"<form action='/HTML' method='POST'>
Name: <input type='text' name='Name'/>
Age: <input type='number' name='Age'/>
<input type='submit' value='Save'/>
</form>";
private const string DownloadForm = @"<form action='/Content' method='POST'>
<input type='submit' value ='Download Sites Content'/>
</form>";
    static async Task Main()
    {
        
        const string fileName = "content.txt";

        await DownloadSitesAsTextFile(
            fileName,
            new[]
            {
        "https://judge.softuni.org/",
        "https://softuni.org/"
            });
        var server = new HttpServer(routes =>
        {
            routes
                .MapGet("/", new TextResponse("Home Page"))
                .MapGet("/HTML", new HtmlResponse(HtmlForm))
                .MapPost("/HTML", new TextResponse("", AddFormDataAction))
                .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
                .MapGet("/Content", new HtmlResponse(DownloadForm))
                .MapPost("/Content", new TextFileResponse(fileName));
        });

        await server.StartAsync();
    }

    private static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";

        foreach (var (key, value) in request.FormData)
        {
            response.Body += $"{key} = {value}\n";
        }
    }
    
    private static async Task<string>
DownloadWebsiteContent(string url)
    {
        using var client = new HttpClient();

        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();

        return content.Substring(0, 2000);
    }

    private static async Task DownloadSitesAsTextFile(
    string fileName,
    string[] urls)
    {
        var tasks = urls
            .Select(DownloadWebsiteContent)
            .ToArray();

        var results = await Task.WhenAll(tasks);

        var joined = string.Join("\n\n", results);

        await File.WriteAllTextAsync(fileName, joined);
    }
}
