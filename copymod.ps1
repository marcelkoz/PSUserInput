# Script which copies the module declaration and dll into another directory
param([string] $PackageDir)

$ModuleDecl = 'Module/PSUserInput.psd1'
$ModuleDLL  = 'Module/bin/netstandard2.0/PSUserInput.dll'

Copy-Item -Path $ModuleDecl, $ModuleDLL -Destination $PackageDir
