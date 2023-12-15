using System.Collections.Generic;
using System.Linq;

namespace WordlyRu
{
    class Context
    {
        private readonly int _numberOfLetters;
        private readonly WordsDictionary _wordsDictionary;
        private readonly FrequencyCalculator _frequencyCalculator;
        private readonly HashSet<string> _allWordsOfAppropriateLenght;

        public HashSet<char> YellowLetters = new HashSet<char>();
        public HashSet<char> GrayLetters = new HashSet<char>();
        public List<Letter> Letters;
        public HashSet<string> UsedWords = new HashSet<string>();
        public string CurrentWord;
        public bool IsEng { get; }

        public Context(int numberOfLetters, bool IsEnglish)
        {
            _numberOfLetters = numberOfLetters;
            IsEng = IsEnglish;
            _wordsDictionary = new WordsDictionary(IsEng);
            _allWordsOfAppropriateLenght = _wordsDictionary.GetWordsWithLenght(numberOfLetters);
            HashSet<string> allWords = _wordsDictionary.GetAllWords();
            _frequencyCalculator = new FrequencyCalculator(allWords);
            Letters = InitLetters(numberOfLetters);
        }

        private List<Letter> InitLetters(int numberOfLetters)
        {
            var letters = new List<Letter>();
            for (int i = 0; i < numberOfLetters; i++)
            {
                letters.Add(new Letter());
            }
            return letters;
        }

        public string GetApropriateWord()
        {
            IEnumerable<string> allApropriateWord = _allWordsOfAppropriateLenght
                .Where(word => HaveKnownPositions(word)
                && HaveAllYellowLetters(word)
                && !HaveAnyGrayLetters(word)
                && !UsedWords.Contains(word));
            string mostProbableApropriateWord = _frequencyCalculator.GetMostProbableWord(allApropriateWord);
            return mostProbableApropriateWord;
        }

        private bool HaveKnownPositions(string word)
        {
            for (int i = 0; i < _numberOfLetters; i++)
            {
                Letter currentLetter = Letters[i];
                char? knownLetter = currentLetter.ExactLetter;
                if (knownLetter == null && !currentLetter.ImpossibleLetters.Contains(word[i]))
                    continue;
                else if (word[i] != knownLetter || currentLetter.ImpossibleLetters.Contains(word[i]))
                    return false;
            }
            return true;
        }

        private bool HaveAllYellowLetters(string word)
        {
            foreach (var letter in YellowLetters)
            {
                if (!word.Contains(letter))
                    return false;
            }
            return true;
        }

        private bool HaveAnyGrayLetters(string word)
        {
            for (int i = 0; i < _numberOfLetters; i++)
            {
                if (GrayLetters.Contains(word[i]) && !YellowLetters.Contains(word[i]) && word[i] != Letters[i].ExactLetter)
                    return true;
            }
            return false;
        }
    }
}
