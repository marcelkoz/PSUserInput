using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSUserInput.Commands
{
    [Cmdlet(VerbsCommunications.Read, "BinaryInput")]
    [OutputType(typeof(bool))]
    public class GetBinaryInput : Cmdlet
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
        public string Prompt { get; set; } = "[y/n]:";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            string input;
            string message = $"{Message}\n{Prompt}";
            Regex pattern = new Regex(
                "(n(o)?)|(y(es)?)",
                RegexOptions.Compiled |
                RegexOptions.IgnoreCase
            );

            do
            {
                Console.Write(message);
                input = Console.ReadLine();
            } while (!pattern.IsMatch(input ?? ""));

            pattern = new Regex("y(es)?", RegexOptions.IgnoreCase);
            WriteObject(pattern.IsMatch(input));
        }
    }
}
