using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

public partial class Program
{
    private static readonly Lazy<string> RootDirectory = new(() => AppDomain.CurrentDomain.BaseDirectory);
    private const string OutputDirectory = "wwwroot";
    private static WebApplicationFactory<Halomakes.Blog.Program>? factory;

    public static async Task Main(string[] _)
    {
        factory = new WebApplicationFactory<Halomakes.Blog.Program>();
        using var client = factory.CreateClient();
        client.Timeout = TimeSpan.FromMinutes(10);

        var pages = await GetApplicationRoutes(client).ToListAsync();
        foreach (var page in pages)
            await StoreResource(client, page, string.IsNullOrEmpty(page) ? "index.html" : $"{page}/index.html");

        var resources = GetStaticResources().ToList();
        foreach (var resource in resources)
            await StoreResource(client, resource, resource);

        await StoreResource(client, "posts/404", "404.html");
        await StoreResource(client, "sitemap.xml", "sitemap.xml");
        await StoreResource(client, "sitemap.txt", "sitemap.txt");
        await StoreResource(client, "", "index.html");
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

    private static async IAsyncEnumerable<string> GetApplicationRoutes(HttpClient client)
    {
        var sitemap = await client.GetStringAsync("sitemap.txt");
        foreach (var url in sitemap.Split(Environment.NewLine)
                     .Select(static u => u.TrimStart('/'))
                     .Select(u => string.Concat(u))
                     .Where(static u => !string.IsNullOrWhiteSpace(u)))
            yield return url;
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