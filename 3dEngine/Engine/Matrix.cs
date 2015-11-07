using System;
using Microsoft.Xna.Framework;

namespace _3dEngine
{
    static class Matrix
    {
        public static float[,] GetIdentity(int w = 4, int h = 4)
        {
            float[,] matrix = new float[w, h];
            for (int xp = 0; xp < w; xp++)
                for (int yp = 0; yp < h; yp++)
                {
                    if (xp == yp)
                        matrix[xp, yp] = 1;
                    else
                        matrix[xp, yp] = 0;
                }
            return matrix;
        }

        public static float[,] CreateLookAt(Vector3 from, Vector3 to, Vector3 up)
        {
            Vector3 zAxis = (from - to);
            zAxis.Normalize();
            Vector3 xAxis = Vector3.Cross(up, zAxis);
            xAxis.Normalize();
            Vector3 yAxis = Vector3.Cross(xAxis, zAxis);

            // Create a 4x4 orientation matrix from the right, up, and forward vectors
            // This is transposed which is equivalent to performing an inverse 
            // if the matrix is orthonormalized (in this case, it is).
            float[,] orientation = {
               { xAxis.X, yAxis.X, zAxis.X, 0 },
               { xAxis.Y, yAxis.Y, zAxis.Y, 0 },
               { xAxis.Z, yAxis.Z, zAxis.Z, 0 },
               {   0,       0,       0,     1 }
            };
            /*float[,] orientation = {
               { xAxis.X, xAxis.Y, xAxis.Z, 0 },
               { yAxis.X, yAxis.Y, yAxis.Z, 0 },
               { zAxis.X, zAxis.Y, zAxis.Z, 0 },
               {   0,       0,       0,     1 }
            };*/

            // Create a 4x4 translation matrix.
            // The eye position is negated which is equivalent
            // to the inverse of the translation matrix. 
            // T(v)^-1 == T(-v)
            float[,] translation = GetIdentity().Translate(-from);

            // Combine the orientation and translation to compute 
            // the final view matrix
            return Multiply(orientation, translation);

            /*float[,] matrix = GetIdentity();
            Vector3 delta = to - from;
            Vector3 polar = delta.ToPolar();
            matrix = Translate(matrix, -to);
            matrix = Translate(matrix, offset.ToVector3());
            matrix = Rotate(matrix, VectorExtensions.LengthDirectionX(polar.Y, polar.Z), VectorExtensions.LengthDirectionY(polar.Y, polar.Z), polar.Z);
            return matrix;*/
        }

        public static float[,] Transpose(this float[,] originalMatrix)
        {
            float[,] matrix = new float[originalMatrix.GetLength(1), originalMatrix.GetLength(0)];
            for (int xp = 0; xp < originalMatrix.GetLength(1); xp++)
                for (int yp = 0; yp < originalMatrix.GetLength(0); yp++)
                    matrix[xp, yp] = originalMatrix[yp, xp];
            return matrix;
        }

        public static float[,] Multiply(this float[,] mat1, float[,] mat2)
        {
            int width = mat2.GetLength(0);
            int height = mat1.GetLength(1);
            float[,] newMatrix = new float[width, height];

            if (width != height) { Console.WriteLine("Illegal matrix multiplication!!"); return Matrix.GetIdentity(width, height); }

            for (int i = 0; i < width; i += 1)
            {
                for (int j = 0; j < height; j += 1)
                {
                    newMatrix[i, j] = 0;
                    for (int x = 0; x < height; x += 1)
                    {
                        newMatrix[i, j] += mat1[x, j] * mat2[i, x];
                    }
                }
            }
            return newMatrix;
        }

        public static float[,] Scale(this float[,] source, float scale)
        {
            float[,] scaleMatrix = GetIdentity();
            scaleMatrix[0, 0] = scale;
            scaleMatrix[1, 1] = scale;
            scaleMatrix[2, 2] = scale;

            return Multiply(source, scaleMatrix);
        }

        public static float[,] Scale(this float[,] source, float xscale, float yscale, float zscale)
        {
            float[,] scaleMatrix = GetIdentity();
            scaleMatrix[0, 0] = xscale;
            scaleMatrix[1, 1] = yscale;
            scaleMatrix[2, 2] = zscale;

            return Multiply(source, scaleMatrix);
        }

        public static float[,] Translate(this float[,] source, float dx, float dy, float dz)
        {
            return Translate(source, new Vector3(dx, dy, dz));
        }

        public static float[,] Translate(this float[,] source, Vector3 vector)
        {
            float[,] translateMatrix = GetIdentity();
            translateMatrix[3, 0] = vector.X;
            translateMatrix[3, 1] = vector.Y;
            translateMatrix[3, 2] = vector.Z;

            return Multiply(source, translateMatrix);
        }

        public static float[,] TranslateTo(float[,] source, float x, float y, float z)
        {
            source[3, 0] = x;
            source[3, 1] = y;
            source[3, 2] = z;

            return source;
        }

        public static float[,] Rotate(this float[,] source, float xrot, float yrot, float zrot)
        {
            float[,] rotationMatrix;
            //vlak xy (zaxis)
            rotationMatrix = GetIdentity();

            rotationMatrix[0, 0] = (float)Math.Cos(MathHelper.ToRadians(zrot)); rotationMatrix[1, 0] = (float)-Math.Sin(MathHelper.ToRadians(zrot));
            rotationMatrix[0, 1] = (float)Math.Sin(MathHelper.ToRadians(zrot)); rotationMatrix[1, 1] = (float)Math.Cos(MathHelper.ToRadians(zrot));

            source = Multiply(source, rotationMatrix);

            //vlak xz (yaxis)
            rotationMatrix = GetIdentity();

            rotationMatrix[1, 1] = (float)Math.Cos(MathHelper.ToRadians(yrot)); rotationMatrix[2, 1] = (float)-Math.Sin(MathHelper.ToRadians(yrot));
            rotationMatrix[1, 2] = (float)Math.Sin(MathHelper.ToRadians(yrot)); rotationMatrix[2, 2] = (float)Math.Cos(MathHelper.ToRadians(yrot));

            source = Multiply(source, rotationMatrix);

            //vlak yz (xaxis)
            rotationMatrix = GetIdentity();

            rotationMatrix[2, 2] = (float)Math.Cos(MathHelper.ToRadians(xrot)); rotationMatrix[2, 0] = (float)-Math.Sin(MathHelper.ToRadians(xrot));
            rotationMatrix[0, 2] = (float)Math.Sin(MathHelper.ToRadians(xrot)); rotationMatrix[0, 0] = (float)Math.Cos(MathHelper.ToRadians(xrot));

            return Multiply(source, rotationMatrix);
        }

        public static Vector3 TransformVector3(Vector3 vector, float[,] matrix)
        {
            float x, y, z;
            x = vector.X * matrix[0, 0] + vector.Y * matrix[1, 0] + vector.Z * matrix[2, 0] + matrix[3, 0];
            y = vector.X * matrix[0, 1] + vector.Y * matrix[1, 1] + vector.Z * matrix[2, 1] + matrix[3, 1];
            z = vector.X * matrix[0, 2] + vector.Y * matrix[1, 2] + vector.Z * matrix[2, 2] + matrix[3, 2];
            return new Vector3(x, y, z);
        }

        public static Vector3 GetTranslation(float[,] matrix)
        {
            return new Vector3(matrix[3, 0], matrix[3, 1], matrix[3, 2]);
        }
    }
}
