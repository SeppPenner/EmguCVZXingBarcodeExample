// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestBarcode;

using System;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;

using TestBarcode.Services;

/// <summary>
/// The main form.
/// </summary>
public partial class Main : Form
{
    /// <summary>
    /// The barcode service.
    /// </summary>
    private readonly IBarcodeService barcodeService = new BarcodeService();

    /// <summary>
    /// The image of the file that was picked last. It is kept so that moving the track bar thresholds that image
    /// again instead of asking for a file once more.
    /// </summary>
    private Image<Bgr, byte>? pickedImage;

    /// <summary>
    /// Initializes a new instance of the <see cref="Main"/> class.
    /// </summary>
    public Main()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Releases the images the form holds after it was closed.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);
        this.pickedImage?.Dispose();
        this.pickedImage = null;
        this.PictureBoxImage.Image?.Dispose();
        this.PictureBoxImage.Image = null;
    }

    /// <summary>
    /// Handles the test image click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void TestImageClick(object sender, EventArgs e)
    {
        using var openFileDialog = new OpenFileDialog { Filter = "Png-Bilder|*.png", Multiselect = false };

        if (openFileDialog.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        this.pickedImage?.Dispose();
        this.pickedImage = this.barcodeService.LoadImage(openFileDialog.FileName);
        this.ReadPickedImage();
    }

    /// <summary>
    /// Handles the track bar scrolling.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void TrackBarScroll(object sender, EventArgs e)
    {
        this.Label.Text = this.TrackBar.Value.ToString();
        this.ReadPickedImage();
    }

    /// <summary>
    /// Thresholds the picked image with the current track bar value, shows the result and reads the barcode from it.
    /// </summary>
    private void ReadPickedImage()
    {
        if (this.pickedImage is null)
        {
            return;
        }

        using var blackAndWhiteImage = this.barcodeService.GetBlackAndWhiteImage(this.pickedImage, this.TrackBar.Value);
        var shownImage = this.PictureBoxImage.Image;
        this.PictureBoxImage.Image = blackAndWhiteImage.ToBitmap();
        shownImage?.Dispose();

        var result = this.barcodeService.ReadBarcode(blackAndWhiteImage);
        this.RichTextBoxText.Text = result?.BarcodeFormat.ToString() ?? string.Empty;
        this.RichTextBoxContent.Text = result?.Text ?? string.Empty;
    }
}
