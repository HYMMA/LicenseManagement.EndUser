# LicenseManagement.EndUser

[![Build and Test](https://github.com/HYMMA/LicenseManagement.EndUser/actions/workflows/build.yml/badge.svg)](https://github.com/HYMMA/LicenseManagement.EndUser/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Hymma.Lm.EndUser.svg)](https://www.nuget.org/packages/Hymma.Lm.EndUser)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Hymma.Lm.EndUser.svg)](https://www.nuget.org/packages/Hymma.Lm.EndUser)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

End-user SDK for [license-management.com](https://license-management.com) - A license management platform for software vendors.

This library is designed for **client-side applications** (desktop apps, plugins, add-ins) to validate licenses, handle trials, and manage activations.

## Features

- **License Validation** - Validate licenses at application launch
- **Trial Management** - Automatic trial period handling with anti-tampering
- **Installation Handling** - Register computers during application install
- **Uninstall Support** - Properly unregister computers to free seats
- **Offline Support** - License validation works offline after initial activation
- **Digital Signatures** - RSA signature verification for license files
- **Time Sync Detection** - Detects system clock tampering
- **Hardware Identification** - Secure device fingerprinting

## Installation

```bash
dotnet add package Hymma.Lm.EndUser
```

Or via NuGet Package Manager:
```
Install-Package Hymma.Lm.EndUser
```

## Quick Start

### 1. Configure Publisher Preferences

```csharp
using LicenseManagement.EndUser;

var preferences = new PublisherPreferences
{
    VendorId = "VDR_01ABC...",      // Your vendor ID
    ProductId = "PRD_01XYZ...",     // Your product ID
    ApiKey = "your-client-api-key", // Client API key (NOT master key)
    PublicKey = "<RSAKeyValue>...</RSAKeyValue>", // For signature verification
    TrialDays = 14,                 // Trial period duration
    ValidDays = 90                  // License cache validity
};
```

### 2. Handle License at Installation

```csharp
// During MSI/Setup installation
var context = new LicHandlingContext(preferences);

var installHandler = new LicenseHandlingInstall(
    context,
    onSuccess: (ctx) => {
        Console.WriteLine("License installed successfully!");
    }
);

await installHandler.HandleLicenseAsync();
```

### 3. Validate License at Launch

```csharp
// At application startup
var context = new LicHandlingContext(preferences);

var launchHandler = new LicenseHandlingLaunch(
    context,
    onLicenseHandledSuccessfully: (license) => {
        Console.WriteLine($"License valid until: {license.Expires}");
    },
    onCustomerMustEnterProductKey: () => {
        // Show product key entry dialog
        return GetProductKeyFromUser();
    },
    onTrialValidated: () => {
        Console.WriteLine("Trial period active");
        return null;
    },
    onTrialEnded: (prefs) => {
        // Show trial expired message
        MessageBox.Show("Your trial has expired. Please purchase a license.");
    },
    onLicFileNotFound: (ctx) => {
        // License file missing - may need reinstall
    }
);

await launchHandler.HandleLicenseAsync();
```

### 4. Handle Uninstallation

```csharp
// During uninstall to free up the license seat
var context = new LicHandlingContext(preferences);

var uninstallHandler = new LicenseHandlingUninstall(
    context,
    onSuccess: (ctx) => {
        Console.WriteLine("Computer unregistered successfully");
    }
);

await uninstallHandler.HandleLicenseAsync();
```

## License States

| State | Description |
|-------|-------------|
| `Valid` | Active paid subscription |
| `ValidTrial` | Within trial period |
| `InValidTrial` | Trial period expired |
| `Expired` | License has expired |
| `ReceiptExpired` | Subscription ended |
| `ReceiptUnregistered` | Computer unregistered from receipt |

## Events

The `LicHandlingContext` provides events for handling license state changes:

```csharp
context.OnCustomerMustEnterProductKey += () => { /* Show key entry UI */ };
context.OnTrialValidated += () => { /* Trial is active */ };
context.OnLicenseFileNotFound += () => { /* Handle missing license */ };
context.OnTrialEnded += () => { /* Trial expired */ };
context.OnLicenseHandledSuccessfully += () => { /* Success */ };
```

## Custom Metadata

Attach custom metadata to licenses during installation:

```csharp
preferences.BeforeLicensePost += (sender, args) => {
    args.Metadata["CustomerName"] = "John Doe";
    args.Metadata["OrderId"] = "ORD-12345";
};
```

## Storage Locations

- **Computer ID**: Windows Registry (`HKLM\Software\Hymma\LicenseManagement`)
- **License File**: `%LocalAppData%\License-Management.com\{VendorId}\{ProductName}.lic`

## Exception Handling

```csharp
try
{
    await launchHandler.HandleLicenseAsync();
}
catch (ComputerOfflineException)
{
    // No internet - but offline validation may still work
}
catch (CouldNotReadLicenseFromDiskException)
{
    // License file corrupted or missing
}
catch (LicenseExpiredException)
{
    // License has expired
}
catch (ReceiptExpiredException)
{
    // Subscription ended
}
catch (ApiException ex)
{
    // API communication error
    Console.WriteLine($"API Error: {ex.StatusCode}");
}
```

## Requirements

- .NET Framework 4.8.1
- Windows OS (uses WMI for hardware identification)
- Internet connection for initial activation (offline afterward)

## Security Features

- **RSA Signature Verification** - Licenses are digitally signed
- **Hardware Fingerprinting** - CPU + Motherboard serial numbers
- **Time Tampering Detection** - NTP sync validation
- **Secure Registry Storage** - Computer ID in HKLM

## License

MIT - See [LICENSE](LICENSE) for details.

## Related Packages

- [LicenseManagement.Client](https://www.nuget.org/packages/LicenseManagement.Client) - Server-side SDK for vendors
- [LicenseManagement.EndUser.Wpf](https://www.nuget.org/packages/Hymma.Lm.EndUser.Wpf) - WPF UI components
