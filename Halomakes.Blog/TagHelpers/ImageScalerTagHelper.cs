using Halomakes.Blog.Models;
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
            var rng = new Random(SourcePath.GetHashCode());
            var scaled = scaler.ScaleImage(SourcePath);

            output.Attributes.SetAttribute("src", SourcePath);
            var srcset = scaled.Steps
                .Select(static s => $"{s.Url} {s.Width}w");
            output.Attributes.SetAttribute("srcset", string.Join(", ", srcset));
            var sizes = scaled.Steps
                .Select((entry, idx) => (width: entry.Width,
                    next: scaled.Steps.Select(static s => s.Width).ElementAtOrDefault(idx + 1)))
                .Select(tuple =>
                    tuple.next > 0
                        ? $"(width <= {(int)(tuple.next * (1 / Basis))!}px) {tuple.width}px"
                        : $"{tuple.width}px");
            output.Attributes.SetAttribute("sizes", string.Join(", ", sizes));
            output.Attributes.SetAttribute("aspect", $"{scaled.Aspect:F1}");
            output.Attributes.SetAttribute("style", BuildDeterministicRandomStyle(scaled, rng));
            output.Attributes.SetAttribute("onclick", $"window.open('{scaled.Steps.Last().Url}', '_blank').focus()");

            output.Attributes.SetAttribute("loading", "lazy");
        }

        private static string BuildDeterministicRandomStyle(ScaledImage image, Random rng)
        {
            const double variability = 30;
            const double scale = 1.05;
            var random = Math.Cos(rng.NextDouble() * Math.PI * 2);
            var aspectAdjustment = Math.Sqrt((Math.Clamp(image.Aspect, .5, 2) - .5) / 1.5);
            var offset = random * variability * aspectAdjustment;
            var zIdx = rng.Next(0, 100);
            return $"--z-idx:{zIdx};--offset:{offset:F1}%";
        }
    }
}