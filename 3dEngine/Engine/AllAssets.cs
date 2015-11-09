using Microsoft.Xna.Framework;
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

        public void LoadAllModels()
        {
            //Console.Write(File.ReadAllLines("Models/test.txt"));
            string allText = File.ReadAllText("models.txt");
            string[] models = allText.Split('|');
            foreach (string str in models)
            {
                str.Replace(" ", string.Empty);
                LoadModel(str.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        void LoadModel(string[] model)
        {
            if (model.Length == 0)
                return;
            
            Mesh myMesh = new Mesh();//System.IO.File.ReadAllText
            string fileName = model[0];
            Console.WriteLine(model[0]);
            int i = 1;
            while (model[i] != "STOP") //read & store vertices
            {
                Console.WriteLine(model[i]);
                string[] parameters = ReadParameters(model[i]);

                myMesh.vertices.Add(new Vector3(float.Parse(parameters[0]), float.Parse(parameters[1]), float.Parse(parameters[2])));
                i++;
            }
            Console.WriteLine(model[i]);
            i++;
            while (model[i] != "STOP") //read and store Triangles
            {
                Console.WriteLine(model[i]);
                string[] parameters = ReadParameters(model[i]);
                Color frontFaceColor = new Color(byte.Parse(parameters[3]), byte.Parse(parameters[4]), byte.Parse(parameters[5]), byte.Parse(parameters[6]));
                Color backFaceColor = new Color(byte.Parse(parameters[7]), byte.Parse(parameters[8]), byte.Parse(parameters[9]), byte.Parse(parameters[10]));
                myMesh.triangles.Add(new Triangle(int.Parse(parameters[0]), int.Parse(parameters[1]), int.Parse(parameters[2]), frontFaceColor, backFaceColor));

                i++;
            }
            models.Add(fileName, myMesh);
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

        public void SaveAllModels()
        {
            string output = "";
            for (int i = 0; i < models.Count; i++)
            {
                output += "|" + Environment.NewLine;
                output +=  models.Keys.ElementAt(i) + Environment.NewLine;
                Mesh mesh = models.Values.ElementAt(i);
                foreach (Vector3 v in mesh.vertices)
                {
                    output += v.X + "; ";
                    output += v.Y + "; ";
                    output += v.Z + ";" + Environment.NewLine;
                }
                output += "STOP" + Environment.NewLine;
                foreach (Triangle tri in mesh.triangles)
                {
                    output += tri.v0 + "; ";
                    output += tri.v1 + "; ";
                    output += tri.v2 + "; ";

                    output += tri.FrontFaceColor.R + "; " + tri.FrontFaceColor.G + "; " + tri.FrontFaceColor.B + "; " + tri.FrontFaceColor.A + ";";
                    output += tri.BackFaceColor.R + "; " + tri.BackFaceColor.G + "; " + tri.BackFaceColor.B + "; " + tri.BackFaceColor.A + ";";
                    output += Environment.NewLine;
                }
                output += "STOP" + Environment.NewLine;
            }
            Console.Write(output);

            File.WriteAllText("models.txt", output);//,Encoding.ASCII);
            Console.WriteLine("saved.");
        }
    }
}
