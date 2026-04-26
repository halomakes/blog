using Slugify;

namespace Halomakes.Blog.Models;

public record BlogPostModel(
    string ViewName,
    string Title,
    DateOnly PublishDate,
    IList<string> Slugs,
    IList<string> Tags,
    string? Description = null,
    (string Url, string Alt)? Thumbnail = null);