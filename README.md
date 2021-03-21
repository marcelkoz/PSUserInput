# PSUserInput
PSUserInput is a Powershell module, providing cmdlets for reciving more complex text input from the user than what the .NET standard library has.

## Building
Running `dotnet build` in the root directory will build the project. The output files will be placed in the `Module/bin` directory.

## Installing The Module

### Script
Running the powershell script `install.ps1` without arguments will enter an interactive install. Select a location for the module to be installed to.

Note: You may require admin privileges/root to install in system directories.

### Manually
1. Create a `PSUserInput` directory in the desired location
2. Run `install.ps1 {PATH TO LOCATION}`