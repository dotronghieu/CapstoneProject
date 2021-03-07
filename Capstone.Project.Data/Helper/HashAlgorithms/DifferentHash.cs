using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;
using System.Text;

namespace Capstone.Project.Data.Helper.HashAlgorithms
{
    public class DifferentHash : IImageHash
    {
        private const int Width = 9;
        private const int Height = 8;

        /// <inheritdoc />
        public ulong Hash(Image<Rgba32> image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            // We first auto orient because with and height differ.
            image.Mutate(ctx => ctx
                                .AutoOrient()
                                .Resize(Width, Height)
                                .Grayscale(GrayscaleMode.Bt601));

            var mask = 1UL << ((Height * (Width - 1)) - 1);
            var hash = 0UL;

            for (var y = 0; y < Height; y++)
            {
                var row = image.GetPixelRowSpan(y);
                var leftPixel = row[0];

                for (var x = 1; x < Width; x++)
                {
                    var rightPixel = row[x];
                    if (leftPixel.R < rightPixel.R)
                        hash |= mask;

                    leftPixel = rightPixel;
                    mask >>= 1;
                }
            }

            return hash;
        }
    }
}
