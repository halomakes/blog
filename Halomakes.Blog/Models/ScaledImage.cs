namespace Halomakes.Blog.Models;

public record ScaledImage(string OriginalUrl, IList<ScaleStep> Steps, double Aspect);

public record ScaleStep(string Url, int Width);

public record Thumbnail(string Url, string Type, int Width, int Height);