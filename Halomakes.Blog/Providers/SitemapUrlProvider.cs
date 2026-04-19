using Sidio.Sitemap.Core;

namespace Halomakes.Blog.Providers;

public class SitemapUrlProvider : IBaseUrlProvider
{
    public Uri BaseUrl => new("https://blog.halomak.es", UriKind.Absolute);
}