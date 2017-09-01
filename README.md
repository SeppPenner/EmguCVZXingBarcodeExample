TestBarcode
====================================

TestBarcode is an example project on how to read barcodes from an image with EmguCV and ZXing.
The project was written and tested in .Net 4.6.2.

[![Build status](https://ci.appveyor.com/api/projects/status/9id69y2gmy4okk30?svg=true)](https://ci.appveyor.com/project/SeppPenner/emgucvzxingbarcodeexample)

## Images that can be read:


## Example code:
```csharp
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using ZXing;

namespace TestBarcode
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void TestImage_Click(object sender, EventArgs e)
        {
            IBarcodeReader reader = new BarcodeReader();
			//Load barcode file.
            var barcodeBitmap = (Bitmap) Image.FromFile("C:\\Users\\ASDF\\Desktop\\Test.jpg");
			//Convert the image to an EmguCV frame
            var frame = new Image<Bgr, byte>(barcodeBitmap);
			//Convert the image to an gray frame.
            var grayFrame = frame.Convert<Gray, byte>();
			//Perform a threshold transformation (usually 87 to 170 is a good value for the first parameter).
            grayFrame = grayFrame.ThresholdBinary(new Gray(TrackBar.Value), new Gray(255));
			//Show the read image.
            PictureBoxImage.Image = grayFrame.ToBitmap();
            reader.Options.TryHarder = true;
			//Set the barcode format.
            reader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.CODE_39};
            reader.Options.UseCode39ExtendedMode = true;
            reader.Options.UseCode39RelaxedExtendedMode = true;
			//Decode and display barcode.
            var result = reader.Decode(grayFrame.ToBitmap());
            if (result != null)
            {
                RichTextBoxText.Text = result.BarcodeFormat.ToString();
                RichTextBoxContent.Text = result.Text;
                return;
            }
            RichTextBoxText.Text = string.Empty;
            RichTextBoxContent.Text = string.Empty;
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            label1.Text = TrackBar.Value.ToString();
            TestImage_Click(sender, e);
        }
    }
}
```

Change history
--------------

* **Version 1.0.0.0 (2017-09-01)** : 1.0 release.