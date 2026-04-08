using Halomakes.Blog.Models;
using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Halomakes.Blog.Controllers;

public class TagsController(PostsService postsService) : Controller
{
    [HttpGet("/tags")]
    public IActionResult GetTags()
    {
        var posts = postsService.GetPosts();
        var tags = posts
            .SelectMany(static p => p.Tags)
            .Distinct()
            .Select(t => new TagModel(t, posts.Count(p => p.Tags.Contains(t))))
            .OrderByDescending(static m => m.Count)
            .ToList();
        return View("Index", tags);
    }

    [HttpGet("/tags/{tag}")]
    public IActionResult GetPostsByTag(string tag)
    {
        ViewBag.Tag = tag;
        var posts = postsService.GetPosts()
            .Where(p => p.Tags.Contains(tag.ToLower()))
            .Take(10)
            .ToList();
        if (!posts.Any())
            return View("NotFound");
        return View("Posts", posts);
    }
}