@{
    # Basic module information
    Author        = 'Marcel Kozlowski'
    Copyright     = '(c) 2021-2022 Marcel Kozlowski. All rights reserved.'
    Description   = 'Provides cmdlets for interactive user input.'
    ModuleVersion = '2.1.0'
    GUID          = 'cc187a61-2aec-4a8f-8972-d15da97f8836'

    RootModule         = 'PSUserInput.dll'
    CmdletsToExport    = @('Read-BinaryInput', 'Read-MultipleChoiceInput', 'Read-TextInput')
    PowerShellVersion  = '5.0'
}