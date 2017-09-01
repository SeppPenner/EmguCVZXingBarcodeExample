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
            var barcodeBitmap = (Bitmap) Image.FromFile("C:\\Users\\ASDF\\Desktop\\Test.jpg");
            var frame = new Image<Bgr, byte>(barcodeBitmap);
            var grayFrame = frame.Convert<Gray, byte>();
            grayFrame = grayFrame.ThresholdBinary(new Gray(TrackBar.Value), new Gray(255));
            PictureBoxImage.Image = grayFrame.ToBitmap();
            reader.Options.TryHarder = true;
            reader.Options.PossibleFormats = new List<BarcodeFormat> {BarcodeFormat.CODE_39};
            reader.Options.UseCode39ExtendedMode = true;
            reader.Options.UseCode39RelaxedExtendedMode = true;
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