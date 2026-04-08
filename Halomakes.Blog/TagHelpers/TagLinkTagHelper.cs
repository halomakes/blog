using System.Text.Encodings.Web;
using Halomakes.Blog.Controllers;
using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

// tag tag tag tag tag tag tag tag tag tag tag tag
[HtmlTargetElement("tag-link")]
public class TagLinkTagHelper(IHtmlGenerator generator, PostsService postsService) : AnchorTagHelper(generator)
{
    [HtmlAttributeName("tag")]
    public required string Tag { get; set; }

    [HtmlAttributeName("count")]
    public int? Count { get; set; }

    [HtmlAttributeName("show-count")]
    public bool ShowCount { get; set; } = true;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.AddClass("tag", HtmlEncoder.Default);
        Controller = "Tags";
        Action = nameof(TagsController.GetPostsByTag);
        RouteValues = new Dictionary<string, string>
        {
            { "tag", Tag }
        };
        if (ShowCount)
        {
            var count = Count ?? postsService.GetPosts().Count(p => p.Tags.Contains(Tag.ToLower()));
            output.Content.SetHtmlContent($"{Tag} <span class=\"count\">({count})</span>");
        }
        else
        {
            output.Content.SetContent(Tag);
        }

        base.Process(context, output);
    }
}