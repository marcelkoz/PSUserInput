#
# Install script for PSUserInput module.
#

param(
    # if set, the script copies module files to that location otherwise goes into interactive install
    [string] $InstallDir,
    # package the module instead in a zip file
    [switch] $Package
)

# module info globals
$ModuleName     = 'PSUserInput'
$ModuleRoot     = './Module'
$ModuleDecl     = "$ModuleRoot/$ModuleName.psd1"
$ModuleDLL      = "$ModuleRoot/bin/$ModuleName.dll"
$ModuleLicense  = './LICENSE.txt'
$ModulePackages = "$ModuleRoot/Packages" 

# copies module files to specified location
function InstallModule($Location)
{
    if (!(Test-Path $Location))
    {
        New-Item -ItemType Directory $Location
    }

    $ModuleFiles = @($ModuleDecl, $ModuleDLL, $ModuleLicense)
    if ($Package)
    {
        Write-Output "Packaging module to location ($location)..."
        $Version = (Get-Module -ListAvailable $ModuleDecl).Version
        Compress-Archive -Path $ModuleFiles -DestinationPath "$Location/$ModuleName-$Version.zip"
    }
    else
    {
        Write-Output "Copying files to module location ($location)..."
        Copy-Item -Path $ModuleFiles -Destination $Location
    }
}

$ErrorActionPreference = 'Stop'
try
{
    dotnet build --configuration Release
    Write-Output 'Installing module...'

    $Location = $InstallDir
    # package module
    if ($Package)
    {
        $Location = $ModulePackages
    }
    # interactive install
    elseif ($InstallDir -eq "")
    {
        Import-Module -Name $ModuleDLL

        $PotentialLocations = $env:PSModulePath.Split(';')
        $Location = Read-MultipleChoiceInput -Message 'Select ONE of the following module locations:' -Answers $PotentialLocations
        $Location = (Join-Path $Location.Answer $ModuleName)

        Remove-Module -Name $ModuleName
    }
    # manual install
    InstallModule($Location)  
}
catch
{
    $_
    exit
}

Write-Output 'Installation or packaging of module was successful.'
