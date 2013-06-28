using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IETGames.Shorewood
{
    public struct WordBuilder
    {
        StringBuilder builder;// = new StringBuilder(60);
        StringBuilder reverseBuilder;// = new StringBuilder(60);
        public List<Letter> letters;// = new List<Letter>(60);
        //public string caller = "";
        public bool isReversed;// = false;

        public float checkSum;// = 0;
        public int gridWidth;// = 0;
        public int points;// = 0;
        public bool isDisposed;// = true;
        public bool isValidWord;// = false;
        public WordBuilder(int width)
        {
            gridWidth = width;
            isDisposed = false;
            builder = new StringBuilder(60);
            reverseBuilder = new StringBuilder(60);
            letters = new List<Letter>(60);
            isReversed = false;
            checkSum = 0;
            points = 0;
            isValidWord = false;

        }

        public WordBuilder(WordBuilder wordBuilder)
        {            
            isDisposed = false;
            builder = new StringBuilder(60);
            reverseBuilder = new StringBuilder(60);
            letters = new List<Letter>(60);
            isReversed = false;
            checkSum = 0;
            gridWidth = 0;
            points = 0;
            isValidWord = false;
            Copy(wordBuilder);
        }

        public void Copy(WordBuilder wordBuilder)
        {
            Clear();
            this.gridWidth = wordBuilder.gridWidth;
            builder.Append(wordBuilder.builder);
            reverseBuilder.Append(wordBuilder.reverseBuilder);            
            this.letters.AddRange(wordBuilder.letters);
            this.checkSum = wordBuilder.checkSum;
            this.points = wordBuilder.points;
            this.reverseBuilder = wordBuilder.reverseBuilder;
            this.isReversed = wordBuilder.isReversed;
        }

        public void Clear()
        {            
            builder.Remove(0, builder.Length);
            reverseBuilder.Remove(0, reverseBuilder.Length);
            checkSum = 0;
            points = 0;
            letters.Clear();
            gridWidth = 0;
            isReversed = false;
            
        }

        public int Points
        {
            get
            {
                int score = 0;
                foreach (Letter letter in letters)
                {
                    score  += (Shorewood.letterSet.GetPoints(letter.letter));           
                }
                return score;
            }
        }

        public bool IsPalindrome
        {
            get
            {
                return CurrentReversedString.Equals(CurrentString);
            }
        }

        public static WordBuilder Merge(WordBuilder left, WordBuilder right)
        {
            WordBuilder builder = new WordBuilder(left);//ShorewoodPool.GetBuilder(left);

            for (int i = 1; i < right.letters.Count; i++)
            {
                builder.Append(right.letters[i]);
                //reverseBuilder.Insert(0, right.letters[i]);
            }
            builder.UpdateReverse();
            //builder.builder.Insert(left.builder.Length, right.builder.Remove(0, 1));
            return builder;
        }

        private void UpdateReverse()
        {
            //reverseBuilder.EnsureCapacity(builder.Length);
            //reverseBuilder.Cop
            reverseBuilder.Length = builder.Length;
            
            for (int i = builder.Length - 1; i >= 0; i--)
            {
                reverseBuilder[i] = builder[(builder.Length - 1) - i];
            }
            //Console.WriteLine(builder.ToString() + " reversed -> " + reverseBuilder.ToString());
        }

        public void Append(Letter letter)
        {
            if (letter.letter != Letter.EmptyLetter.letter)
            {
                builder.Append(letter.letter);
                //reverseBuilder.Append(letter.letter);
                letters.Add(letter);
                //UpdateReverse();
                checkSum += letter.gridPosition.X + gridWidth * letter.gridPosition.Y;
            }
        }

        public void Prepend(Letter letter)
        {
            if (letter.letter != Letter.EmptyLetter.letter)
            {
                char[] microsoftSucks = { letter.letter };
                builder.Insert(0, microsoftSucks);
                //reverseBuilder.Append(letter.letter);
                //UpdateReverse();
                letters.Insert(0, letter);
                checkSum += letter.gridPosition.X + gridWidth * letter.gridPosition.Y;
            }
        }

        public StringBuilder CurrentString
        {
            get
            {
                return builder;
            }
        }

        public StringBuilder CurrentReversedString
        {
            get
            {
                UpdateReverse();
                return reverseBuilder;
            }
        }       

        //public override string ToString()
        //{
        //    string rtnString = "WordBuilder:(" + CurrentString + ") -> ";
        //    foreach (var letter in letters)
        //    {
        //        rtnString += "[" + letter + "]";
        //    }
        //    return rtnString;
        //}     

        public static bool operator ==(WordBuilder leftBuilder, WordBuilder rightBuilder)
        {
            return leftBuilder.checkSum == rightBuilder.checkSum;
        }

        public static bool operator !=(WordBuilder leftBuilder, WordBuilder rightBuilder)
        {
            return leftBuilder.checkSum != rightBuilder.checkSum;
        }

        public override bool Equals(object obj)
        {
            return this == (WordBuilder)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
