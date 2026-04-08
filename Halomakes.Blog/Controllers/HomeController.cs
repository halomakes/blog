using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Halomakes.Blog.Models;

namespace Halomakes.Blog.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(PostsController.GetRecentPosts), "Posts");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}