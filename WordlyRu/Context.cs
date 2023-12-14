using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordlyRu
{
    class Context
    {
        string DICTIONARY_DIRECTORY_NAME = "Dictionaries";

        public HashSet<string> AllWords 
        {
            get
            {
                if (IsEng)
                    return _allEngWords;
                else
                    return _allRusWords;
            }
        }

        private Dictionary<char, double> _letterFreq
        {
            get
            {
                if (IsEng)
                    return _engletterFreq;
                else
                    return _rusletterFreq;
            }
        }

        private readonly HashSet<string> _allEngWords;
        private readonly HashSet<string> _allRusWords;
        private readonly int _numberOfLetters;
        private Dictionary<char, double> _rusletterFreq;
        private Dictionary<char, double> _engletterFreq;

        public HashSet<char> YellowChars = new HashSet<char>();
        public HashSet<char> GrayChars = new HashSet<char>();
        public List<Letter> Letters;
        public HashSet<string> UsedWords = new HashSet<string>();
        public Dictionary<string, double> AcceptableWords;
        public string CurrentWord;
        public bool IsEng = false;

        public string GetApropriateWord()
        {
            AcceptableWords = GetProbabilityDictForWords(AllWords
                .Where(word => HaveKnownPositions(word)
                && HaveAllYellowLetters(word)
                && !HaveAnyGrayLetters(word)
                && !UsedWords.Contains(word)).ToHashSet());
            return AcceptableWords.FirstOrDefault().Key;
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
            foreach (var letter in YellowChars)
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
                if (GrayChars.Contains(word[i]) && !YellowChars.Contains(word[i]) && word[i] != Letters[i].ExactLetter)
                    return true;
            }
            return false;
        }


        public Context(int numberOfLetters)
        {
            _numberOfLetters = numberOfLetters;
            Letters = InitLetters(numberOfLetters);
            _allRusWords = GetAllWordsFromFileDictionary(Path.Combine(Environment.CurrentDirectory, DICTIONARY_DIRECTORY_NAME, "russian.txt"));
            _allEngWords = GetAllWordsFromFileDictionary(Path.Combine(Environment.CurrentDirectory, DICTIONARY_DIRECTORY_NAME, "english.txt"));
            _rusletterFreq = CalcFrequency(_allRusWords);
            _engletterFreq = CalcFrequency(_allEngWords);
            AcceptableWords = GetProbabilityDictForWords(AllWords);
        }

        private Dictionary<char, double> CalcFrequency(HashSet<string> words)
        {
            int allCharsCount = words.Sum(word => word.Length);
            Dictionary<char, double> result = new Dictionary<char, double>();
            foreach (string word in words)
            {
                foreach (var ch in word)
                {
                    if(!result.ContainsKey(ch))
                        result.Add(ch, 1);
                    else
                        result[ch]++;
                }
            }

            foreach (var item in result.ToDictionary(i=>i.Key, i=>i.Value))
            {
                result[item.Key] = item.Value / allCharsCount;
            }
            return result;
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

        private HashSet<string> GetAllWordsFromFileDictionary(string currentDictionaryPath)
        {
            return File.ReadAllLines(currentDictionaryPath)
                .Where(word => word.Length == _numberOfLetters
                && word.All(ch => char.IsLetter(ch)))
                .Select(word => word.ToLower())
                .ToHashSet();
        }

        internal void ClearData()
        {
            YellowChars.Clear();
            GrayChars.Clear();
            Letters = InitLetters(_numberOfLetters);
            UsedWords.Clear();
            AcceptableWords = GetProbabilityDictForWords(AllWords);
            CurrentWord = null;
        }

        public string GetNextMostProbableWord()
        {
            if (CurrentWord != null && AcceptableWords.ContainsKey(CurrentWord))
            {
                var currentProbability = AcceptableWords[CurrentWord];
                var lessProbableWords = AcceptableWords.Where(kv => kv.Value < currentProbability);
                if(lessProbableWords.Count()!=0)
                  return  lessProbableWords.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
            }
            return AcceptableWords.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;
        }

        public  Dictionary<string, double> GetProbabilityDictForWords(IEnumerable<string> words)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (var word in words)
            {
                double wordProbability = 0;
                foreach (var ch in word)
                {
                    wordProbability += _letterFreq[ch];
                }
                result[word] = wordProbability;
            }

            return result;
        }
    }
}
