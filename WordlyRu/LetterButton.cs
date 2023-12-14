using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WordlyRu
{
    internal class LetterButton : Button
    {
        public LetterButton(int letterPosition)
        {
            Name = letterPosition.ToString();
            HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
            FontSize = 100;
            Margin = new Windows.UI.Xaml.Thickness(10);
            Background = new SolidColorBrush(Colors.Gray);
        }
    }
}
