using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3dEngine
{
    class Camera : GameObject
    {
        public Vector3 Target = Vector3.Zero;// new Vector3(-50, -50, 0);// * -50;
        public float xyOrbitRot = 0;
        public float zOrbitRot = 0;
        public float distance = 200;
        public float[,] ViewMatrix = Matrix.GetIdentity();
        public float Scale = 1;
        public override void Create()
        {
            base.Create();
            ViewMatrix = ViewMatrix.Translate(-Position + Drawing.ScreenSize.ToVector2().ToVector3() / 2);
            //ViewMatrix = ViewMatrix.Rotate(0, 0, zOrbitRot);
        }

        public override void Update(GameTime gameTime)
        {
            float dZRot = 0;
            float dXYRot = 0;
            if (Input.MouseCheckDown(MouseButton.RightButton))
            {
                dXYRot += Input.MouseSpeed().X / 4;
                dZRot += Input.MouseSpeed().Y / 4;
            }

            float dScale = 0.001f * Input.MouseScrollDelta();
            Scale *= 1 + dScale;
            ViewMatrix = ViewMatrix.Scale(1 + dScale);

            xyOrbitRot += dXYRot;
            zOrbitRot += dZRot;
            ViewMatrix = ViewMatrix.Rotate(VectorExtensions.LengthDirectionY(dZRot, xyOrbitRot), VectorExtensions.LengthDirectionX(dZRot, xyOrbitRot), dXYRot);
            
            //Console.WriteLine(hOrbitRot + ", " + vOrbitRot);
            Position = new Vector3(distance, xyOrbitRot, zOrbitRot).ToCartesian();
            //Position.Z = 50;
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
            //Drawing.DrawTriangle(Vector3.Zero, Vector3.UnitX * 100, Vector3.UnitY * 100, Color.Red, Color.Red);
            //Drawing.DrawTriangle(Vector3.Zero, Vector3.UnitY * 100, Vector3.UnitZ * 100, Color.Blue, Color.Blue);
            //Drawing.DrawTriangle(Vector3.Zero, Vector3.UnitZ * 100, Vector3.UnitX * 100, Color.Green, Color.Green);

            Drawing.DrawLine(Vector3.Zero, Vector3.UnitX * 1000, 3, Color.Red);
            Drawing.DrawLine(Vector3.Zero, Vector3.UnitY * 1000, 3, Color.Green);
            Drawing.DrawLine(Vector3.Zero, Vector3.UnitZ * 1000, 3, Color.Blue);

            Drawing.DrawPoint(Position, Color.Black, Matrix.Translate(Matrix.GetIdentity(), Drawing.ScreenSize.ToVector2().ToVector3() * 0.25f));
            Drawing.DrawPoint(Target, Color.Black, Matrix.Translate(Matrix.GetIdentity(), Drawing.ScreenSize.ToVector2().ToVector3() * 0.25f));

           // World.AllAssets.GetModelWithName("test").Draw(ViewMatrix, Drawing);
            //World.AllAssets.GetModelWithName("test").Draw(Matrix.GetIdentity(), Drawing);
        }
    }
}
