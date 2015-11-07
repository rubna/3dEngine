using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace _3dEngine
{
    class Triangle
    {
        public Color FrontFaceColor, BackFaceColor;
        public int v0, v1, v2;

        public Vector3 GetNormal(List<Vector3> vertices, float[,] matrix)
        {
            Vector3 p0 = vertices[v0].Transform(matrix);
            Vector3 p1 = vertices[v1].Transform(matrix);
            Vector3 p2 = vertices[v2].Transform(matrix);
            Vector3 normal = Vector3.Cross(p0 - p1, p0 - p2);
            normal.Normalize();
            return normal;
        }

        public Triangle(int v0, int v1, int v2, Color FrontFaceColor, Color BackFaceColor)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.FrontFaceColor = FrontFaceColor;
            this.BackFaceColor = BackFaceColor;
        }
    }
}
