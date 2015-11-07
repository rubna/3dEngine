using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3dEngine
{
    static class VectorExtensions
    {
        public static Vector2 ClampVector2(this Vector2 vector, Vector2 min, Vector2 max)
        {
            if (vector.X < min.X) { vector.X = min.X; }
            if (vector.X > max.X) { vector.X = max.X; }
            if (vector.Y < min.Y) { vector.Y = min.Y; }
            if (vector.Y > max.Y) { vector.Y = max.Y; }

            return vector;
        }

        public static float AngleDifference(float source, float destination)
        {
            double a;
            a = MathHelper.ToRadians(destination) - MathHelper.ToRadians(source);
            a = Math.Atan2(Math.Sin(a), Math.Cos(a));
            return MathHelper.ToDegrees((float)a);
        }

        public static float Angle(this Vector2 vector)
        {
            return MathHelper.ToDegrees((float)Math.Atan2(vector.Y, vector.X));
        }

        public static float LengthDirectionX(float length, float direction)
        {
            return (float)Math.Cos(MathHelper.ToRadians(direction)) * length;
        }
        public static float LengthDirectionY(float length, float direction)
        {
            return (float)Math.Sin(MathHelper.ToRadians(direction)) * length;
        }

        public static Vector2 ToCartesian(float length, float direction)
        {
            var radDir = MathHelper.ToRadians(direction);
            return new Vector2((float)Math.Cos(radDir) * length, (float)Math.Sin(radDir) * length);
        }

        public static Vector2 ToCartesian(this Vector2 vector)
        {
            return ToCartesian(vector.X, vector.Y);
        }

        public static Vector2 ToPolar(this Vector2 vector)
        {
            return new Vector2(vector.Length(), vector.Angle());
        }

        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector, 0f);
        }
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static Vector3 ToCartesian(this Vector3 vector)
        {
            float xyLength = LengthDirectionX(vector.X, vector.Z);
            return new Vector3(new Vector2(xyLength, vector.Y).ToCartesian(), LengthDirectionY(vector.X, vector.Z));
        }

        public static Vector3 ToPolar(this Vector3 vector)
        {
            return new Vector3(vector.Length(), vector.ToVector2().Angle(), new Vector2(vector.Length(), vector.Z).Angle());
        }

        public static Vector3 Transform(this Vector3 vector, float[,] matrix)
        {
            float x, y, z;
            x = vector.X * matrix[0, 0] + vector.Y * matrix[1, 0] + vector.Z * matrix[2, 0] + matrix[3, 0];
            y = vector.X * matrix[0, 1] + vector.Y * matrix[1, 1] + vector.Z * matrix[2, 1] + matrix[3, 1];
            z = vector.X * matrix[0, 2] + vector.Y * matrix[1, 2] + vector.Z * matrix[2, 2] + matrix[3, 2];
            return new Vector3(x, y, z);
        }

        public static bool PointInsideTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            bool b1, b2, b3;
            b1 = Side(pt, v1, v2) < 0;
            b2 = Side(pt, v2, v3) < 0;
            b3 = Side(pt, v3, v1) < 0;

            return ((b1 == b2) && (b2 == b3));
        }

        public static float Side(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        /*public static Vector3 PointOnLineClosestToLine(Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            float at, bt;
            Vector3 pa = a1 + at * (a2 - a1);
            Vector3 pb = b1 + bt * (b2 - b1);
            float distSquared = (pa.X - pb.X)^2 + (pa.Y - pb.Y)^2 + (pa.Z - pb.Z)^2;
            distSquared = ((a1.X + at * (a2.X -a1.X)) - (b1.X + bt * (b2.X - b1.X))) ^2 + (pa.Y - pb.Y)^2 + (pa.Z - pb.Z)^2;
            float deriv = 2*(-1 + t)*(b + a*(-1 + t) - s t - b y + n y)
        }*/

        /*
           Calculate the line segment PaPb that is the shortest route between
           two lines P1P2 and P3P4. Calculate also the values of mua and mub where
              Pa = P1 + mua (P2 - P1)
              Pb = P3 + mub (P4 - P3)
           Return FALSE if no solution exists.
        */
        public static Vector3 LineLineIntersect(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            float d1343, d4321, d1321, d4343, d2121;
            float numer, denom;
            d1343 = Vector3.Dot(p1 - p3, p4 - p3);
            d4321 = Vector3.Dot(p4 - p3, p2 - p1);
            d1321 = Vector3.Dot(p1 - p3, p2 - p1);
            d4343 = Vector3.Dot(p4 - p3, p4 - p3);
            d2121 = Vector3.Dot(p2 - p1, p2 - p1);

            denom = d2121 * d4343 - d4321 * d4321;
            //if (ABS(denom) < EPS)
            //    return (FALSE);
            numer = d1343 * d4321 - d1321 * d4343;

            float mua = numer / denom;
            //*mub = (d1343 + d4321 * (*mua)) / d4343;

            /*pa->x = p1.x + *mua * p21.x;
            pa->y = p1.y + *mua * p21.y;
            pa->z = p1.z + *mua * p21.z;
            pb->x = p3.x + *mub * p43.x;
            pb->y = p3.y + *mub * p43.y;
            pb->z = p3.z + *mub * p43.z;*/

            return (p1 + mua * (p2 - p1));
        }
    }
}
