# PSUserInput
PSUserInput is a Powershell module which provides easy to use cmdlets for receiving more complex input from the user than what the .NET standard library provides.

## Building
Building the module is as simple as running `dotnet build` on .NET Core in the root directory. The resultant files should have been placed in the `Module/bin` directory.

## Installing The Module

### Script
Running the powershell script `install.ps1` without arguements will enter an interactive install. Select a location for the module to be installed to.

Note: You may require admin privileges/root to install in system directories.

### Manually
1. Create a `PSUserInput` directory in the desired location
2. Run `install.ps1 {PATH TO LOCATION}`