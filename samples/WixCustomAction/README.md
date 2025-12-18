# WiX Custom Action Sample

This sample demonstrates how to integrate license management into a WiX installer using custom actions.

## Overview

The custom actions handle:
- **Installation**: Registers the computer and downloads the license file
- **Uninstallation**: Unregisters the computer to free up the license seat

## Files

- `CustomAction.csproj` - Project file for the custom action DLL
- `HandleLicenseAction.cs` - Custom action implementation
- `Package.wxs` - Sample WiX package showing custom action configuration

## Prerequisites

- [WiX Toolset v5](https://wixtoolset.org/)
- .NET Framework 4.8.1
- [LicenseManagement.EndUser](https://www.nuget.org/packages/LicenseManagement.EndUser) NuGet package (v2.0+)
- Account at [license-management.com](https://license-management.com)

## Setup

### 1. Get Your Credentials

From the License Management dashboard, note:
- **Vendor ID**: `VDR_01...`
- **Product ID**: `PRD_01...`
- **Client API Key**: `PUB_01...` (NOT your Master key!)

### 2. Build the Custom Action

```bash
dotnet build CustomAction.csproj -c Release
```

### 3. Configure Package.wxs

Replace the placeholder values in `Package.wxs`:

```xml
<CustomAction Id="CA_SetInstallProperties"
              Property="CA_InstallLicense"
              Value="ApiKey=PUB_YOUR_KEY;ProductId=PRD_YOUR_ID;VendorId=VDR_YOUR_ID;ValidDays=90"/>
```

### 4. Reference in Your Installer

1. Copy the built `CustomAction.CA.dll` to your WiX project
2. Include the custom action configuration from `Package.wxs` in your installer

## How It Works

### During Installation

1. `CA_SetInstallProperties` sets the configuration data
2. `CA_InstallLicense` runs with these steps:
   - Generates a unique computer ID (MAC address)
   - Registers the computer with the API
   - Downloads and saves the license file

### During Uninstallation

1. `CA_SetUninstallProperties` sets the configuration data
2. `CA_UninstallLicense` runs:
   - Calls the API to unregister this computer
   - Frees up the license seat for use elsewhere

## Multiple Products

To handle multiple products in one installer:

```csharp
// In HandleLicenseAction.cs
var productKeys = new[] { "Product1Id", "Product2Id" };
```

```xml
<!-- In Package.wxs -->
<CustomAction Id="CA_SetInstallProperties"
              Property="CA_InstallLicense"
              Value="ApiKey=PUB_xxx;Product1Id=PRD_aaa;Product2Id=PRD_bbb;VendorId=VDR_xxx;ValidDays=90"/>
```

## Error Handling

The sample is configured with `Return="ignore"` which means:
- Installation continues even if license registration fails
- The application can retry license download at first launch

For stricter enforcement, change to `Return="check"` and return `ActionResult.Failure` from the custom action.

## Debugging

View MSI logs to troubleshoot:

```bash
msiexec /i YourInstaller.msi /l*v install.log
```

Search the log for "InstallLicense" or "UnInstallLicense" to see custom action output.

## Related

- [LicenseManagement.EndUser README](../../README.md)
- [WPF Sample](https://github.com/HYMMA/LicenseManagement.EndUser.Wpf/tree/master/samples/WpfSampleApp)
- [Documentation](https://license-management.com/docs/)
