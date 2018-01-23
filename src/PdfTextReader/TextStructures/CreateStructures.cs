using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class CreateStructures : IAggregateStructure<TextLine, TextStructure>
    {
        const float FLOATING_TEXT_RIGHT = 10f;
        const float MAXIMUM_CENTER_DIFFERENCE = 1f;
        const float MAXIMUM_CENTER_MARGIN = 4F;
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
                if ((float)line.BeforeSpace > (float)line.FontSize * 0.75)
                    return false;

                // same spacing as structure
                if (!IsZero(_structure.AfterSpace - line.BeforeSpace))
                    return false;

                //has margin at second line
                //if (!IsZero(line.MarginLeft))
                //{
                //    //has not the same margin
                //    if (!IsZero(_structure.MarginLeft - line.MarginLeft))
                //        return false;
                //}
            }

            // update the current space        
            _structure.AfterSpace = line.AfterSpace;

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

            bool isFloatingLine = false;

            // ONE line with zero margin LEFT and RIGHT
            // Can be considered CENTERED if it is Bold or Uppercase
            if (lineset.Count == 1)
            {
                bool lineMarginLeft = IsZero(lineset[0].MarginLeft);
                bool lineMarginRight = IsZero(lineset[0].MarginRight);
                bool titleBold = lineset[0].FontStyle == "Bold";
                bool titleUpper = IsUpperCase(lineset[0].Text);

                if(lineMarginLeft && lineMarginRight)
                {
                    if( titleBold || titleUpper )
                    {
                        _structure.TextAlignment = TextAlignment.CENTER;
                    }
                }
            }

            if ( lineset.Count > 1 )
            {
                bool firstLineMarginLeft = IsZero(lineset[0].MarginLeft);
                bool secondLineMarginLeft = IsZero(lineset[1].MarginLeft);

                bool checkFloatingLine = !(firstLineMarginLeft && secondLineMarginLeft);

                if (checkFloatingLine)
                {
                    bool firstLineMarginZero = IsZero(lineset[0].MarginLeft) && IsZero(lineset[0].MarginRight);

                    int firstValidCenter = 0;

                    // if the first line has zero margin at RIGHT and LEFT, then consider the second line
                    if (firstLineMarginZero)
                    {
                        firstValidCenter = 1;
                    }

                    float structureCenterAt = lineset[firstValidCenter].CenteredAt;

                    isFloatingLine = true;
                    _structure.CenteredAt = structureCenterAt;

                    foreach (var line in lineset)
                    {
                        if (Math.Abs(line.CenteredAt - structureCenterAt) > MAXIMUM_CENTER_DIFFERENCE)
                        {
                            isFloatingLine = false;
                            _structure.CenteredAt = null;
                            break;
                        }
                    }
                }
            }

            if (isFloatingLine && (_structure.CenteredAt == null))
                throw new InvalidOperationException();

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
            bool isCentered = (lineset.All(t => IsZeroCenter(t.MarginLeft - t.MarginRight))
                            && lineset.Any(t => !IsZeroCenter(t.MarginLeft)));
            
            if ( isCentered )
            {
                _structure.TextAlignment = TextAlignment.CENTER;
            }
            
            var textArray = lineset.Select(t => t.Text);

            _structure.Text = String.Join("\n", textArray);

            _structure.MarginLeft = lineset.Min(l => l.MarginLeft);
            _structure.MarginRight = lineset.Min(l => l.MarginRight);

            // if the text has a CENTER, but it is slightly to the right
            // consider this a RIGHT ALIGNMENT
            if(_structure.CenteredAt != null && _structure.TextAlignment == TextAlignment.JUSTIFY )
            {
                if (_structure.CenteredAt >  FLOATING_TEXT_RIGHT)
                {
                    _structure.TextAlignment = TextAlignment.RIGHT;
                }
            }

            // Centralized text must NOT finish with "."
            if(( _structure.TextAlignment == TextAlignment.CENTER ) && _structure.Text.EndsWith("."))
            {
                _structure.TextAlignment = TextAlignment.JUSTIFY;

                if (lineset.Count != 1)
                    PdfReaderException.Throw("We only tested for 1 line");
            }

            if( lineset[0].HasLargeSpace )
            {
                _structure.TextAlignment = TextAlignment.JUSTIFY;
            }

            return _structure;            
        }
                
        bool IsZero(float? value)
        {
            return ((value > -difference_margin_center_text) && (value < difference_margin_center_text));                
        }
        bool IsZeroCenter(float? value)
        {
            return ((value > -MAXIMUM_CENTER_MARGIN) && (value < MAXIMUM_CENTER_MARGIN));
        }

        bool IsUpperCase(string text)
        {
            var upper = text.ToUpper();
            return (upper == text);
        }

    }
}
