// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BarcodeService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A service to load an image and to read a barcode from it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestBarcode.Services;

using System;
using System.IO;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using ZXing;
using ZXing.EmguCV;

/// <inheritdoc cref="IBarcodeService"/>
/// <summary>
/// A service to load an image and to read a barcode from it.
/// </summary>
/// <seealso cref="IBarcodeService"/>
public class BarcodeService : IBarcodeService
{
    /// <summary>
    /// The lowest allowed threshold, the lowest gray value there is. The track bar of the main form starts at 1,
    /// because a threshold of 0 turns every pixel that isn't pitch black into white.
    /// </summary>
    public const int MinimumThreshold = 0;

    /// <summary>
    /// The highest allowed threshold, the highest gray value there is. It is the maximum of the track bar of the
    /// main form as well.
    /// </summary>
    public const int MaximumThreshold = 255;

    /// <summary>
    /// The value a black and white pixel gets if it is above the threshold.
    /// </summary>
    private const byte White = 255;

    /// <inheritdoc cref="IBarcodeService"/>
    /// <summary>
    /// Loads an image file. A transparent background is flattened onto white, see the implementation.
    /// </summary>
    /// <param name="fileName">The name of the file to be loaded.</param>
    /// <returns>The loaded file as <see cref="Image{TColor, TDepth}"/>. The caller owns it and needs to dispose it.</returns>
    /// <exception cref="FileNotFoundException">Thrown if the file doesn't exist.</exception>
    /// <exception cref="InvalidDataException">Thrown if the file exists but isn't an image OpenCV can read.</exception>
    /// <seealso cref="IBarcodeService"/>
    public Image<Bgr, byte> LoadImage(string fileName)
    {
        if (!File.Exists(fileName))
        {
            throw new FileNotFoundException("The image file doesn't exist.", fileName);
        }

        using var loadedImage = ReadImageFile(fileName);

        return loadedImage.NumberOfChannels == 4
            ? FlattenAlphaChannelOnWhite(loadedImage)
            : loadedImage.ToImage<Bgr, byte>();
    }

    /// <inheritdoc cref="IBarcodeService"/>
    /// <summary>
    /// Converts an image to a black and white image with the given threshold.
    /// </summary>
    /// <param name="image">The <see cref="Image{TColor, TDepth}"/> to be converted.</param>
    /// <param name="threshold">The threshold, from <see cref="MinimumThreshold"/> to <see cref="MaximumThreshold"/>. Everything above it becomes white, everything else black.</param>
    /// <returns>The black and white <see cref="Image{TColor, TDepth}"/>. The caller owns it and needs to dispose it.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the threshold is outside of the allowed range.</exception>
    /// <seealso cref="IBarcodeService"/>
    public Image<Bgr, byte> GetBlackAndWhiteImage(Image<Bgr, byte> image, int threshold)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentOutOfRangeException.ThrowIfLessThan(threshold, MinimumThreshold);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(threshold, MaximumThreshold);

        using var grayImage = image.Convert<Gray, byte>();
        using var blackAndWhiteImage = grayImage.ThresholdBinary(new Gray(threshold), new Gray(White));

        // Back to three channels: that is what the luminance source of the ZXing binding takes and what the
        // picture box of the main form shows.
        return blackAndWhiteImage.Convert<Bgr, byte>();
    }

    /// <inheritdoc cref="IBarcodeService"/>
    /// <summary>
    /// Reads a Code 39 barcode from an image.
    /// </summary>
    /// <param name="image">The <see cref="Image{TColor, TDepth}"/> to be read.</param>
    /// <returns>The <see cref="Result"/> of the barcode or <c>null</c> if the image holds no readable Code 39 barcode.</returns>
    /// <seealso cref="IBarcodeService"/>
    public Result? ReadBarcode(Image<Bgr, byte> image)
    {
        ArgumentNullException.ThrowIfNull(image);

        // The reader comes from the ZXing EmguCV binding, so the Emgu image is decoded without a detour over
        // a System.Drawing bitmap. Swapping the binding is a matter of this one line.
        var reader = new BarcodeReader();

        // Without TryHarder ZXing only scans a single row in the middle of the image, which is the row the red
        // reference line covers in the sample images.
        reader.Options.TryHarder = true;
        reader.Options.PossibleFormats = [BarcodeFormat.CODE_39];
        reader.Options.UseCode39ExtendedMode = true;
        reader.Options.UseCode39RelaxedExtendedMode = true;
        return reader.Decode(image);
    }

    /// <summary>
    /// Reads an image file into a <see cref="Mat"/>.
    /// </summary>
    /// <param name="fileName">The name of the file to be read.</param>
    /// <returns>The file as <see cref="Mat"/>, with as many channels as the file holds.</returns>
    /// <exception cref="InvalidDataException">Thrown if the file isn't an image OpenCV can read.</exception>
    private static Mat ReadImageFile(string fileName)
    {
        try
        {
            // Unchanged keeps the alpha channel. ColorBgr would drop it and leave black behind every transparent
            // pixel, which turns the white gaps of a barcode on a transparent background into black bars.
            return CvInvoke.Imread(fileName, ImreadModes.Unchanged);
        }
        catch (Exception ex) when (ex is ArgumentException or CvException)
        {
            // A file that holds no image throws an ArgumentException, an empty file a CvException. Both mean the
            // same thing for a caller, so they are reported as one.
            throw new InvalidDataException($"The file {fileName} couldn't be read as an image.", ex);
        }
    }

    /// <summary>
    /// Puts an image with an alpha channel onto a white background and drops the alpha channel.
    /// </summary>
    /// <param name="loadedImage">The <see cref="Mat"/> with four channels as it was loaded from the file.</param>
    /// <returns>The flattened <see cref="Image{TColor, TDepth}"/> with three channels.</returns>
    private static Image<Bgr, byte> FlattenAlphaChannelOnWhite(Mat loadedImage)
    {
        using var imageWithAlpha = loadedImage.ToImage<Bgra, byte>();
        using var alphaChannel = imageWithAlpha[3];

        // Every pixel that isn't fully transparent is taken as it is. That is enough here because the alpha
        // channel of the sample images only ever holds 0 or 255, partial transparency isn't blended.
        using var alphaMask = alphaChannel.ThresholdBinary(new Gray(0), new Gray(White));
        using var colours = imageWithAlpha.Convert<Bgr, byte>();
        var flattenedImage = new Image<Bgr, byte>(imageWithAlpha.Size);
        flattenedImage.SetValue(new Bgr(White, White, White));
        colours.Mat.CopyTo(flattenedImage.Mat, alphaMask.Mat);
        return flattenedImage;
    }
}
