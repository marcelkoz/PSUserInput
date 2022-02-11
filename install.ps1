<#
    .SYNOPSIS
    Install script for PSUserInput module.

    .DESCRIPTION
    Installs or packages the PSUserInput module. Installing means copying
    the module files into one of the $env:PSModulePath entries. Packaging
    means placing the module files into a zip archive located in
    {PROJECT-ROOT}/Module/Packages. If unsatisfied by the default
    locations, the path can also be given for both.

    .PARAMETER Path
    Specifies the path to which the module should be installed or packaged
    in. Defaults are used when not set.

    .PARAMETER Package
    Specifies whether the script should package the module. By default it
    is false.

    .EXAMPLE
    Interactive install
    PS> ./install.ps1

    .EXAMPLE
    Manual install
    PS> ./install.ps1 -Path 'C:\Program Files\Powershell'

    .EXAMPLE
    Package module
    PS> ./install.ps1 -Package

#>

param(
    [string] $Path,
    [switch] $Package
)

$Module = @{
    # info
    Name     = 'PSUserInput'
    Root     = './Module'
    Packages = "./Module/Packages"

    # files
    Manifest = "./Module/PSUserInput.psd1"
    Binary   = "./Module/bin/PSUserInput.dll"
    License  = './LICENSE.txt'
}

function ShowMessage($message)
{
    Write-Output "[INFO] $message"
}

function PackageModule($files, $pathGiven)
{
    ShowMessage 'Preparing to package module...'

    $location = $Path
    if (!$pathGiven)
    {
        ShowMessage "No package path provided, using default ($($Module['Packages']))"
        $location = $Module['Packages']
    }

    $version = (Get-Module -ListAvailable $Module['Manifest']).Version
    $packagePath = (Join-Path $location "$($Module['Name'])-$version.zip")
    Compress-Archive -Force -Path $files -DestinationPath $packagePath

    ShowMessage "Package created ($packagePath)"
}

function InstallModule($files, $pathGiven)
{
    ShowMessage 'Preparing to install module...'

    $location = $Path
    if (!$pathGiven)
    {
        Import-Module $Module.Binary
        ShowMessage 'No install path provided, interactive install selected'
        $location = (Read-MultipleChoiceInput -Message 'Select path to install module to:' -Answers $env:PSModulePath.Split(';')).Answer
        Remove-Module $Module.Name
    }

    ShowMessage "Install path provided ($Location)"

    $modulePath = (Join-Path $location $Module['Name'])
    if (!(Test-Path $modulePath))
    {
        New-Item -ItemType Directory $modulePath
    }
    ShowMessage "Created module directory ($modulePath)"

    Copy-Item $files $modulePath
}

function main()
{
    ShowMessage 'Building module in Release mode...'
    dotnet build --configuration Release

    $pathGiven = ($Path.Trim() -ne '')
    $files = @($Module['Manifest'], $Module['Binary'], $Module['License'])

    if ($Package)
    {
        PackageModule $files $pathGiven
        ShowMessage 'Module packaging has been successful'
    }
    else
    {
        InstallModule $files $pathGiven
        ShowMessage 'Module installation has been successful'
    }
}

$ErrorActionPreference = 'Stop'
try
{
    main
}
catch
{
    $_
    exit
}
