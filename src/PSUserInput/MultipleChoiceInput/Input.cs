using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSUserInput.Commands
{
    using Containers;
    using Parsers.MultipleChoice;

    using Choices = List<int>;

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

            var message      = _constructMessage(List == "Accept");
            var parser       = new Parser();
            var finalChoices = new List<int>();
            while (true)
            {
                Console.Write(message);
                var input = Console.ReadLine();

                var (success, choices) = parser.Parse(input);
                if (!success) continue;

                (success, choices) = _validateChoices(choices);
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

        private (bool, Choices) _validateChoices(Choices choices)
        {
            #if DEBUG
                for (var i = 0; i < choices.Count; i++)
                {
                    Console.WriteLine($"Choice {i + 1}: {choices[i]}");
                }

                Console.WriteLine($"List: {List}");
                Console.WriteLine($"Duplicates: {Duplicates}");
            #endif

            if (List == "Deny")
            {
                if (choices.Count == 1 && _isWithinChoiceRange(choices[0]))
                    return (true, choices);
                else
                    return (false, new Choices());
            }

            return _validateManyChoices(choices);
        }

        private (bool, Choices) _validateManyChoices(Choices choices)
        {
            var invalidChoices = (false, new Choices());
            var sameChoices    = (true, choices);
            var hasDuplicates  = _hasChoiceDuplicates(choices);

            if (!_choicesAreWithinRange(choices))
                return invalidChoices;

            switch (Duplicates.ToLower())
            {
                case "accept":
                    return sameChoices;

                case "remove":
                    return hasDuplicates
                        ? (true, _getUniqueChoices(choices))
                        : sameChoices;
        
                case "deny":
                    return hasDuplicates
                        ? invalidChoices
                        : sameChoices;
            }

            // unreachable
            return invalidChoices;
        }

        private bool _isWithinChoiceRange(int choice)
        {
            return choice > 0 && choice <= Answers.Length;
        }

        private bool _choicesAreWithinRange(Choices choices)
        {
            foreach (var choice in choices)
            {
                if (!_isWithinChoiceRange(choice)) return false;
            }

            return true;
        }

        private bool _hasChoiceDuplicates(Choices choices)
        {
            // clone list to preserve insertion order
            var numbers = new List<int>(choices);
            numbers.Sort();

            var previous = -1;
            foreach (var choice in numbers)
            {
                if (choice == previous)
                    return true;

                previous = choice;
            }

            return false;
        }
    
        private Choices _getUniqueChoices(Choices choices)
        {
            var table = new Dictionary<int, int>();
            var numbers = new List<int>();
            foreach(var choice in choices)
            {
                if (!table.ContainsKey(choice))
                {
                    numbers.Add(choice);
                    table.Add(choice, 1);
                }
            }

            return numbers;
        }
    
        private List<MultipleChoiceAnswer> _getAnswers(Choices choices)
        {
            var answers = new List<MultipleChoiceAnswer>();
            foreach (var choice in choices)
            {
                var index = choice - 1;
                answers.Add(
                    new MultipleChoiceAnswer
                    {
                        Position = index + 1,
                        Index    = index,
                        Answer   = Answers[index]
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
