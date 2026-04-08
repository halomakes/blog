using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

public partial class Program
{
    private static readonly Lazy<string> RootDirectory = new(() => AppDomain.CurrentDomain.BaseDirectory);
    private const string OutputDirectory = "wwwroot";
    private static WebApplicationFactory<Halomakes.Blog.Program>? factory;

    public static async Task Main(string[] args)
    {
        // var resources = new[]
        // {
        //     "dist/main.es.js",
        //     "dist/particles.es.js",
        //     "dist/theme.css",
        //     "snippets/2026_04_BurnCdWithPowershell.ps1",
        //     "favicon.svg",
        //     "favicon.ico",
        //     "apple-touch-icon.png",
        //     "favicon-96x96.png",
        //     "site.webmanifest",
        //     "web-app-manifest-192x192.png",
        //     "web-app-manifest-512x512.png"
        // };

        factory = new WebApplicationFactory<Halomakes.Blog.Program>();
        using var client = factory.CreateClient();


        var resources = GetStaticResources().ToList();
        var pages = GetApplicationRoutes().ToList();

        foreach (var resource in resources)
            await StoreResource(client, resource, resource);
        foreach (var page in pages)
            await StoreResource(client, page, string.IsNullOrEmpty(page) ? "index.html" : $"{page}/index.html");
        await StoreResource(client, "posts/404", "404.html");
    }

    private static async Task StoreResource(HttpClient client, string fetchUrl, string filePath)
    {
        var content = await client.GetStreamAsync(fetchUrl);
        Console.WriteLine($"Storing content from {fetchUrl} to {filePath}");
        await using var memoryStream = new MemoryStream();
        await content.CopyToAsync(memoryStream); // might need to read this multiple times
        await WriteFileAsync(filePath, memoryStream);
        if (filePath.Any(char.IsUpper)) // workaround case-sensitive gh-pages hosting
            await WriteFileAsync(filePath.ToLower(), memoryStream);
    }

    private static async Task WriteFileAsync(string filePath, Stream content)
    {
        var fullPath = Path.Combine(RootDirectory.Value, OutputDirectory, filePath);
        EnsureDirectoryExists(fullPath);
        content.Seek(default, SeekOrigin.Begin);
        var file = new FileStream(fullPath, FileMode.Create);
        await content.CopyToAsync(file);
        file.Close();
    }

    private static void EnsureDirectoryExists(string path)
    {
        var directory = Path.GetDirectoryName(path); // get directory if it's a file path
        if (directory is not null && !Directory.Exists(directory))
        {
            Console.WriteLine($"Creating directory {directory}...");
            Directory.CreateDirectory(directory);
        }
    }

    private static IEnumerable<string> GetApplicationRoutes()
    {
        var posts = factory!.Services.GetRequiredService<PostsService>().GetPosts();
        foreach (var post in posts)
        foreach (var slug in post.Slugs)
        {
            yield return $"posts/{post.PublishDate.Year}/{post.PublishDate.Month:00}/{slug}";
            if (post.PublishDate.Month < 10)
                yield return $"posts/{post.PublishDate.Year}/{post.PublishDate.Month}/{slug}";
        }

        foreach (var tag in posts.SelectMany(static p => p.Tags).Distinct())
            yield return $"tags/{tag}";

        yield return "tags";
        yield return "posts";
        yield return "";
        yield return "404";
    }

    private static IEnumerable<string> GetStaticResources()
    {
        var hostEnv = factory!.Services.GetRequiredService<IWebHostEnvironment>();
        return GetFilesRecursive("");

        IEnumerable<string> GetFilesRecursive(string path, string? parent = null)
        {
            foreach (var item in hostEnv.WebRootFileProvider.GetDirectoryContents(path))
            {
                if (!item.PhysicalPath?.Contains("wwwroot") ?? false)
                    continue;
                var relativeToRoot = parent is null
                    ? path
                    : Path.Combine(parent, path);
                if (item.IsDirectory)
                {
                    foreach (var r in GetFilesRecursive(item.Name, relativeToRoot))
                        yield return r;
                }

                if (item.PhysicalPath is not null &&
                    !item.PhysicalPath.EndsWith(".map", StringComparison.InvariantCultureIgnoreCase))
                    yield return Path.Combine(relativeToRoot, item.Name);
            }
        }
    }
}