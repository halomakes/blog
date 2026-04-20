using System.Text.Encodings.Web;
using Halomakes.Blog.Models;
using Halomakes.Blog.Services;
using Jering.Web.SyntaxHighlighters.HighlightJS;
using Lucide.Icons.TagHelper.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;

namespace Halomakes.Blog.TagHelpers;

/**
 * Formats a code bock for use with highlight.js
 */
[HtmlTargetElement("code-snippet")]
[HtmlTargetElement("code", Attributes = "file")]
public class CodeSnippetTagHelper(IWebHostEnvironment environment, IHighlightJSService highlighter) : TagHelper
{
    [HtmlAttributeName("file")]
    public required string Filename { get; set; }

    [HtmlAttributeName("lang")]
    public CodeLanguage? Language { get; set; }

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
            downloadButton.Attributes.Add("title", "Download");
            downloadButton.InnerHtml.SetHtmlContent(GetIcon("download"));

            var copyButton = new TagBuilder("a");
            copyButton.MergeAttribute("href", "#");
            copyButton.MergeAttribute("data-action", "copy");
            copyButton.MergeAttribute("title", "Copy to Clipboard");
            copyButton.InnerHtml.SetHtmlContent(GetIcon("clipboard-copy"));

            toolbarDiv.InnerHtml.AppendHtml(labelDiv);
            toolbarDiv.InnerHtml.AppendHtml(downloadButton);
            toolbarDiv.InnerHtml.AppendHtml(copyButton);

            var contentDiv = new TagBuilder("code");
            var language = GetLanguage(file);
            await using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var content = await reader.ReadToEndAsync();

            var highlighted = await highlighter.HighlightAsync(content, language.ToString().ToLower());

            var display = highlighted?.Replace(Environment.NewLine, "<br/>") ?? content;
            contentDiv.InnerHtml.SetHtmlContent(display);

            output.Content.AppendHtml(toolbarDiv);
            output.Content.AppendHtml(contentDiv);
        }
        else
        {
            output.Content.SetContent("Couldn't load this code snippet, sorry.");
        }

        TagHelperOutput GetIcon(string iconName)
        {
            var helper = new LucideIconTagHelper(environment)
            {
                Name = iconName
            };
            var resultContext =
                new TagHelperOutput("lucide-icon", [],
                    (_, _) => Task.FromResult(new DefaultTagHelperContent() as TagHelperContent));
            helper.Process(context, resultContext);
            return resultContext;
        }
    }

    private static CodeLanguage GetLanguage(IFileInfo file)
    {
        var extension = file.Name[(file.Name.LastIndexOf('.') + 1)..];
        if (Enum.TryParse<CodeLanguage>(extension, ignoreCase: true, out var lang))
            return lang;
        return extension?.ToLower() switch
        {
            "ps1" => CodeLanguage.PowerShell,
            "js" => CodeLanguage.JavaScript,
            "cs" => CodeLanguage.Csharp,
            "sh" => CodeLanguage.Shell,
            "md" => CodeLanguage.Markdown,
            "ts" => CodeLanguage.TypeScript,
            "make" => CodeLanguage.MakeFile,
            "kt" => CodeLanguage.Kotlin,
            "kts" => CodeLanguage.Kotlin,
            _ => CodeLanguage.PlainText
        };
    }
}