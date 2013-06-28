using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IETGames.Shorewood.Goals;

namespace IETGames.Shorewood
{

    public class NormalGameplayLevelManager:LevelManager
    {
        public NormalGameplayLevelManager()
            :base(30)
        {
            SetupLevels();
        }

        private void SetupLevels()
        {            
            Easy();
            Easy();
            Medium();
            Hard();
            _1337();
            //WTF();
        }

        private void Easy()
        {            
            Level level = new Level(0, this);
            level.goalFrequency = new TimeSpan(0, 0, 25);
            level.goalRewardTime = new TimeSpan(0, 0, 20);
            level.levelDuration = new TimeSpan(0, 5, 0);
            level.AddStartGoal(new WordCountGoal(25, 3, level.levelDuration));
            level.AddGoal(new SpecificWordGoal(new StringBuilder("fun"), TimeSpan.Zero));
            level.AddGoal(new BackwardsWordCountGoal(10, 0, TimeSpan.Zero));                  
            if (Shorewood.IsTrial)
            {
                   
                level.AddGoal(new SpecificWordGoal(new StringBuilder("buy"), TimeSpan.Zero));
                level.AddGoal(new BackwardsWordCountGoal(15, 0, TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("this"), TimeSpan.Zero));
                level.AddGoal(new WordCountGoal(20, 0, TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("game"), TimeSpan.Zero));                
            }
            else
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("you"), TimeSpan.Zero));
                level.AddGoal(new BackwardsWordCountGoal(15, 0, TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("rock"), TimeSpan.Zero));
                
            }
            level.AddGoal(new WordCountGoal(2, 4, TimeSpan.Zero));   
           
            PredeterminedLevels.Add(level);
        }


        private void Medium()
        {
            Level level = new Level(0, this);
            level.levelDuration = new TimeSpan(0, 5, 0);
            level.goalFrequency = new TimeSpan(0, 0, 20);
            level.goalRewardTime = new TimeSpan(0, 0, 20);
            level.AddStartGoal(new WordCountGoal(40, 3, level.levelDuration));
            level.AddGoal(new SpecificWordGoal(new StringBuilder("dude"), TimeSpan.Zero));
            level.AddGoal(new WordCountGoal(5, 4, TimeSpan.Zero));
            level.AddGoal(new GoalSubstring(new StringBuilder("hat"), TimeSpan.Zero));
            level.AddGoal(new _1337Goal(25, 3, TimeSpan.Zero));
            if (Shorewood.IsTrial)
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("buy"), TimeSpan.Zero));
                level.AddGoal(new WordCountGoal(5, 4, TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("this"), TimeSpan.Zero));
                level.AddGoal(new BackwardsWordCountGoal(3, 4, TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("game"), TimeSpan.Zero));
                
            }
            else
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("you"), TimeSpan.Zero));
                level.AddGoal(new WordCountGoal(5, 4, TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("rock"), TimeSpan.Zero));                
            }
            level.AddGoal(new BackwardsWordCountGoal(3, 4, TimeSpan.Zero));            
            PredeterminedLevels.Add(level);
        }

        private void Hard()
        {
            Level level = new Level(0.25f, this);
            level.levelDuration = new TimeSpan(0, 3, 0);
            level.goalFrequency = new TimeSpan(0, 0, 15);
            level.goalRewardTime = new TimeSpan(0, 0, 15);
            level.AddStartGoal(new PalindromeGoal(1, 3, TimeSpan.Zero));
            level.AddGoal(new _1337Goal(10, 4, TimeSpan.Zero));
            if (Shorewood.IsTrial)
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("buy"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("this"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("game"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("please"), TimeSpan.Zero));
            }
            else
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("you"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("are"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("cool"), TimeSpan.Zero));                
            }
            level.AddGoal(new PalindromeGoal(1, 4, TimeSpan.Zero));            
            level.AddGoal(new WordScoreGoal(9000, 1, TimeSpan.Zero));
            level.AddGoal(new GoalSubstring(new StringBuilder("amp"), TimeSpan.Zero));
            level.AddGoal(new GoalSubstring(new StringBuilder("ion"), TimeSpan.Zero));         
            PredeterminedLevels.Add(level);
        }

        //private void LOL()
        //{
        //    Level level = new Level(0f, this);
        //    level.levelDuration = new TimeSpan(0, 2, 30);
        //    level.goalFrequency = new TimeSpan(0, 0, 20);
        //    level.goalRewardTime = new TimeSpan(0, 0, 20);
        //    level.AddStartGoal(new SpecificWordGoal(new StringBuilder("this"), level.levelDuration));
        //    level.AddGoal(new SpecificWordGoal(new StringBuilder("mode"), TimeSpan.Zero));
        //    level.AddGoal(new SpecificWordGoal(new StringBuilder("hurts"), TimeSpan.Zero));
        //    //level.AddStartGoal(new BackwardsWordCountGoal(40, 0, level.levelDuration));
        //    if (Shorewood.IsTrial)
        //    {
        //        level.AddGoal(new SpecificWordGoal(new StringBuilder("buy"), TimeSpan.Zero));
        //        level.AddGoal(new SpecificWordGoal(new StringBuilder("this"), TimeSpan.Zero));
        //        level.AddGoal(new SpecificWordGoal(new StringBuilder("game"), TimeSpan.Zero));                
        //    }
        //    else
        //    {
        //        level.AddGoal(new SpecificWordGoal(new StringBuilder("you"), TimeSpan.Zero));
        //        level.AddGoal(new SpecificWordGoal(new StringBuilder("are"), TimeSpan.Zero));
        //        level.AddGoal(new SpecificWordGoal(new StringBuilder("cool"), TimeSpan.Zero));
        //    }            
        //    //level.AddGoal(new WordScoreGoal(10000, 1, TimeSpan.Zero));

        //    level.AddGoal(new SpecificWordGoal(new StringBuilder("good"), TimeSpan.Zero));
        //    level.AddGoal(new SpecificWordGoal(new StringBuilder("luck"), TimeSpan.Zero));

        //    //level.AddGoal(new PalindromeGoal(1, 5, TimeSpan.Zero));
        //    //level.AddGoal(new _1337Goal(40, 3, TimeSpan.Zero));
        //    PredeterminedLevels.Add(level);
        //}

        private void _1337()
        {
            Level level = new Level(0.5f, this);
            level.levelDuration = new TimeSpan(0, 3, 0);
            level.goalFrequency = new TimeSpan(0, 0, 10);
            level.goalRewardTime = new TimeSpan(0, 0, 5);
            level.AddStartGoal(new WordCountGoal(30, 4, level.levelDuration));

            if (Shorewood.IsTrial)
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("buy"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("this"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("game"), TimeSpan.Zero));                
            }
            else
            {
                level.AddGoal(new SpecificWordGoal(new StringBuilder("you"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("are"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("very"), TimeSpan.Zero));
                level.AddGoal(new SpecificWordGoal(new StringBuilder("cool"), TimeSpan.Zero));
            }
            level.AddGoal(new SpecificWordGoal(new StringBuilder("game"), TimeSpan.Zero));
            level.AddGoal(new SpecificWordGoal(new StringBuilder("over"), TimeSpan.Zero));
            level.AddGoal(new SpecificWordGoal(new StringBuilder("really"), TimeSpan.Zero));
            level.AddGoal(new SpecificWordGoal(new StringBuilder("soon"), TimeSpan.Zero));
            level.AddGoal(new PalindromeGoal(1, 3, TimeSpan.Zero));
            level.AddGoal(new PalindromeGoal(1, 4, TimeSpan.Zero));
            level.AddGoal(new PalindromeGoal(1, 3, TimeSpan.Zero));
            level.AddGoal(new GoalSubstring(new StringBuilder("ninth"), TimeSpan.Zero));
            level.AddGoal(new GoalSubstring(new StringBuilder("planet"), TimeSpan.Zero));
            level.AddGoal(new GoalSubstring(new StringBuilder("games"), TimeSpan.Zero));
            level.is1337 = true;
            PredeterminedLevels.Add(level);
            
        }

   }
}