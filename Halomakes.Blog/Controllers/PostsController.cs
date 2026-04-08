using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Halomakes.Blog.Controllers;

public class PostsController(PostsService postsService) : Controller
{
    [HttpGet("/posts/{year:int}/{month:int}/{slug}")]
    public IActionResult GetPost(int year, int month, string slug)
    {
        var post = postsService.GetPosts()
            .FirstOrDefault(p => p.PublishDate.Year == year
                                 && p.PublishDate.Month == month
                                 && p.Slugs.Contains(slug.ToLower()));
        return post is not null
            ? View("Post", post)
            : View("NotFound");
    }

    [HttpGet("/posts")]
    public IActionResult GetRecentPosts()
    {
        var posts = postsService.GetPosts()
            .OrderByDescending(static p => p.PublishDate)
            .Take(10)
            .ToList();
        return View("Recent", posts);
    }
}