using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WordlyRu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const int NUMBER_OF_LETTERS = 5;
        private Context currentContext = new Context(NUMBER_OF_LETTERS, false);
        private List<Button> WordButtons => LettersPanel.Children.Cast<Button>().ToList();

        public MainPage()
        {
            InitializeComponent();
            CreateLettersButtons();
            string mostProbableWord = currentContext.GetApropriateWord();
            DisplayWord(mostProbableWord);
        }

        private void CreateLettersButtons()
        {
            for (int i = 0; i < NUMBER_OF_LETTERS; i++)
            {
                AddLetterToUi(i);
            }
        }

        private void AddLetterToUi(int i)
        {
            Button letterButton = CreateLetterButton(i);
            AddButtonToUi(letterButton);
        }

        private Button CreateLetterButton(int i)
        {
            LetterButton letterButton = new LetterButton(i);
            letterButton.Click += Letter_Click;
            return letterButton;
        }

        private void AddButtonToUi(Button letterButton)
        {
            LettersPanel.Children.Add(letterButton);
        }

        #region Buttons callbacks

        private void SerachButton_Click(object _, RoutedEventArgs __)
        {
            UpdateContext();
            string foundWord = currentContext.GetApropriateWord();
            if (foundWord == null)
                InfoText.Text = "Больше вариантов нет";
            else
                DisplayWord(foundWord);
        }

        #endregion

        private void DisplayWord(string word)
        {
            for (int i = 0; i < NUMBER_OF_LETTERS; i++)
            {
                WordButtons[i].Content = word[i];
            }

            currentContext.CurrentWord = word;
        }

        private void UpdateContext()
        {
            currentContext.UsedWords.Add(currentContext.CurrentWord);
            for (int i = 0; i < NUMBER_OF_LETTERS; i++)
            {
                var backgroundBrush = WordButtons[i].Background as SolidColorBrush;
                if (backgroundBrush.Color == Colors.Gray)
                {
                    currentContext.GrayLetters.Add(WordButtons[i].Content.ToString()[0]);
                }
                else if (backgroundBrush.Color == Colors.Yellow)
                {
                    char letter = WordButtons[i].Content.ToString()[0];
                    currentContext.YellowLetters.Add(letter);
                    currentContext.Letters[i].ImpossibleLetters.Add(letter);
                }
                else if (backgroundBrush.Color == Colors.Green)
                {
                    currentContext.Letters[i].ExactLetter = WordButtons[i].Content.ToString()[0];
                }
            }
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

        private void AnotherWord_Click(object _, RoutedEventArgs __)
        {
            DisplayWord(currentContext.GetApropriateWord());
        }
        private void SwitchLangButton_Click(object sender, RoutedEventArgs _)
        {
            bool isEnglish = !currentContext.IsEng;
            currentContext = new Context(NUMBER_OF_LETTERS, isEnglish);
            DisplayWord(currentContext.GetApropriateWord());
            (sender as Button).Content = currentContext.IsEng ? "RU" : "EN";
        }
    }

    class Letter
    {
        public char? ExactLetter = null;
        public HashSet<char> ImpossibleLetters = new HashSet<char>();
    }
}
