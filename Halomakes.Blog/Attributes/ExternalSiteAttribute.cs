namespace Halomakes.Blog.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ExternalSiteAttribute(string format, string? color = null, string? iconPath = null) : Attribute
{
    public string Format => format;
    public string? Color => color;
    public string? IconPath => iconPath;
}