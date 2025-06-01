using System.Collections.Generic;

namespace WordlyRu
{
    class Letter
    {
        public char? ExactLetter = null;
        public HashSet<char> ImpossibleLetters = new HashSet<char>();
    }
}
