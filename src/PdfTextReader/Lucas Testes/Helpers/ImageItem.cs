using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.Lucas_Testes.Helpers
{
    public class ImageItem : MainItem
    {
        public static Color IMG_COLOR = ColorConstants.CYAN;

        public ImageItem(ImageRenderInfo imageRenderInfo) : base()
        {
            rectangle = GetRectangle(imageRenderInfo);
            color = IMG_COLOR;
        }

        private static Rectangle GetRectangle(ImageRenderInfo imageRenderInfo)
        {
            Matrix ctm = imageRenderInfo.GetImageCtm();
            return new Rectangle(ctm.Get(6), ctm.Get(7),ctm.Get(0), ctm.Get(4));
            //Não é necessário somar o ponto X e o Y para pegar o tamanho em C#
            //return new Rectangle(ctm.Get(6), ctm.Get(7), ctm.Get(6) + ctm.Get(0), ctm.Get(7) + ctm.Get(4));
        }
    }
}
