using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace PSUserInput.Commands
{
    using Containers;
    using Parsers.MultipleChoice;

    [Cmdlet(VerbsCommunications.Read, "MultipleChoiceInput")]
    [OutputType(typeof(MultipleChoiceAnswer), typeof(MultipleChoiceAnswer[]))]
    public class MultipleChoiceInput : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0
        )]
        public string Message { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1
        )]
        public string[] Answers { get; set; }

        [Parameter(
            Position = 2
        )]
        public string Prompt { get; set; } = ":";

        [ValidateSet("Accept", "Deny")]
        [Parameter()]
        public String List { get; set; } = "Deny";

        [ValidateSet("Accept", "Deny", "Remove")]
        [Parameter()]
        public String Duplicates { get; set; } = "Deny";

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var message = _constructMessage(List == "Accept");
            var parser = new Parser(Answers, List, Duplicates);
            var finalChoices = new List<int>();
            while (true)
            {
                Console.Write(message);
                var input = Console.ReadLine();

                var (success, choices) = parser.Parse(input);
                if (success)
                {
                    finalChoices = choices;
                    break;
                }
            }

            var answers = _getAnswers(finalChoices);
            if (List == "Accept")
                WriteObject(answers);
            else
                WriteObject(answers[0]);
        }

        private string _constructMessage(bool multipleAnswers)
        {
            var max = Answers.Length;
            var builder = new StringBuilder(Message, 250).Append('\n');
            for (int i = 0; i < max; i++)
            {
                builder.Append(
                    $"{i + 1}) {Answers[i]}\n"
                );
            }
            if (multipleAnswers) builder.Append("(Select ONE or MANY)");
            else builder.Append("(Select ONE)");
            builder.Append(Prompt);

            return builder.ToString();
        }

        private List<MultipleChoiceAnswer> _getAnswers(List<int> choices)
        {
            var answers = new List<MultipleChoiceAnswer>();
            foreach (var choice in choices)
            {
                var index = choice - 1;
                answers.Add(
                    new MultipleChoiceAnswer
                    {
                        Position = index + 1,
                        Index = index,
                        Answer = Answers[index]
                    }
                );
            }

            return answers;
        }
    }
}

namespace PSUserInput.Containers
{
    public class MultipleChoiceAnswer
    {
        public int Index { get; set; }
        public int Position { get; set; }
        public string Answer { get; set; }
    }
}
