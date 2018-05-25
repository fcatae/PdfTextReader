using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ParserFrontend.Logic;

namespace ParserFrontend.TagHelpers
{
    public class ArticleHtmlTagHelper : TagHelper
    {
        public string Text { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.AppendHtml(Text);
        }
    }
}
