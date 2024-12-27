# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.1] - 2024-12-27

### Changed

- Updated source code license from GPL 3.0 to LGPL 3.0, so that larger works can use the library without disclosing their source code.

## [3.0.0] - 2024-09-04

### Added

- Interface that defines the functionality of a locale.

### Fixed

- Composed localized strings have their own instances of arguments now, so joined strings don't use the same arguments for all items.

## [2.0.2] - 2024-09-02

**Note: this release is not suitable to use in a production environment. Please use a higher version instead.**

### Fixed

- Updated reference format function to use the localized string interface in the localization system.

## [2.0.1] - 2024-09-02

**Note: this release is not suitable to use in a production environment. Please use a higher version instead.**

### Added

- Default methods are also defined in implementations of interfaces now.
- Support for string separators when joining localized strings.
- Methods to create empty, function and asset localized strings.

### Fixed

- Handling of empty separators when joining localized strings.

### Removed

- Multiplication operator for localized strings to join them with a separator.

## [2.0.0] - 2024-09-02

**Note: this release is not suitable to use in a production environment. Please use a higher version instead.**

### Added

- Interface that defines the functionality of the localization system.
- Interface that defines the functionality of a localized string.

### Changed

- Refactoring of the localization system script.
- Better support for concatenating and joining localized strings using specialized classes.

## [1.2.2] - 2024-08-14

### Added

- Support for formatting messages with a specific locale in the localization system.

## [1.2.1] - 2024-05-28

### Fixed

- Fixes and improvements for the editor windows.

## [1.2.0] - 2024-04-19

### Added

- Support for different `StringComparison` types in localized tables.

### Fixed

- Fixes and improvements for the editor windows.

### Removed

- Removed property search scripts in favor of hosting them in a dedicated UPM package.

## [1.1.0] - 2024-04-06

### Added

- Events for missing references and text assets in the localization system.
- Support for an interface that defines delegated arguments for localized strings.
- Display missing references in the inspector.
- Editor windows to search the project for localized strings.

### Removed

- Locales don't inherit from `ILocalizedReference<string>` anymore in favor of a dedicated property.

## [1.0.0] - 2024-03-20

### Added

- Support for concatenating and joining localized strings.

### Removed

- List of localized references in favor of joining support.

## [0.3.0] - 2024-03-11

**Note: this release is not suitable to use in a production environment. Please use a higher version instead.**

### Added

- Support for a message function to load a text asset located in a Resources folder.

## [0.2.0] - 2024-03-10

**Note: this release is not suitable to use in a production environment. Please use a higher version instead.**

### Added

- Support for compound messages in message functions.

## [0.1.0] - 2024-02-29

**Note: this release is not suitable to use in a production environment. Please use a higher version instead.**

### Added

- Localization system that handles loading locales and selecting a default locale based on loaders and selectors respectively.
- Locale assets that can be created using scriptable objects, or parsed from TOML files.
- Support for formatting strings in the ICU message format.
- References to localized strings for use in scripts.