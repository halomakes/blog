using System.Reflection;
using Halomakes.Blog.Attributes;
using Halomakes.Blog.Models;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Slugify;
using TagsAttribute = Halomakes.Blog.Attributes.TagsAttribute;

namespace Halomakes.Blog.Services;

public class PostsService(ApplicationPartManager applicationPartManager)
{
    private IList<BlogPostModel>? _posts;

    public IList<BlogPostModel> GetPosts() => _posts ??= GeneratePostList();

    private List<BlogPostModel> GeneratePostList()
    {
        var feature = new ViewsFeature();
        applicationPartManager.PopulateFeature(feature);
        var views = feature.ViewDescriptors.Where(static v => v.RelativePath.StartsWith("/Views/Posts/"));
        var slugHelper = new SlugHelper();
        return views
            .Where(static v => v.Type is not null)
            .Select(v =>
            {
                var postAttribute = v.Type!.GetCustomAttribute<PostAttribute>();
                if (postAttribute is null)
                    return null;
                var slugAttributes = v.Type!.GetCustomAttributes<SlugAttribute>();
                var tagAttributes = v.Type!.GetCustomAttributes<TagsAttribute>();
                var ogAttribute = v.Type!.GetCustomAttribute<OpenGraphAttribute>();

                return new BlogPostModel(
                    v.RelativePath,
                    postAttribute!.Title,
                    postAttribute!.Published,
                    [..slugAttributes.SelectMany(static a => a.Slugs), slugHelper.GenerateSlug(postAttribute.Title)],
                    tagAttributes.SelectMany(static a => a.Tags).ToList(),
                    ogAttribute?.Description,
                    !string.IsNullOrEmpty(ogAttribute?.Thumbnail) && !string.IsNullOrEmpty(ogAttribute?.ThumbnailAlt)
                        ? (ogAttribute!.Thumbnail, ogAttribute!.ThumbnailAlt)
                        : null
                );
            })
            .Where(static p => p is not null)
            .Cast<BlogPostModel>()
            .ToList();
    }
}