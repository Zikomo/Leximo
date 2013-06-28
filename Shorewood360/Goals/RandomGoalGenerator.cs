using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algorithms;

namespace IETGames.Shorewood.Goals
{
    
    public class RandomGoalGenerator
    {
        static int index = 0;
        static Type previousGoalType = null;
        static Random random = new Random();
        
        RandomGoalGenerator()
        {
        }

        private static IGoal RandomEasy()
        {
            int roll = random.Next(3);
            switch (roll)
            {
                case 0:
                    return new WordCountGoal(10, random.Next(3,4), TimeSpan.Zero);
                case 1:
                    return new BackwardsWordCountGoal(10 + random.Next(5,11),3, TimeSpan.Zero);
                case 2:
                    return new WordScoreGoal(3000 + random.Next(1,8) * 1000, 1, TimeSpan.Zero);
                case 3:
                    //return new SpecificWordGoal(SuffixTree.FourLetterWords[random.Next(SuffixTree.FourLetterWords.Length)], TimeSpan.Zero);
                default:
                    return new WordCountGoal(10, random.Next(3,4), TimeSpan.Zero);
            }
        }

        private static IGoal RandomMedium()
        {
            int roll = random.Next(6);
            int randomWordLength = random.Next(3, 6);
            switch (roll)
            {
                case 0:
                    if (randomWordLength == 3)
                    {
                        return new WordCountGoal(40, 3, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 4)
                    {
                        return new WordCountGoal(10, 4, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new WordCountGoal(1, 5, TimeSpan.Zero);
                    }
                    else
                    {
                        return new WordCountGoal(40, 3, TimeSpan.Zero);
                    }                    
                case 1:
                    if (randomWordLength == 3)
                    {
                        return new BackwardsWordCountGoal(20, 3, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 4)
                    {
                        return new BackwardsWordCountGoal(10, 4, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new BackwardsWordCountGoal(1, 5, TimeSpan.Zero);
                    }
                    else
                    {
                        return new BackwardsWordCountGoal(20, 3, TimeSpan.Zero);
                    }                    
                case 2:
                    return new WordScoreGoal(3000 + random.Next(1, 9) * 1000, randomWordLength - 2, TimeSpan.Zero);
                case 3:
                    //return new SpecificWordGoal(SuffixTree.FourLetterWords[random.Next(SuffixTree.FourLetterWords.Length)], TimeSpan.Zero);
                case 4:
                    return new _1337Goal(5, 3, TimeSpan.Zero);
                case 5:
                    return new GoalSubstring(SuffixTree.SubStrings[random.Next(SuffixTree.SubStrings.Length)], TimeSpan.Zero);
                default:
                    return new WordCountGoal(10, random.Next(3, 4), TimeSpan.Zero);
            }
        }

        private static IGoal RandomHard()
        {
            int roll = random.Next(7);
            int randomWordLength = random.Next(4, 7);
            switch (roll)
            {
                case 0:
                    if (randomWordLength == 4)
                    {
                        return new WordCountGoal(15, 4, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new WordCountGoal(10, 5, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 6)
                    {
                        return new WordCountGoal(1, 6, TimeSpan.Zero);
                    }
                    else
                    {
                        return new WordCountGoal(30, 4, TimeSpan.Zero);
                    }                    
                case 1:
                    if (randomWordLength == 4)
                    {
                        return new BackwardsWordCountGoal(15, 4, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new BackwardsWordCountGoal(5, 5, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 6)
                    {
                        return new BackwardsWordCountGoal(1, 6, TimeSpan.Zero);
                    }
                    else
                    {
                        return new BackwardsWordCountGoal(20, 4, TimeSpan.Zero);
                    }                    
                case 2:
                    if (randomWordLength == 4)
                    {
                        return new WordScoreGoal(8000, 3, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new WordScoreGoal(10000, 2, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 6)
                    {
                        return new WordScoreGoal(12000, 1, TimeSpan.Zero);
                    }
                    else
                    {
                        return new WordScoreGoal(8000, 3, TimeSpan.Zero);
                    }                    
                case 3:
                    //if (randomWordLength == 6)
                    //{
                    //    return new SpecificWordGoal(SuffixTree.SixLetterWords[random.Next(SuffixTree.SixLetterWords.Length)], TimeSpan.Zero);
                    //}
                    //else 
                    //{
                    //    return new SpecificWordGoal(SuffixTree.FourLetterWords[random.Next(SuffixTree.FourLetterWords.Length)], TimeSpan.Zero);
                    //}
                case 4:
                    return new _1337Goal(20, 3, TimeSpan.Zero);
                case 5:
                    return new PalindromeGoal(randomWordLength - 3, 3, TimeSpan.Zero);
                case 6:
                    return new GoalSubstring(SuffixTree.SubStrings[random.Next(SuffixTree.SubStrings.Length)], TimeSpan.Zero);
                default:
                    return new WordCountGoal(10, random.Next(3, 4), TimeSpan.Zero);
            }
        }

        private static IGoal RandomExtreme()
        {
            int roll = random.Next(7);
            int randomWordLength = random.Next(4, 8);
            switch (roll)
            {
                case 0:
                    if (randomWordLength == 4)
                    {
                        return new WordCountGoal(30, 4, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new WordCountGoal(20, 5, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 6)
                    {
                        return new WordCountGoal(5, 6, TimeSpan.Zero);
                    }
                    else
                    {
                        return new WordCountGoal(30, 4, TimeSpan.Zero);
                    }
                case 1:
                    if (randomWordLength == 4)
                    {
                        return new BackwardsWordCountGoal(30, 4, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new BackwardsWordCountGoal(10, 5, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 6)
                    {
                        return new BackwardsWordCountGoal(5, 6, TimeSpan.Zero);
                    }
                    else
                    {
                        return new BackwardsWordCountGoal(20, 4, TimeSpan.Zero);
                    }
                case 2:
                    if (randomWordLength == 4)
                    {
                        return new WordScoreGoal(9000, 5, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 5)
                    {
                        return new WordScoreGoal(11000, 3, TimeSpan.Zero);
                    }
                    else if (randomWordLength == 6)
                    {
                        return new WordScoreGoal(13000, 2, TimeSpan.Zero);
                    }
                    else
                    {
                        return new WordScoreGoal(8000, 5, TimeSpan.Zero);
                    }
                case 3:
                    //if (randomWordLength == 6)
                    //{
                    //    return new SpecificWordGoal(SuffixTree.SixLetterWords[random.Next(SuffixTree.SixLetterWords.Length)], TimeSpan.Zero);
                    //}
                    //else if (randomWordLength == 4)
                    //{
                    //    return new SpecificWordGoal(SuffixTree.FourLetterWords[random.Next(SuffixTree.FourLetterWords.Length)], TimeSpan.Zero);
                    //}
                    //else
                    //{
                    //    return new SpecificWordGoal(SuffixTree.FiveLetterWords[random.Next(SuffixTree.FiveLetterWords.Length)], TimeSpan.Zero);
                    //}
                case 4:
                    //return new _1337Goal(25, 4, TimeSpan.Zero);
                case 5:
                    return new PalindromeGoal(5, 3, TimeSpan.Zero);
                case 6:
                    return new GoalSubstring(SuffixTree.SubStrings[random.Next(SuffixTree.SubStrings.Length)], TimeSpan.Zero);
                default:
                    return new GoalSubstring(SuffixTree.SubStrings[random.Next(SuffixTree.SubStrings.Length)], TimeSpan.Zero);                    
            }
        }

        private static IGoal GenerateGoal(Difficulty difficulty)
        {
            IGoal randomGoal = null;
            if (index % 2 == 0)
            {
                index++;
                switch (difficulty)
                {                       
                    case Difficulty.Medium:
                    case Difficulty.Easy:
                        return new SpecificWordGoal(SuffixTree.FourLetterWords[random.Next(SuffixTree.FourLetterWords.Length)], TimeSpan.Zero);
                    case Difficulty.Hard:
                        return new SpecificWordGoal(SuffixTree.FiveLetterWords[random.Next(SuffixTree.FiveLetterWords.Length)], TimeSpan.Zero);
                    case Difficulty._1337:
                        return new SpecificWordGoal(SuffixTree.SixLetterWords[random.Next(SuffixTree.SixLetterWords.Length)], TimeSpan.Zero);
                }                    
            }
            switch (difficulty)
            {
                case Difficulty.FreeSpell:
                    randomGoal = RandomEasy();
                    while (randomGoal.GetType() == previousGoalType)
                    {
                        randomGoal = RandomEasy();
                    }
                    previousGoalType = randomGoal.GetType();
                    
                    break;
                case Difficulty.Easy:
                    randomGoal = RandomMedium();
                    while (randomGoal.GetType() == previousGoalType)
                    {
                        randomGoal = RandomMedium();
                    }
                    previousGoalType = randomGoal.GetType();
                    
                    break;
                case Difficulty.Medium:
                    randomGoal = RandomHard();
                    while (randomGoal.GetType() == previousGoalType)
                    {
                        randomGoal = RandomHard();
                    }
                    previousGoalType = randomGoal.GetType();
                    
                    break;
                case Difficulty.Hard:
                    randomGoal = new SpecificWordGoal(SuffixTree.FiveLetterWords[random.Next(SuffixTree.FourLetterWords.Length)], TimeSpan.Zero);
                    
                    break;
                case Difficulty._1337:
                    randomGoal = RandomExtreme();
                    while (randomGoal.GetType() == previousGoalType)
                    {
                        randomGoal = RandomExtreme();
                    }
                    previousGoalType = randomGoal.GetType();
                    
                    break;                    
            }
            index++;
            return randomGoal;
        }

        public static void Reset(int start)
        {
            index = start;
        }

        public static IGoal GenerateGoal()
        {
            return GenerateGoal(Shorewood.Difficulty);
        }
    }
}