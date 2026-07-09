# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.2.0] - 2026-07-09

### Added
- **Compact license formats for embedded consumers.** New
  `CompactLicense.FetchAsync(apiKey, computerId, productId, format, validDays, ct)`
  and `LicenseApiEndPoint.GetCompactLicense[Async]` fetch a compact signed token in
  `jws`, `es256`, or `eddsa` format (via the API `?format=` parameter), for hosts that
  verify a signed token offline (e.g. the CADshift nesting engine). The XML license
  flow is unchanged and remains the default. Shipped on both `net481` and
  `net8.0-windows7.0`.

### Fixed
- **Null-safe `ReceiptExpiredHandler`** for receipt-less license files — it no longer
  throws when a license file has no attached receipt.

## [3.1.1] - 2026-06-17

### Fixed
- **net8 `WebApiClient` DNS staleness.** The net8.0-windows client now uses a
  `SocketsHttpHandler` with `PooledConnectionLifetime = 2 min`, so a long-lived,
  process-wide client re-resolves DNS periodically (matching the net481 factory's
  connection-lease recycle). Previously it pinned connections to the first resolved
  IP for the life of the process — a server IP/failover change broke license checks
  until the host restarted. net481 path unchanged.
- **HTTPS guard now checks the address actually used.** The public-key fetch guard
  validated the compile-time `Constants.BaseAddress` (always https) rather than the
  effective `LicenseHandlingOptions.ServerBaseAddress` used by the HTTP layer. It now
  guards the effective address and permits http only for a loopback host, so the
  local test/dev server still works while a misconfigured non-local http endpoint is
  refused (the public key is the signature trust root).

## [3.1.0] - 2026-06-16

### Added
- **net8.0-windows target.** The package now ships `lib/net8.0-windows7.0` alongside the
  existing `lib/net481`, so .NET 8 desktop hosts (e.g. the CadShift for Inventor add-in)
  can consume the SDK directly. Built by a sibling project
  (`LicenseManagement.EndUser.Net8`) that links the exact same sources — there is **no
  change to the net481 assembly** the existing SolidWorks add-in depends on.

### Internal
- `WebApiClient` selects a plain pooled `HttpClient` on net8 (the built-in
  `SocketsHttpHandler` already pools per host) via `#if NET8_0_OR_GREATER`; the net481
  path still uses `PerHostHttpClientFactory` unchanged. No behavioural difference.
- On net8, `System.Management` and `System.Security.Cryptography.Xml` (BCL on net481) are
  referenced as packages; `DeviceId*` are package references instead of ILRepack-merged.
  The net481 build remains ILRepack-merged exactly as before.

## [3.0.2] - 2026-05-15

### Fixed
- `LicenseHandlingUninstall`: reverted the 3.0.1 disk-read approach and restored the original
  server chain (`ApiGetComputerHandler` → `ApiGetProductHandler` → `ApiPostLicenseHandler` →
  `ApiGetLicenseHandler`). The disk-read approach broke two invariants: (1) if the license file
  was deleted the computer could never be unregistered; (2) a license file shared between machines
  could unregister the wrong seat because computer identity was read from the file instead of from
  the live hardware `ComputerId.Instance.MachineId`. The server chain always resolves identity from
  hardware, handles a missing file gracefully (POST returns 409 = license exists → GET it), and
  writes the updated unregistered license back to disk via `LastLicenseHandler`.

## [3.0.1] - 2026-05-15

### Fixed
- `LicenseHandlingUninstall` now reads the license from disk instead of going through the install
  chain; the old path required the real machine's DeviceId to be registered on the server, which
  broke uninstall when the machine identity differed from the seed data.
- `LicenseSignatureValidationHandler`: the "stale license" recovery branch now fetches the current
  server public key first. If the fresh key validates the file the handler proceeds (genuine key
  rotation); if the fresh key still fails the handler throws `CryptographicException` (file was
  tampered). Previously the branch always re-fetched the full license from the server, silently
  healing tampered files rather than surfacing the error.

### Internal
- `ComputerId.MachineId` and `MachineName` setters changed from `private` to `internal` to allow
  test fixtures to spoof machine identity without DeviceId hardware reads.
- CI `build.yml`: test step disabled (`if: false`) — all 62 tests are integration tests that
  require the audit-fix backend. See workflow comments for the plan to re-enable in CI.

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
