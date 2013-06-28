using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IETGames.Shorewood.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KiloWatt.Runtime.Support;

namespace IETGames.Shorewood.Threads
{
    public static class PredictiveTextAnalyzer
    {
        static ManualResetEvent threadStop = new ManualResetEvent(false);
        static bool isActive = false;

        public static void ClearSuggestedLetters()
        {
            if (suggestedLetters != null)
            {
                for (int i = 0; i < suggestedLetters.Length; i++)
                {
                    suggestedLetters[i] = Letter.EmptyLetter;
                }
            }
        }

        static public void Start()
        {
            if (!isActive)
            {
                if (suggestedLetters == null)
                {
                    suggestedLetters = new Letter[(int)Shorewood.normalGameplayRenderer.grid.width];
                }
                ClearSuggestedLetters();
                threadStop.Reset();
                Shorewood.threadPool.AddTask(new TaskFunction(Analyzer),null,null,null);
            }
            else
            {
                ClearSuggestedLetters();
            }
        }

        static public void Stop()
        {
            threadStop.Set();
        }

        static private void CheckForWords(Letter origin)
        {
            if (origin != Letter.EmptyLetter)
            {
                List<WordBuilder> forwardBuilder = ShorewoodPool.builderPool.GetList();
                WordBuilder originBuilder = new WordBuilder((int)Shorewood.normalGameplayRenderer.grid.width);
                ScanForwardLetters(origin, forwardBuilder, originBuilder);
                List<WordBuilder> reverseBuilder = ShorewoodPool.builderPool.GetList();
                ScanReverseLetters(origin, reverseBuilder, originBuilder);
                MergeBuilderLists(reverseBuilder, forwardBuilder);
                ShorewoodPool.builderPool.ReturnList(forwardBuilder);
                ShorewoodPool.builderPool.ReturnList(reverseBuilder);
            }
        }

        static private void ScanReverseLetters(Letter currentLetter, List<WordBuilder> builderList, WordBuilder previousBuilder)
        {
            WordBuilder newBuilder = new WordBuilder(previousBuilder);
            newBuilder.Prepend(currentLetter);
            builderList.Add(new WordBuilder(newBuilder));
            if ((newBuilder.CurrentString.Length > Shorewood.normalGameplayRenderer.grid.minimumWordLength))
            {
                if (!CheckValidWord(newBuilder))
                {
                    return;
                }
            }

            if (Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.upperLeftNeighbor) != Letter.EmptyLetter)
            {
                ScanReverseLetters(Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.upperLeftNeighbor), builderList, newBuilder);
            }
            if (Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.leftNeighbor) != Letter.EmptyLetter)
            {
                ScanReverseLetters(Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.leftNeighbor), builderList, newBuilder);
            }
            if (Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.lowerLeftNeighbor) != Letter.EmptyLetter)
            {
                ScanReverseLetters(Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.lowerLeftNeighbor), builderList, newBuilder);
            }
        }

        static private void ScanForwardLetters(Letter currentLetter, List<WordBuilder> builderList, WordBuilder previousBuilder)
        {
            WordBuilder newBuilder = new WordBuilder(previousBuilder);//ShorewoodPool.GetBuilder(previousBuilder);
            newBuilder.Append(currentLetter);
            builderList.Add(new WordBuilder(newBuilder));//ShorewoodPool.GetBuilder(newBuilder));

            if ((newBuilder.CurrentString.Length > Shorewood.normalGameplayRenderer.grid.minimumWordLength))
            {
                if (!CheckValidWord(newBuilder))
                {
                    //ShorewoodPool.ReturnBuilder(newBuilder);
                    return;
                }
            }

            if (Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.upperRightNeighbor) != Letter.EmptyLetter)
            {
                ScanForwardLetters(Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.upperRightNeighbor), builderList, newBuilder);
            }
            if (Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.rightNeighbor) != Letter.EmptyLetter)
            {
                ScanForwardLetters(Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.rightNeighbor), builderList, newBuilder);
            }
            if (Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.lowerRightNeighbor) != Letter.EmptyLetter)
            {
                ScanForwardLetters(Shorewood.normalGameplayRenderer.grid.GetLetter(currentLetter.lowerRightNeighbor), builderList, newBuilder);
            }
            //ShorewoodPool.ReturnBuilder(newBuilder);
            return;
        }

        static private bool MergeBuilderLists(List<WordBuilder> leftBuilderList, List<WordBuilder> rightBuilderList)
        {
            bool rtnValue = false;
            foreach (WordBuilder rightBuilder in rightBuilderList)
            {
                foreach (WordBuilder leftBuilder in leftBuilderList)
                {
                    if (rightBuilder.CurrentString.Length + leftBuilder.CurrentString.Length - 1 > Shorewood.normalGameplayRenderer.grid.minimumWordLength)
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

        static bool wordAvailable = false;

        static private bool CheckValidWord(WordBuilder builder)
        {
            StringBuilder searchString = new StringBuilder(builder.CurrentString.Length);
            StringBuilder reverseSearchString = new StringBuilder(builder.CurrentString.Length);
            searchString.Copy(builder.CurrentString);
            reverseSearchString.Copy(builder.CurrentReversedString);
            searchString.Decorate();
            reverseSearchString.Decorate();
            if (Shorewood.wordDatabase.Search(searchString) || Shorewood.wordDatabase.Search(reverseSearchString))
            {
                wordAvailable = true;
                return true;
            }
            return false;
        }

        static Letter[] suggestedLetters;
        public static Letter[] SuggestedLetters
        {
            get
            {
                return suggestedLetters;
            }
        }

        static private void Analyzer(object nothing)
        {
            isActive = true;
            Grid grid = Shorewood.normalGameplayRenderer.grid;
            NormalGameplay gameplay = Shorewood.normalModeGamePlay;
            ILetterSet letterSet = Shorewood.letterSet;
            Dictionary<char, Letter> alphabet = Shorewood.alphabet;
            
            //Dictionary<char, lett
            while (!threadStop.WaitOne(250, false)&&Shorewood.normalModeGamePlay.gameplayState == NormalGameplay.NormalGameplayState.CurrentLetterActive)
            {
                
                for (int x = 0; x < grid.width; x++)
                {
                    for (int y = 1; y < grid.height; y++)
                    {
                        Letter letter = grid.GetLetter(new Vector2(x, y));
                        int offset = y % 2;
                        if ((letter != Letter.EmptyLetter)&&(letter.gridPosition  != gameplay.currentLetter.gridPosition ))
                        {                            
                            for (int i = 0; i < alphabet.Values.Count; i++)
                            {
                                wordAvailable = false;
                                Letter trialLetter = alphabet.Values.ElementAt(i);
                                trialLetter.gridPosition = new Vector2(x, y - 1);
                                trialLetter.leftNeighbor = new Vector2(x - 1, y - 1);
                                trialLetter.lowerLeftNeighbor = new Vector2(x - offset, y - 1);
                                trialLetter.lowerRightNeighbor = new Vector2(x + offset, y - 1);
                                trialLetter.rightNeighbor = new Vector2(x + 1, y - 1);
                                CheckForWords(trialLetter);
                                Monitor.Enter(SuggestedLetters);
                                if (wordAvailable)
                                {
                                    SuggestedLetters[x] = trialLetter;
                                }
                                Monitor.Exit(SuggestedLetters);
                            }
                            break;
                        }
                    }
                }
            }
            isActive = false;
        }
    }
}