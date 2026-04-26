using System.Text.Encodings.Web;
using Halomakes.Blog.Controllers;
using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

[HtmlTargetElement("filament-tracker", TagStructure = TagStructure.NormalOrSelfClosing)]
public class FilamentTrackerTagHelper(FilamentTrackerService tracker) : TagHelper
{
    public required uint Grams { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        tracker.Use(Grams);

        var label = new TagBuilder("span");
        label.InnerHtml.Append("Filament Used");
        var values = new TagBuilder("dl");
        foreach (var tag in GetAmount("step", Grams).Concat(GetAmount("total", tracker.Total)))
            values.InnerHtml.AppendHtml(tag);
        output.Content.AppendHtml(label);
        output.Content.AppendHtml(values);
    }

    private static IEnumerable<TagBuilder> GetAmount(string label, uint amount)
    {
        var labelElement = new TagBuilder("dd");
        labelElement.InnerHtml.Append(label);
        yield return labelElement;
        var amountElement = new TagBuilder("dt");
        amountElement.InnerHtml.Append(amount > 1000 ? $"{(decimal)amount / 1000:d2}" : $"{amount:d0}");
        yield return amountElement;
    }
}