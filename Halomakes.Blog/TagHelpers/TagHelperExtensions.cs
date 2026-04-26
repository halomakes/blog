using Lucide.Icons.TagHelper.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Halomakes.Blog.TagHelpers;

public static class TagHelperExtensions
{
    public static TagHelperOutput GetIcon(this TagHelper _, string iconName, IWebHostEnvironment environment,
        TagHelperContext context)
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