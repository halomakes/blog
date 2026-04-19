using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Sidio.Sitemap.AspNetCore;
using Sidio.Sitemap.Core;

namespace Halomakes.Blog.Controllers;

public class SitemapController(PostsService postsService) : Controller
{
    [HttpGet]
    [Route("sitemap.xml")]
    public IActionResult Sitemap()
    {
        var nodes = BuildUrls()
            .Where(static u => !string.IsNullOrEmpty(u))
            .Select(static r => new SitemapNode(r!));
        var sitemap = new Sitemap(nodes);
        return new SitemapResult(sitemap);
    }

    [HttpGet]
    [Route("sitemap.txt")]
    public IActionResult PageList()
    {
        return Content(string.Join(Environment.NewLine, BuildUrls()), "text/plain", System.Text.Encoding.UTF8);
    }

    private IEnumerable<string?> BuildUrls()
    {
        yield return Url.Action(nameof(HomeController.Index), "home");
        yield return Url.Action(nameof(PostsController.GetRecentPosts), "posts");
        var posts = postsService.GetPosts();
        foreach (var postPage in posts
                     .SelectMany(static p => p.Slugs, (p, s) => (post: p, slug: s))
                     .Select(t => Url.Action(nameof(PostsController.GetPost), "posts",
                         new
                         {
                             slug = t.slug,
                             year = t.post.PublishDate.Year,
                             month = t.post.PublishDate.Month
                         })))
            yield return postPage;
        yield return Url.Action(nameof(TagsController.GetTags), "tags");
        foreach (var tag in posts
                     .SelectMany(static p => p.Tags)
                     .Distinct())
            yield return Url.Action(nameof(TagsController.GetPostsByTag), "tags",
                new { tag = tag });
    }
}