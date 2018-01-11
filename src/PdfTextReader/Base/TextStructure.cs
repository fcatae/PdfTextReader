using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Base
{
    class TextStructure : IBlock 
    {
        public string FontName { get; set; }
        public float FontSize { get; set; }
        public string FontStyle { get; set; }
        public string Text { get; set; }
        public float MarginRight { get; set; }
        public float MarginLeft { get; set; }
        public float? AfterSpace { get; set; }
        public TextAlignment TextAlignment { get; set; }
        public bool HasBackColor { get; set; }
        //For ContentType
        public List<TextLine> Lines { get; set; }

        public int CountLines() => Lines.Count;

        public string GetText()
        {
            return _block.GetText();
        }

        public float GetX()
        {
            return _block.GetX();
        }

        public float GetH()
        {
            return _block.GetH();
        }

        public float GetWidth()
        {
            return _block.GetWidth();
        }

        public float GetHeight()
        {
            return _block.GetHeight();
        }

        public float GetWordSpacing()
        {
            return _block.GetWordSpacing();
        }

        IBlock _blockVariable = null;

        IBlock _block
        {
            get
            {
                if(_blockVariable == null)
                {
                    var bset = new BlockSet<IBlock>();
                    bset.AddRange(Lines);
                    _blockVariable = bset;
                }
                return _blockVariable;
            }
        }
    }
}
