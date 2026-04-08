using Halomakes.Blog.Services;

namespace Halomakes.Blog;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
        builder.Services.AddControllersWithViews()
            .AddRazorRuntimeCompilation();
        builder.Services.AddSingleton<PostsService>();

        var app = builder.Build();

// Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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