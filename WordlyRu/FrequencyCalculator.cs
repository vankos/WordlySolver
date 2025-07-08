using System.Collections.Generic;
using System.Linq;

namespace WordlyRu
{
    internal class FrequencyCalculator
    {
        private readonly Dictionary<char, double> _letterFrequency;
        public FrequencyCalculator(IEnumerable<string> wordsToCalculateLetterFrequencyFrom)
        {
            _letterFrequency = CalcLettersFrequency(wordsToCalculateLetterFrequencyFrom);
        }

        public string GetMostProbableWord(IEnumerable<string> words)
        {
            Dictionary<string, double> probabilityDictionaty =  GetProbabilityDictForWords(words);
            IOrderedEnumerable<KeyValuePair<string, double>> orderedProbabilityDictionary =  probabilityDictionaty.OrderByDescending(kv => kv.Value);
            string mostProbableWord = orderedProbabilityDictionary.FirstOrDefault().Key;
            return mostProbableWord;
        }

        private static Dictionary<char, double> CalcLettersFrequency(IEnumerable<string> words)
        {
            IEnumerable<string> enumerable = words.ToList();
            var allCharsCount = enumerable.Sum(word => word.Length);
            var charFrequency = new Dictionary<char, double>();
            foreach (var word in enumerable)
            {
                foreach (var ch in word.Where(ch => !charFrequency.TryAdd(ch, 1)))
                {
                    charFrequency[ch]++;
                }
            }

            foreach (var item in charFrequency
                         .ToDictionary(i => i.Key, i => i.Value))
            {
                charFrequency[item.Key] = item.Value / allCharsCount;
            }
            return charFrequency;
        }

        private Dictionary<string, double> GetProbabilityDictForWords(IEnumerable<string> words)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (var word in words)
            {
                double wordProbability = 0;
                foreach (var ch in word.Distinct())
                {
                    wordProbability += _letterFrequency[ch];
                }
                result[word] = wordProbability;
            }

            return result;
        }
    }
}
