using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

/**
 * Formats a code bock for use with highlight.js
 */
[HtmlTargetElement("code-snippet")]
[HtmlTargetElement("code", Attributes = "file")]
public class CodeSnippetTagHelper(IWebHostEnvironment environment) : TagHelper
{
    [HtmlAttributeName("file")]
    public required string Filename { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var filePath = $"snippets/{Filename}";
        var file = environment.WebRootFileProvider.GetFileInfo(filePath);
        if (file.Exists)
        {
            output.TagName = "div";
            output.AddClass("code-snippet", HtmlEncoder.Default);

            var toolbarDiv = new TagBuilder("div");

            var labelDiv = new TagBuilder("label");
            labelDiv.InnerHtml.SetContent(Filename);

            var downloadButton = new TagBuilder("a");
            downloadButton.MergeAttribute("href", $"/{filePath}");
            downloadButton.MergeAttribute("target", "_blank");
            downloadButton.Attributes.Add("title","Download");
            downloadButton.InnerHtml.SetHtmlContent("<i data-lucide=\"download\"></i>");

            var copyButton = new TagBuilder("a");
            copyButton.MergeAttribute("href", "#");
            copyButton.MergeAttribute("data-action", "copy");
            copyButton.MergeAttribute("title", "Copy to Clipboard");
            copyButton.InnerHtml.SetHtmlContent("<i data-lucide=\"clipboard-copy\"></i>");

            toolbarDiv.InnerHtml.AppendHtml(labelDiv);
            toolbarDiv.InnerHtml.AppendHtml(downloadButton);
            toolbarDiv.InnerHtml.AppendHtml(copyButton);

            var contentDiv = new TagBuilder("code");
            contentDiv.Attributes.Add("lang", file.Name[(file.Name.LastIndexOf('.') + 1)..]);
            await using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            contentDiv.InnerHtml.SetContent(content);

            output.Content.AppendHtml(toolbarDiv);
            output.Content.AppendHtml(contentDiv);
        }
        else
        {
            output.Content.SetContent("Couldn't load this code snippet, sorry.");
        }
    }
}