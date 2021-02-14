# OnAirSync

[![.github/workflows/build.yml](https://github.com/LinqLover/OnAirSync/workflows/.github/workflows/build.yml/badge.svg)](https://github.com/LinqLover/OnAirSync/actions) [![latest release](https://img.shields.io/github/v/release/LinqLover/OnAirSync.svg)](https://github.com/LinqLover/OnAirSync/releases)

Automatically monitor whether you are in a (video) call and push updates to [openHAB](http://www.openhab.org/).
Works for Win32 apps only (no Linux, no UWP apps).
For more information, also read the [official announcement thread in the openHAB Community forum](https://community.openhab.org/t/ann-onairsync-let-openhab-know-when-you-are-in-a-meeting/116991?u=linqlover).

## Installation

- Download the latest release from the [release section](https://github.com/LinqLover/OnAirSync/releases)
- Extract it
- Execute it using `OnAirSync -i <your_openHAB_item_name>`

For better convenience, you may want to create an automated task in the Windows scheduled task settings (trigger: on log-on, action: start `cmd.exe /c "start /b C:\path\to\OnAirSync.exe -i <your_openHAB_item_name>"`, run whether user is logged on or not).

## Development

See [`build.yml`](.github/workflows/build.yml) on how to build the project using `dotnet`.
VS Code can also generate some convenient `{launch,task}.json` files for you.

Contributions or questions of all kinds are highly welcome!
