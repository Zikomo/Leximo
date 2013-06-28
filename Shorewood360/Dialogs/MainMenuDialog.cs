using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IETGames.Shorewood
{
    public class MainMenuDialog:Dialog
    {
        private Vector2 originalCenter;
        

        public MainMenuDialog()
            : base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MainMenuDialogTitle],
            @"screens\menus\baseMainDialog",
            @"screens\menus\baseMainDialogHighlight")
        {
            AddOption(new StringBuilder(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuPlay].ToString()));
            AddOption(new StringBuilder(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuHighScores].ToString()));
            AddOption(new StringBuilder(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuCredits].ToString()));
            AddOption(new StringBuilder(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuSwitch].ToString()));
            AddOption(new StringBuilder(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuQuit].ToString()));
            originalCenter = center;
            center = Vector2.Zero;
            titleColor = Color.Black;            
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch (state)
            {
                case DialogState.FadingIn:
                    center = Vector2.SmoothStep(Vector2.Zero, originalCenter, ((float)gameTime.TotalGameTime.TotalMilliseconds - fadeStartTime) / fadeDuration);    
                    break;
                case DialogState.FadingOut:
                    center = Vector2.SmoothStep(originalCenter, Vector2.Zero, ((float)gameTime.TotalGameTime.TotalMilliseconds - fadeStartTime) / fadeDuration);
                    break;
                case DialogState.Active:
                    break;
                case DialogState.Inactive:
                    break;
            }
            base.Update(gameTime);
        }
    }
}
