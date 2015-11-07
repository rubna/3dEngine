using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace _3dEngine
{
    class Mesh
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<Triangle> triangles = new List<Triangle>();

        public virtual void Draw(DrawWrapper Drawing, float[,] transform)
        {
            foreach (Triangle tri in triangles)
                Drawing.DrawTriangle(vertices[tri.v0], vertices[tri.v1], vertices[tri.v2],  tri.FrontFaceColor, tri.BackFaceColor, transform);
        }

        public virtual void DrawDebug(DrawWrapper Drawing, float[,] transform)
        {
            foreach (Triangle tri in triangles)
            {
                Drawing.DrawLine(vertices[tri.v0], vertices[tri.v1], 3, Color.DarkGray);
                Drawing.DrawLine(vertices[tri.v0], vertices[tri.v2], 3, Color.DarkGray);
                Drawing.DrawLine(vertices[tri.v1], vertices[tri.v2], 3, Color.DarkGray);
            }
            foreach (Vector3 p in vertices)
            {
                Drawing.DrawPoint(p, 2, Color.Black);
            }
        }

        public void MakeCube(Color frontFaceColor, Color backFaceColor)
        {
            List<Vector3> verts = new List<Vector3>()
            {
                new Vector3(-10, -10, -10),
                new Vector3(-10, 10, -10),
                new Vector3(-10, -10, 10),
                new Vector3(-10, 10, 10),
                new Vector3(10, -10, -10),
                new Vector3(10, 10, -10),
                new Vector3(10, -10, 10),
                new Vector3(10, 10, 10)
            };
            vertices.AddRange(verts);
            List<Triangle> tris = new List<Triangle>()
            {
                //bottom
                /*new Triangle(0, 1, 5, frontFaceColor, backFaceColor),
                new Triangle(5, 4, 0, frontFaceColor, backFaceColor),
                
                //sides
                new Triangle(0, 2, 1, frontFaceColor, backFaceColor),
                new Triangle(1, 2, 3, frontFaceColor, backFaceColor),

                new Triangle(5, 6, 4, frontFaceColor, backFaceColor),
                new Triangle(7, 6, 5, frontFaceColor, backFaceColor),

                new Triangle(0, 4, 2, frontFaceColor, backFaceColor),
                new Triangle(4, 6, 2, frontFaceColor, backFaceColor),

                new Triangle(5, 1, 3, frontFaceColor, backFaceColor),
                new Triangle(7, 5, 3, frontFaceColor, backFaceColor),

                //top
                new Triangle(3, 2, 7, frontFaceColor, backFaceColor),
                new Triangle(7, 2, 6, frontFaceColor, backFaceColor),*/
            };
            triangles.AddRange(tris);
        }
    }
}
