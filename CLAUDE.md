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
- `Main.cs`: the one and only window. It owns the whole example: pick a file, threshold it, decode
  it, show the result.
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

- Single target framework `net9.0-windows` in the one project, no multi-targeting.
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
- `LangVersion latest` is enabled. `Nullable` and `ImplicitUsings` are **not** enabled in this
  project, unlike in the sibling repositories.
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

- **The user interface is German, the code is English.** The button says `Bild konvertieren`, the
  file dialog filter says `Png-Bilder|*.png`. There is no language file and no resource lookup, the
  strings sit in the code. Comments, identifiers and commit messages stay English regardless.
- **The example is Code 39 only.** `Main.cs` sets `PossibleFormats` to `BarcodeFormat.CODE_39`
  together with `UseCode39ExtendedMode` and `UseCode39RelaxedExtendedMode`. `barcode3.png` holds a
  UPC-E barcode, so it is decodable in principle but never by this example. That is what the README
  means by "can't be read", not that the file is broken.
- **`TryHarder` is on.** Without it ZXing scans a single row in the middle of the image, which is
  exactly the row the red reference line covers in `Barcode_1.png` to `Barcode_3.png`.
- **The track bar drives the whole run.** Its value is the threshold passed to `ThresholdBinary`,
  the label next to it shows that value. The initial value is 1.
- **`src/Images/Barcodes.pptx` is tracked.** It is the source of the three `Barcode_*.png` files and
  the only way to produce a new rotation, so it stays in the repository.
- **`.gitattributes` sets `* text=auto`** and every rule of the Visual Studio template below it is
  commented out, the image rules included. The PNG and PPTX files are only kept binary by git's own
  heuristic. Any file where that heuristic could fail needs its own rule.
- **`src/TestBarcode.sln.DotSettings`** is tracked and holds nothing but a ReSharper user dictionary
  (`H_00E4mmer`). Leave it alone.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository. There is no `.github` folder and no pipeline file here.

## Defects found on 2026-08-11, fix them, do not preserve them

- **The build is broken.** `dotnet build src/TestBarcode.sln -c Release` fails with four errors in
  `Main.cs`. ZXing.Net has no `System.Drawing` based `BarcodeReader` for .NET Core any more, the
  non-generic type only exists in the `net45` to `net48` assets of the package, so `BarcodeReader`
  resolves to `BarcodeReader<T>` and `Decode(Bitmap)` is gone. `Image<Gray, byte>.ToBitmap()` was
  moved out of `Emgu.CV` into the separate package `Emgu.CV.Bitmap`. The last commit
  (`b4e77ab`, version 1.0.4.0) bumped Emgu.CV from 4.8.1.5350 to 4.9.0.5494 and edited `Main.cs`
  without ever compiling it.
- **The native OpenCV binaries are missing.** The project references only the managed `Emgu.CV`
  package. Without `Emgu.CV.runtime.windows` the first call into `CvInvoke` throws
  `TypeInitializationException`, so the application could not work even if it compiled.
- **The track bar reopens the file dialog.** `TrackBarScroll` calls `TestImageClick`, which starts
  with `OpenFileDialog.ShowDialog()`. Every single scroll tick asks for the file again instead of
  rethresholding the image that is already loaded.
- **Nothing is disposed.** `Image.FromFile`, the `Image<Bgr, byte>`, the `Image<Gray, byte>`, both
  `ToBitmap()` results and the previous `PictureBoxImage.Image` are all left to the finalizer. The
  Emgu images hold native memory.
- **The sample images cannot be decoded at all.** All six PNGs have an alpha channel and the three
  `Barcode_*.png` have a transparent background. Loading them without flattening the alpha puts
  black behind the black bars and the barcode is gone. Measured over all 254 thresholds: zero hits
  for `Barcode_1.png` and `Barcode_2.png`. Flattened onto white both decode as Code 39 `ABC-1234`,
  which is what the README claims.
- **The README image links are broken.** They point at
  `https://github.com/SeppPenner/EmguCVZXingBarcodeExample/blob/master/Images/...`, the files live
  in `src/Images/`. On top of that a `blob` link renders the GitHub page, not the image, so they
  need `raw.githubusercontent.com`.
- **`Label.Text` is `label1` and the window title is `Form1`.** Both are designer defaults that
  were never touched.

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
