using Slugify;

namespace Halomakes.Blog.Models;

public record BlogPostModel(string ViewName, string Title, DateOnly PublishDate, IList<string> Slugs, IList<string> Tags);