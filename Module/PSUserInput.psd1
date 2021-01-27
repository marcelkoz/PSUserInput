@{
    # Basic Module Info
    Author        = 'turtleshell123'
    Copyright     = '(c) 2020 turtleshell123. All rights reserved.'
    Description   = 'Provides cmdlets for interactive user input.'
    ModuleVersion = '1.1'
    GUID          = 'cc187a61-2aec-4a8f-8972-d15da97f8836'

    RootModule         = 'PSUserInput.dll'
    CmdletsToExport    = @('Get-BinaryInput', 'Get-MultipleChoiceInput', 'Get-TextInput')
    PowerShellVersion  = '5.0'
}