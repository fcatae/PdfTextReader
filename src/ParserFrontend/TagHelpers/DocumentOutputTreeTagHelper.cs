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
            output.TagName = "pre";
            try
            {
                string text = _outputFiles.GetOutputTree(Name);

                var output_tree = new OutputTreeInfo();
                var tree = output_tree.Process(text);

                GenerateOutputRecursive(output, tree);
            }
            catch
            {

            }
            //output.Content.Append(tree);
        }

        void GenerateOutputRecursive(TagHelperOutput output, OutputTreeInfo.Node node)
        {
            if (node == null)
                return;

            if( node.Ident >= 0 )
            {
                output.Content.AppendHtml("<li>");
                output.Content.Append(node.Title);
                output.Content.AppendHtml("</li>");
            }

            if (node.Children == null)
                return;

            output.Content.AppendHtml("<ul>");

            foreach (var child in node.Children)
            {
                GenerateOutputRecursive(output, child);
            }

            output.Content.AppendHtml("</ul>");
        }
        
    }
}
