using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using _3dEngine;

namespace _3dEngine
{
    class DrawWrapper
    {
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        public Point ScreenSize { get; private set; }
        private Color[,] screen;
        private float[,] zBuffer;
        public float ZFar = 10000;
        public float ZNear = 0;
        private Texture2D canvas;
        float pixelScale = 2;
        public Camera Camera;
        public bool EnableShading = true;

        public DrawWrapper(SpriteBatch batch, GraphicsDevice device)
        {
            graphicsDevice = device;
            spriteBatch = batch;
        }

        public void Create()
        {
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            ScreenSize = new Point((int)(graphicsDevice.Viewport.Width / pixelScale), (int)(graphicsDevice.Viewport.Height / pixelScale));
            screen = new Color[ScreenSize.X, ScreenSize.Y];
            zBuffer = new float[ScreenSize.X, ScreenSize.Y];
            DrawClear(Color.White);
            canvas = new Texture2D(graphicsDevice, ScreenSize.X, ScreenSize.Y);
        }

        public void DrawClear(Color color)
        {
            for (int xp = 0; xp < ScreenSize.X; xp++)
                for (int yp = 0; yp < ScreenSize.Y; yp++)
                {
                    zBuffer[xp,yp] = ZFar;
                    screen[xp, yp] = color;
                }
        }

        public void DrawScreen()
        {
            Color[] data = new Color[ScreenSize.X * ScreenSize.Y];
            for (int xp = 0; xp < ScreenSize.X; xp++)
                for (int yp = 0; yp < ScreenSize.Y; yp++)
                    data[yp * ScreenSize.X + xp] = screen[xp, yp];
            canvas.SetData(data);

            spriteBatch.Begin(SpriteSortMode.Immediate);
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            spriteBatch.Draw(canvas, Vector2.Zero, null, Color.White, 0, Vector2.Zero, pixelScale, SpriteEffects.None, 0 );
            spriteBatch.End();
        }

        public void DrawPoint(Vector3 v, Color color)
        {
            DrawPoint(v, color, Camera.ViewMatrix);
        }

        public void DrawPoint(Vector3 v, Color color, float[,] matrix)//, Color color2)
        {
            v = v.Transform(matrix);

            if (v.X > 0 && v.Y > 0 && v.X < ScreenSize.X && v.Y < ScreenSize.Y)
            {
                if (v.Z < zBuffer[(int)v.X, (int)v.Y] && v.Z > ZNear)
                {
                    screen[(int)v.X, (int)v.Y] = color;
                    zBuffer[(int)v.X, (int)v.Y] = v.Z;
                }
            }
        }

        public void DrawPoint(Vector3 point, float overrideZ, float size, Color color)
        {
            DrawPoint(point, overrideZ, size, color, Camera.ViewMatrix);
        }

        public void DrawPoint(Vector3 point, float size, Color color)
        {
            DrawPoint(point, 0, size, color, Camera.ViewMatrix);
        }
        public void DrawPoint(Vector3 point, float size, Color color, float[,] matrix)
        {
            DrawPoint(point, 0, size, color, matrix);
        }

        public void DrawPoint(Vector3 point, float overrideZ, float size, Color color, float[,] matrix)
        {
            point = point.Transform(matrix);
            Vector2 min = (point.ToVector2() - new Vector2(size)).ClampVector2(Vector2.Zero, ScreenSize.ToVector2());
            Vector2 max = (point.ToVector2() + new Vector2(size)).ClampVector2(Vector2.Zero, ScreenSize.ToVector2());
            if (overrideZ == 0)
                overrideZ = point.Z;
            for (int xp = (int)min.X; xp < max.X; xp++)
                for (int yp = (int)min.Y; yp < max.Y; yp++)
                    DrawPoint(new Vector3(xp ,yp, overrideZ), color, Matrix.GetIdentity());
        }

        public void DrawLine(Vector3 v1, Vector3 v2, float width, Color color)
        {
            DrawLine(v1, v2, width, color, Camera.ViewMatrix);
        }

        public void DrawLine(Vector3 v1, Vector3 v2, float width, Color color, float[,] matrix)
        {
            v1 = v1.Transform(matrix);
            v2 = v2.Transform(matrix);

            float angle = (v1.ToVector2() - v2.ToVector2()).Angle() + 90;
            Vector3 offset = new Vector2(width / 2, angle).ToCartesian().ToVector3();
            DrawTriangle(v1 + offset, v1 - offset, v2 + offset, color, Color.Transparent, Matrix.GetIdentity());
            DrawTriangle(v1 - offset, v2 - offset, v2 + offset, color, Color.Transparent, Matrix.GetIdentity());
        }

        public void DrawQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color frontFaceColor, Color backFaceColor)
        {
            DrawTriangle(v1, v2, v3, frontFaceColor, backFaceColor);
            DrawTriangle(v4, v3, v2, frontFaceColor, backFaceColor);
        }

        public void DrawQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color frontFaceColor, bool backFaceCulling = true)
        {
            Color backFaceColor = frontFaceColor;
            if (backFaceCulling)
                backFaceColor = Color.Transparent;
            DrawQuad(v1, v2, v3, v4, frontFaceColor, backFaceColor);
        }

        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color frontFaceColor)
        {
            DrawTriangle(v1, v2, v3, frontFaceColor, Color.Transparent, Camera.ViewMatrix);
        }

        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color frontFaceColor, bool backFaceCulling)
        {
            if (backFaceCulling)
                DrawTriangle(v1, v2, v3, frontFaceColor);
            else
                DrawTriangle(v1, v2, v3, frontFaceColor, frontFaceColor);
        }

        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color frontFaceColor, Color backFaceColor)
        {
            DrawTriangle(v1, v2, v3, frontFaceColor, backFaceColor, Camera.ViewMatrix);
        }

        public void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color frontFaceColor, Color backFaceColor, float[,] matrix)
        {
            v1 = v1.Transform(matrix);
            v2 = v2.Transform(matrix);
            v3 = v3.Transform(matrix);
            Vector3 normal = Vector3.Cross(v1 - v2, v1 - v3);
            normal.Normalize();

            //set color or return for culling
            Color color = frontFaceColor;
            if (normal.Z > 0)
                color = backFaceColor;
            if (color == Color.Transparent)
                return;
            if (EnableShading)
            {
                float shading = 0.5f + 0.5f * Math.Abs(normal.Z);
                color.R = (byte)(color.R * shading);
                color.G = (byte)(color.G * shading);
                color.B = (byte)(color.B * shading);
            }

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();
            min.X = Math.Min(v1.X, Math.Min(v2.X, v3.X));
            min.Y = Math.Min(v1.Y, Math.Min(v2.Y, v3.Y));
            max.X = Math.Max(v1.X, Math.Max(v2.X, v3.X));
            max.Y = Math.Max(v1.Y, Math.Max(v2.Y, v3.Y));

            min = min.ClampVector2(Vector2.Zero, ScreenSize.ToVector2());
            max = max.ClampVector2(Vector2.Zero, ScreenSize.ToVector2());

            Vector3[] verts = new Vector3[3] { v1, v2, v3 };

            for (int yp = (int)min.Y; yp < (int)max.Y; yp++)
            {
                List<Vector3> intersections = new List<Vector3>();

                //voor alle lijnen van de triangle, vind alle intersecties (als het goed is altijd 2)
                for (int l = 0; l < 3; l++)
                {
                    Vector3 vert1, vert2;
                    vert1 = verts[l];
                    vert2 = verts[(l + 1) % 3];

                    float ymin, ymax;
                    ymin = Math.Min(vert1.Y, vert2.Y);
                    ymax = Math.Max(vert1.Y, vert2.Y);
                    if (yp > ymin && yp < ymax)//check of yp tussen de y-en van de vertices ligt, zo ja, intersectie
                    {
                        float xSlope = (vert2.X - vert1.X) / (vert2.Y - vert1.Y);
                        float xIntersect = vert1.X + xSlope * (yp - vert1.Y);

                        float zSlope = (vert2.Z - vert1.Z) / (vert2.Y - vert1.Y);
                        float zIntersect = vert1.Z + zSlope * (yp - vert1.Y);
                        intersections.Add(new Vector3((int)xIntersect, yp, zIntersect));
                    }
                }

                //vul scanline
                if (intersections.Count > 1)
                {
                    Vector3 pStart, pEnd;
                    if (intersections[0].X < intersections[1].X) { pStart = intersections[0]; pEnd = intersections[1]; }
                    else
                    { pStart = intersections[1]; pEnd = intersections[0]; }

                    //vul de rij met pixels
                    for (int xp = MathHelper.Clamp((int)pStart.X, 0, ScreenSize.X); xp < MathHelper.Clamp(pEnd.X, 0, ScreenSize.X); xp++)
                    {
                        float zSlope = (pEnd.Z - pStart.Z) / (pEnd.X - pStart.X);
                        float zCurrent = pStart.Z + zSlope * (xp - pStart.X);

                        //DrawPoint(new Vector3(xp, yp, zCurrent), color, Matrix.GetIdentity());
                        if (zCurrent < zBuffer[xp, yp] && zCurrent > ZNear)
                        {
                            screen[xp, yp] = color;
                            zBuffer[xp, yp] = zCurrent;
                        }
                    }
                }
            }
        }
    }
}
