TestBarcode
====================================

TestBarcode is an example project on how to read barcodes from an image with [EmguCV](http://www.emgu.com/wiki/index.php/Main_Page) and [ZXing](https://github.com/micjahn/ZXing.Net). 
The project was written and tested in .Net 4.6.2.

[![Build status](https://ci.appveyor.com/api/projects/status/9id69y2gmy4okk30?svg=true)](https://ci.appveyor.com/project/SeppPenner/emgucvzxingbarcodeexample)

## Images that can be read:

The images below can be read by the barcode reader (because the rotation is correct according to a horizontal line):

![](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/Barcode_1.png)
![](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/Barcode_2.png)

## Images that can't be read:

The image below can't be read by the barcode reader (because the rotation is wrong according to a horizontal line):

![](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/Barcode_3.png)

## Example images that can be read:

![](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/barcode.png)
![](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/barcode2.png)

## Example images that can't be read:

![](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/barcode3.png)

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
			//Convert the image to a gray frame.
            var grayFrame = frame.Convert<Gray, byte>();
			//Perform a threshold transformation (usually 87 to 170 is a good value for the first parameter).
            grayFrame = grayFrame.ThresholdBinary(new Gray(TrackBar.Value), new Gray(255));
			//Show the read image.
            PictureBoxImage.Image = grayFrame.ToBitmap();
            reader.Options.TryHarder = true;
			//Set the barcode format.
            reader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.CODE_39};
			//Specify some options.
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