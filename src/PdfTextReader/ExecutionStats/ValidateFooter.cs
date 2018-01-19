using PdfTextReader.Base;
using PdfTextReader.PDFCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace PdfTextReader.ExecutionStats
{
    class ValidateFooter : ICalculateStats<StatsPageFooter>
    {
        const float statRegionTooLarge = 200f;

        public object Calculate(IEnumerable<StatsPageFooter> stats)
        {
            float total = 0;
            int count = 0;
            int missingFooter = 0;

            foreach(var stat in stats)
            {
                if( stat.HasFooter )
                {
                    float height = (float)stat.FooterHeight;

                    if (height > statRegionTooLarge)
                    {
                        PdfReaderException.AlwaysThrow("height > statRegionTooLarge");
                    }

                    total += height;
                    count++;
                }
                else
                {
                    missingFooter++;
                }
            }

            return new
            {
                PagesWithoutFooter = missingFooter,
                AverageFooterHeight = total / count
            };
        }
    }
}
