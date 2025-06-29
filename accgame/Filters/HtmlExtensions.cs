using System;
using System.Text.RegularExpressions;
using System.Web.Mvc;
public static class HtmlExtensions
{
    public static MvcHtmlString StripParagraphTags(this HtmlHelper htmlHelper, string input)
    {
        if (String.IsNullOrEmpty(input))
            return MvcHtmlString.Create(input);

        // Use Regex to remove <p> and </p> tags
        var output = Regex.Replace(input, @"<\/?p[^>]*>", String.Empty);
        return MvcHtmlString.Create(output);
    }
}
