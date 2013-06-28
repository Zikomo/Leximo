using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Algorithms;
using ListPool;
using IETGames.Shorewood.Utilities;
using IETGames.Shorewood.Threads;
using System.Threading;
using KiloWatt.Runtime.Support;

namespace IETGames.Shorewood
{
    public class Grid : List<Letter[]>
    {
        public float width = 6;
        public float height = 8;
        public Vector2 position = new Vector2(0,0);
        public Dictionary<char, Letter> alphabet;
        public float xPadding = 5f;
        public float yPadding = 7f;
        public SuffixTree wordDatabase;
        public Queue<WordBuilder> validWords = new Queue<WordBuilder>();
        public int minimumWordLength = 2;
        public int currentScore = 0;
        public ILetterSet letterSet;
        public bool isSearching = false;
        public bool changed = true;
        //protected TaskFunction wordSearch

        protected TaskFunction wordSearchFunction;// = new WaitCallback(CheckForWords);
        //public Letter CurrentLetter
        
        public StringBuilder largestWord = new StringBuilder("");

        //public int level = 0;
        int previousLargestWordLength = 0;


        public Grid(Dictionary<char, Letter> alphabet)
            : base()
        {
            this.alphabet = alphabet;
            wordSearchFunction = new TaskFunction(CheckForWords);
        }

        //public Grid(Dictionary<char, Letter> alphabet, int width, int height)
        //    : base(width*height)
        //{
        //    this.width = width;
        //    this.height = height;
        //    this.alphabet = alphabet;
        //    wordSearchFunction = new WaitCallback(CheckForWords);
        //}

        public void Allocate()
        {
            for (int y = 0; y < height; y++)
            {
                Letter[] row = new Letter[(int)width];
                for (int x = 0; x < width; x++)
                {
                    Letter newLetter = Letter.EmptyLetter;//new Letter(null, ' ');

                    newLetter.gridPosition = new Vector2(x, y);
                    row[x] = newLetter;
                }
                Add(row);
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {                    
                    PrepareLocation(x, y);
                }                
            }
        }

        public void ClearGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    EraseLetter(x, y);
                }
            }
            validWords.Clear();
            currentScore = 0;
        }

        public void PositionLetter(int x, int y, Letter letter)
        {
            this[y][x].texture = letter.texture;
            this[y][x].texture1337 = letter.texture1337;
            this[y][x].letter = letter.letter;
            this[y][x].color = letter.color;
            this[y][x].isPartOfAValidWord = false;
            this[y][x].changed = letter.changed;
            this[y][x].isGlowing = letter.isGlowing;
            changed = true;
            //PredictiveTextAnalyzer.ClearSuggestedLetters();
            //Console.WriteLine("PositionLetter:" + letter + " -> x:" + x + " y:" + y);
            
        }
        
        public void PrepareLocation(float x, float y)
        {
            Letter letter = this[(int)y][(int)x];            
            int offset = (int)y % 2;
            int antiOffset = 1 - offset;
            if (offset == 0)
            {
                letter.position.X = x * (letter.width + this.xPadding) + position.X;
            }
            else
            {
                letter.position.X = x * (letter.width + this.xPadding) + (letter.width + this.xPadding) / 2.0f + position.X;                
            }
            letter.position.Y = y * (letter.height + this.yPadding - 50) + position.Y ;
            letter.startPosition = letter.position + new Vector2(820, 180);
            letter.upperRightNeighbor = new Vector2(x + offset, y - 1);
            letter.upperLeftNeighbor = new Vector2(x - antiOffset, y - 1);
            letter.rightNeighbor = new Vector2(x + 1, y);
            letter.leftNeighbor = new Vector2(x - 1, y);
            letter.lowerRightNeighbor = new Vector2(x + offset, y + 1);
            letter.lowerLeftNeighbor = new Vector2(x - antiOffset, y + 1);
            this[(int)y][(int)x] = letter;

        }

        public List<Vector2> CleanUpLetters(List<Letter> letters)
        {            
            List<Vector2> positions = ShorewoodPool.vectorPool.GetList();
            foreach (var letter in letters)
            {
                int x = (int)letter.gridPosition.X;
                int y = (int)letter.gridPosition.Y;
                if ((GetLetter(x, y) != Letter.EmptyLetter)&&(!positions.Contains(new Vector2(x,y))))
                {
                    EraseLetter(x, y);
                    positions.Add(letter.gridPosition);
                }
            }
            return positions;
        }

       

        public List<Vector2> ShiftLetters(List<Vector2> positions)
        {
            List<Vector2> moved = ShorewoodPool.vectorPool.GetList();
            foreach (var location in positions)
            {
                int x = (int)location.X;
                int y = (int)location.Y;                

                for (; y > 0; y--)
                {
                    Letter above = GetLetter(x, y - 1);
                    if (above != Letter.EmptyLetter)
                    {
                        above.isPartOfAValidWord = false;
                        PositionLetter(x, y, above);
                        moved.Add(new Vector2(x,y));
                        EraseLetter(x, y - 1);
                    }
                }
                EraseLetter(x, 0);
            }
            return moved;
        }


        public WordBuilder ScoreValidWord(WordBuilder word)
        {
            //WordBuilder word = validWords.Dequeue();
            UpdateScore(ref word);
            return word; 
        }

        private void UpdateScore(ref WordBuilder builder)
        {
            int points = 0 ;
            
            foreach (Letter letter in builder.letters)
            {
                points = (letterSet.GetPoints(letter.letter));
                Shorewood.scoreBox.CurrentScore += points;

                builder.points += points;
            }            
            if (previousLargestWordLength < builder.CurrentString.Length)
            {                
                previousLargestWordLength = builder.CurrentString.Length;
            } 

        }

        public void SetLetter(float x, float y, Letter letter)
        {
            PositionLetter((int)x, (int)y, letter);
            letter.isVisible = true;

            letter.changed = true;
            //CheckForWords(GetLetter(x,y));
            //ThreadPool.QueueUserWorkItem(wordSearchFunction, GetLetter(x, y));
            Shorewood.threadPool.AddTask(wordSearchFunction, null, null, GetLetter(x, y));
        }

        //public void CheckForChanges()
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            Letter letter = GetLetter(x,y);
        //            if ((letter != Letter.EmptyLetter) && (letter.changed) && letter != Shorewood.normalModeGamePlay.currentLetter)
        //            {
        //                CheckForWords(letter);
        //            }
        //            letter.changed = false;
        //            PositionLetter(x, y, letter);
        //        }
        //    }
        //}

        public void EraseLetter(int x, int y)
        {             
            PositionLetter(x, y, Letter.EmptyLetter);
        }

        public Letter GetLetter(Vector2 position)
        {
            return GetLetter(position.X, position.Y);
        }

        public void ResetGlowers()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Letter letter = GetLetter(x, y);
                    //letter.isGlowing = false;
                    //PositionLetter(x, y, letter);
                    this[y][x].isGlowing = false;
                }

            }
        }

        public Letter GetLetter(float x, float y)
        {
            if ((x >= 0) && (x < width) && (y >= 0) && (y < height))
            {
                return this[(int)y][(int)x];
            }
            else
            {
                return Letter.EmptyLetter;
            }
        }

        private void HandleFoundWord(WordBuilder builder)
        {
            WordBuilder newBuilder = new WordBuilder(builder);
            if ((!validWords.Contains(newBuilder)
                && Shorewood.levelManager.CheckIfWordMeetsGoal(newBuilder, false)))
            {


                validWords.Enqueue(newBuilder);
                for (int i = 0; i < newBuilder.letters.Count; i++)
                {
                    Letter letter = GetLetter(newBuilder.letters[i].gridPosition);
                    letter.isPartOfAValidWord = true;
                    letter.isGlowing = true;
                    this[(int)letter.gridPosition.Y][(int)letter.gridPosition.X] = letter;
                    //Shorewood.particleSystem.TriggerExplosion(new GameTime(), letter.startPosition  * Shorewood.normalGameplayRenderer.scale * letter.scale);
                }
            }
            else if ((Shorewood.levelManager.AllGoalsCompleted)&&(!validWords.Contains(newBuilder)))
            {
                validWords.Enqueue(newBuilder);
                for (int i = 0; i < newBuilder.letters.Count; i++)
                {
                    Letter letter = GetLetter(newBuilder.letters[i].gridPosition);
                    letter.isPartOfAValidWord = true;
                    letter.isGlowing = true;
                    this[(int)letter.gridPosition.Y][(int)letter.gridPosition.X] = letter;
                    //Shorewood.particleSystem.TriggerExplosion(new GameTime(), letter.startPosition*Shorewood.normalGameplayRenderer.scale*letter.scale);
                }

            }
        }

        private bool CheckValidWord(WordBuilder builder)
        {
            if (wordDatabase.Search(builder.CurrentString) || wordDatabase.Search(builder.CurrentReversedString))
            {
                StringBuilder searchString = new StringBuilder(builder.CurrentString.Length);
                StringBuilder reverseSearchString = new StringBuilder(builder.CurrentString.Length);
                searchString.Copy(builder.CurrentString);
                reverseSearchString.Copy(builder.CurrentReversedString);
                searchString.Decorate();
                reverseSearchString.Decorate();
                if (wordDatabase.Search(searchString))
                {
                    HandleFoundWord(builder);
                }
                else if (wordDatabase.Search(reverseSearchString))
                {
                    builder.isReversed = true;
                    HandleFoundWord(builder);
                }
                return true;
            }
            return false;
        }

        //public void CheckForWords(Vector2 position)
        //{
        //    CheckForWords(GetLetter(position.X, position.Y));
        //}

        public void CheckForWords(object origin)
        {
            isSearching = true;
            CheckForWords((Letter)origin);
            isSearching = false;
        }

        public void CheckForWords(Letter origin)
        {
            if (origin != Letter.EmptyLetter)
            {
                List<WordBuilder> forwardBuilder = ShorewoodPool.builderPool.GetList();
                WordBuilder originBuilder = new WordBuilder((int)width);
                ScanForwardLetters(origin, forwardBuilder, originBuilder);
                List<WordBuilder> reverseBuilder = ShorewoodPool.builderPool.GetList();
                ScanReverseLetters(origin, reverseBuilder, originBuilder);
                MergeBuilderLists(reverseBuilder, forwardBuilder);
                ShorewoodPool.builderPool.ReturnList(forwardBuilder);
                ShorewoodPool.builderPool.ReturnList(reverseBuilder);
            }
        }

        public void ScanReverseLetters(Letter currentLetter, List<WordBuilder> builderList, WordBuilder previousBuilder)
        {
            WordBuilder newBuilder = new WordBuilder(previousBuilder);
            newBuilder.Prepend(currentLetter);
            builderList.Add(new WordBuilder(newBuilder));
            if ((newBuilder.CurrentString.Length > minimumWordLength))
            {
                if (!CheckValidWord(newBuilder))
                {                    
                    return;
                }
            }

            if (GetLetter(currentLetter.upperLeftNeighbor) != Letter.EmptyLetter)
            {
                ScanReverseLetters(GetLetter(currentLetter.upperLeftNeighbor), builderList, newBuilder);
            }
            if (GetLetter(currentLetter.leftNeighbor) != Letter.EmptyLetter)
            {
                ScanReverseLetters(GetLetter(currentLetter.leftNeighbor), builderList, newBuilder);
            }
            if (GetLetter(currentLetter.lowerLeftNeighbor) != Letter.EmptyLetter)
            {
                ScanReverseLetters(GetLetter(currentLetter.lowerLeftNeighbor), builderList, newBuilder);
            }            
        }

        public void ScanForwardLetters(Letter currentLetter, List<WordBuilder> builderList, WordBuilder previousBuilder)
        {
            WordBuilder newBuilder = new WordBuilder(previousBuilder);//ShorewoodPool.GetBuilder(previousBuilder);
            newBuilder.Append(currentLetter);
            builderList.Add(new WordBuilder(newBuilder));//ShorewoodPool.GetBuilder(newBuilder));

            if ((newBuilder.CurrentString.Length > minimumWordLength))
            {
                if (!CheckValidWord(newBuilder))
                {
                    //ShorewoodPool.ReturnBuilder(newBuilder);
                    return;
                }
            }

            if (GetLetter(currentLetter.upperRightNeighbor) != Letter.EmptyLetter)
            {
                ScanForwardLetters(GetLetter(currentLetter.upperRightNeighbor), builderList, newBuilder);                
            }
            if (GetLetter(currentLetter.rightNeighbor) != Letter.EmptyLetter)
            {
                ScanForwardLetters(GetLetter(currentLetter.rightNeighbor), builderList, newBuilder);                
            }
            if (GetLetter(currentLetter.lowerRightNeighbor) != Letter.EmptyLetter)
            {
                ScanForwardLetters(GetLetter(currentLetter.lowerRightNeighbor), builderList, newBuilder);                
            }
            //ShorewoodPool.ReturnBuilder(newBuilder);
            return;
        }

        public bool MergeBuilderLists(List<WordBuilder> leftBuilderList, List<WordBuilder> rightBuilderList)
        {
            bool rtnValue = false;
            foreach (WordBuilder rightBuilder in rightBuilderList)
            {
                foreach (WordBuilder leftBuilder in leftBuilderList)
                {
                    if (rightBuilder.CurrentString.Length + leftBuilder.CurrentString.Length - 1 > minimumWordLength)
                    {
                        WordBuilder merged = WordBuilder.Merge(leftBuilder, rightBuilder);
                        if (CheckValidWord(merged))
                        {
                            merged.isValidWord = true;
                            rtnValue = true;
                        }
                        //ShorewoodPool.ReturnBuilder(merged);
                    }
                }
            }
            return rtnValue;
        }

        public bool CheckIfLetterCanShiftDown(Letter letter)
        {
            if (letter.gridPosition.Y == height - 1)
            {
                return false;
            }
            else
            {
                return GetLetter(letter.gridPosition.X, letter.gridPosition.Y + 1) == Letter.EmptyLetter;
            }
        }

        public void ShiftLetter(Letter letter)
        {            
            PositionLetter((int)letter.gridPosition.X, (int)letter.gridPosition.Y + 1, letter);
            EraseLetter((int)letter.gridPosition.X, (int)letter.gridPosition.Y);
            letter = GetLetter(letter.gridPosition.X, letter.gridPosition.Y + 1);
            if (!CheckIfLetterCanShiftDown(letter))
            {
                //CheckForWords(letter);
            }
        }

        public bool ValidateGrid(GameTime gameTime, Letter currentLetter)
        {
            bool shiftHappened = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = (int)height-1; y >= 0; y--)
                {
                    Letter letter = GetLetter(x, y);
                    if (letter.gridPosition != currentLetter.gridPosition)
                    {
                        if ((letter != Letter.EmptyLetter)&&(CheckIfLetterCanShiftDown(letter)))
                        {
                            ShiftLetter(letter);
                            shiftHappened = true;
                        }
                        else if (letter == Letter.EmptyLetter)
                        {
                            letter.isPartOfAValidWord = false;
                        }

                    }
                }
            }
            return shiftHappened;
        }
    }
}