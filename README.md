# OnAirSync

[![.github/workflows/build.yml](https://github.com/LinqLover/OnAirSync/workflows/.github/workflows/build.yml/badge.svg)](https://github.com/LinqLover/OnAirSync/actions) [![latest release](https://img.shields.io/github/v/release/LinqLover/OnAirSync.svg)](https://github.com/LinqLover/OnAirSync/releases)

Automatically monitor whether you are in a (video) call and push updates to openHAB.
Works for Win32 apps only (no Linux, no UWP apps).

## Installation

- Download the latest release from the [release section](https://github.com/LinqLover/OnAirSync/releases)
- Extract it
- Execute it using `OnAirSync -i <your_openHAB_item_name>`

For better convenience, you may want to create an automated task in the Windows scheduled task settings (trigger: on log-on, action: start `cmd.exe /c start "" C:\path\to\OnAirSync.exe -i <your_openHAB_item_name>`).

## Development

See [`build.yml`](.github/workflows/build.yml) on how to build the project using `dotnet`.
VS Code can also generate some convenient `{launch,task}.json` files for you.

Contributions or questions of all kinds are highly welcome!
