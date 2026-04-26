namespace Halomakes.Blog.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PostAttribute(string title, int year, int month, int day) : Attribute
{
    public string Title => title;
    public DateOnly Published => new DateOnly(year, month, day);
}

[AttributeUsage(AttributeTargets.Class)]
public class SlugAttribute(params string[] slugs) : Attribute
{
    public string[] Slugs { get; } = slugs;
}

[AttributeUsage(AttributeTargets.Class)]
public class TagsAttribute(params string[] tags) : Attribute
{
    public string[] Tags { get; } = tags;
}

[AttributeUsage(AttributeTargets.Class)]
public class OpenGraphAttribute(string description, string? thumbnail = null, string? thumbnailAlt = null) : Attribute
{
    public string Description => description;
    public string? Thumbnail => thumbnail;
    public string? ThumbnailAlt => thumbnailAlt;
}