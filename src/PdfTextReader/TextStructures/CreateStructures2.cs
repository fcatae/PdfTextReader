using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class CreateStructures2 : IAggregateStructure<TextLine2, TextStructure>
    {
        const float FLOATING_TEXT_RIGHT = 10f;
        const float MAXIMUM_CENTER_DIFFERENCE = 1f;
        const float MAXIMUM_CENTER_MARGIN = 4F;
        const float difference_margin_center_text = 1F;

        float STAT_FIRST_TABSTOP = float.NaN;

        TextStructure _structure;
        TextLine2 _last = null;

        public void Init(TextLine2 line)
        {
            _last = line;
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

        public bool Aggregate(TextLine2 next)
        {
            var current = _last;
            _last = next;

            if ((_structure.FontName != next.FontName) ||
                (_structure.FontStyle != next.FontStyle) ||
                (_structure.FontSize != next.FontSize))
                return false;
            
            // different page or column
            if (_structure.AfterSpace == null)
                return false;

            // if there is next line
            if( next.BeforeSpace != null )
            {
                // too far
                if ((float)next.BeforeSpace > (float)next.FontSize * 0.75)
                    return false;

                // same spacing as structure
                if (!IsZero(_structure.AfterSpace - next.BeforeSpace))
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
            _structure.AfterSpace = next.AfterSpace;

            if (current.AlignedCenter && (!next.AlignedCenter) && (!next.HasContinuation))
                return false;

            return true;
        }

        public TextStructure Create(List<TextLine2> lineset)
        {
            _structure.Lines = lineset.Cast<TextLine>().ToList();
            
            // just a wild guess
            if ( lineset[0].MarginRight < lineset[0].MarginLeft/2 )
            {
                if (lineset.Count == 1)
                    _structure.TextAlignment = TextAlignment.RIGHT;
                else
                {
                    // floating right?
                    if (!IsZero(lineset[0].MarginRight))
                    {
                        _structure.TextAlignment = TextAlignment.RIGHT;
                    }
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
            if(( _structure.TextAlignment == TextAlignment.CENTER ) && lineset[0].Text.EndsWith("."))
            {
                _structure.TextAlignment = TextAlignment.JUSTIFY;

                if (_structure.FontStyle == "Bold" )
                {
                    PdfReaderException.Warning("It should be a title");
                }

                if (lineset.Count != 1)
                {
                    PdfReaderException.Warning("We only tested for 1 line");
                    // sometimes... 
                    // _structure.Text.EndsWith(".") fails
                }
            }
                        
            if( lineset[0].HasLargeSpace )
            {
                _structure.TextAlignment = TextAlignment.JUSTIFY;
            }

            float boundaryLeft = BoundaryLeft(lineset);
            float boundaryRight = BoundaryRight(lineset);

            // cannot be on the right - with text in the left
            if (_structure.TextAlignment == TextAlignment.RIGHT && IsZero(boundaryLeft))
            {
                _structure.TextAlignment = TextAlignment.JUSTIFY;
            }

            //// hack?
            //if((_structure.TextAlignment == TextAlignment.JUSTIFY) && float.IsNaN(STAT_FIRST_TABSTOP))
            //{
            //    if(!IsZero(lineset[0].MarginLeft))
            //        STAT_FIRST_TABSTOP = lineset[0].MarginLeft;
            //}

            //if (_structure.TextAlignment == TextAlignment.RIGHT && IsZero(StatAtFirstTabStop(lineset[0].MarginLeft)))
            //{
            //    _structure.TextAlignment = TextAlignment.JUSTIFY;
            //}
            var align = GetAligmnent(lineset);

            if( align != _structure.TextAlignment )
            {
                PdfReaderException.Warning("aligment calculation diverging");
                _structure.TextAlignment = align;
                var text = String.Join("\n", _structure.Lines.Select(t=>t.Text));

                GetAligmnent(lineset);
            }

            _structure.TabStop = lineset[0].MarginLeft;
            _structure.MarginLeft = lineset.Min(t => t.MarginLeft);

            return _structure;            
        }

        TextAlignment GetAligmnent(List<TextLine2> lineset)
        {
            if (lineset == null || lineset.Count == 0)
                throw new ArgumentNullException();

            if (HasInlineTables(lineset))
                return TextAlignment.JUSTIFY;

            TextAlignment alignment = TextAlignment.UNKNOWN;

            float boundaryLeft = BoundaryLeft(lineset);
            float boundaryRight = BoundaryRight(lineset);

            if (lineset.Count == 1)
            {
                float centeredAt = lineset[0].CenteredAt;
                string text = lineset[0].Text;

                // check boundary distance (small text)
                alignment = (boundaryLeft < boundaryRight) ? TextAlignment.JUSTIFY : TextAlignment.RIGHT;

                if (IsZero(centeredAt))
                {
                    alignment = TextAlignment.CENTER;
                }

                // overrides small margin right
                if ((alignment == TextAlignment.RIGHT) && StatBeforeFirstTabStop(boundaryLeft))
                {
                    alignment = TextAlignment.JUSTIFY;
                }

                // text is centered AND justified
                float guessTabStop = StatAtFirstTabStop(boundaryLeft);
                if ((alignment == TextAlignment.CENTER) && IsZero(guessTabStop))
                {
                    // very difficult to decide
                    //bool centerHasPrecision = Math.Abs(centeredAt) < Math.Abs(guessTabStop);
                    //alignment = centerHasPrecision ? TextAlignment.CENTER : TextAlignment.JUSTIFY;

                    // prefer center
                    alignment = TextAlignment.CENTER;
                }

                if (IsZero(boundaryRight) && IsZero(boundaryLeft))
                {
                    // CENTER or JUSTIFIED
                    // choose center
                    alignment = TextAlignment.CENTER;

                    // Exception 1: summary
                    if (text.Contains("..."))
                        alignment = TextAlignment.JUSTIFY;

                    // Exception 2: empty
                    if (text == "")
                        alignment = TextAlignment.JUSTIFY;
                }
            }
            else if (lineset.Count == 2)
            {
                float center1 = lineset[0].CenteredAt;
                float center2 = lineset[1].CenteredAt;
                float left1 = lineset[0].MarginLeft;
                float left2 = lineset[1].MarginLeft;
                float right1 = lineset[0].MarginRight;
                float right2 = lineset[1].MarginRight;

                // use general rule
                alignment = (boundaryLeft < boundaryRight) ? TextAlignment.JUSTIFY : TextAlignment.RIGHT;

                if (IsZero(boundaryRight) && IsZero(boundaryLeft))
                {
                    alignment = TextAlignment.JUSTIFY;
                }

                // overrides small margin right
                if ((alignment == TextAlignment.RIGHT) && StatBeforeFirstTabStop(boundaryLeft))
                {
                    alignment = TextAlignment.JUSTIFY;
                }

                // specific case: CENTER and JUSTIFIED
                if (IsZero(left1) && IsZero(right1))
                {
                    // the second line decide
                }

                if (IsZero(center1) && IsZero(center2))
                {
                    alignment = TextAlignment.CENTER;
                }

                // difficult decision
                if (IsZero(left1) && IsZero(left2) && IsZero(right1) && IsZero(right2))
                {
                    // opt for CENTER
                    alignment = TextAlignment.CENTER;
                }
            }
            else
            {
                float? centeredAt = CenteredAt(lineset);

                // use general rule
                alignment = (boundaryLeft < boundaryRight) ? TextAlignment.JUSTIFY : TextAlignment.RIGHT;

                if (centeredAt != null && IsZero(centeredAt))
                {
                    alignment = TextAlignment.CENTER;
                }

                // overrides small margin right
                if ((alignment == TextAlignment.RIGHT) && StatBeforeFirstTabStop(boundaryLeft))
                {
                    alignment = TextAlignment.JUSTIFY;
                }

                // cannot be RIGHT align
                if (IsZero(boundaryRight) && IsZero(boundaryLeft))
                {
                    var textline = FirstNonFullLine(lineset);

                    if (textline != null)
                    {
                        alignment = (IsZero(textline.CenteredAt)) ? TextAlignment.CENTER : TextAlignment.JUSTIFY;
                    }
                    else
                    {
                        // ALL LINES are FULL?
                        alignment = TextAlignment.JUSTIFY;
                    }
                }
            }


            // hack?
            if ((alignment == TextAlignment.JUSTIFY) && float.IsNaN(STAT_FIRST_TABSTOP))
            {
                float margin = lineset[0].MarginLeft;

                // protect
                bool validMargin = (margin > 20 && margin < 40);

                if(validMargin)
                {
                    if (!IsZero(margin))
                        STAT_FIRST_TABSTOP = margin;
                }
            }

            return alignment;
        }

        bool HasInlineTables(IEnumerable<TextLine> lineset)
        {
            return lineset.Any(t => t.HasLargeSpace);
        }

        float StatAtFirstTabStop(float value)
        {
            return (float.IsNaN(STAT_FIRST_TABSTOP)) ? 10000f : Math.Abs(value - STAT_FIRST_TABSTOP);
        }
        bool StatBeforeFirstTabStop(float value)
        {
            return (float.IsNaN(STAT_FIRST_TABSTOP)) ? false : value <= STAT_FIRST_TABSTOP + difference_margin_center_text;
        }
        TextLine FirstNonFullLine(IEnumerable<TextLine> lineset)
        {
            return lineset.FirstOrDefault(t => IsStrictlyPositive(t.MarginRight) || IsStrictlyPositive(t.MarginLeft));
        }

        float? CenteredAt(IEnumerable<TextLine> lineset)
        {
            float min = CenteredMinimumAt(lineset);
            float max = CenteredMaximumAt(lineset);

            if (!IsZero(min - max))
                return null;

            return (max + min)/2.0f;
        }
        float CenteredMinimumAt(IEnumerable<TextLine> lineset)
        {
            return lineset.Min(t => t.CenteredAt);
        }
        float CenteredMaximumAt(IEnumerable<TextLine> lineset)
        {
            return lineset.Max(t => t.CenteredAt);
        }
        float BoundaryLeft(IEnumerable<TextLine> lineset)
        {
            return lineset.Min(t => t.MarginLeft);
        }
        float BoundaryRight(IEnumerable<TextLine> lineset)
        {
            return lineset.Min(t => t.MarginRight);
        }
        bool IsAlignedMarginLeft()
        {
            throw new NotImplementedException();
        }
        bool IsAlignedMarginRight()
        {
            throw new NotImplementedException();
        }
        bool IsAllCentralized()
        {
            throw new NotImplementedException();
        }
        bool IsStrictlyPositive(float value)
        {
            return value >= difference_margin_center_text;
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
