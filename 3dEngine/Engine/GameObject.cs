using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3dEngine
{
    abstract class GameObject
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 PositionPrevious = Vector3.Zero;
        public Vector2 DrawingOffset = Vector2.Zero;
        public Vector3 Speed = Vector3.Zero;
        public float Friction = 0.75f;
        public float ZFriction = 1f;

        public float Depth = 0f;

        protected float WobblePlus;
        protected float WobbleRot;
        protected float WobbleFriction = 0.93f;
        protected float WobbleAmount = 0.1f;
        
        public World World;

        protected InputHelper Input;
        public DrawWrapper Drawing;
        public Mesh myMesh = null;
        public float[,] Transform = Matrix.GetIdentity();

        public virtual void Create()
        {
            Input = InputHelper.Instance;
        }

        public virtual void Update(GameTime gameTime)
        {
            PositionPrevious = Position;

            //resolve speeds
            Position += Speed;
            Speed = new Vector3(Speed.X * Friction, Speed.Y * Friction, Speed.Z * ZFriction);
        }

        public virtual void EndUpdate(GameTime gameTime)
        {
        }

        public virtual void BeginDraw()
        {

        }

        public virtual void Draw()
        {
            if (myMesh != null)
                myMesh.Draw(Drawing, Matrix.Multiply(Transform, World.Camera.ViewMatrix));
        }
    }
}
