using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood
{
    public interface ILetterSet
    {
        Dictionary<char,Letter> Alphabet
        {
            get;
            set;
        }

        Dictionary<char, int> Scoring
        {
            get;
            set;
        }

        List<char> InjectedLetterList
        {
            get;            
        }

        int GetPoints(char character);

        Letter GetRandomLetter();

        void Reset();
    }
}
