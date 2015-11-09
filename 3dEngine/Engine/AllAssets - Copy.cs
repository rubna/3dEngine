/*using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace _3dEngine
{
    class AllAssets
    {
        public Dictionary<string, Mesh> models = new Dictionary<string,Mesh>();

        public void LoadModel(string file, ContentManager content)
        {
            Mesh myMesh = new Mesh();//System.IO.File.ReadAllText
            //Console.Write(File.ReadAllLines("Models/test.txt"));
            string[] model = File.ReadAllLines("Models/test.txt");
            int i = 0;

            while (model[i] != "STOP") //read & store vertices
            {
                string[] parameters = ReadParameters(model[i]);

                myMesh.vertices.Add(new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2])));
                i++;
            }
            i++;
            while (model[i] != "STOP") //read and store Triangles
            {
                string[] parameters = ReadParameters(model[i]);
                Color frontFaceColor = new Color(int.Parse(parameters[3]), int.Parse(parameters[4]), int.Parse(parameters[5]));
                Color backFaceColor = new Color(int.Parse(parameters[6]), int.Parse(parameters[7]), int.Parse(parameters[8]));
                myMesh.triangles.Add(new Triangle(int.Parse(parameters[0]), int.Parse(parameters[1]), int.Parse(parameters[2]), frontFaceColor, backFaceColor));

                i++;
            }

            //foreach (Triangle tri in myMesh.triangles)
                //myMesh.normals.Add(Vector3.Cross(myMesh.vertices[tri.v0] - myMesh.vertices[tri.v1], myMesh.vertices[tri.v0] - myMesh.vertices[tri.v2]));
            models.Add(file, myMesh);
        }

        string[] ReadParameters(string str)
        {
            str = str.Replace(" ", string.Empty);
            string[] parameters = new string[str.Split(';').Length - 1];

            for (int i = 0; i < parameters.Length; i++)
            {
                string subStr = str.Substring(0, str.IndexOf(';'));

                parameters[i] = subStr;
                //Console.WriteLine(parameters[i]);

                str = str.Remove(0, str.IndexOf(';') + 1);
            }
            return parameters;
        }

        public Mesh GetModelWithName(string name)
        {
            if (models.ContainsKey(name))
                return models[name];
            else
                throw new Exception("Model file not found !! - rub");

        }
    }
}
*/