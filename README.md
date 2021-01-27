# PSUserInput
PSUserInput is a Powershell module which provides easy to use cmdlets for receiving more complex input from the user than what the .NET standard library provides.

## Building
Building the module is as simple as running `dotnet build` on .NET Core in the root directory. The resultant files should have been placed in the `Module/bin` directory.

## Installing The Module

### Script
The script `installmod.ps1` in the project root will install the Powershell module.

### Manually
1. Get the Powershell Module locations from the environmental variable `$env:PSModulePath.Split(';')`
2. Make a `PSUserInput` directory in one of those locations
3. Run the `copymod.ps1` script in the root of the project and give the script the absolute path to the newly created `PSUserInput` directory