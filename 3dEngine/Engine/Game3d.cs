using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3dEngine
{
    class Game3d : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private DrawWrapper drawWrapper;
        private World world;

        public Game3d()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Window.Title = "let's make the wind waker";
        }

        protected override void Initialize()
        {
            world = new World();
            //graphics.ToggleFullScreen();
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;

            graphics.ApplyChanges();

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(1000f / 60f);

            //IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            drawWrapper = new DrawWrapper(spriteBatch, GraphicsDevice);
            world.AllAssets.LoadModel("test", Content);

            world.DrawWrapper = drawWrapper;
            drawWrapper.Create();
            world.Create();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            var inputHelper = InputHelper.Instance;
            inputHelper.Update();

            if (inputHelper.KeyboardCheckPressed(Keys.Escape))
                Exit();

            world.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            world.Draw();

            drawWrapper.DrawScreen();

            base.Draw(gameTime);
        }
    }
}
