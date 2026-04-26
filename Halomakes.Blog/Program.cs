using Halomakes.Blog.Providers;
using Halomakes.Blog.Services;
using Jering.Web.SyntaxHighlighters.HighlightJS;
using PhotoSauce.MagicScaler;
using PhotoSauce.NativeCodecs.Libheif;
using PhotoSauce.NativeCodecs.Libjpeg;
using PhotoSauce.NativeCodecs.Libpng;
using PhotoSauce.NativeCodecs.Libwebp;
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
        builder.Services.AddHighlightJS();
        builder.Services.AddTransient<ImageScalerService>();
        CodecManager.Configure(codecs =>
        {
            codecs.UseLibwebp();
            codecs.UseLibheif();
            codecs.UseLibjpeg();
            codecs.UseLibpng();
        });
        builder.Services.AddScoped<FilamentTrackerService>();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseStaticFiles();

        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;

            if (response.StatusCode == 404)
            {
                response.Redirect("/posts/not-found");
            }

            if (response.StatusCode is < 200 or >= 400)
            {
                response.StatusCode = 200;
            }
        });

        app.Run();
    }
}