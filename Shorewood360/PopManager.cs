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

    public enum PopUpType { Confirmation, None, ControllerDisconnected, Purchase, CannotPurchase};
    public class PopUpManager : DrawableGameComponent 
    {
        public PopUpType activePopUp = PopUpType.None;
        Dictionary<PopUpType, PopUpDialog> popUps = new Dictionary<PopUpType, PopUpDialog>(10);
        public PopUpManager(Game game)
            :base(game)
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
            popUps.Add(PopUpType.Confirmation, new ConfirmationPopUp());
            popUps.Add(PopUpType.ControllerDisconnected, new ControllerDisconnectedPopUp());
            popUps.Add(PopUpType.Purchase, new MarketPlaceConfirmation());
            popUps.Add(PopUpType.CannotPurchase, new CannontPurchaseWarning());
            popUps.Add(PopUpType.None, null);            
            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (activePopUp != PopUpType.None)
            {
                if (popUps[activePopUp].IsActive)
                {
                    popUps[activePopUp].Update(gameTime);
                    UpdateRotationMatrix(ref popUps[activePopUp].center, popUps[activePopUp].currentScale * 2 * (float)MathHelper.Pi);
                    if (popUps[activePopUp].IsActive)
                    {
                        base.Update(gameTime);
                        return;
                    }
                }
            }

            Visible = false;
            Enabled = false;
            activePopUp = PopUpType.None;
            base.Update(gameTime);
        }

        public Dialog GetPopUp(PopUpType dialogType)
        {
            return popUps[dialogType];
        }

        public Dialog ActiveDialog
        {
            get
            {
                return popUps[activePopUp];
            }
        }

        public PopUpType ActivePopUpType
        {
            get
            {
                return activePopUp;
            }
        }

        public void ShowDialog(PopUpType dialogType, GameTime gameTime, PopUpCallback popUpCallback)
        {
            if (!IsActive)
            {
                activePopUp = dialogType;
                popUps[activePopUp].Activate(gameTime);
                popUps[activePopUp].PopUpCallback = popUpCallback;
                Visible = true;
                Enabled = true;
                Shorewood.pop.Play();
                if (popUps[activePopUp].blur)
                {
                    Shorewood.bloom.Enabled = true;
                    Shorewood.bloom.Visible = true;
                }
            }
        }

        public void CloseDialog(GameTime gameTime)
        {
            if ((activePopUp != PopUpType.None) && (popUps[activePopUp].state == DialogState.Active))
            {
                popUps[activePopUp].Deactivate(gameTime);
                Shorewood.bloom.Enabled = false;
                Shorewood.bloom.Visible = false;
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

            scalingMatrix = Matrix.CreateScale(popUps[activePopUp].currentScale);
            transformationMatrix = Matrix.CreateTranslation(-matrixorigin) * rotationMatrix * scalingMatrix * Matrix.CreateTranslation(matrixorigin);
        }

        public override void Draw(GameTime gameTime)
        {
            if (activePopUp != PopUpType.None)
            {
                Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, transformationMatrix);
                popUps[activePopUp].Draw(Shorewood.spriteBatch, gameTime);
                Shorewood.spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public bool IsActive
        {
            get
            {
                return activePopUp != PopUpType.None;
            }
        }

    }
}