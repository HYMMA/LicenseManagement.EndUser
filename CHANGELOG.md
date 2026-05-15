# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - 2026-05-15

Audit-driven hardening release. See `docs/audit2026-05-15.md` for the full audit report.

### Added
- `ILicenseLogger` interface for consumer-supplied observability (request URL, status code, latency, masked API key, correlation id).
- Centralized HTTP helper (`Utilities/ApiHttp.cs`) with: 15s default `Timeout`, 3-attempt retry-with-backoff for 429/503 (capped ~10s for CA safety), `Retry-After` honoring, `X-Correlation-Id` propagation, `Idempotency-Key` on POSTs, RFC 7807 problem+json body parsing on non-2xx, `CancellationToken` end-to-end.
- New typed exceptions: `InvalidApiKeyException`, `RateLimitException`, `NetworkUnavailableException`, `ProblemDetailsException`.
- `PublisherPreferences.ToString()` override that masks the API key (last 4 chars only) so reflexive `session.Log("{0}", preferences)` calls in WiX custom actions stop leaking secrets.
- `LicenseHandlingOptions` with `MaxRetryDuration`, `RequestTimeout`, `Logger` so MSI custom actions can bound wall-clock duration.

### Changed
- **BREAKING**: `LicenseValidationHandler` is now `internal`. Consumers should not subclass it.
- **BREAKING**: `*ApiEndPoint` classes (`ComputerApiEndPoint`, `LicenseApiEndPoint`, `ProductApiEndPoint`, `DateTimeApiEndPoint`) are now `internal`.
- **BREAKING**: `MainRegistryKeyInHKLM`, `MainRegistryKeyInHkcu`, `NullDeviceIdFormatter` are now `internal`.
- **BREAKING**: `Constants.DefaultLicFileRootDir` is now a get-only property (was a mutable static field).
- `LicHandlingContext.SetLicenseData` is now `internal` (exposed to the test project via `InternalsVisibleTo`; the test project is signed with the same `HymmaLm.snk` key).
- `LicenseSignatureValidationHandler` now sets `XmlResolver = null` before `LoadXml`, validates the public key contains no private-key elements (`<P>`, `<Q>`, `<D>`), and refuses to fetch the public key over plain HTTP.
- `ErrorHandler` now throws typed exceptions instead of base `System.Exception`. Sync and async paths translate exceptions consistently.
- `ConfigureAwait(false)` applied across all `await` calls inside library handlers and endpoint helpers.
- Sync endpoint methods now wrap `.GetAwaiter().GetResult()` in `Task.Run(…)` to defensively escape any caller-supplied sync context (matters for SolidWorks STA host; CA host is unaffected).
- `AuthorizedRequest` now ORs `SecurityProtocolType.Tls12` onto `ServicePointManager.SecurityProtocol` instead of replacing it (no longer downgrades hosts that have TLS 1.3 enabled).

### Fixed
- `NtpConnection.GetUtc` socket leak when `Connect`/`Send` throws before the `try` block (now wrapped in `using`).
- `TimeSyncDiagnostic` `ManagementObjectSearcher` and `ManagementObject` items not disposed → WMI handle leak.
- `Console.WriteLine` calls removed from library code (was polluting WiX custom-action MSI logs and service hosts).
- `await nextHandler?.HandleContextAsync(context)` patterns in `LastLicenseHandler` and `UnregisterReceiptHandler` (would `await null` → `NullReferenceException`).
- `ArgumentNullException("computer info is empty")` parameter-name vs message bug across all `*ApiEndPoint` classes.
- ILRepack `Delete` task had a stray space in `$(OutDir) Microsoft.DotNet.PlatformAbstractions.dll` causing the file to leak through to the package.
- Version drift: `nugetSpec.nuspec`, `AssemblyInfo.cs`, and CHANGELOG are now reconciled via this 3.0.0 entry.
- Empty stub files `License/Handlers/ApiPatchLicenseHandler.cs` and `License/Handlers/LicenseHandler.cs` deleted.

### Test project
- Renamed remaining `Hymma.Lm.EndUser` namespace references to `LicenseManagement.EndUser` (they had been missed in the 2.0.0 rename, so the test project was uncompilable).

### Migration from 2.0.x
- If you were subclassing `LicenseValidationHandler` directly, your code will no longer compile. Use the public entry points (`LicenseHandlingInstall`, `LicenseHandlingLaunch`, `LicenseHandlingUninstall`) and supply behavior via `LicenseHandlingOptions` callbacks instead.
- If you were calling `*ApiEndPoint` classes directly, switch to the high-level handlers.
- If you were assigning to `Constants.DefaultLicFileRootDir`, remove the assignment — the path is now fixed.

## [2.0.1] - 2026-02-05

### Changed
- Clarified that trial end date is server-driven in documentation.

## [2.0.0] - 2024-12-18

### Changed
- **BREAKING**: Renamed package from `Hymma.Lm.EndUser` to `LicenseManagement.EndUser`
- **BREAKING**: Renamed namespace from `Hymma.Lm.EndUser` to `LicenseManagement.EndUser`
- **BREAKING**: Renamed assembly from `Hymma.Lm.EndUser.dll` to `LicenseManagement.EndUser.dll`

### Migration
- Update NuGet package reference from `Hymma.Lm.EndUser` to `LicenseManagement.EndUser`
- Update all `using Hymma.Lm.EndUser;` to `using LicenseManagement.EndUser;`
- Update any direct DLL references to the new assembly name

## [1.2.0] - 2024-12-14

### Changed
- Bumped version to align with assembly version 1.3.0.0

### Fixed
- Fixed Dictionary serialization issue with Metadata property - changed to List<MetadataEntry> for XML serialization compatibility
- License files now saved with .lic extension with backwards compatibility for reading old .xml files

## [1.0.0] - 2024-12-11

### Added

- Initial public release of LicenseManagement.EndUser SDK
- `LicenseHandlingInstall` - Installation-time license registration
- `LicenseHandlingLaunch` - Application launch license validation
- `LicenseHandlingUninstall` - Uninstall-time seat release
- `PublisherPreferences` - Configuration class for vendor settings
- `LicHandlingContext` - State management with event-driven callbacks
- License status evaluation (Valid, ValidTrial, InValidTrial, Expired, etc.)
- RSA digital signature verification for license files
- Hardware identification using DeviceId library
- Registry-based computer ID storage
- Local license file caching for offline support
- NTP time sync detection for anti-tampering
- Custom metadata support for licenses
- Comprehensive exception hierarchy
