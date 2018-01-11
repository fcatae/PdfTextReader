using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class CreateParagraphs : IAggregateStructure<TextLine, TextStructure>
    {
        TextStructure _structure;

        public void Init(TextLine line)
        {
            _structure = new TextStructure()
            {
                FontName = line.FontName,
                FontStyle = line.FontStyle,
                FontSize = line.FontSize,
                VSpacing = line.Breakline,
                TextAlignment = TextAlignment.JUSTIFY
            };            
        }

        public bool Aggregate(TextLine line)
        {
            if ((_structure.FontName != line.FontName) ||
                (_structure.FontStyle != line.FontStyle) ||
                (_structure.FontSize != line.FontSize))
                return false;
            
            // different page or column
            if (_structure.VSpacing == null)
                return false;

            // if there is next line
            if( line.VSpacing != null )
            {
                // too far
                if ((float)line.VSpacing > (float)line.FontSize / 2)
                    return false;

                // same spacing as structure
                if (!IsZero(_structure.VSpacing - line.VSpacing))
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

            // this is quite accurate
            bool isCentered = (lineset.All(t => IsZero(t.MarginLeft - t.MarginRight))
                                && lineset.Any(t => !IsZero(t.MarginLeft)));

            if ( isCentered )
            {
                _structure.TextAlignment = TextAlignment.CENTER;
            }

            var textArray = lineset.Select(t => t.Text);

            _structure.Text = String.Join("\n", textArray);

            return _structure;            
        }
                
        bool IsZero(float? value)
        {
            float error = 0.1F;

            return ((value > -error) && (value < error));                
        }

        bool IsAlmostZero(decimal value)
        {
            decimal error = 4M;

            return ((value > -error) && (value < error));
        }

        bool IsUpperCase(string text)
        {
            var upper = text.ToUpper();
            return (upper == text);
        }

    }
}
