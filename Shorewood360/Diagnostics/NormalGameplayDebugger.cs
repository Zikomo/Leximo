using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IETGames.Shorewood;
namespace IETGames.Shorewood.Diagnostics
{
    public class NormalGameplayDebugger : NormalGameplay
    {
        int columnIndex = 0;
        public NormalGameplayDebugger(Game game, PlayerIndex player, Dictionary<char,Letter> alphabet,GridRenderer renderer,ILetterSet letterSet )
            :base(game, player, alphabet, renderer, letterSet)
        {
        }

        protected override void ChangeCurrentLetterToRandom(GameTime now)
        {

            Letter letter = new Letter();
            letter = nextLetter;
            letter.gridPosition.X = (float)(columnIndex % (int)gridRenderer.grid.width);

            letter.gridPosition.Y = 0;
            gridRenderer.grid.PositionLetter((int)letter.gridPosition.X, (int)letter.gridPosition.Y, letter);

            currentLetter = letter;
            gridRenderer.currentLetter = currentLetter;

            if (!CheckMoveDown(now))
            {
                isGameOver = true;
                Shorewood.gameState = GameState.GameOver;
            }
            if (now != null)
            {
                lastLetterUpdate = (float)now.ElapsedGameTime.TotalMilliseconds;
            }
            //gridRenderer.scoreString = "Score: " + gridRenderer.grid.currentScore.ToString();
            //gridRenderer.levelString = "Level: " + (levelManager.currentLevel).ToString();
            nextLetter = letterSet.GetRandomLetter();
            gridRenderer.nextLetter = nextLetter;
            gameplayState = NormalGameplayState.CurrentLetterActive;
            columnIndex++;
        }

        private void ValidateGrid()
        {
            int something = 0;
            for (int x = 0; x < gridRenderer.grid.width; x++)
            {
                for (int y = 1; y < gridRenderer.grid.height; y++)
                {
                    Letter upper = gridRenderer.grid.GetLetter(x, y-1);
                    Letter lower = gridRenderer.grid.GetLetter(x, y);
                    if ((upper != Letter.EmptyLetter)&&(lower == Letter.EmptyLetter)&&(upper.gridPosition != currentLetter.gridPosition))
                    {
                        something++;//System.Diagnostics.Debugger.Break();
                    }
                }
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);            
            ValidateGrid();
        }
    }
}