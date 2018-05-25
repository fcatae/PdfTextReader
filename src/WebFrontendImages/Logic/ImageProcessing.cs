using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebFrontendImages.Logic
{
    public class ImageProcessing
    {
        static readonly JpegDecoder JPEG = new JpegDecoder();

        public static Stream Crop(Stream stream, float tx, float ty, float tw, float th)
        {
            Stream output = new MemoryStream();

            using (Image<Rgba32> image = Image.Load(stream, JPEG))
            {
                int x1 = (int)(image.Width * tx);
                int y1 = (int)(image.Height * ty);
                int dx = (int)(image.Width * tw);
                int dy = (int)(image.Height * th);

                image.Mutate(x => x
                    .Crop(new Rectangle(x1, y1, dx, dy))
                    //.Resize(image.Width / 2, image.Height / 2)
                    );
                                
                image.Save(output, new JpegEncoder());
            }

            output.Seek(0, SeekOrigin.Begin);

            return output;
        }
    }
}
