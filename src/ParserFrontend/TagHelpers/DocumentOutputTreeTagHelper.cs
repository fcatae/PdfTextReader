using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ParserFrontend.Logic;

namespace ParserFrontend.TagHelpers
{
    public class DocumentOutputTreeTagHelper : TagHelper
    {
        private OutputFiles _outputFiles;

        public string Name { get; set; }

        public DocumentOutputTreeTagHelper()
        {
            var web = new WebVirtualFS();
            _outputFiles = new OutputFiles(web);
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Append(_outputFiles.GetOutputTree(Name).ToString());
        }
    }
}
