// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BarcodeServiceTests.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to test the <see cref="BarcodeService" /> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestBarcode.Tests;

using System;
using System.IO;

using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using TestBarcode.Services;

using ZXing;

/// <summary>
/// A class to test the <see cref="BarcodeService"/> class.
/// </summary>
[TestClass]
public class BarcodeServiceTests
{
    /// <summary>
    /// The service under test.
    /// </summary>
    private readonly IBarcodeService barcodeService = new BarcodeService();

    /// <summary>
    /// The directory the files of a single test are written to.
    /// </summary>
    private string testDirectory = string.Empty;

    /// <summary>
    /// Creates an empty directory outside of the repository for the files of the running test.
    /// </summary>
    [TestInitialize]
    public void CreateTestDirectory()
    {
        this.testDirectory = Path.Combine(Path.GetTempPath(), $"EmguCVZXingBarcodeExample_{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.testDirectory);
    }

    /// <summary>
    /// Removes the directory of the finished test.
    /// </summary>
    [TestCleanup]
    public void DeleteTestDirectory()
    {
        if (Directory.Exists(this.testDirectory))
        {
            Directory.Delete(this.testDirectory, true);
        }
    }

    /// <summary>
    /// Checks whether the image is returned with its original size and with the three channels the reader and the
    /// picture box expect, no matter that the file has four.
    /// </summary>
    [TestMethod]
    public void LoadImageReturnsTheImageWithItsOriginalSizeAndThreeChannels()
    {
        using var image = this.barcodeService.LoadImage(TestDataProvider.GetImageFileName(TestDataProvider.StraightBarcodeFile));

        Assert.AreEqual(689, image.Width);
        Assert.AreEqual(95, image.Height);
        Assert.AreEqual(3, image.NumberOfChannels);
    }

    /// <summary>
    /// Checks whether the transparent background becomes white. This is the whole point of loading with
    /// <c>ImreadModes.Unchanged</c>: with a black background the white gaps of the barcode disappear.
    /// </summary>
    [TestMethod]
    public void LoadImageFlattensTheTransparentBackgroundOnWhite()
    {
        using var image = this.barcodeService.LoadImage(TestDataProvider.GetImageFileName(TestDataProvider.StraightBarcodeFile));

        Assert.AreEqual<byte>(255, image.Data[0, 0, 0], "The corner of the image is transparent and has to end up white.");
        Assert.AreEqual<byte>(255, image.Data[0, 0, 1]);
        Assert.AreEqual<byte>(255, image.Data[0, 0, 2]);
    }

    /// <summary>
    /// Checks whether a missing file is reported instead of being handed to OpenCV, which returns an empty image.
    /// </summary>
    [TestMethod]
    public void LoadImageThrowsAFileNotFoundExceptionForAMissingFile()
    {
        var fileName = Path.Combine(this.testDirectory, "DoesNotExist.png");

        Assert.ThrowsExactly<FileNotFoundException>(() => this.barcodeService.LoadImage(fileName));
    }

    /// <summary>
    /// Checks whether a file that exists but holds no image is reported. OpenCV throws an
    /// <see cref="ArgumentException"/> here, the service turns that into something a caller can tell apart from a
    /// wrong argument of its own.
    /// </summary>
    [TestMethod]
    public void LoadImageThrowsAnInvalidDataExceptionForAFileThatIsNoImage()
    {
        var fileName = Path.Combine(this.testDirectory, "NoImage.png");
        File.WriteAllText(fileName, "This is not a PNG file.");

        Assert.ThrowsExactly<InvalidDataException>(() => this.barcodeService.LoadImage(fileName));
    }

    /// <summary>
    /// Checks whether an empty file is reported the same way. OpenCV throws a <c>CvException</c> for this one, not
    /// an <see cref="ArgumentException"/>, which is why the service catches both.
    /// </summary>
    [TestMethod]
    public void LoadImageThrowsAnInvalidDataExceptionForAnEmptyFile()
    {
        var fileName = Path.Combine(this.testDirectory, "Empty.png");
        File.WriteAllBytes(fileName, []);

        Assert.ThrowsExactly<InvalidDataException>(() => this.barcodeService.LoadImage(fileName));
    }

    /// <summary>
    /// Checks whether the thresholded image really only holds black and white pixels.
    /// </summary>
    [TestMethod]
    public void GetBlackAndWhiteImageReturnsOnlyBlackAndWhitePixels()
    {
        using var image = this.barcodeService.LoadImage(TestDataProvider.GetImageFileName(TestDataProvider.ExampleBarcodeFile));
        using var blackAndWhiteImage = this.barcodeService.GetBlackAndWhiteImage(image, TestDataProvider.Threshold);

        for (var y = 0; y < blackAndWhiteImage.Height; y++)
        {
            for (var x = 0; x < blackAndWhiteImage.Width; x++)
            {
                for (var channel = 0; channel < blackAndWhiteImage.NumberOfChannels; channel++)
                {
                    var value = blackAndWhiteImage.Data[y, x, channel];
                    Assert.IsTrue(value is 0 or 255, $"The pixel {x}/{y} has the value {value} in channel {channel}.");
                }
            }
        }
    }

    /// <summary>
    /// Checks whether a threshold outside of the range of a gray value is reported.
    /// </summary>
    [TestMethod]
    public void GetBlackAndWhiteImageThrowsForAThresholdOutsideOfTheGrayValues()
    {
        using var image = this.barcodeService.LoadImage(TestDataProvider.GetImageFileName(TestDataProvider.ExampleBarcodeFile));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => this.barcodeService.GetBlackAndWhiteImage(image, BarcodeService.MinimumThreshold - 1));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => this.barcodeService.GetBlackAndWhiteImage(image, BarcodeService.MaximumThreshold + 1));
    }

    /// <summary>
    /// Checks whether the thresholded image can be handed to the picture box of the main form. The extension method
    /// comes from the package Emgu.CV.Bitmap, which is exactly the one that was missing in version 1.0.4.0.
    /// </summary>
    [TestMethod]
    public void GetBlackAndWhiteImageReturnsAnImageThatCanBeConvertedToABitmap()
    {
        using var image = this.barcodeService.LoadImage(TestDataProvider.GetImageFileName(TestDataProvider.ExampleBarcodeFile));
        using var blackAndWhiteImage = this.barcodeService.GetBlackAndWhiteImage(image, TestDataProvider.Threshold);

        using var bitmap = blackAndWhiteImage.ToBitmap();

        Assert.AreEqual(blackAndWhiteImage.Width, bitmap.Width);
        Assert.AreEqual(blackAndWhiteImage.Height, bitmap.Height);
    }

    /// <summary>
    /// Checks whether the barcode that sits on a horizontal line is read, as the readme claims.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeReadsTheStraightBarcode()
    {
        AssertBarcodeIsRead(TestDataProvider.StraightBarcodeFile);
    }

    /// <summary>
    /// Checks whether the barcode that is rotated by a few degrees is read, as the readme claims.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeReadsTheSlightlyRotatedBarcode()
    {
        AssertBarcodeIsRead(TestDataProvider.SlightlyRotatedBarcodeFile);
    }

    /// <summary>
    /// Checks whether the barcode that is rotated too far is not read, as the readme claims. The image is fine, the
    /// rotation is what ZXing cannot handle.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeDoesNotReadTheRotatedBarcode()
    {
        AssertBarcodeIsNotRead(TestDataProvider.RotatedBarcodeFile);
    }

    /// <summary>
    /// Checks whether the first of the older example images is read.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeReadsTheExampleBarcode()
    {
        AssertBarcodeIsRead(TestDataProvider.ExampleBarcodeFile);
    }

    /// <summary>
    /// Checks whether the second of the older example images is read.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeReadsTheSecondExampleBarcode()
    {
        AssertBarcodeIsRead(TestDataProvider.SecondExampleBarcodeFile);
    }

    /// <summary>
    /// Checks whether the UPC-E barcode is not returned. The example asks for Code 39 only, so a barcode of another
    /// format has to stay unread even though ZXing could decode it.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeDoesNotReadTheUpcBarcode()
    {
        AssertBarcodeIsNotRead(TestDataProvider.UpcBarcodeFile);
    }

    /// <summary>
    /// Checks whether the barcode is read at the lowest and at the highest useful track bar value. Everything above
    /// the threshold becomes white, so a threshold of 255 leaves a completely black image and reads nothing.
    /// </summary>
    [TestMethod]
    public void ReadBarcodeReadsTheBarcodeAtTheLowestAndAtTheHighestUsefulThreshold()
    {
        using var image = this.barcodeService.LoadImage(TestDataProvider.GetImageFileName(TestDataProvider.StraightBarcodeFile));

        Assert.IsNotNull(this.ReadWithThreshold(image, 1), "The barcode was not read with the threshold 1.");
        Assert.IsNotNull(this.ReadWithThreshold(image, BarcodeService.MaximumThreshold - 1), "The barcode was not read with the threshold 254.");
        Assert.IsNull(this.ReadWithThreshold(image, BarcodeService.MaximumThreshold), "A completely black image must not return a barcode.");
    }

    /// <summary>
    /// Checks whether the given image file holds the Code 39 barcode of this repository.
    /// </summary>
    /// <param name="fileName">The name of the image file.</param>
    private static void AssertBarcodeIsRead(string fileName)
    {
        var result = ReadBarcodeFromFile(fileName);

        Assert.IsNotNull(result, $"No barcode was read from {fileName}.");
        Assert.AreEqual(BarcodeFormat.CODE_39, result.BarcodeFormat);
        Assert.AreEqual(TestDataProvider.BarcodeContent, result.Text);
    }

    /// <summary>
    /// Checks whether the given image file holds no readable Code 39 barcode.
    /// </summary>
    /// <param name="fileName">The name of the image file.</param>
    private static void AssertBarcodeIsNotRead(string fileName)
    {
        var result = ReadBarcodeFromFile(fileName);

        Assert.IsNull(result, $"The barcode {result?.Text} was read from {fileName}, which was not expected.");
    }

    /// <summary>
    /// Runs the three steps of the example on an image file, exactly like the main form does.
    /// </summary>
    /// <param name="fileName">The name of the image file.</param>
    /// <returns>The <see cref="Result"/> of the barcode or <c>null</c>.</returns>
    private static Result? ReadBarcodeFromFile(string fileName)
    {
        var barcodeService = new BarcodeService();
        using var image = barcodeService.LoadImage(TestDataProvider.GetImageFileName(fileName));
        using var blackAndWhiteImage = barcodeService.GetBlackAndWhiteImage(image, TestDataProvider.Threshold);
        return barcodeService.ReadBarcode(blackAndWhiteImage);
    }

    /// <summary>
    /// Thresholds an image with the given threshold and reads the barcode from it.
    /// </summary>
    /// <param name="image">The <see cref="Image{TColor, TDepth}"/> to be read.</param>
    /// <param name="threshold">The threshold to be used.</param>
    /// <returns>The <see cref="Result"/> of the barcode or <c>null</c>.</returns>
    private Result? ReadWithThreshold(Image<Bgr, byte> image, int threshold)
    {
        using var blackAndWhiteImage = this.barcodeService.GetBlackAndWhiteImage(image, threshold);
        return this.barcodeService.ReadBarcode(blackAndWhiteImage);
    }
}
