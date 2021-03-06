﻿using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSUserInput
{
    [Cmdlet(VerbsCommon.Get, "TextInput")]
    [OutputType(typeof(string))]
    public class GetTextInput : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0
        )]
        [Alias("Question")]
        public string Message { get; set; }

        [Parameter(
            Position = 1
        )]
        public string Prompt { get; set; } = ":";

        [Parameter(
            Position = 2
        )]
        public string Match { get; set; } = "*";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            string input;
            string message = $"{Message}\n{Prompt}";
            Regex pattern = new Regex(
                Match,
                RegexOptions.Compiled
            );

            do
            {
                Console.Write(message);
                input = Console.ReadLine();
            } while (!pattern.IsMatch(input ?? ""));

            WriteObject(input);
        }
    }
}
