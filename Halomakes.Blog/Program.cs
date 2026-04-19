using Halomakes.Blog.Providers;
using Halomakes.Blog.Services;
using Sidio.Sitemap.Core.Services;

namespace Halomakes.Blog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews()
            .AddRazorRuntimeCompilation();
        builder.Services.AddSingleton<PostsService>();
        builder.Services.AddDefaultSitemapServices<SitemapUrlProvider>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.MapStaticAssets();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.UseStatusCodePages(context =>
        {
            var response = context.HttpContext.Response;

            if (response.StatusCode == 404)
                response.Redirect($"/posts/404");

            // just keeping static generator from getting amgy
            if (response.StatusCode is < 200 or >= 300)
                response.StatusCode = 200;

            return Task.CompletedTask;
        });

        app.Run();
    }
}