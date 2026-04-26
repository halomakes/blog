namespace Halomakes.Blog.Models;

public record ScaledImage(string OriginalUrl, IList<ScaleStep> Steps);

public record ScaleStep(string Url, int Width);