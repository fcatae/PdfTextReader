using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class TextItem : MainItem
    {
        static Color artifactColor = ColorConstants.GRAY;
        float baseline;

        public static Dictionary<TextStyle, Color> textStyles = new Dictionary<TextStyle, Color>()
        {
            { new TextStyle("Times New Roman", 10.7357378f), ColorConstants.ORANGE }
        };

        public TextItem(TextRenderInfo textRenderInfo, float top)
        {
            baseline = textRenderInfo.GetBaseline().GetStartPoint().Get(1);
            rectangle = GetRectangle(textRenderInfo);
            color = GetColor(textRenderInfo, top);
        }

        public Point GetLL()
        {
            return new Point(GetRectangle().GetLeft(), baseline);
        }

        static Color GetColor(TextRenderInfo textRenderInfo, float top)
        {
            if (textRenderInfo.GetBaseline().GetStartPoint().Get(1) > top)
                return artifactColor;
            if (textRenderInfo.GetBaseline().GetStartPoint().Get(1) < 20)
                return artifactColor;
            TextStyle ts = new TextStyle(textRenderInfo);
            Color cc = ColorConstants.GRAY;
            if (ts.fontSize > 12)
            {
                textStyles.Add(ts, ColorConstants.ORANGE);
            }
            else if (ts.fontSize > 10)
            {
                if (ts.fontName.ToLower().Contains("bold"))
                {
                    textStyles.Add(ts, ColorConstants.GREEN);
                }
                else
                {
                    textStyles.Add(ts, ColorConstants.RED);
                }
            }
            else
            {
                textStyles.Add(ts, ColorConstants.BLUE);
            }

            if (textStyles.TryGetValue(ts, out Color c))
            {
                cc = c;
            }
            return cc;
        }

        static Rectangle GetRectangle(TextRenderInfo textRenderInfo)
        {
            LineSegment descentLine = textRenderInfo.GetDescentLine();
            LineSegment ascentLine = textRenderInfo.GetAscentLine();
            float x0 = descentLine.GetStartPoint().Get(0);
            float x1 = descentLine.GetEndPoint().Get(0);
            x1 = ascentLine.GetEndPoint().Get(0) - descentLine.GetStartPoint().Get(0);
            float y0 = descentLine.GetStartPoint().Get(1);
            float y1 = ascentLine.GetEndPoint().Get(1);
            y1 = ascentLine.GetEndPoint().Get(1) - descentLine.GetStartPoint().Get(1);
            return new Rectangle(x0, y0, x1, y1);
        }
    }
}

