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
using System.IO;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

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

        try
        {
            // The picked image is replaced only after the new one is loaded, so a file that can't be read leaves
            // the image that is already on screen alone.
            var loadedImage = this.barcodeService.LoadImage(openFileDialog.FileName);
            this.pickedImage?.Dispose();
            this.pickedImage = loadedImage;
            this.ReadPickedImage();
        }
        catch (Exception ex) when (IsFileError(ex))
        {
            ShowError(ex);
        }
    }

    /// <summary>
    /// Handles the track bar scrolling.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void TrackBarScroll(object sender, EventArgs e)
    {
        this.Label.Text = this.TrackBar.Value.ToString();

        try
        {
            this.ReadPickedImage();
        }
        catch (Exception ex) when (IsFileError(ex))
        {
            ShowError(ex);
        }
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

    /// <summary>
    /// Checks whether an exception is one of the errors an unreadable image file causes. Note that
    /// <see cref="InvalidDataException"/> is no <see cref="IOException"/>, it derives from
    /// <see cref="SystemException"/>, so it has to be named on its own.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to be checked.</param>
    /// <returns><c>true</c> if the exception belongs to the file, <c>false</c> if it is a bug of this program.</returns>
    private static bool IsFileError(Exception ex)
    {
        return ex is IOException or InvalidDataException or UnauthorizedAccessException or CvException;
    }

    /// <summary>
    /// Shows an error in a message box. The title is German because the rest of this user interface is.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to be shown.</param>
    private static void ShowError(Exception ex)
    {
        MessageBox.Show(ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
