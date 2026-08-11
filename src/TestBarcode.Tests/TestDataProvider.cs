// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataProvider.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   A class to provide the test data used in the tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestBarcode.Tests;

using System;
using System.IO;

/// <summary>
/// A class to provide the test data used in the tests. The images are the ones in <c>src/Images</c>, the project file
/// copies them next to the test assembly.
/// </summary>
public static class TestDataProvider
{
    /// <summary>
    /// The content of the barcode. Every image of this repository that holds a readable Code 39 barcode holds this one.
    /// </summary>
    public const string BarcodeContent = "ABC-1234";

    /// <summary>
    /// The threshold used wherever a test doesn't check the threshold itself. It is the middle of the track bar.
    /// </summary>
    public const int Threshold = 128;

    /// <summary>
    /// The barcode without any rotation, exported out of <c>Barcodes.pptx</c>, with a transparent background.
    /// </summary>
    public const string StraightBarcodeFile = "Barcode_1.png";

    /// <summary>
    /// The same barcode rotated by a few degrees, still readable according to the readme.
    /// </summary>
    public const string SlightlyRotatedBarcodeFile = "Barcode_2.png";

    /// <summary>
    /// The same barcode rotated further, not readable according to the readme.
    /// </summary>
    public const string RotatedBarcodeFile = "Barcode_3.png";

    /// <summary>
    /// The first of the older example images, an opaque Code 39 barcode.
    /// </summary>
    public const string ExampleBarcodeFile = "barcode.png";

    /// <summary>
    /// The second of the older example images, an opaque Code 39 barcode.
    /// </summary>
    public const string SecondExampleBarcodeFile = "barcode2.png";

    /// <summary>
    /// The third of the older example images. It holds a UPC-E barcode, so the Code 39 reader never returns it.
    /// </summary>
    public const string UpcBarcodeFile = "barcode3.png";

    /// <summary>
    /// Gets the full name of one of the sample images next to the test assembly.
    /// </summary>
    /// <param name="fileName">The name of the image file.</param>
    /// <returns>The full name of the file as <see cref="string"/>.</returns>
    public static string GetImageFileName(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
    }
}
