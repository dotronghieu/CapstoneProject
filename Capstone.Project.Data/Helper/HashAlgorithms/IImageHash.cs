using System;
using System.Collections.Generic;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Capstone.Project.Data.Helper.HashAlgorithms
{
    public interface IImageHash
    {
        ulong Hash(Image<Rgba32> image);
    }
}
