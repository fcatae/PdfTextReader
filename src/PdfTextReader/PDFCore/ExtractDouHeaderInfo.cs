using PdfTextReader.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfTextReader.PDFCore
{
    class ExtractDouHeaderInfo : IProcessBlock
    {
        PageInfoStats _pageInfoStats;

        const float CONSIDERED_VERY_SMALL_FONTSIZE = 0.3f;
        const float SAME_LINE_SMALL_FONTSIZE = 0.1f;

        readonly Regex _regexJornal = new Regex(@"(.+)Nº(.+)");
        readonly Regex _regexISSN = new Regex(@"ISSN(\d+)(.)(\d+)");
        readonly Regex _regexLocalData = new Regex(@"(.*),(.*),(\d+)de(jan|fev|mar|abr|mai|jun|jul|ago|set|out|nov|dez)(.*)de(\d+)");

        public PageInfoStats InfoStats => _pageInfoStats;

        public BlockPage Process(BlockPage page)
        {
            if (_pageInfoStats != null)
                return page;

            PageInfoStats pageInfo = new PageInfoStats();
            
            var headerInfo = new PageInfoStats.HeaderInfo();

            int fieldsCompleted = 0;
            int maxFields = 10;

            var lines = GetLines(page).Take(maxFields).ToArray();

            foreach (string text in lines)
            {
                if (fieldsCompleted == 3)
                    break;

                var matchISSN = _regexISSN.Match(text);
                var matchLocalData = _regexLocalData.Match(text);
                var matchJornal = _regexJornal.Match(text);

                if( matchISSN.Success )
                {
                    headerInfo.ISSN = matchISSN.Groups[1].Value + "-" + matchISSN.Groups[3].Value;
                    fieldsCompleted++;
                    continue;
                }

                if (matchLocalData.Success)
                {
                    headerInfo.Local = matchLocalData.Groups[1].Value;
                    headerInfo.DataDia = matchLocalData.Groups[2].Value;
                    headerInfo.DataYMD = matchLocalData.Groups[3].Value + "-" + matchLocalData.Groups[4].Value + "-" + matchLocalData.Groups[6].Value;
                    fieldsCompleted++;
                    continue;
                }
                
                if (matchJornal.Success)
                {
                    headerInfo.JornalAnoSupl = matchJornal.Groups[1].Value;
                    headerInfo.JornalEdicao = matchJornal.Groups[2].Value;
                    fieldsCompleted++;
                    continue;
                }
            }

            pageInfo.SetInfo(headerInfo);

            _pageInfoStats = pageInfo;

            return page;
        }

        public IEnumerable<string> GetLines(BlockPage page)
        {
            Block last_line = null;
            Block last_block = null;
            string last_text = String.Empty;

            foreach (var block in page.AllBlocks)
            {
                if ( last_block != null )
                {
                    string text = block.GetText();

                    if(NewBoxHeight(last_line, block))
                    {
                        string cleanText = last_text
                                            .Replace("_o_-", "_o")
                                            .Replace("_o", "º")
                                            .Replace(" ", "");

                        yield return cleanText;

                        last_line = null;
                        last_text = String.Empty;
                    }
                    else if (SmallerFont(last_line, (Block)block))
                    {
                        string smallText = "_" + block.GetText();
                        last_text += smallText;
                        continue;
                    }
                }

                if (last_line == null)
                {
                    last_line = (Block)block;
                }
                last_block = (Block)block;
                last_text += block.GetText();
            }
        }

        bool NewBoxHeight(IBlock height, IBlock next)
        {
            float averageHeight = next.GetH() + next.GetHeight() / 2.0F;

            return (averageHeight < height.GetH()) || (averageHeight > height.GetH() + height.GetHeight());
        }
        

        bool SmallerFont(Block height, Block next)
        {
            return (next.FontSize < height.FontSize - 2F);
        }
    }
}
