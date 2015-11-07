using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3dEngine
{
    class World
    {
        public List<GameObject> GameObjects;
        public AllAssets AllAssets { get; private set; }
        public DrawWrapper DrawWrapper { get; set; }
        public Color BackgroundColor = Color.White;

        public Camera Camera { get; private set; }
        public ModelEditor ModelEditor;

        public World()
        {
            GameObjects = new List<GameObject>();
            AllAssets = new AllAssets();
        }

        public virtual void Create()
        {
            Camera = new Camera() { Position = new Vector3(0, 0, -400) };
            AddObject(Camera);
            DrawWrapper.Camera = Camera;

            ModelEditor = new ModelEditor();
            AddObject(ModelEditor);

            for (int i = 0; i < GameObjects.Count(); i++)
            {
                var go = GameObjects[i];
                go.Create();
            }
        }

        public void AddObject(GameObject gameObject)
        {
            gameObject.World = this;
            gameObject.Drawing = DrawWrapper;

            GameObjects.Add(gameObject);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var gameObject in GameObjects)
                gameObject.Update(gameTime);
            foreach (var gameObject in GameObjects)
                gameObject.EndUpdate(gameTime);
        }
        
        public void Draw()
        {
            DrawWrapper.DrawClear(BackgroundColor);

            foreach (var gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.BeginDraw();
            foreach (var gameObject in GameObjects.OrderByDescending(x => x.Depth))
                gameObject.Draw();
        }
    }
}
