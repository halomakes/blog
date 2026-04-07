using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

/**
 * Formats a code bock for use with highlight.js
 */
[HtmlTargetElement("code-snippet")]
[HtmlTargetElement("code", Attributes = "lang")]
public class CodeSnippetTagHelper : TagHelper
{
    [HtmlAttributeName("lang")]
    public required string Language { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "code";
        output.Attributes.Add("lang", Language);
        output.AddClass($"code-{Language}", HtmlEncoder.Default);
    }
}