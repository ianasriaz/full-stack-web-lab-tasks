# .NET MAUI Blazor Hybrid Lab — Windows Installation Instructions

**Target:** .NET 10 (LTS) • .NET MAUI 10 • Windows
**Purpose:** Environment setup for the multi-tier lab: `Presentation.Client`, `Application`, `Domain`, `Infrastructure`

> **Recommended setup:** Use Visual Studio 2026 Community (or higher edition) with the **.NET Multiplatform App UI development** workload. This is the simplest and most reliable setup for .NET 10 on Windows.

---

## 1. What You Need

| Component | Required / Recommended | Purpose |
|---|---|---|
| Windows | Windows 11 recommended | Development host for Windows and Android targets |
| Visual Studio | Visual Studio 2026 Community or later | IDE, MAUI tooling, Windows SDK, Android tooling |
| .NET SDK | .NET 10 SDK | Builds the lab projects and provides the .NET CLI |
| .NET MAUI | .NET MAUI 10 workload | Cross-platform application framework and templates |
| Optional | Android emulator or physical device | Testing the Android target from Windows |

---

## 2. Step 1 — Install Visual Studio 2026

Visual Studio is the recommended development environment on Windows because it can install the .NET SDK, MAUI tooling, Windows SDK, and Android dependencies through one installer.

1. Open the official Visual Studio download page: <https://visualstudio.microsoft.com/downloads/>
2. Download **Visual Studio Community 2026**. Community is free for eligible individual users, students, classrooms, and open-source development.
3. Run the downloaded Visual Studio Installer.
4. When the Workloads screen appears, select: **.NET Multi-platform App UI development**
5. Keep the default optional components selected unless your instructor specifies otherwise.
6. Click **Install** and wait for the installation to finish. Restart Windows if the installer requests it.

> **Why Visual Studio 2026?** .NET 10 requires Visual Studio 2026 version 18.0 or later when using the Visual Studio IDE. Visual Studio 2022 is suitable for earlier .NET SDK generations but should not be the standard environment for this .NET 10 lab.

---

## 3. Step 2 — Verify the .NET 10 SDK

Open Windows Terminal, PowerShell, or Command Prompt and run:

```
dotnet --version
```

The result should begin with `10.` (for example, `10.0.400`).

To view every installed SDK:

```
dotnet --list-sdks
```

> **If .NET 10 is missing:** Open Visual Studio Installer → Modify → Individual components and install the .NET 10 SDK, or install it from the official .NET download page: <https://dotnet.microsoft.com/download/dotnet/10.0>

---

## 4. Step 3 — Verify the .NET MAUI Workload

```
dotnet workload list
```

If MAUI was installed through Visual Studio Installer, the list may show component workload IDs such as:

- `android`
- `maui-windows`
- `maccatalyst`
- `ios`

A manually installed SDK may instead show a workload named `maui`.

> **Important:** On Windows, prefer the Visual Studio Installer to install and maintain the MAUI workload. Avoid mixing Visual Studio Installer workload installation with repeated CLI workload installation, because Microsoft documents cases where mixing the two mechanisms can cause workload-resolution problems.

---

## 5. Optional CLI-Only MAUI Installation

Use this only when MAUI was **not** installed through Visual Studio Installer, or when your instructor specifically requires CLI-based setup. Run as a normal user in Windows Terminal or PowerShell:

```
dotnet workload install maui
dotnet workload list
```

No `sudo` command is used on Windows.

---

## 6. Step 4 — Verify the MAUI Templates

```
dotnet new list maui
```

You should see MAUI templates, including an entry for a .NET MAUI Blazor Hybrid application. The short name required in this lab is `maui-blazor`.

Confirm the exact template is available:

```
dotnet new list maui-blazor
```

---

## 7. If the MAUI Template Is Missing

If the workload appears installed but the template does not appear:

1. Close Visual Studio and try repairing/updating the MAUI workload through Visual Studio Installer.
2. If the template is still missing, install it manually:

```
dotnet new install Microsoft.Maui.Templates
```

3. Verify again:

```
dotnet new list maui
```

---

## 8. Step 5 — Enable Windows Developer Mode

Developer Mode is recommended when building and testing Windows applications locally.

1. Open Windows Settings.
2. Navigate to **System → Advanced → For developers** (the exact path may vary slightly by Windows build).
3. Turn on **Developer Mode** and accept the confirmation prompt.

> **Note:** If your university or organization manages the computer, Developer Mode may be controlled by administrator policy.

---

## 9. Step 6 — Verify MAUI in Visual Studio

1. Open Visual Studio 2026.
2. Select **Create a new project**.
3. In the template search box, type: `MAUI Blazor`
4. Confirm that a template similar to **.NET MAUI Blazor Hybrid App** is available.
5. You do not need to create the actual lab solution from this screen if the lab uses the command line. This step only confirms that Visual Studio recognizes the MAUI templates.

---

## 10. Step 7 — Create a Small Test Project

Before building the four-tier lab solution, create a small test project. This separates environment problems from architecture/code problems.

```
cd %USERPROFILE%\Desktop
mkdir MauiEnvironmentTest
cd MauiEnvironmentTest
dotnet new maui-blazor -n MauiBlazorTest
```

> **PowerShell users:** The same commands work in PowerShell. If the Desktop folder is redirected through OneDrive, use a folder you can easily locate, such as `C:\Projects\MauiEnvironmentTest`.

---

## 11. Step 8 — Run the Test App on Windows

**Recommended Visual Studio method:**

1. Open `MauiBlazorTest.csproj` in Visual Studio 2026.
2. Wait for project restore and background setup to complete.
3. In the target/device selector, choose **Windows Machine**.
4. Press **F5**, or click the Start button.

The default .NET MAUI Blazor Hybrid app should open as a native Windows application.

**Command-line verification** (from the test project folder):

```
dotnet build -f net10.0-windows10.0.19041.0
```

The generated MAUI project file is the authoritative source for its exact Windows target framework. If your generated target differs, use the value shown in the project's `TargetFrameworks` property.

---

## 12. Optional — Prepare Android Testing on Windows

A Windows development computer can directly target Windows and Android. The MAUI workload normally installs the required Android development components.

1. Open Visual Studio 2026 and load a MAUI project.
2. Choose an Android emulator from the device selector. If none exists, use the Android Device Manager to create one.
3. If prompted, allow Visual Studio to install missing Android SDK/emulator components.
4. Ensure CPU virtualization is enabled in your computer firmware and Windows virtualization features are available when using an emulator.
5. Alternatively, connect a physical Android device with USB debugging enabled.

> **Platform limitation:** Windows can directly build and run MAUI apps for Windows and Android. Building/signing iOS or Mac Catalyst apps requires access to a compatible Mac.

---

## 13. Optional — Use Visual Studio Code Instead

VS Code can also be used on Windows, but Visual Studio 2026 is recommended for this lab because the Windows MAUI toolchain is easier to install and manage through Visual Studio Installer.

1. Install the latest Visual Studio Code.
2. Open Extensions (`Ctrl+Shift+X`).
3. Search for and install the **Microsoft .NET MAUI** extension.
4. The extension also installs the C# Dev Kit and C# extensions required for MAUI development.
5. Use the *Get Started with .NET MAUI* walkthrough to verify your .NET and target-platform configuration.

---

## 14. Step 9 — Create the Actual Lab Solution

Once the test project runs successfully, delete the temporary test project if desired and continue with the lab manual. The actual lab starts with the four-tier structure:

```
MauiHybridLab/
├── MauiHybridLab.slnx
└── src/
    ├── Presentation.Client/
    ├── Application/
    ├── Domain/
    └── Infrastructure/
```

The MAUI Blazor Hybrid presentation project is created with:

```
dotnet new maui-blazor -n Presentation.Client -o src\Presentation.Client
```

The remaining three projects are standard .NET 10 class libraries:

```
dotnet new classlib -n Domain -o src\Domain -f net10.0
dotnet new classlib -n Application -o src\Application -f net10.0
dotnet new classlib -n Infrastructure -o src\Infrastructure -f net10.0
```

---

## 15. Common Windows Setup Problems

| Problem | Recommended action |
|---|---|
| `dotnet` is not recognized | Restart Windows or reopen the terminal. Confirm the .NET 10 SDK is installed through Visual Studio Installer. |
| `dotnet new list maui` finds nothing | Confirm the MAUI workload is installed. Repair the Visual Studio MAUI workload; if necessary install `Microsoft.Maui.Templates`. |
| Windows target does not appear in Visual Studio | Open Visual Studio Installer → Modify and verify **.NET Multi-platform App UI development** and Windows SDK components are installed. |
| Build reports a missing Windows SDK | Modify Visual Studio and install a current Windows SDK, then rebuild. |
| Android emulator will not start | Check hardware virtualization and Windows virtualization support; use a physical Android device if needed. |
| Workload errors after mixing installers | Prefer one workload-management path. On Windows, repair/reinstall MAUI through Visual Studio Installer. |

---

## 16. Windows Installation Checklist

- [ ] Visual Studio 2026 is installed.
- [ ] .NET Multi-platform App UI development workload is installed.
- [ ] .NET 10 SDK is detected by `dotnet --version`.
- [ ] MAUI workloads are visible in `dotnet workload list`.
- [ ] `maui-blazor` appears in `dotnet new list maui`.
- [ ] Windows Developer Mode is enabled (where permitted).
- [ ] A test .NET MAUI Blazor Hybrid application builds successfully.
- [ ] The test application runs using the Windows Machine target.

---

## 17. Official References

- Microsoft Learn — .NET MAUI installation
- Microsoft Learn — Install .NET on Windows
- .NET 10 downloads: <https://dotnet.microsoft.com/download/dotnet/10.0>
- Visual Studio downloads: <https://visualstudio.microsoft.com/downloads/>
- Microsoft Learn — MAUI template missing troubleshooting
