using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace IETGames.Shorewood
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 
    public enum DialogType {MainMenuDialog, NewChallengeDialog, GameOverDialog, Pause, Tutorial ,None, HighScores, Difficulty };
    public class DialogManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public DialogType activeDialog = DialogType.None;
        Dictionary<DialogType,Dialog> dialogs = new Dictionary<DialogType,Dialog>(20);
        public DialogManager(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            dialogs.Add(DialogType.MainMenuDialog, new MainMenuDialog());
            dialogs.Add(DialogType.NewChallengeDialog, new NewGoalDialog());
            dialogs.Add(DialogType.GameOverDialog, new GameOverDialog());
            dialogs.Add(DialogType.Pause, new PauseMenu());
            dialogs.Add(DialogType.Tutorial, new HowToPlayDialog());
            dialogs.Add(DialogType.HighScores, new HighScoresDialog());
            dialogs.Add(DialogType.Difficulty, new DifficultyDialog());            
            dialogs.Add(DialogType.None, null);

            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (activeDialog != DialogType.None)
            {
                if (dialogs[activeDialog].IsActive)
                {
                    dialogs[activeDialog].Update(gameTime);
                    UpdateRotationMatrix(ref dialogs[activeDialog].center, dialogs[activeDialog].currentScale * 2 * (float)MathHelper.Pi);
                    if (dialogs[activeDialog].IsActive)
                    {
                        base.Update(gameTime);
                        return;
                    }                    
                }
            }
            
            Visible = false;
            Enabled = false;
            activeDialog = DialogType.None;
            base.Update(gameTime);
        }

        public Dialog GetDialog(DialogType dialogType)
        {
            return dialogs[dialogType];
        }

        public Dialog ActiveDialog
        {
            get
            {
                return dialogs[activeDialog];
            }
        }

        public DialogType ActiveDialogType
        {
            get
            {
                return activeDialog;
            }
        }

        public void ShowDialog(DialogType dialogType, GameTime gameTime)
        {
            activeDialog = dialogType;
            dialogs[activeDialog].Activate(gameTime);
            Visible = true;
            Enabled = true;
            if (dialogs[activeDialog].blur)
            {
                Shorewood.bloom.Enabled = true;
                Shorewood.bloom.Visible = true;
            }
            Shorewood.whooshin.Play();
        }

        public void CloseDialog(GameTime gameTime)
        {
            if ((activeDialog != DialogType.None)&&(dialogs[activeDialog].state == DialogState.Active))
            {
                dialogs[activeDialog].Deactivate(gameTime);
                Shorewood.bloom.Enabled = false;
                Shorewood.bloom.Visible = false;
                Shorewood.whooshout.Play();
            }
        }

        private Matrix rotationMatrix = Matrix.Identity;
        private Matrix scalingMatrix = Matrix.Identity;
        private Matrix transformationMatrix = Matrix.Identity;
        private void UpdateRotationMatrix(ref Vector2 origin, float radians)
        {
            // Translate sprites to center around screen (0,0), rotate them, and
            // translate them back to their original positions
            Vector3 matrixorigin = new Vector3(origin, 0);
            rotationMatrix = Matrix.CreateRotationZ(radians);
                
            scalingMatrix = Matrix.CreateScale(dialogs[activeDialog].currentScale);
            transformationMatrix = Matrix.CreateTranslation(-matrixorigin) * rotationMatrix * scalingMatrix * Matrix.CreateTranslation(matrixorigin);
        }

        public override void Draw(GameTime gameTime)
        {
            if (activeDialog != DialogType.None)
            {
                Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, transformationMatrix);
                dialogs[activeDialog].Draw(Shorewood.spriteBatch, gameTime);
                Shorewood.spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public bool IsActive
        {
            get
            {
                return activeDialog != DialogType.None;
            }
        }
    }
}