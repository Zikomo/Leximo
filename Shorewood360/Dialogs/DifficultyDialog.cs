using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IETGames.Shorewood
{
    public class DifficultyDialog:MultiPageDialog 
    {
        Vector2 tutorialPosition = Vector2.Zero;
        Vector2 tutorialOrigin = Vector2.Zero;
        Texture2D textBackgroundTexture;
        Texture2D controller;
        public DifficultyDialog()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.Difficulties], null, new Vector2(500, 100))
        {
            isSinglePage = true;
            singlePage = Shorewood.Content.Load<Texture2D>(@"screens\menus\difficulties");
            controller = Shorewood.Content.Load<Texture2D>(@"screens\menus\controller");
            pageCount = 5;
            tutorialOrigin = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuTutorial]) / 2;
            tutorialPosition = new Vector2(bounds.Center.X, bounds.Top - 20);
            textBackgroundTexture = Shorewood.Content.Load<Texture2D>("screens\\textBackground");
            MoveRight(new GameTime());
        }

        protected override void OnSelect(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs args)
        {
            if (!args.previouslyDown)
            {
                Shorewood.dialogManager.CloseDialog(args.gameTime);
                Shorewood.Difficulty = (Difficulty)MathHelper.Clamp(index, 0, 4);
                Shorewood.normalModeGamePlay.Start();
                Shorewood.gameState = GameState.PlayingNormalGameplay;                
            }
            base.OnSelect(sender, args);
        }

        protected override void MoveRight(GameTime gameTimeStart)
        {
            if (index == 4)
            {
                Shorewood.Is1337Compliant = true;
            }
            base.MoveRight(gameTimeStart);
        }

        protected override void MoveLeft(GameTime gameTimeStart)
        {
            Shorewood.Is1337Compliant = false;
            base.MoveLeft(gameTimeStart);
        }

        public override void OnB(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs args)
        {
            if (!args.previouslyDown)
            {
                Shorewood.dialogManager.CloseDialog(args.gameTime);
                Shorewood.normalModeGamePlay.Hide();
                Shorewood.gameState = GameState.Menu;
                Shorewood.Is1337Compliant = false;
            }
            base.OnB(sender, args);
        }

        public override void OnX(object sender, IETGames.Shorewood.Input.ButtonFireEventArgs a)
        {
            if (!a.previouslyDown)
            {
                TransitionTo(DialogType.Tutorial, a.gameTime);
                Shorewood.Is1337Compliant = false;
            }
            base.OnX(sender, a);
        }
        protected override void OnActivated(GameTime gameTime)
        {
            if (index == 4)
            {
                Shorewood.Is1337Compliant = true;
            }
            base.OnActivated(gameTime);
        }
        public override void CustomDraw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 stringBounds = Shorewood.fonts[FontTypes.MenuButtonFont].MeasureString(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuTutorial]);
            Vector2 txBounds = new Vector2(textBackgroundTexture.Width, textBackgroundTexture.Height);
            spriteBatch.Draw(textBackgroundTexture, new Rectangle((int)tutorialPosition.X, (int)tutorialPosition.Y, (int)stringBounds.X, (int)stringBounds.Y), null, Color.White, 0, txBounds / 2, SpriteEffects.None, 0);
            spriteBatch.DrawString(Shorewood.fonts[FontTypes.MenuButtonFont],
                Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuTutorial], 
                tutorialPosition, Color.White, 0f, tutorialOrigin, 1, SpriteEffects.None, 1);
            spriteBatch.Draw(controller, new Vector2(bounds.Right, center.Y - controller.Height / 2 ) , Color.White);

            base.CustomDraw(spriteBatch, gameTime);
        }
    }
}