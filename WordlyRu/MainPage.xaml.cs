using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WordlyRu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int NumberOfLetters = 5;

        private DataContext dataContext = new DataContext(NumberOfLetters);
        private List<Button> wordButtons = new List<Button>();

        public MainPage()
        {
            InitializeComponent();
            AddWordButtonsToList();
            DisplayWord(dataContext.GetNextMostProbableWord());
        }

        private void AddWordButtonsToList()
        {
            wordButtons.Add(FirstLetter);
            wordButtons.Add(SecondLetter);
            wordButtons.Add(ThirdLetter);
            wordButtons.Add(FourthLetter);
            wordButtons.Add(FifthLetter);
        }

        private string FindApropriateWord()
        {
            UpdateContext();
            dataContext.AcceptableWords = dataContext.GetProbabilityDictFromHash(dataContext.AllWords
                .Where(word => HaveKnownPositions(word)
                && HaveAllYellowLetters(word)
                && !HaveAnyGrayLetters(word)
                && !dataContext.UsedWords.Contains(word)).ToHashSet());
            return dataContext.AcceptableWords.FirstOrDefault().Key;


        }

        private bool HaveAnyGrayLetters(string word)
        {

                for (int i = 0; i < NumberOfLetters; i++)
                {
                if (dataContext.GrayChars.Contains(word[i]) && !dataContext.YellowChars.Contains(word[i]) && word[i]!=dataContext.Letters[i].ExactLetter)
                    return true;
                }
            return false;
        }

        private bool HaveAllYellowLetters(string word)
        {
            foreach (var letter in dataContext.YellowChars)
            {
                if (!word.Contains(letter))
                    return false;
            }
            return true;
        }

        private bool HaveKnownPositions(string word)
        {
            for (int i = 0; i < NumberOfLetters; i++)
            {
                Letter currentLetter = dataContext.Letters[i];
                char? knownLetter = currentLetter.ExactLetter;
                if (knownLetter == null && !currentLetter.ImpossibleLetters.Contains(word[i]))
                    continue;
                else if (word[i] != knownLetter || currentLetter.ImpossibleLetters.Contains(word[i]))
                    return false;
            }
            return true;
        }

        private void UpdateContext()
        {
            dataContext.UsedWords.Add(dataContext.CurrentWord);
            for (int i = 0; i < NumberOfLetters; i++)
            {
                var backgroundBrush = wordButtons[i].Background as SolidColorBrush;
                if (backgroundBrush.Color == Colors.Gray)
                    dataContext.GrayChars.Add(wordButtons[i].Content.ToString()[0]);
                else if (backgroundBrush.Color == Colors.Yellow)
                {
                    char letter = wordButtons[i].Content.ToString()[0];
                    dataContext.YellowChars.Add(letter);
                    dataContext.Letters[i].ImpossibleLetters.Add(letter);
                }
                else if (backgroundBrush.Color == Colors.Green)
                    dataContext.Letters[i].ExactLetter = wordButtons[i].Content.ToString()[0];
            }
        }

        

        private void SerachButton_Click(object sender, RoutedEventArgs e)
        {
           
            string foundWord = FindApropriateWord();
            if (foundWord == null)
                InfoText.Text = "Больше вариантов нет";
            else
                DisplayWord(foundWord);

        }

        private void DisplayWord(string word)
        {
            for (int i = 0; i < NumberOfLetters; i++)
            {
                wordButtons[i].Content = word[i];
            }
            dataContext.CurrentWord = word;
        }

        private void Letter_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = sender as Button;
            var backgroundBrush = senderButton.Background as SolidColorBrush;
            if (backgroundBrush.Color == Colors.Gray)
                backgroundBrush.Color = Colors.Yellow;
            else if (backgroundBrush.Color == Colors.Yellow)
                backgroundBrush.Color = Colors.Green;
            else if (backgroundBrush.Color == Colors.Green)
                backgroundBrush.Color = Colors.Gray;
        }

        private void AnotherWord_Click(object sender, RoutedEventArgs e)
        {
            DisplayWord(dataContext.GetNextMostProbableWord());
        }
        private void SwitchLangButton_Click(object sender, RoutedEventArgs e)
        {
            dataContext.IsEng = !dataContext.IsEng;
            (sender as Button).Content = dataContext.IsEng ? "RU" : "EN";
            ClearContext();
        }

        private void ClearContext()
        {
            dataContext.ClearData();
            DisplayWord(dataContext.GetNextMostProbableWord());
        }
    }

    class DataContext
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




        public DataContext(int numberOfLetters)
        {
            _numberOfLetters = numberOfLetters;
            Letters = InitLetters(numberOfLetters);
            _allRusWords = GetAllWordsFromFileDictionary(Path.Combine(Environment.CurrentDirectory, DICTIONARY_DIRECTORY_NAME, "russian.txt"));
            _allEngWords = GetAllWordsFromFileDictionary(Path.Combine(Environment.CurrentDirectory, DICTIONARY_DIRECTORY_NAME, "english.txt"));
            _rusletterFreq = CalcFrequency(_allRusWords);
            _engletterFreq = CalcFrequency(_allEngWords);
            AcceptableWords = GetProbabilityDictFromHash(AllWords);
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
            AcceptableWords = GetProbabilityDictFromHash(AllWords);
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

        public  Dictionary<string, double> GetProbabilityDictFromHash(HashSet<string> hashSet)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (var word in hashSet)
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

    class Letter
    {
        public char? ExactLetter = null;
        public HashSet<char> ImpossibleLetters = new HashSet<char>();
    }
}
