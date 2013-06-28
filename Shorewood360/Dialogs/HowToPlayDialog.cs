using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using IETGames.Shorewood.Input;

namespace IETGames.Shorewood
{
    class HowToPlayDialog:MultiPageDialog 
    {
        public HowToPlayDialog()
            :base(Shorewood.localization[Shorewood.language][IETGames.Shorewood.Localization.GameStrings.MenuHighScores],
            Shorewood.Content.Load<Texture2D>(@"screens\menus\highScoresBackground"), 
            new Microsoft.Xna.Framework.Vector2(600,400))           
        {
            isSinglePage = true;
            pageCount = 5;
            overlay = Shorewood.Content.Load<Texture2D>(@"screens\menus\highScoresOverlay");
            singlePage = Shorewood.Content.Load<Texture2D>(@"screens\menus\lrn2ply");
            parentDialogType = DialogType.Difficulty;
            resetOnDeactivate = true;
        }

        public override void OnB(object sender, ButtonFireEventArgs args)
        {
            TransitionTo(parentDialogType,args.gameTime);            
        }
    }
}