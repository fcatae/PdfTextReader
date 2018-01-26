using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class PreCreateTextSegments : IAggregateStructure<TextStructure, TextStructureAgg>
    {
        const float ERR_ALIGNMENT = 1f;

        TextStructureAgg _curr = null;
        TextStructureAgg _next = null;
        TextStructure _last = null;

        public void Init(TextStructure structure)
        {
            _last = structure;

            _curr = _next;

            if (_curr == null)
            {
                _curr = new TextStructureAgg()
                {
                    TextStruct = structure
                };
            }
        }

        public bool Aggregate(TextStructure current)
        {
            bool sameFont = ((_last.FontName == current.FontName) &&
                             (_last.FontStyle == current.FontStyle) &&
                             (_last.FontSize == current.FontSize));

            bool sameSpacing = (_last.AfterSpace == null) || ((current.AfterSpace != null) && Equal((float)_last.AfterSpace, (float)current.AfterSpace));

            bool sameAlignTabStop = Equal(_last.TabStop, current.TabStop);

            bool hasContinuation = _last.HasContinuation && IsZero(current.MarginLeft);

            float verticalSpacing = (current.AfterSpace == null) ? float.NaN : (float)current.AfterSpace/current.FontSize;

            //bool sameAlignLeft = Equal(_last.MarginLeft, current.MarginLeft);
            //bool sameAlignRight = Equal(_last.MarginRight, current.MarginRight);
            //bool sameAlignCenter = Equal(_last.CenteredAt, current.CenteredAt);

            //bool bothCentered = IsZero(_last.CenteredAt) && IsZero(current.CenteredAt);

            //float minimumLeft = Math.Min(_last.MarginLeft, current.MarginLeft);
            //float minimumRight = Math.Min(_last.MarginRight, current.MarginRight);

            //bool isContinuation = IsZero(_last.MarginRight) && IsZero(current.MarginLeft);
            //bool wasFullLine = IsZero(_last.MarginLeft) && IsZero(_last.MarginRight);
            //bool isFullLine = IsZero(current.MarginLeft) && IsZero(current.MarginRight);

            _next = new TextStructureAgg()
            {
                TextStruct = current,
                SameFont = sameFont,
                SameSpacing = sameSpacing,
                AlignedTabStop = sameAlignTabStop,
                HasContinuation = hasContinuation,
                VerticalSpacing = verticalSpacing
            };

            return false;
        }

        public TextStructureAgg Create(List<TextStructure> lineset)
        {
            return _curr;
        }

        bool IsZero(float a)
        {
            return (Math.Abs(a) < ERR_ALIGNMENT);
        }
        bool Equal(float a, float b)
        {
            return (Math.Abs(a - b) < ERR_ALIGNMENT);
        }
        bool Equal(float a, float b, float err)
        {
            return (Math.Abs(a - b) < err);
        }
    }
}
