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
using System.Drawing;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;

using ZXing;

/// <summary>
/// The main form.
/// </summary>
public partial class Main : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Main"/> class.
    /// </summary>
    public Main()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Handles the test image click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event args.</param>
    private void TestImageClick(object sender, EventArgs e)
    {
        var openFileDialog = new OpenFileDialog { Filter = "Png-Bilder|*.png", Multiselect = false};

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            IBarcodeReader reader = new BarcodeReader();
            var barcodeBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
            var frame = new Image<Bgr, byte>(barcodeBitmap);
            var grayFrame = frame.Convert<Gray, byte>();
            grayFrame = grayFrame.ThresholdBinary(new Gray(this.TrackBar.Value), new Gray(255));
            this.PictureBoxImage.Image = grayFrame.ToBitmap();
            reader.Options.TryHarder = true;
            reader.Options.PossibleFormats = [BarcodeFormat.CODE_39];
            reader.Options.UseCode39ExtendedMode = true;
            reader.Options.UseCode39RelaxedExtendedMode = true;

            var result = reader.Decode(grayFrame.ToBitmap());

            if (result != null)
            {
                this.RichTextBoxText.Text = result.BarcodeFormat.ToString();
                this.RichTextBoxContent.Text = result.Text;
                return;
            }

            this.RichTextBoxText.Text = string.Empty;
            this.RichTextBoxContent.Text = string.Empty;
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
        this.TestImageClick(sender, e);
    }
}