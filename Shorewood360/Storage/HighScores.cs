using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace IETGames.Shorewood.Storage
{
    
    public class HighScores
    {
        public static readonly string[] Names = {  "Linda",    "Henry",    "Odetta",  "Coffie",   "Tim",     "Sheri",   "Mike",    "Arielle",   "Bill",   "Graham",
                                                   "Arlo",     "Jackie",   "Kevin",   "Steve",    "Terry",   "Brooke",  "Amy",     "Jason",     "Dustin", "Ashley",                                                   
                                                   "Sally",    "Al",       "Luke",    "Jeff",     "Monica",  "Maggie",  "Rachel",  "Jamin",     "Kami",   "Brent",
                                                   "Janet",    "Jen",      "Keith",   "Kathleen", "Katie",   "Leah",    "Michelle","Sue",       "Briana", "Carale",                                                   
                                                   "Marianne", "Maxine",   "Heidi",   "Clair",    "Carolyn", "Belinda", "Natalie", "Christina", "Emily",  "Mary",
                                                   };


        List<ScoreEntry> easyScores = new List<ScoreEntry>();
        List<ScoreEntry> mediumScores = new List<ScoreEntry>();
        List<ScoreEntry> hardScores = new List<ScoreEntry>();
        List<ScoreEntry> lolScores = new List<ScoreEntry>();
        List<ScoreEntry> wtfScores = new List<ScoreEntry>();
        [XmlIgnore]
        List<GamerEntry> gamers = new List<GamerEntry>();
        [XmlIgnore]
        RenderTarget2D[] scoreTarget;
        [XmlIgnore]
        Texture2D backgroundTexture;
        [XmlIgnore]
        Texture2D highlightTexture;
        [XmlIgnore]
        public static int scoreWidth = 600;
        [XmlIgnore]
        public Texture2D[] renderedScores;
        [XmlIgnore]
        bool loaded = false;
        public HighScores()            
        {

            //Initalize();
        }

        public void Load()
        {
            if (!loaded)
            {
                backgroundTexture = Shorewood.Content.Load<Texture2D>(@"screens\menus\hs.trippy");
                highlightTexture = new Texture2D(Shorewood.graphics.GraphicsDevice, 1, 1);
                Color[] highlightColor = { Color.White };
                highlightTexture.SetData<Color>(highlightColor);
                scoreTarget = new RenderTarget2D[5];
                for (int i = 0; i < scoreTarget.Length; i++)
                {
                    scoreTarget[i] = Shorewood.CreateRenderTarget(Shorewood.graphics.GraphicsDevice, 1, backgroundTexture.Format);
                }
                renderedScores = new Texture2D[5];
                loaded = true;
            }
        }
        
        public void Populate()
        {
            int i = 0;
            easyScores = AllocateList(3000000, new TimeSpan(0,6,0), Difficulty.FreeSpell, ref i, 200000);
            mediumScores = AllocateList(2000000, new TimeSpan(0, 8, 0), Difficulty.Easy, ref i, 100000);
            hardScores = AllocateList(1000000, new TimeSpan(0, 6, 0), Difficulty.Medium, ref i, 50000);
            lolScores = AllocateList(1000000, new TimeSpan(0, 2, 0), Difficulty.Hard, ref i, 50000);
            wtfScores = AllocateList(1000000, new TimeSpan(0, 1, 0), Difficulty._1337, ref i, 50000);
        }

        public List<ScoreEntry> GetList(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.FreeSpell:
                    return easyScores;
                case Difficulty.Easy:
                    return mediumScores;
                case Difficulty.Medium:
                    return hardScores;
                case Difficulty.Hard:
                    return lolScores;
                case Difficulty._1337:
                    return wtfScores;
                default:
                    return easyScores;
            }
        }

        public void UpdateScoreTexture()
        {
            renderedScores[0] = DrawScores(EasyScores, Difficulty.FreeSpell, 0);            
            renderedScores[1] = DrawScores(MediumScores, Difficulty.Easy, 1);            
            renderedScores[2] = DrawScores(HardScores, Difficulty.Medium, 2);            
            renderedScores[3] = DrawScores(LOLScores, Difficulty.Hard, 3);            
            renderedScores[4] = DrawScores(WTFScores, Difficulty._1337, 4);
        }

        StringBuilder GetDifficultyString(Difficulty difficulty)
        {            
            switch (difficulty)
            {
                case Difficulty.FreeSpell:
                    return Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DifficultyEasy];
                case Difficulty.Easy:
                    return Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DifficultyMedium];
                case Difficulty.Medium:
                    return Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DifficultyHard];
                case Difficulty.Hard:
                    return Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DifficultyLOL];
                case Difficulty._1337:
                    return Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DifficultyWTF];
                default:
                    return Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.DifficultyEasy];

            }
        }

        private Texture2D DrawScores(List<ScoreEntry> scores, Difficulty difficulty, int index)
        {
            Shorewood.graphics.GraphicsDevice.SetRenderTarget(0, scoreTarget[index]);
            Shorewood.graphics.GraphicsDevice.Clear(Color.TransparentBlack);
            Shorewood.spriteBatch.Begin();
            Vector2 position = Vector2.One * 10;
            StringBuilder scoreString = new StringBuilder(100);

            scoreString.Append(GetDifficultyString(difficulty));
            Vector2 header = position;
            header.X += scoreWidth/2 -  Shorewood.fonts[FontTypes.ScoreFont].MeasureString(scoreString).X / 2;
            Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.ScoreFont], scoreString, header, Color.Black);
            position.Y += Shorewood.fonts[FontTypes.ScoreFont].MeasureString(scoreString).Y;
            int count = 0;
            foreach (var score in scores)
            {                
                scoreString.Length = 0;
                GamerEntry entry = new GamerEntry();
                entry.gamerTag = score.GamerTag;
                if (Gamers.Contains(entry))
                {
                    entry = Gamers[Gamers.IndexOf(entry)];
                }
                //Shorewood.spriteBatch.Draw(entry.GamerPicture, position, Color.White);
                if (count % 2 == 0)
                {
                    Shorewood.spriteBatch.Draw(highlightTexture, new Rectangle((int)position.X, (int)position.Y, scoreWidth, 33), new Color(Color.White,220));
                }
                else
                {
                    Shorewood.spriteBatch.Draw(highlightTexture, new Rectangle((int)position.X, (int)position.Y, scoreWidth, 33), new Color(Color.WhiteSmoke, 220));
                }

                Shorewood.spriteBatch.Draw(entry.GamerPicture, position, null, Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 1);
                scoreString.Append(score.GamerTag);
                Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.ScoreFont], scoreString, position + Vector2.UnitX * 35, Color.Black);
                scoreString.Length = 0;
                scoreString.Append(score.Score);
                Shorewood.spriteBatch.DrawString(Shorewood.fonts[FontTypes.ScoreFont], scoreString, position + Vector2.UnitX * (scoreWidth - Shorewood.fonts[FontTypes.ScoreFont].MeasureString(scoreString).X - 20), Color.Black);
                position.Y += 33;
                count++;
            }

            Shorewood.spriteBatch.End();
            Shorewood.graphics.GraphicsDevice.SetRenderTarget(0, null);
            return scoreTarget[index].GetTexture();
        }

        public List<ScoreEntry> EasyScores
        {
            get
            {
                return easyScores;
            }
        }

        public List<ScoreEntry> MediumScores
        {
            get
            {
                return mediumScores;
            }
        }

        public List<ScoreEntry> HardScores
        {
            get
            {
                return hardScores;
            }
        }

        public List<ScoreEntry> LOLScores
        {
            get
            {
                return lolScores;
            }
        }

        public List<ScoreEntry> WTFScores
        {
            get
            {
                return wtfScores;
            }
        }

        public List<GamerEntry> Gamers
        {
            get
            {
                return gamers;
            }
        }

        private List<ScoreEntry> AllocateList(int scoreIndex, TimeSpan durationIndex, Difficulty difficulty, ref int name, int step)
        {
            List<ScoreEntry> rtnList = GetList(difficulty);
            for (int i = 0; i < 10; i++)
            {
                ScoreEntry score = new ScoreEntry();
                score.GamerTag = Names[name];                
                score.Score = scoreIndex;
                score.Duration = durationIndex;
                score.Difficulty = difficulty;
                rtnList.Add(score);
                scoreIndex -= step;
                durationIndex -= TimeSpan.FromSeconds(10);
                name++;
            }
            return rtnList;
        }
    }
}