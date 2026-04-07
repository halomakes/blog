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
        output.TagName = "code";
        var file = environment.WebRootFileProvider.GetFileInfo($"snippets/{Filename}");
        if (file.Exists)
        {
            output.Attributes.Add("lang", file.Name[(file.Name.LastIndexOf('.') + 1)..]);
            await using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();
            output.Content.SetContent(content);
        }
        else
        {
            output.Content.SetContent("Couldn't load this code snippet, sorry.");
        }
    }
}