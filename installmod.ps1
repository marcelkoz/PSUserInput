Write-Output 'Installing module...'

$ErrorActionPreference = 'Stop'
try {
    dotnet build
    $ModuleDir = (Join-Path $env:PSModulePath.Split(';')[0] 'PSUserInput')
    if (!(Test-Path $ModuleDir)) {
        New-Item -ItemType Directory $ModuleDir
    }
    & './copymod.ps1' $ModuleDir
} catch {
    Write-Error "An error occured while installing module:`n$_"
    exit
}

Write-Output 'Installation of module was successful.'
