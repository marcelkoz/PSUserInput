using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;

namespace PSUserInput.Commands
{
    using Containers;

    [Cmdlet(VerbsCommunications.Read, "MultipleChoiceInput")]
    [OutputType(typeof(MultipleChoiceAnswer), typeof(MultipleChoiceAnswer[]))]
    public class MultipleChoiceInput : Cmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0
        )]
        [Alias("Question")]
        public string Message { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1
        )]
        public string[] Answers { get; set; }

        [Parameter(
            Position = 2
        )]
        public SwitchParameter AcceptList { get; set; }

        [Parameter(
            Position = 3
        )]
        public string Prompt { get; set; } = ":";

        private MultipleChoiceAnswer GetInput(string message)
        {
            int number;
            bool success;
            do
            {
                Console.Write(message);
                var input = Console.ReadLine();
                success = Int32.TryParse(input, out number);
            } while (
                !(
                    success &&
                    number >= 1 &&
                    number <= Answers.Length
                )
            );

            return new MultipleChoiceAnswer
            {
                Position = number,
                Index = number - 1,
                Answer = Answers[number - 1]
            };
        }

        private List<int> ValidateListInput(string input, int max)
        {
            string[] inputs = input.Split(' ')
                .Where(str => !string.IsNullOrEmpty(str))
                .ToArray();

            var numbers = new List<int>();
            foreach (var str in inputs)
            {
                var success = Int32.TryParse(str, out int number);
                if (!success || number < 1 || number > max)
                    return null;
                numbers.Add(number);
            }

            return numbers;
        }

        private List<MultipleChoiceAnswer> GetInputs(string message)
        {
            List<int> numbers;
            do
            {
                Console.Write(message);
                var input = Console.ReadLine();
                numbers = ValidateListInput(input, Answers.Length);
            } while (numbers == null);

            var answers = new List<MultipleChoiceAnswer>();
            foreach (var number in numbers)
            {
                answers.Add(
                    new MultipleChoiceAnswer
                    {
                        Position = number,
                        Index = number - 1,
                        Answer = Answers[number - 1]
                    }
                );
            }

            return answers;
        }

        private string ConstructMessage()
        {
            var max = Answers.Length;
            var builder = new StringBuilder(Message, 250).Append('\n');
            for (int i = 0; i < max; i++)
            {
                builder.Append(
                    $"{i + 1}) {Answers[i]}\n"
                );
            }
            builder.Append(Prompt);

            return builder.ToString();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if (AcceptList)
                WriteObject(GetInputs(ConstructMessage()));
            else
                WriteObject(GetInput(ConstructMessage()));
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
