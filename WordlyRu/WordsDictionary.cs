using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordlyRu
{
    internal class WordsDictionary
    {
        private const string DICTIONARY_DIRECTORY_NAME = "Dictionaries";
        private const string RUSSIAN_DICTIONARY_FILE_NAME = "russian.txt";
        private const string ENGLISH_DICTIONARY_FILE_NAME = "english.txt";
        private readonly HashSet<string> _allWords;

        public WordsDictionary(bool isEnglish)
        {
            _allWords = ExtractAllWords(isEnglish);
        }

        public HashSet<string> GetAllWords()
        {
            return _allWords.ToHashSet();
        }

        public HashSet<string> GetWordsWithLenght(int wordLenght)
        {
            return _allWords.Where(word => word.Length == wordLenght).ToHashSet();
        }

        private HashSet<string>  ExtractAllWords(bool isEnglish)
        {
            string dictionaryFileName = RUSSIAN_DICTIONARY_FILE_NAME;
            if(isEnglish)
            {
                dictionaryFileName = ENGLISH_DICTIONARY_FILE_NAME;
            }

            string dictionaryFilePath = Path.Combine(Environment.CurrentDirectory, DICTIONARY_DIRECTORY_NAME, dictionaryFileName);
            return GetAllWordsFromFileDictionary(dictionaryFilePath);
        }

        private HashSet<string> GetAllWordsFromFileDictionary(string dictionaryPath)
        {
            return File.ReadAllLines(dictionaryPath)
                .Where(word => word.All(ch => char.IsLetter(ch)))
                .Select(word => word.ToLower())
                .ToHashSet();
        }
    }
}
