using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Slugify;

namespace Halomakes.Blog.TagHelpers;

[HtmlTargetElement("h1", Attributes = nameof(Linkable))]
[HtmlTargetElement("h2", Attributes = nameof(Linkable))]
[HtmlTargetElement("h3", Attributes = nameof(Linkable))]
[HtmlTargetElement("h4", Attributes = nameof(Linkable))]
[HtmlTargetElement("h5", Attributes = nameof(Linkable))]
public class HeaderTagHelper(IWebHostEnvironment environment) : TagHelper
{
    public string? Slug { get; set; }

    public bool Linkable { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        const string idAttribute = "id";
        var slugHelper = new SlugHelper();
        var content = await output.GetChildContentAsync();
        var slug = Slug ?? slugHelper.GenerateSlug(content.GetContent());
        if (!output.Attributes.Any(static a => a.Name == idAttribute))
            output.Attributes.SetAttribute(idAttribute, slug);
        else
            slug = output.Attributes.First(static a => a.Name == idAttribute).Value as string;
        var link = new TagBuilder("a");
        link.Attributes.Add("href", $"#{slug}");
        link.InnerHtml.SetHtmlContent(this.GetIcon("link", environment, context));
        link.AddCssClass("header-link");
        content.AppendHtml(link);
        output.Content = content;
    }
}