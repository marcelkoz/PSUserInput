using System;
using System.Collections.Generic;

namespace PSUserInput.Parsers.MultipleChoice
{
    using Choices = List<int>;
    
    public class DecisionEngine
    {
        private string m_list { get; init; }
        private string m_duplicates { get; init; }
        private String[] m_answers { get; set; }

        public DecisionEngine(String[] answers, string list, string duplicates)
        {
            m_answers = answers;
            m_list = list.ToLower();
            m_duplicates = duplicates.ToLower();
        }

        public (bool, Choices) ValidateChoices(Choices choices)
        {
#if DEBUG
            for (var i = 0; i < choices.Count; i++)
            {
                Console.WriteLine($"Choice {i + 1}: {choices[i]}");
            }

            Console.WriteLine($"List: {m_list}");
            Console.WriteLine($"Duplicates: {m_duplicates}");
#endif

            if (m_list == "deny")
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
            var sameChoices = (true, choices);
            var hasDuplicates = _hasChoiceDuplicates(choices);

            if (!_choicesAreWithinRange(choices))
                return invalidChoices;

            switch (m_duplicates)
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
            return choice > 0 && choice <= m_answers.Length;
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
            foreach (var choice in choices)
            {
                if (!table.ContainsKey(choice))
                {
                    numbers.Add(choice);
                    table.Add(choice, 1);
                }
            }

            return numbers;
        }
    }
}
