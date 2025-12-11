# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
