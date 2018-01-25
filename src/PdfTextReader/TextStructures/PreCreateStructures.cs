using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PdfTextReader.Base;

namespace PdfTextReader.TextStructures
{
    class PreCreateStructures : IAggregateStructure<TextLine, TextLine2>
    {
        const float ERR_ALIGNMENT = 1f;

        TextLine2 _last;

        public void Init(TextLine line)
        {
            _last = new TextLine2()
            {
                FontName = line.FontName,
                FontStyle = line.FontStyle,
                FontSize = line.FontSize,
                AfterSpace = line.AfterSpace,
                HasBackColor = line.HasBackColor,
                BeforeSpace = line.BeforeSpace,
                Block = line.Block,
                CenteredAt = line.CenteredAt,
                HasLargeSpace = line.HasLargeSpace,
                MarginLeft = line.MarginLeft,
                MarginRight = line.MarginRight,
                PageInfo = line.PageInfo,
                Text = line.Text
            };
        }

        public bool Aggregate(TextLine current)
        {
            bool sameFont = ((_last.FontName == current.FontName) &&
                             (_last.FontStyle == current.FontStyle) &&
                             (_last.FontSize == current.FontSize));

            bool sameSpacing = (_last.BeforeSpace == current.AfterSpace) && (current.AfterSpace != null);

            bool sameAlignLeft = Equal(_last.MarginLeft, current.MarginLeft);
            bool sameAlignRight = Equal(_last.MarginRight, current.MarginRight);
            bool sameAlignCenter = Equal(_last.CenteredAt, current.CenteredAt);

            bool bothCentered = IsZero(_last.CenteredAt) && IsZero(current.CenteredAt);

            float minimumLeft = Math.Min(_last.MarginLeft, current.MarginLeft);
            float minimumRight = Math.Min(_last.MarginRight, current.MarginRight);

            bool isContinuation = IsZero(_last.MarginRight) && IsZero(current.MarginLeft);
            bool isFullLine = IsZero(current.MarginLeft) && IsZero(current.MarginRight);

            // center = both Centered
            // isContinuation == false

            return false;
        }

        public TextLine2 Create(List<TextLine> lineset)
        {
            return _last;
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
