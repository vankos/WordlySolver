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

        private Dictionary<char, double> CalcLettersFrequency(IEnumerable<string> words)
        {
            int allCharsCount = words.Sum(word => word.Length);
            Dictionary<char, double> charFrequency = new Dictionary<char, double>();
            foreach (string word in words)
            {
                foreach (var ch in word)
                {
                    if (!charFrequency.ContainsKey(ch))
                        charFrequency.Add(ch, 1);
                    else
                        charFrequency[ch]++;
                }
            }

            foreach (var item in charFrequency.ToDictionary(i => i.Key, i => i.Value))
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
                foreach (var ch in word)
                {
                    wordProbability += _letterFrequency[ch];
                }
                result[word] = wordProbability;
            }

            return result;
        }
    }
}
