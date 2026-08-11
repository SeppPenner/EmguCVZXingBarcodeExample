EmguCVZXingBarcodeExample
====================================

EmguCVZXingBarcodeExample is an example project on how to read barcodes from an image with [EmguCV](https://www.emgu.com/wiki/index.php/Main_Page) and [ZXing](https://github.com/micjahn/ZXing.Net). EmguCV loads the image and turns it into a black and white image, ZXing decodes the Code 39 barcode from it.

[![Build status](https://ci.appveyor.com/api/projects/status/9id69y2gmy4okk30?svg=true)](https://ci.appveyor.com/project/SeppPenner/emgucvzxingbarcodeexample)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/EmguCVZXingBarcodeExample.svg)](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/EmguCVZXingBarcodeExample.svg)](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/EmguCVZXingBarcodeExample.svg)](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/EmguCVZXingBarcodeExample/badge.svg)](https://snyk.io/test/github/SeppPenner/EmguCVZXingBarcodeExample)
[![Blogger](https://img.shields.io/badge/Follow_me_on-blogger-orange)](https://franzhuber23.blogspot.de/)
[![Patreon](https://img.shields.io/badge/Patreon-F96854?logo=patreon&logoColor=white)](https://patreon.com/SeppPennerOpenSourceDevelopment)
[![PayPal](https://img.shields.io/badge/PayPal-00457C?logo=paypal&logoColor=white)](https://paypal.me/th070795)

## How to run it

Build `src/TestBarcode.sln` and start `TestBarcode`. The button opens a PNG file, the track bar is
the threshold that turns the image into black and white, the picture box shows the result of that and
the two text boxes show the format and the content of the barcode. Every image below is in
`src/Images` and holds the barcode `ABC-1234`, apart from the last one.

`dotnet test src/TestBarcode.sln` checks every claim this readme makes about those images.

## Images that can be read

The images below can be read by the barcode reader (because the rotation is correct according to a horizontal line)

![Barcode_1.png](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/src/Images/Barcode_1.png)
![Barcode_2.png](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/src/Images/Barcode_2.png)

## Images that can't be read

The image below can't be read by the barcode reader (because the rotation is wrong according to a horizontal line)

![Barcode_3.png](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/src/Images/Barcode_3.png)

## Example images that can be read

![barcode.png](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/src/Images/barcode.png)
![barcode2.png](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/src/Images/barcode2.png)

## Example images that can't be read

The image below holds a UPC-E barcode, and this example asks ZXing for Code 39 only.

![barcode3.png](https://raw.githubusercontent.com/SeppPenner/EmguCVZXingBarcodeExample/master/src/Images/barcode3.png)

## Known issues

* A barcode that is rotated by more than a few degrees is not found. That is what the red line in the
  first three images marks: it is the horizontal the barcode is measured against.
* The three images exported out of `src/Images/Barcodes.pptx` have a transparent background. An image
  like that has to be put onto white while it is loaded, otherwise the transparent parts end up black,
  merge with the bars and the barcode is gone. `BarcodeService.LoadImage` does that.
* EmguCV needs its native binaries. `Emgu.CV` alone is only the managed wrapper, without
  `Emgu.CV.runtime.windows` the first call into OpenCV throws a `TypeInitializationException`.
* ZXing.Net has no `System.Drawing` based reader on .NET Core any more, so there is no `Bitmap` detour
  here. The package `ZXing.Net.Bindings.EmguCV` reads the EmguCV image directly, which also settles
  [this question](https://stackoverflow.com/questions/65029152/c-sharp-how-to-convert-a-system-drawing-bitmap-to-a-emgucv-imagebgr-byte-and).

Change history
--------------

See the [Changelog](https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Changelog.md).