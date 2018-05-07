using System;
using System.Collections.Generic;
using System.Text;
using PdfTextReader.Base;
using System.Linq;

namespace PdfTextReader.PDFCore
{
    class GroupLines : IProcessBlock
    {
        const float LARGE_SPACE_BETWEEN_WORDS = 10f;
        const float MINIMUM_CHARACTER_DISTANCE = 5f;
        const float SAME_LINE_DISTANCE = 0.1f;

        int statSubfonts = 0;
        int statBackspace = 0;

        public BlockPage Process(BlockPage page)
        {
            GroupFontLineHelper groupFont = null;
            BlockLine line = null;
            IBlock last = null;
            string last_hidden = null;
            var result = new BlockPage();
            
            foreach (var block in page.AllBlocks)
            {
                if( block is BlockHidden )
                {
                    var blockHidden = (BlockHidden)block;

                    if (last_hidden != null)
                        PdfReaderException.Warning("last_hidden != null: hidden text will be overwritten");

                    last_hidden = blockHidden.GetHiddenText();
                    continue;
                }

                if(last != null)
                {
                    if ( CheckSubfonts(line, (Block)block) )
                    {
                        bool isBackspace = CheckBackspace(line, block);

                        float endofblock = block.GetX() + block.GetWidth();
                        float endofline = line.GetX() + line.GetWidth();

                        if ( endofblock > endofline )
                        {
                            line.Width = block.GetX() + block.GetWidth() - line.GetX();
                        }

                        if (line.Width <= 0)
                            PdfReaderException.AlwaysThrow("line.Width <= 0");
                        
                        // conside same line: update text and Width
                        // we dont add space character (should we?)
                        line.Text += block.GetText();

                        // gather statistics
                        statBackspace += (isBackspace) ? 1 : 0;
                        statSubfonts++;

                        // does not update 'last' variable!!
                        continue;
                    }
                }
                
                if (( last == null ) || (CompareLine(block, last) != 0))
                {
                    var b = (Block)block;
                    string text = block.GetText();

                    if (last_hidden != null)
                    {
                        text = last_hidden + text;
                        last_hidden = null;
                    }

                    line = new BlockLine()
                    {
                        Text = text,
                        X = block.GetX(),
                        H = block.GetH(),
                        Width = block.GetWidth(),
                        Height = block.GetHeight(),

                        HasBackColor = b.HasBackColor,

                        HasLargeSpace = false,

                        // might be inaccurate 
                        FontFullName = b.FontFullName,
                        FontName = b.FontName,
                        FontSize = b.FontSize, // BE CAREFUL!
                        FontStyle = b.FontStyle
                        // now the settings are done in GroupFontLineHelper
                    };

                    // TODO: validar a entrada duas vezes

                    if (groupFont != null)
                        groupFont.Done();

                    groupFont = new GroupFontLineHelper(line, b);
                    
                    if (line.Width <= 0 || line.Height <= 0)
                        PdfReaderException.AlwaysThrow("line.Width <= 0 || line.Height <= 0");

                    result.Add(line);
                }
                else                
                {
                    string separator = (ShouldAddSpace(last, block)) ? " " : "";

                    // same line: update text and Width
                    float startOfBlock = block.GetX();
                    float endOfBlock = block.GetX() + block.GetWidth();
                    float endOfLine = line.GetX() + line.GetWidth();

                    line.Text += separator + block.GetText();
                    line.Width = block.GetX() + block.GetWidth() - line.GetX();
                    
                    if (line.Width <= 0)
                        PdfReaderException.AlwaysThrow("line.Width <= 0");

                    bool couldBeTable = ShouldAddLargeSpace(last, block);

                    if (couldBeTable)
                        line.HasLargeSpace = true;

                    // walking backwards
                    // very strict check: sometimes the start overlaps with the ending
                    //if (startOfBlock < endOfLine)
                    //    throw new InvalidOperationException();
                    // soft check: end of block should never that low unless it is an overlap
                    if (endOfBlock < endOfLine)
                        PdfReaderException.AlwaysThrow("endOfBlock < endOfLine", new IBlock[] { last, block });

                    groupFont.MergeFont((Block)block);
                }

                last = block;
            }

            if (groupFont != null)
                groupFont.Done();

            return result;
        }

        int CompareLine(IBlock a, IBlock b)
        {
            float diff = a.GetH() - b.GetH();

            if (Math.Abs(diff) < SAME_LINE_DISTANCE)
                return 0;

            return (diff > 0) ? 1 : -1;
        }

        bool ShouldAddLargeSpace(IBlock before, IBlock after)
        {
            float x1 = before.GetX() + before.GetWidth();
            float x2 = after.GetX();
            float distance = x2 - x1;
            
            return (distance > LARGE_SPACE_BETWEEN_WORDS);
        }

        bool ShouldAddSpace(IBlock before, IBlock after)
        {
            float wordSpacing = before.GetWordSpacing();
            float x1 = before.GetX() + before.GetWidth();
            float x2 = after.GetX();
            float distance = x2 - x1;
            
            if (wordSpacing == 0)
            {
                // WEIRD, but assume it is at the end of the line?
                // Do not add space
                return false;
            }

            return (distance > wordSpacing);
        }

        bool CheckSubfonts(Block normal, Block sub)
        {
            float subX1 = sub.GetX();
            float subH1 = sub.GetH();
            float subH2 = sub.GetH() + sub.GetHeight();
            float norX2 = normal.GetX() + normal.GetWidth();
            float norH1 = normal.GetH();
            float norH2 = normal.GetH() + normal.GetHeight();

            bool baselineSlightlyHigher = (norH1 < subH1) && (norH2 > subH1);
            bool fontSizeIsSmaller = (normal.FontSize > sub.FontSize);
            bool charactersAreClose = Math.Abs(subX1 - norX2) < MINIMUM_CHARACTER_DISTANCE;
            
            return baselineSlightlyHigher && fontSizeIsSmaller && charactersAreClose;
        }

        bool CheckBackspace(IBlock line, IBlock block)
        {
            float lineX = line.GetX() + line.GetWidth();
            float wordX = block.GetX();

            return (wordX < lineX);
        }

        class GroupFontLineHelper
        {
            readonly BlockLine _line;
            readonly GroupFontLineItem _currentFont;
            List<GroupFontLineItem> _conflictItems = null;

            public GroupFontLineHelper(BlockLine line, Block block)
            {
                _line = line;
                _currentFont = CreateFont(block);
            }

            // TODO: implementar a diferenca de fonte (MODEM)
            public void MergeFont(Block line)
            {
                if(HasConflict(line))
                {
                    if (_conflictItems == null)
                    {
                        _conflictItems = new List<GroupFontLineItem>();
                        _conflictItems.Add(_currentFont);
                    }                        

                    _conflictItems.Add(CreateFont(line));
                }
            }

            public void Done()
            {
                _line.FontFullName = _currentFont.FontFullName;
                _line.FontName = _currentFont.FontName;
                _line.FontSize = _currentFont.FontSize;
                _line.FontStyle = _currentFont.FontStyle;

                if ( _conflictItems != null )
                {
                    var regularItem = _conflictItems.Where(f => f.FontStyle == "Regular").FirstOrDefault();
                    if (regularItem != null)
                    {
                        _line.FontStyle = "Regular";
                        _line.FontName = regularItem.FontName;
                        _line.FontSize = regularItem.FontSize;

                        if (_conflictItems.Where(f => f.FontStyle == "Regular").Count() > 1)
                            PdfReaderException.Warning("Multiple regular fonts");
                    }
                }
            }

            bool HasFullConflict(Block block)
            {
                return ((_currentFont.FontFullName != block.FontFullName) ||
                    (_currentFont.FontName != block.FontName) ||
                    (_currentFont.FontSize != block.FontSize) ||
                    (_currentFont.FontStyle != block.FontStyle));
            }
            bool HasConflict(Block block)
            {
                return (_currentFont.FontStyle != block.FontStyle);
            }

            GroupFontLineItem CreateFont(Block block)
            {
                return new GroupFontLineItem
                {
                    FontFullName = block.FontFullName,
                    FontName = block.FontName,
                    FontSize = block.FontSize,
                    FontStyle = block.FontStyle
                };
            }
        }
        class GroupFontLineItem
        {
            public string FontFullName;
            public string FontName;
            public float FontSize;
            public string FontStyle;
        }
    }
}
