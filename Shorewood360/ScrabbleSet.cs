using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Threads;
using System.Threading;

namespace IETGames.Shorewood
{


    public class ScrabbleSet: ILetterSet
    {
        public enum ScrabblePoints
        {
            OnePoint = 1000,
            TwoPoints = 2000,
            ThreePoints = 3000,
            FourPoints = 4000,
            FivePoints = 5000,
            EightPoints = 8000,
            TenPoints = 10000
        }

        List<Letter> letterSet = new List<Letter>();
        Dictionary<char, Letter> alphabet;// = new Dictionary<char, Letter>();
        Dictionary<char, int> scoring = new Dictionary<char, int>();
        Random random;// = new Random();

        public ScrabbleSet()
        {
            //scrabble one points
            scoring.Add('a', (int)ScrabblePoints.OnePoint);
            scoring.Add('e', (int)ScrabblePoints.OnePoint);
            scoring.Add('i', (int)ScrabblePoints.OnePoint);
            scoring.Add('o', (int)ScrabblePoints.OnePoint);
            scoring.Add('n', (int)ScrabblePoints.OnePoint);
            scoring.Add('r', (int)ScrabblePoints.OnePoint);
            scoring.Add('t', (int)ScrabblePoints.OnePoint);
            scoring.Add('l', (int)ScrabblePoints.OnePoint);
            scoring.Add('s', (int)ScrabblePoints.OnePoint);
            scoring.Add('u', (int)ScrabblePoints.OnePoint);

            //scrabble two points
            scoring.Add('d', (int)ScrabblePoints.TwoPoints);
            scoring.Add('g', (int)ScrabblePoints.TwoPoints);

            //scrabble three points
            scoring.Add('b', (int)ScrabblePoints.ThreePoints);
            scoring.Add('c', (int)ScrabblePoints.ThreePoints);
            scoring.Add('m', (int)ScrabblePoints.ThreePoints);
            scoring.Add('p', (int)ScrabblePoints.ThreePoints);

            //scrabble four points
            scoring.Add('f', (int)ScrabblePoints.FourPoints);
            scoring.Add('h', (int)ScrabblePoints.FourPoints);
            scoring.Add('v', (int)ScrabblePoints.FourPoints);
            scoring.Add('w', (int)ScrabblePoints.FourPoints);
            scoring.Add('y', (int)ScrabblePoints.FourPoints);

            //scrabble five points
            scoring.Add('k', (int)ScrabblePoints.FivePoints);
            
            //scrabble eight points
            scoring.Add('j', (int)ScrabblePoints.EightPoints);
            scoring.Add('x', (int)ScrabblePoints.EightPoints);

            //scrabble ten points
            scoring.Add('q', (int)ScrabblePoints.TenPoints);
            scoring.Add('z', (int)ScrabblePoints.TenPoints);
        }

        private void BuildLetterSet()
        {
            for (int i = 0; i < 12; i++)
            {
                letterSet.Add(alphabet['e']);
                if (i < 9)
                {
                    letterSet.Add(alphabet['a']);
                    letterSet.Add(alphabet['i']);
                }
                if (i < 8)
                {
                    letterSet.Add(alphabet['o']);
                }
                if (i < 6)
                {
                    letterSet.Add(alphabet['n']);
                    letterSet.Add(alphabet['r']);
                    letterSet.Add(alphabet['t']);
                    
                }
                if (i < 4)
                {
                    letterSet.Add(alphabet['l']);
                    letterSet.Add(alphabet['s']);
                    letterSet.Add(alphabet['u']);
                    letterSet.Add(alphabet['d']);
                }
                if (i < 3)
                {
                    letterSet.Add(alphabet['g']);
                }
                if (i < 2)
                {
                    letterSet.Add(alphabet['b']);
                    letterSet.Add(alphabet['c']);
                    letterSet.Add(alphabet['m']);
                    letterSet.Add(alphabet['p']);
                    letterSet.Add(alphabet['f']);
                    letterSet.Add(alphabet['h']);
                    letterSet.Add(alphabet['v']);
                    letterSet.Add(alphabet['w']);
                    letterSet.Add(alphabet['y']);
                }
                if (i < 1)
                {
                    letterSet.Add(alphabet['k']);
                    letterSet.Add(alphabet['j']);
                    letterSet.Add(alphabet['x']);
                    letterSet.Add(alphabet['q']);
                    letterSet.Add(alphabet['z']);
                }
            }
            random = new Random((int)DateTime.Now.Ticks);
            //random = new Random(121);
        }

        #region ILetterSet Members
        public Dictionary<char, Letter> Alphabet
        {
            get
            {
                return alphabet;
            }
            set
            {
                alphabet = new Dictionary<char, Letter>(value);
                BuildLetterSet();
            }
        }

        public Dictionary<char, int> Scoring
        {
            get
            {
                return scoring;
            }
            set
            {
                scoring = new Dictionary<char, int>(value);
            }
        }

        public Letter PullFromFromTextAnalyzer()
        {
            Letter availableLetter = Letter.EmptyLetter;
            if (Monitor.TryEnter(PredictiveTextAnalyzer.SuggestedLetters))
            {
                for (int i = 0; i < PredictiveTextAnalyzer.SuggestedLetters.Length; i++)
                {
                    if (PredictiveTextAnalyzer.SuggestedLetters[i] != Letter.EmptyLetter)
                    {
                        availableLetter = PredictiveTextAnalyzer.SuggestedLetters[i];
                        break;
                    }
                }
                PredictiveTextAnalyzer.ClearSuggestedLetters();
                Monitor.Exit(PredictiveTextAnalyzer.SuggestedLetters);
            }
            return availableLetter;
        }
        
        //private Letter previousSelectedLetter;
        int injectedLetterIndex = 0;

        private Letter PullFromInjectedList()
        {
            Letter nextLetter = Letter.EmptyLetter;
            if (InjectedLetterList.Count > 0)
            {

                nextLetter = alphabet[InjectedLetterList[injectedLetterIndex % InjectedLetterList.Count]];
                injectedLetterIndex++;
            }
            return nextLetter;
        }

        Letter previousLetter = Letter.EmptyLetter;
        public Letter GetRandomLetter()
        {
            Letter nextLetter = Letter.EmptyLetter;
            int roll = random.Next(7);

            switch (roll)
            {
                case 4:
                    nextLetter = PullFromFromTextAnalyzer();
                    break;                
                case 2:
                case 1:
                    nextLetter = PullFromInjectedList();
                    break;
                default:
                    nextLetter = letterSet[random.Next(letterSet.Count)];
                    break;
            }

            if ((nextLetter == Letter.EmptyLetter)||(nextLetter == previousLetter))
            {
                nextLetter = letterSet[random.Next(letterSet.Count)];
                while (nextLetter == previousLetter)
                {
                    nextLetter = letterSet[random.Next(letterSet.Count)];
                }
            }
            previousLetter = nextLetter;

            return nextLetter;

        }
        List<char> injectedLetterSet = new List<char>(40);
        public List<char> InjectedLetterList
        {
            get
            {
                return injectedLetterSet;
            }
            
        }

        public int GetPoints(char character)
        {
            return scoring[character];
        }

        public void Reset()
        {
            InjectedLetterList.Clear();
        }
        #endregion
    }
}
