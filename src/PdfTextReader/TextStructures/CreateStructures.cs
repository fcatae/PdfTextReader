using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class CreateStructures : IAggregateStructure<TextLine, TextStructure>
    {
        const float difference_margin_center_text = 1F;

        TextStructure _structure;

        public void Init(TextLine line)
        {
            _structure = new TextStructure()
            {
                FontName = line.FontName,
                FontStyle = line.FontStyle,
                FontSize = line.FontSize,
                AfterSpace = line.AfterSpace,
                TextAlignment = TextAlignment.JUSTIFY,
                HasBackColor = line.HasBackColor
            };            
        }

        public bool Aggregate(TextLine line)
        {
            if ((_structure.FontName != line.FontName) ||
                (_structure.FontStyle != line.FontStyle) ||
                (_structure.FontSize != line.FontSize))
                return false;
            
            // different page or column
            if (_structure.AfterSpace == null)
                return false;

            // if there is next line
            if( line.BeforeSpace != null )
            {
                // too far
                if ((float)line.BeforeSpace > (float)line.FontSize / 2)
                    return false;

                // same spacing as structure
                if (!IsZero(_structure.AfterSpace - line.BeforeSpace))
                    return false;
            }


            return true;
        }

        public TextStructure Create(List<TextLine> lineset)
        {
            _structure.Lines = lineset;
            
            // just a wild guess
            if ( lineset[0].MarginRight < lineset[0].MarginLeft/2 )
            {
                if (lineset.Count == 1)
                    _structure.TextAlignment = TextAlignment.RIGHT;
                else
                {
                    if(!IsZero(lineset[0].MarginRight))
                        _structure.TextAlignment = TextAlignment.RIGHT;
                }
            }

            // this is slightly wrong... needs to work on this later
            if (lineset[0].MarginRight < lineset[0].MarginLeft / 2)
            {
                if (lineset.Count == 1)
                {
                    _structure.TextAlignment = TextAlignment.RIGHT;
                }
                else if (lineset.Count >= 2)
                {
                    bool marginRightLine1 = IsZero(lineset[0].MarginRight);
                    bool marginLeftLine2 = !IsZero(lineset[1].MarginLeft);

                    if(marginRightLine1 && marginLeftLine2)
                        _structure.TextAlignment = TextAlignment.RIGHT;
                } 
            }

            // this is quite accurate
            bool isCentered = (lineset.All(t => IsZero(t.MarginLeft - t.MarginRight))
                            && lineset.Any(t => !IsZero(t.MarginLeft)));

            if ( isCentered )
            {
                _structure.TextAlignment = TextAlignment.CENTER;
            }

            var textArray = lineset.Select(t => t.Text);

            _structure.Text = String.Join("\n", textArray);

            _structure.MarginLeft = lineset.Min(l => l.MarginLeft);
            _structure.MarginRight = lineset.Min(l => l.MarginRight);

            return _structure;            
        }
                
        bool IsZero(float? value)
        {
            return ((value > -difference_margin_center_text) && (value < difference_margin_center_text));                
        }

        bool IsUpperCase(string text)
        {
            var upper = text.ToUpper();
            return (upper == text);
        }

    }
}
