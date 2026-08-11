// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBarcodeService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A service to load an image and to read a barcode from it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestBarcode.Services;

using Emgu.CV;
using Emgu.CV.Structure;

using ZXing;

/// <summary>
/// A service to load an image and to read a barcode from it.
/// </summary>
public interface IBarcodeService
{
    /// <summary>
    /// Loads an image file. A transparent background is flattened onto white, see the implementation.
    /// </summary>
    /// <param name="fileName">The name of the file to be loaded.</param>
    /// <returns>The loaded file as <see cref="Image{TColor, TDepth}"/>. The caller owns it and needs to dispose it.</returns>
    Image<Bgr, byte> LoadImage(string fileName);

    /// <summary>
    /// Converts an image to a black and white image with the given threshold.
    /// </summary>
    /// <param name="image">The <see cref="Image{TColor, TDepth}"/> to be converted.</param>
    /// <param name="threshold">The threshold, from <see cref="BarcodeService.MinimumThreshold"/> to <see cref="BarcodeService.MaximumThreshold"/>. Everything above it becomes white, everything else black.</param>
    /// <returns>The black and white <see cref="Image{TColor, TDepth}"/>. The caller owns it and needs to dispose it.</returns>
    Image<Bgr, byte> GetBlackAndWhiteImage(Image<Bgr, byte> image, int threshold);

    /// <summary>
    /// Reads a Code 39 barcode from an image.
    /// </summary>
    /// <param name="image">The <see cref="Image{TColor, TDepth}"/> to be read.</param>
    /// <returns>The <see cref="Result"/> of the barcode or <c>null</c> if the image holds no readable Code 39 barcode.</returns>
    Result? ReadBarcode(Image<Bgr, byte> image);
}
