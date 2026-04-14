# AGENTS.md

## Project Snapshot
- This repository contains a single C# Windows Forms desktop application for previewing SATO SBPL label content.
- The solution is `sbpl-visualizer.sln`; the only project is `sbpl-visualizer/sbpl-visualizer.csproj`.
- Target framework is `.NET Framework 4.7.2`.
- There is no web app, API, service host, class library, or test project in the repo.
- The application currently has two operating modes in the same UI:
  - offline template preview from `sbpl-visualizer/Example/*.S84`
  - DB-backed label hydration using a SQL Server stored procedure, then rendering the resulting SBPL
- There are no NuGet package references in the project file. Dependencies are .NET Framework assemblies only.

## Actual Architecture
- Entry point: `sbpl-visualizer/Program.cs`
- Main UI and orchestration: `sbpl-visualizer/Form1.cs`
- Designer-generated UI layout: `sbpl-visualizer/Form1.Designer.cs`
- Parsing and command dispatch: `sbpl-visualizer/SBPLParser.cs`
- Shared render state: `sbpl-visualizer/SBPLContext.cs`
- Command implementations: `sbpl-visualizer/Commands/*.cs`
- Sample templates and notes: `sbpl-visualizer/Example/*`
- Bundled font asset: `sbpl-visualizer/Fonts/PressStart2P-Regular.ttf`

Runtime flow:
1. `Program.Main()` starts `Form1`.
2. `Form1` loads available `.S84` template files from the `Example` folder into the combo box.
3. The user can:
   - paste or edit SBPL directly in `txtSBPL`
   - load a sample template from `Example`
   - enter a work order and fetch label data from SQL Server
4. For DB-backed rendering, `Form1` calls stored procedure `CAM_Label_Details`, reads the first row, substitutes placeholders in the selected template, and writes the final SBPL into `txtSBPL`.
5. The render buttons create a `Bitmap`, create a `Graphics`, and call `SBPLParser.ParseAndRender(...)`.
6. `SBPLParser` splits SBPL by either the ESC character (`"\u001b"`) or literal `<ESC>`, matches the longest registered command key, and dispatches to an `ISBPLCommand`.
7. Commands mutate `SBPLContext` and/or draw directly onto the `Graphics`.
8. The main render path rotates the rendered bitmap 180 degrees, saves it to `C:\APPS\label.jpg`, and assigns it to the preview picture box.

## Key Files And Responsibilities
- `sbpl-visualizer/Program.cs`
  Application bootstrap only.
- `sbpl-visualizer/Form1.cs`
  Primary orchestration layer. Contains:
  - sample template discovery
  - render button handlers
  - direct SQL Server access via `System.Data.SqlClient`
  - placeholder replacement logic for hydrated templates
  - preview image assignment
  - hardcoded JPG export path
- `sbpl-visualizer/Form1.Designer.cs`
  Generated WinForms layout. Keep business logic out of this file.
- `sbpl-visualizer/SBPLParser.cs`
  Command registry plus tokenization and dispatch.
- `sbpl-visualizer/SBPLContext.cs`
  Mutable render state shared across commands:
  - `X`, `Y`
  - `OffsetX`, `OffsetY`
  - `ScaleX`, `ScaleY`
  - `CurrentFont`
  - `FontCode`
  - `Rotation`
- `sbpl-visualizer/ISBPLCommand.cs`
  Command interface for all parser actions.
- `sbpl-visualizer/Commands/*.cs`
  Concrete handlers for positioning, fonts, scaling, rotation, text, and barcode drawing.

## Supported Parser Commands
Current command registration in `SBPLParser`:
- `H` -> `MoveXCommand`
- `V` -> `MoveYCommand`
- `T` -> `DrawTextCommand`
- `S` -> `SetSmallFontCommand` + `DrawTextCommand`
- `M` -> `SetMediumFontCommand` + `DrawTextCommand`
- `B` -> `DrawBarcodeCommand`
- `A3` -> `SetOffsetCommand`
- `L` -> `SetScaleCommand`
- `%` -> `SetRotationCommand`

Notes:
- Command matching is longest-first. Changes to keys can affect existing dispatch behavior.
- Unsupported tokens are silently ignored today.
- The parser currently supports only a subset of SBPL used by the included templates.
- `DrawBarcodeCommand` currently supports only ITF-like barcode generation for symbology `2`.

## Data Sources And External Dependencies
- SQL Server access is present in `Form1.cs` through `System.Data.SqlClient`.
- The current connection string is hardcoded in `Form1.cs`.
- The DB call uses stored procedure `CAM_Label_Details` with parameter `@WorkOrderID`.
- No ORM, repository layer, service abstraction, or configuration-driven connection management exists yet.
- The app reads `.S84` templates directly from `Application.StartupPath\\Example`.
- The app writes a rendered JPG to `C:\APPS\label.jpg` in the main render path.

## Fonts And Rendering Behavior
- `SBPLParser` tries to create `new Font("Press Start 2P", 8)`.
- If that installed font is unavailable, it falls back to `SystemFonts.DefaultFont`.
- The bundled `Fonts/PressStart2P-Regular.ttf` file is present in the repo but is not loaded directly by the current code.
- Rendering uses GDI+ through `System.Drawing`.
- Commands use `Graphics.Save()` / `Graphics.Restore()` around transforms.
- Text rendering resets scale state after drawing.
- Barcode rendering applies scale but does not currently reset scale state inside the command.
- Rotation handling is stateful and command-dependent. Inspect the full render path before changing it.

## Key Directories
- `sbpl-visualizer/`
  Main application source.
- `sbpl-visualizer/Commands/`
  SBPL command handlers.
- `sbpl-visualizer/Example/`
  Sample SBPL templates and notes used for manual verification.
- `sbpl-visualizer/Fonts/`
  Bundled font asset.
- `sbpl-visualizer/Properties/`
  Standard WinForms assembly, resource, and settings metadata.
- `sbpl-visualizer/bin`, `sbpl-visualizer/obj`, `.vs/`
  Generated output and IDE state. Do not edit manually.

## Build, Run, Test
- Preferred build: open `sbpl-visualizer.sln` in Visual Studio and build `Debug|Any CPU` or `Release|Any CPU`.
- Command-line build, when a compatible MSBuild toolset is installed:
  `MSBuild.exe sbpl-visualizer.sln /t:Build /p:Configuration=Debug /p:Platform="Any CPU"`
- Use a Visual Studio/MSBuild toolset that supports `ToolsVersion="15.0"` and `.NET Framework 4.7.2`.
- Run from Visual Studio or launch `sbpl-visualizer/bin/Debug/sbpl-visualizer.exe` after a successful build.
- There is no automated test project in this repository.

Manual verification:
- Use `sbpl-visualizer/Example/*.S84` as sample input.
- Verify both render modes:
  - raw ESC parsing
  - literal `<ESC>` parsing
- Verify text placement, scaling, offsets, rotation, and barcode output visually in the preview pane.
- If touching DB-backed template hydration, validate at least one work-order path and one file-template path.
- If touching output behavior, verify whether `C:\APPS\label.jpg` is expected to exist on the target machine.

## Existing Code Patterns
- Keep WinForms behavior in `Form1.cs` and layout code in `Form1.Designer.cs`.
- Preserve the command-based parser design. New SBPL behavior should be added as `ISBPLCommand` implementations and registered in `SBPLParser`.
- Keep shared render-state mutations in `SBPLContext` rather than threading flags through unrelated methods.
- Follow the current style in touched files:
  - Allman braces
  - tabs in older files, though some newer edits are space-indented
  - PascalCase for types, methods, properties, and constants
  - camelCase for locals and parameters
  - early returns for malformed command arguments
- Avoid cleanup-only refactors. The code has inconsistencies; do not normalize unrelated files while fixing behavior.

## Safe Modification Rules
- Inspect the full render path before changing behavior:
  `Form1` -> `SBPLParser` -> `ISBPLCommand` -> `SBPLContext`
- Keep both parser input modes working:
  - raw ESC
  - literal `<ESC>`
- Be careful with command key changes in `SBPLParser`; dispatch is order-sensitive because matching is longest-first.
- Be explicit about command side effects:
  - some commands only mutate context
  - some commands only draw
  - some commands do both
- Preserve current public behavior unless the task explicitly changes it:
  - template file loading from `Example`
  - DB-backed work-order loading
  - preview rendering into `picPreview`
- Do not move business logic into the designer file.
- Do not edit generated output under `bin/`, `obj/`, or `.vs/`.
- If changing font handling, decide explicitly whether to:
  - keep using the installed font name
  - load the bundled `.ttf`
  - add a fallback strategy
- If changing SQL access, keep in mind there is no abstraction layer. Introduce one only if the task justifies it.

## Performance
- Rendering is synchronous on the UI thread.
- DB access is also synchronous on the UI thread.
- Avoid expensive work inside parser token loops or button handlers.
- Keep parsing linear and predictable.
- Avoid repeated allocations in per-token command execution where possible.
- Dispose any new `Bitmap`, `Font`, `Brush`, `Pen`, `Graphics`, or DB resources you introduce.
- Always restore `Graphics` transforms after scaling or rotation changes.

## Security And Reliability
- Treat SBPL input as untrusted text.
- Parse and render only. Do not execute processes, shell commands, file writes beyond the explicit image export path, or network calls based on label content.
- Keep numeric bounds checks on scale, rotation, offsets, and barcode dimensions.
- The hardcoded SQL connection string is sensitive operational configuration. Do not duplicate it into new locations without a clear reason.
- The hardcoded output path `C:\APPS\label.jpg` may fail on machines where the directory does not exist or is not writable. Be explicit if you change that behavior.

## Debugging Guidance
- When fixing rendering bugs:
  - identify whether the fault is in tokenization, command matching, context mutation, or drawing
  - validate one command at a time
  - avoid refactoring parser and UI logic together unless necessary
- When fixing DB/template bugs:
  - separate stored-procedure result issues from placeholder replacement issues
  - check whether the selected template file actually exists in `Example`
- When fixing layout issues:
  - verify the effect of `OffsetX`, `OffsetY`, `ScaleX`, `ScaleY`, and `Rotation` together
  - verify the final `Bitmap` rotation and any saved-file side effects

## Database And Template Notes
- Placeholder replacement is currently implemented as chained `string.Replace(...)` calls inside `Form1`.
- The template is uppercased before replacements in the work-order flow.
- Sample templates include tokens outside the currently supported parser feature set.
- `.info` files in `Example` document parts of the sample SBPL command usage and are useful for manual inspection.

## Change Verification
- For parser or command changes, validate against at least one sample file in `sbpl-visualizer/Example/`.
- For DB-related changes, validate both:
  - successful stored-procedure population
  - empty or error-path behavior in the UI
- For new command support, document:
  - command key
  - expected argument format
  - whether it mutates context, draws, or both
  - interactions with offsets, scaling, and rotation
- If you cannot run the app locally, state that clearly and explain what was inspected instead.
