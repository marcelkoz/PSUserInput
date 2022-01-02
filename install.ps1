#
# Install script for PSUserInput module.
#

# if set, the script copies module files to that location otherwise goes into interactive install
param([string] $InstallDir)

# module info globals
$ModuleName = 'PSUserInput'
$ModuleRoot = './Module'
$ModuleDecl = "$ModuleRoot/$ModuleName.psd1"
$ModuleDLL  = "$ModuleRoot/bin/$ModuleName.dll"

# copies module files to specified location
function InstallModule($Location)
{
    if (!(Test-Path $Location))
    {
        New-Item -ItemType Directory $Location
    }
    Write-Output "Copying files to module location ($location)..."
    Copy-Item -Path $ModuleDecl, $ModuleDLL -Destination $Location
}

$ErrorActionPreference = 'Stop'
try
{
    dotnet build --configuration Release
    Write-Output 'Installing module...'

    $Location = $InstallDir
    # interactive install
    if ($InstallDir -eq "")
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
    Write-Error "An error occured while installing module:`n$_"
    exit
}

Write-Output 'Installation of module was successful.'
