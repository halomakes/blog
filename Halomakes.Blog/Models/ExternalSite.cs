using Halomakes.Blog.Attributes;

namespace Halomakes.Blog.Models;

public enum ExternalSite
{
    [ExternalSite("https://www.printables.com/model/{0}", "#fb6931", "printables.svg")]
    Printables,

    [ExternalSite("https://www.youtube.com/watch?v={0}", "#ff0000", "youtube.svg")]
    YouTube
}