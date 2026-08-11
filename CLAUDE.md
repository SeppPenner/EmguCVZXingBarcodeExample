# Project rules for Claude

## What this is

EmguCVZXingBarcodeExample is a code example that shows how to read a Code 39 barcode out of a PNG
file: [EmguCV](https://www.emgu.com/wiki/index.php/Main_Page) loads the image and turns it into a
black and white image, [ZXing.Net](https://github.com/micjahn/ZXing.Net) decodes the barcode from
that image. Neither of the two is implemented here, both come from NuGet. The repository is an
example, it is **not** published as a NuGet package and it has **no** installer: no
`GeneratePackageOnBuild`, no push script, no `Setup` folder. Consumers read the code or copy it.

Beware of the names, nothing here is called like the repository:

- The repository is `EmguCVZXingBarcodeExample`.
- The solution is `src/TestBarcode.sln`.
- The project is `src/TestBarcode/TestBarcode.csproj`, `OutputType` `WinExe`, `UseWindowsForms`.
- The namespace is `TestBarcode`.

Layout inside `src/TestBarcode`:

- `Program.cs`: `Main` with `[STAThread]`, `EnableVisualStyles`, `SetCompatibleTextRenderingDefault`
  and `Application.Run(new Main())`. It has no console output, it is a `WinExe`.
- `Main.cs`: the one and only window. It wires the user interface to the service: pick a file, keep
  the loaded image, threshold it with the track bar value, show it, read the barcode from it.
- `Services/BarcodeService.cs` plus `Services/IBarcodeService.cs`: everything that touches EmguCV or
  ZXing. `LoadImage` reads a file, `GetBlackAndWhiteImage` thresholds it, `ReadBarcode` decodes it.
  The form holds no image processing of its own, which is what makes the three steps testable.
- `Main.Designer.cs` plus `Main.resx`: designer generated. A 3x3 `TableLayoutPanel` with a button,
  two rich text boxes (format and content of the barcode), a picture box for the thresholded image,
  a track bar (1 to 255, the threshold) and a label showing the track bar value.
- `Barcode.ico`: the application icon. `License.txt`: copied to the output directory.

`src/Images` holds the sample material: `Barcode_1.png`, `Barcode_2.png` and `Barcode_3.png` are
exported out of `Barcodes.pptx` (the same barcode at three rotations, with a red horizontal line as
the reference for the rotation), `barcode.png`, `barcode2.png` and `barcode3.png` are the older
example images. All six carry an alpha channel. The application does not read this folder by itself,
the file is always picked in the dialog.

Repository root: `README.md` (the only user documentation), `Changelog.md`, `License.txt` (MIT),
`.gitattributes`, `.gitignore`. Below `src`: `.editorconfig` and `TestBarcode.sln.DotSettings`.
There is no `.github` folder, no pipeline file, no `Directory.Build.props` and no `Updating.md`.

## Build

```powershell
dotnet build src/TestBarcode.sln
```

- Single target framework `net10.0-windows` in the one project, no multi-targeting.
  `RuntimeIdentifiers` is `win-x64`.
- All build properties live directly in `src/TestBarcode/TestBarcode.csproj`. There is **no**
  `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled, so every warning breaks the build, NuGet warnings (`NU****`)
  from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.5-1` for the first
  commit after tag `1.0.4`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and answers 404
  for public packages, restore fails with `NU1301`. Then build with an explicit source:
  `dotnet build src/TestBarcode.sln --source https://api.nuget.org/v3/index.json`.
- There is no test project, so a behaviour change is verified by running the application: pick one
  of the files in `src/Images`, move the track bar and look at the two rich text boxes.

## Code conventions

Follow the surrounding code, it is consistent throughout every hand written file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace, then the usings inside the namespace.
- XML doc comments on every type and every member, private members and event handlers included, no
  exceptions.
- `LangVersion latest` and `Nullable` are enabled, `ImplicitUsings` is **not**, unlike in the
  sibling repositories. Every file therefore lists its own usings inside the namespace and there is
  no `GlobalUsings.cs`. Keep it that way, the editorconfig asks for usings inside the namespace and
  global usings cannot satisfy that without a pragma.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` requires usings inside the namespace
  (`csharp_using_directive_placement=inside_namespace:warning`), braces everywhere, no multiple
  blank lines, four spaces, CRLF, UTF-8, file scoped namespaces, `System` usings sorted first and
  `IDE0005` as warning. Analyzer warnings are fixed, not silenced.
- `Main.Designer.cs` is generated code. It is not file-scoped, it does not qualify with `this.` in
  `Dispose`, and it has no header block. Leave that shape alone, the designer rewrites the file.
- The control names in the designer are Pascal case and say what the control is
  (`ButtonStart`, `RichTextBoxText`, `RichTextBoxContent`, `PictureBoxImage`,
  `TableLayoutPanelMain`, `TrackBar`, `Label`), which is not what the Windows Forms designer
  generates by default. Keep new controls in that scheme.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **Five packages, and every one of them is needed.** `Emgu.CV` is the managed wrapper,
  `Emgu.CV.runtime.windows` brings the native OpenCV binaries (without it the first call into
  `CvInvoke` throws `TypeInitializationException`), `Emgu.CV.Bitmap` holds the `ToBitmap` extension
  method the picture box needs, `ZXing.Net` is the decoder and `ZXing.Net.Bindings.EmguCV` is the
  luminance source that lets ZXing read an `Image<Bgr, byte>` directly. ZXing.Net itself has no
  `System.Drawing` based reader for .NET Core, so the binding is not a convenience, it is the
  connection between the two libraries this example is about. The runtime package is what makes the
  output directory 180 MB.
- **The images are always three channel `Image<Bgr, byte>`.** The threshold step converts to gray
  and straight back to `Bgr`, because that is what both the ZXing luminance source and the picture
  box take. A single channel image would save the conversion but fits neither.
- **A transparent background is flattened onto white while loading.** All six sample PNGs carry an
  alpha channel and the three `Barcode_*.png` are transparent around the barcode. `Imread` with
  `ImreadModes.ColorBgr` would put black there, which merges with the black bars and makes the
  barcode unreadable. `LoadImage` therefore reads with `ImreadModes.Unchanged` and copies the pixels
  onto a white image. Partial transparency is not blended, the alpha channel of these files only
  holds 0 or 255.
- **The user interface is German, the code is English.** The button says `Bild konvertieren`, the
  file dialog filter says `Png-Bilder|*.png`. There is no language file and no resource lookup, the
  strings sit in the code. Comments, identifiers and commit messages stay English regardless.
- **The example is Code 39 only.** `ReadBarcode` sets `PossibleFormats` to `BarcodeFormat.CODE_39`
  together with `UseCode39ExtendedMode` and `UseCode39RelaxedExtendedMode`. `barcode3.png` holds a
  UPC-E barcode, so it is decodable in principle but never by this example. That is what the README
  means by "can't be read", not that the file is broken.
- **`TryHarder` is on.** Without it ZXing scans a single row in the middle of the image, which is
  exactly the row the red reference line covers in `Barcode_1.png` to `Barcode_3.png`.
- **The track bar drives the whole run.** Its value is the threshold that goes into
  `GetBlackAndWhiteImage`, the label next to it shows that value. Moving it thresholds and reads the
  image that is already loaded again, it does not ask for a file. The initial value is 1, and the
  designer sets the label to `1` to match it, so both live next to each other in the same file.
- **The form owns two images and releases them itself.** `pickedImage` is the loaded file, kept for
  the next track bar move, and `PictureBoxImage.Image` is the bitmap on screen. A picture box does
  not dispose the image it shows, so both are released in `OnFormClosed` and the previous bitmap is
  released on every run. The Emgu images hold native memory, this is not decoration.
- **`src/Images/Barcodes.pptx` is tracked.** It is the source of the three `Barcode_*.png` files and
  the only way to produce a new rotation, so it stays in the repository.
- **`.gitattributes` sets `* text=auto`** and every rule of the Visual Studio template below it is
  commented out, the image rules included. The PNG and PPTX files are only kept binary by git's own
  heuristic. Any file where that heuristic could fail needs its own rule.
- **`src/TestBarcode.sln.DotSettings`** is tracked and holds nothing but a ReSharper user dictionary
  (`H_00E4mmer`). Leave it alone.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no `.github` folder and no pipeline file here.

## What version 1.0.4.0 got wrong, do not walk back into it

The state before 2026-08-11 is worth knowing, because most of it is invisible in a diff:

- **The repository did not compile.** Four errors in `Main.cs`. Commit `b4e77ab` (version 1.0.4.0)
  bumped Emgu.CV from 4.8.1.5350 to 4.9.0.5494, moved to `net9.0-windows`, edited `Main.cs` and
  never compiled the result. ZXing.Net has no `System.Drawing` based `BarcodeReader` for .NET Core,
  the non-generic type only exists in the `net45` to `net48` assets, so `BarcodeReader` resolved to
  `BarcodeReader<T>` and `Decode(Bitmap)` was gone. `Image<Gray, byte>.ToBitmap()` had moved into
  `Emgu.CV.Bitmap`.
- **The native OpenCV binaries were missing**, only the managed `Emgu.CV` package was referenced.
- **The track bar reopened the file dialog** on every scroll tick, because `TrackBarScroll` called
  the click handler of the button, dialog included.
- **Nothing was disposed**, neither the Emgu images nor the two bitmaps per run.
- **Not one of the six sample images could be decoded**, because the transparent background became
  black. Measured over all 254 thresholds: zero hits for `Barcode_1.png` and `Barcode_2.png`.
- **`System.Private.Uri 4.3.2` and `System.Text.Json 9.0.0` were pinned** to silence the NuGet
  audit. With the current packages the audit is clean without them, so they are gone. Do not add
  such a pin back without checking that a restore actually reports something.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.5.0 (2026-08-11)** : Short description.`
3. Commit that.
4. Tag the commit with the plain version number, no `v` prefix (`1.0.4`, `1.0.3`, ...). The existing
   tags are lightweight tags, create new ones the same way.
5. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.5.0`), the tag has three (`1.0.5`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.0.5-1+Branch.master.Sha...`. There is no installer to build and no package to push, so the
release ends with the push.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation. The user interface strings are the one exception, they
  are German.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
