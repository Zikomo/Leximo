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


namespace Fluids
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FluidSolverComponent : Microsoft.Xna.Framework.GameComponent
    {
        // frame dimensions (dxd pixels)
        //public int d = 300;

        // solver variables
        public int n = 12;
        public float dt = .2f;
        FluidSolver fs = new FluidSolver();

        // flag to display velocity field
        

        // mouse position
        int x, xOld;
        int y, yOld;

        // cell index
        int i, j;

        // cell dimensions
        //int dg, dg_2;

        // cell position
        

        // fluid velocity
        

        
        


        public FluidSolverComponent(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
            reset();
        }

        public void Trigger(int x, int y, int density)
        {
            //MouseState mouse = Mouse.GetState();
            xOld = (int)(this.x );
            yOld = (int)(this.y );
            this.x = (int)(x);
            this.y = (int)(y);
            i = (int)((x / (float)Differences.X) * n + 1);
            j = (int)((y / (float)Differences.Y) * n + 1);

            // set boundries
            if (i > n) i = n;
            if (i < 1) i = 1;
            if (j > n) j = n;
            if (j < 1) j = 1;

            // add density or velocity
            
            fs.dOld[(i) + (n + 2) * (j)] = density;
            fs.uOld[(i) + (n + 2) * (j)] = (x - xOld) * 5;
            fs.vOld[(i) + (n + 2) * (j)] = (y - yOld) * 5;
        }

        public Vector2 Differences
        {
            get;
            set;
        }

        public Vector2 GetVelocity(int i, int j)
        {
            i = (int)MathHelper.Clamp(((float)i / Differences.X) * n, 0, n);
            j = (int)MathHelper.Clamp(((float)j / Differences.Y) * n, 0, n);
            Vector2 rtnVector = new Vector2(fs.u[(i) + (n + 2) * (j)], fs.v[(i) + (n + 2) * (j)]);
            return rtnVector;
        }

        public float GetDensity(int i, int j)
        {
            //i = (int)(i / Differences.X);
            //j = (int)(j / Differences.Y);
            return fs.d[(i) + (n + 2) * (j)];
        }

        public void reset()
        {
            // calculate cell deimensions
            //dg = d / n;
            //dg_2 = dg / 2;

            fs.setup(n, dt);
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            //UpdateMouse();
            fs.velocitySolver();
            fs.densitySolver();
            base.Update(gameTime);
        }
    }
}