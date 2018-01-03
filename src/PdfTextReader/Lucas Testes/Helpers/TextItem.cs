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
        public string text;
        public string fontName;
        public float fontSize;
        public TextStyle textStyle;

        public static Dictionary<TextStyle, Color> textStyles = new Dictionary<TextStyle, Color>()
        {
            { new TextStyle("Times New Roman", 10.7357378f), ColorConstants.ORANGE }
        };

        public TextItem(TextRenderInfo textRenderInfo, double top)
        {
            text = textRenderInfo.GetText();
            baseline = textRenderInfo.GetBaseline().GetStartPoint().Get(1);
            rectangle = GetRectangle(textRenderInfo);
            color = GetColor(textRenderInfo, top);
            fontName = GetFontName(textRenderInfo);
            fontSize = GetFontSize(textRenderInfo);
            textStyle = new TextStyle(fontName, fontSize);
        }

        static string GetFontName(TextRenderInfo textRenderInfo)
        {
            string font = String.Empty;
            try
            {
                font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFullName()[0][3];
            }
            catch (Exception ex) { }
            if (String.IsNullOrWhiteSpace(font))
            {
                try
                {
                    font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFamilyName()[0][3];
                }
                catch (Exception ex) { }
            }
            if (String.IsNullOrWhiteSpace(font))
            {
                try
                {
                    font = textRenderInfo.GetFont().GetFontProgram().GetFontNames().GetFontName();
                }
                catch (Exception ex) { }
            }
            return font;
        }

        static float GetFontSize(TextRenderInfo textRenderInfo)
        {
            return textRenderInfo.GetAscentLine().GetStartPoint().Get(1) - textRenderInfo.GetDescentLine().GetStartPoint().Get(1);
        }

        public Point GetLL()
        {
            return new Point(GetRectangle().GetLeft(), baseline);
        }

        static Color GetColor(TextRenderInfo textRenderInfo, double top)
        {
            if (textRenderInfo.GetBaseline().GetStartPoint().Get(1) > top)
                return artifactColor;
            if (textRenderInfo.GetBaseline().GetStartPoint().Get(1) < 20)
                return artifactColor;
            TextStyle ts = new TextStyle(textRenderInfo);
            Color cc = ColorConstants.GRAY;
            if (ts.fontName.ToLower().Contains("times"))
            {
                if (ts.fontSize > 13.6)
                {
                    textStyles.Add(ts, ColorConstants.BLUE);
                }
                else if (ts.fontSize > 13)
                {
                    textStyles.Add(ts, ColorConstants.ORANGE);
                }
                else if (ts.fontSize > 11)
                {
                    textStyles.Add(ts, ColorConstants.YELLOW);
                    
                }
                else if (ts.fontSize > 7.16)
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
                    textStyles.Add(ts, ColorConstants.MAGENTA);
                }
            }
            else // OttawaV
            {
                if (ts.fontSize > 14)
                {
                    textStyles.Add(ts, ColorConstants.BLACK);
                }
                else if (ts.fontSize > 12)
                {
                    textStyles.Add(ts, ColorConstants.BLACK);
                }
                else if (ts.fontSize > 10)
                {
                    if (ts.fontName.ToLower().Contains("bold"))
                    {
                        textStyles.Add(ts, ColorConstants.DARK_GRAY);
                    }
                    else
                    {
                        textStyles.Add(ts, ColorConstants.GRAY);
                    }
                }
                else
                {
                    textStyles.Add(ts, ColorConstants.LIGHT_GRAY);
                }
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

