using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Factories;

namespace IETGames.Shorewood
{
    public class GridScreenRenderer:GridRenderer 
    {
        Texture2D gridBackground;
        RenderTarget2D fullTarget;
        Body gridBody;
        public int width = 450;
        public int height = 590;

        public Texture2D fullGridTexture;
        
        public float gridMass = 10000;
        public GridScreenRenderer(Game game)
            :base(game)
        {
            base.internalPosition = new Vector2(58, 0);
        }


        

        protected override void LoadContent()
        {
            int width = Shorewood.titleSafeArea.Width;
            int height = Shorewood.titleSafeArea.Height;
            gridBackground = Shorewood.Content.Load<Texture2D>("screens\\baseGridBackground");
            gridBody = BodyFactory.Instance.CreateRectangleBody(Shorewood.physicsEngine, gridBackground.Width, gridBackground.Height, gridMass);
            width = height;
            gridTarget = new RenderTarget2D(GraphicsDevice, gridBackground.Width, gridBackground.Height, 1, gridBackground.Format);
            fullTarget = new RenderTarget2D(GraphicsDevice, gridBackground.Width, gridBackground.Height, 1, gridBackground.Format);
            rippleTarget = new RenderTarget2D(GraphicsDevice, gridBackground.Width, gridBackground.Height, 1, gridBackground.Format);

            base.LoadContent();
        }

        //protected override void OnEnabledChanged(object sender, EventArgs args)
        //{
        //    if (Enabled)
        //    {
        //        for (int y = 0; y < grid.Count; y++)
        //        {
        //            for (int x = 0; x < grid[y].Length; x++)
        //            {
        //                grid[y][x].startPosition += position;
        //                grid[y][x].startPosition *= scale;
        //            }
        //        }
        //    }
        //    base.OnEnabledChanged(sender, args);
        //}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public void DrawGridOntoBackground(GameTime gameTime)
        {
            //RenderTarget2D originalTarget = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
            //GraphicsDevice.SetRenderTarget(0, fullTarget);
            //GraphicsDevice.Clear(Color.TransparentBlack);
            Shorewood.spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            DrawBackground(gameTime);
            DrawGrid(gameTime);
            Shorewood.spriteBatch.End();
            //GraphicsDevice.SetRenderTarget(0, originalTarget);
            //fullGridTexture = gridTarget.GetTexture();
        }

        public void DrawGrid(GameTime gameTime)
        {            
            Shorewood.spriteBatch.Draw(gridTexture, position + new Vector2(0,30), Color.White);
        }

        public void DrawBackground(GameTime gameTime)
        {
            Shorewood.spriteBatch.Draw(gridBackground, position, Color.White);            
        }

        public void DrawToScreen(GameTime gameTime)
        {
            Shorewood.spriteBatch.Begin();
            Shorewood.spriteBatch.Draw(fullGridTexture, position, Color.White);
            Shorewood.spriteBatch.End();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


            DrawGridOntoBackground(gameTime);
        }
    }
}