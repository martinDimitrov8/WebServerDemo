using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
    private const string LoginForm = @"<form action='/Login' method='POST'>
Username: <input type='text' name='Username'/>
Password: <input type='text' name='Password'/>
<input type='submit' value ='Log In'/>
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
                .MapGet("/Cookies", new HtmlResponse("", AddCookiesAction))
                .MapGet("/Session", new TextResponse("", DisplaySessionInfoAction))
                .MapGet("/Login", new HtmlResponse(LoginForm))
                .MapPost("/Login", new HtmlResponse("", LoginAction))
                .MapGet("/Logout", new HtmlResponse("", LogoutAction))
                .MapGet("/UserProfile", new HtmlResponse("", GetUserDataAction))
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
    private static void AddCookiesAction(Request request, Response response)
    {
        var body = "";

        if (request.Cookies.Any())
        {
            foreach (var cookie in request.Cookies)
            {
                body += $"<p>{cookie.Name}: {cookie.Value}</p>";
            }
        }
        else
        {
            body = "<p>No cookies yet!</p>";
        }

        if (!request.Cookies.Contains("MyCookie"))
        {
            response.Cookies.Add(new Cookie("MyCookie", "TestValue"));
        }

        response.Body = body;
    }
    private static void DisplaySessionInfoAction(
    Request request,
    Response response)
    {
        if (!request.Session.ContainsKey(Session.CurrentDateKey))
        {
            response.Body = "<p>Current date stored!</p>";
        }
        else
        {
            response.Body =
                $"<p>Session created on: {request.Session[Session.CurrentDateKey]}</p>";
        }
    }
    private static void LoginAction(Request request, Response response)
    {
        request.Session.Clear();

        var username = request.FormData["Username"];
        var password = request.FormData["Password"];

        if (username == "user" && password == "user123")
        {
            request.Session[Session.UserKey] = username;
            response.Body = "<h2>Login successful!</h2>";
        }
        else
        {
            response.Body = LoginForm + "<p>Invalid credentials!</p>";
        }
    }
    private static void LogoutAction(Request request, Response response)
    {
        request.Session.Clear();
        response.Body = "<h2>Logged out successfully!</h2>";
    }
    private static void GetUserDataAction(
    Request request,
    Response response)
    {
        if (request.Session.ContainsKey(Session.UserKey))
        {
            response.Body =
                $"<h2>Welcome, {request.Session[Session.UserKey]}</h2>";
        }
        else
        {
            response.Body =
                "<p>You are not logged in. <a href='/Login'>Login</a></p>";
        }
    }
}
