using System.Reflection;
using System.Text.Encodings.Web;
using System.Web;
using Halomakes.Blog.Attributes;
using Halomakes.Blog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

[HtmlTargetElement("external-link", TagStructure = TagStructure.NormalOrSelfClosing)]
public class ExternalLinkTagHelper : TagHelper
{
    private static readonly IDictionary<ExternalSite, ExternalSiteAttribute> SiteInfo =
        Enum.GetValues<ExternalSite>()
            .Select(static v => (val: v, attrib: typeof(ExternalSite).GetField(v.ToString())!
                .GetCustomAttribute<ExternalSiteAttribute>()))
            .Where(static p => p.attrib is not null)
            .ToDictionary(static p => p.val, static p => p.attrib)!;

    public required string Value { get; set; }

    public ExternalSite? Site { get; set; }

    public required string Label { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.AddClass("external-link", HtmlEncoder.Default);

        ExternalSiteAttribute? siteInfo = null;
        if (Site is not null)
            SiteInfo.TryGetValue(Site.Value, out siteInfo);

        var iconPath = siteInfo?.IconPath ?? "generic.svg";
        var icon = new TagBuilder("img");
        icon.Attributes.Add("src", $"/images/logos/{iconPath}");
        output.Content.AppendHtml(icon);

        var siteLabel = new TagBuilder("span");
        siteLabel.InnerHtml.Append(Site?.ToString() ?? "External Link");
        output.Content.AppendHtml(siteLabel);

        if (Site is not null
            && SiteInfo.TryGetValue(Site.Value, out var colorInfo)
            && !string.IsNullOrEmpty(colorInfo.Color))
        {
            var styleAttribute = new TagHelperAttribute("style", $"--link-brand: {colorInfo.Color};",
                HtmlAttributeValueStyle.DoubleQuotes);
            output.Attributes.Add(styleAttribute);
        }

        var url = Site is not null
                  && SiteInfo.TryGetValue(Site.Value, out var linkInfo)
                  && !string.IsNullOrEmpty(linkInfo.Format)
            ? string.Format(linkInfo.Format, HttpUtility.UrlEncode(Value))
            : Value;
        output.Attributes.Add(new TagHelperAttribute("href", url));
        output.Attributes.Add(new TagHelperAttribute("target", "_blank"));

        var pageLabel = new TagBuilder("span");
        pageLabel.InnerHtml.Append(Label);
        output.Content.AppendHtml(pageLabel);
    }
}