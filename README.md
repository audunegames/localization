# Audune Localization

[![openupm](https://img.shields.io/npm/v/com.audune.localization?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.audune.localization/)


## Features

* A localization system component that is configured solely with components on a GameObject. Add locale loaders and selectors to the system to control the ways locales are loaded and selectors. Acces the functonality of the system through scripting.
* Create locales with scriptable objects, or import them from TOML files. Custom parsers for importing locales can also be defined.
* Format strings in a locale using the [ICU message format](https://lokalise.com/blog/complete-guide-to-icu-message-format/). Messages have support for `number`, `date`, `plural`, `selectordinal`, and `select` formats. Plural rules for all locales defined by the [CLDR](https://www.unicode.org/cldr/charts/44/supplemental/language_plural_rules.html#cs) are included in the package and automatically applied.
* Reference strings from the table of locales in your code or in the inspector to localize them on demand. Add arguments to a reference or format the result via scripting.
* Define custom locale loaders to define sources where the system loads locales from. A locale loader that loads locales from assets, as well as a loader that loads additional locale sat runtime from the StreamingAssets folder, are included.
* Define custom locale selectors to control which locale is selected when the game starts. Selectors that are include can select a locale based on a command line argument, the system language, and a specific asset that acts as the defualt locale.

## Installation

### Requirements

This package depends on the following packages:

* [Serializable Dictionary](https://openupm.com/packages/com.audune.utils.dictionary/), version **1.0.3** or higher.
* [UnityEditor Utilities](https://openupm.com/packages/com.audune.utils.unityeditor/), version **2.0.5** or higher.

If you're installing the required packages from the [OpenUPM registry](https://openupm.com/), make sure to add a scoped registry with the URL `https://package.openupm.com` and the required scopes before installing the packages.

### Installing from the OpenUPM registry

To install this package as a package from the OpenUPM registry in the Unity Editor, use the following steps:

* In the Unity editor, navigate to **Edit › Project Settings... › Package Manager**.
* Add the following Scoped Registry, or edit the existing OpenUPM entry to include the new Scope:

```
Name:     package.openupm.com
URL:      https://package.openupm.com
Scope(s): com.audune.localization
```

* Navigate to **Window › Package Manager**.
* Click the **+** icon and click **Add package by name...**
* Enter the following name in the corresponding field and click **Add**:

```
com.audune.localization
```

### Installing as a Git package

To install this package as a Git package in the Unity Editor, use the following steps:

* In the Unity editor, navigate to **Window › Package Manager**.
* Click the **+** icon and click **Add package from git URL...**
* Enter the following URL in the URL field and click **Add**:

```
https://github.com/audunegames/localization.git
```

## Usage

### Example locale file in the TOML format

```toml
# The code of the locale, specified as an IETF language tag
code = "nl"

# The names of the locale
english_name = "Dutch"
native_name = "Nederlands"

# Alternative codes that are accessible via scripting
alt_codes = {steam = "dutch"}

# Formats for formatting messages, specified as C# number formats
number_format = {decimal = '0.##', percent = '0.##%', currency = 'c'}
date_format = {short = 'd', long = 'D'}
time_format = {short = 't', long = 'T'}

# Example of a string table
[strings.ui.main_menu]
play = "Game spelen"
options = "Opties"
quit = "Game afsluiten"
```

## License

This package is licensed under the GNU LGPL 3.0 license. See `LICENSE.txt` for more information.
