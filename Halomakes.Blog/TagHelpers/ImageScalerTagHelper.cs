using Halomakes.Blog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

public class ImageScalerTagHelper
{
    [HtmlTargetElement("img", Attributes = "scaled", TagStructure = TagStructure.WithoutEndTag)]
    public class TagLinkTagHelper(ImageScalerService scaler) : TagHelper
    {
        [HtmlAttributeName("src")]
        public string SourcePath { get; set; } = null!;

        /// <summary>
        /// Percentage of container image is expected to occupy
        /// </summary>
        public decimal? Basis
        {
            get => field is > 0 ? field : 1;
            set => field = value;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var scaled = scaler.ScaleImage(SourcePath);

            output.Attributes.SetAttribute("src", SourcePath);
            var srcset = scaled.Steps
                .Select(static s => $"{s.Url} {s.Width}w");
            output.Attributes.SetAttribute("srcset", string.Join(", ", srcset));
            var sizes = scaled.Steps
                .Select((entry, idx) => (width: entry.Width,
                    next: scaled.Steps.Select(static s => s.Width).ElementAtOrDefault(idx + 1)))
                .Select(tuple =>
                    tuple.width > 0
                        ? $"(width <= {(int)(tuple.next * (1 / Basis))!}px) {tuple.width}px"
                        : $"{tuple.width}px");
            output.Attributes.SetAttribute("sizes", string.Join(", ", sizes));
            output.Attributes.SetAttribute("aspect", $"{Math.Clamp(scaled.Aspect, .5, 2):F1}");

            output.Attributes.SetAttribute("loading", "lazy");
        }
    }
}