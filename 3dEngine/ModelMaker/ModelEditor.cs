using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace _3dEngine
{
    class ModelEditor : GameObject
    {
        List<int> selectedVertices = new List<int>();
        bool showMoveTool = true;
        bool drawDebug = true;
        bool wireframe = false;
        float selectionRadius = 12;
        Vector3 moveConstraint = Vector3.Zero;
        Vector3 moveToolPosition = Vector3.Zero;
        float moveToolSize = 40;
        float paletteSize = 40;
        int selectedColorLeft = 0;
        int selectedColorRight = 0;
        Color[] colors = { new Color(165, 177, 63), Color.Gray, Color.Transparent, new Color(72, 119, 44), new Color(224, 163, 49), new Color(82, 149, 204) };

        string currentModel = "test";
        string tool = "select";

        List<Mesh> history = new List<Mesh>();
        int historyIndex = 0;

        public ModelEditor()
        {
        }

        public override void Create()
        {
            base.Create();
            myMesh = LoadCurrentModel();
            AddHistoryEntry();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //toggle tool
            if (Input.KeyboardCheckPressed(Keys.Space))
                tool = "select";
            if (Input.KeyboardCheckPressed(Keys.T))
                tool = "color";
            if (Input.KeyboardCheckPressed(Keys.G))
                showMoveTool = !showMoveTool;
            if (Input.KeyboardCheckPressed(Keys.D))
                drawDebug = !drawDebug;
            if (Input.KeyboardCheckPressed(Keys.W))
                wireframe = !wireframe;
            //if (selectedVertices.Count == 0)
            //showMoveTool = false;

            //undo and redo
            if (Input.KeyboardCheckDown(Keys.LeftControl))
            {
                if (Input.KeyboardCheckPressed(Keys.Z))
                    Undo();
                if (Input.KeyboardCheckPressed(Keys.Y))
                    Redo();
            }

            //save changes
            if (Input.KeyboardCheckPressed(Keys.S) && Input.KeyboardCheckDown(Keys.LeftControl))
                SaveModel();
            else //toggle shading
            if (Input.KeyboardCheckPressed(Keys.S))
                Drawing.EnableShading = !Drawing.EnableShading;

            //click on stuff
            if (Input.MouseCheckPressed(MouseButton.LeftButton))
                HandleLeftClick();
            if (Input.MouseCheckPressed(MouseButton.RightButton))
                HandleRightClick();

            if (Input.MouseCheckReleased(MouseButton.LeftButton))
                moveConstraint = Vector3.Zero;

            //move stuff around
            if (showMoveTool)
            {
                //set constraint vector
                if (Input.KeyboardCheckPressed(Keys.X))
                    moveConstraint = Vector3.UnitX;
                if (Input.KeyboardCheckPressed(Keys.Y))
                    moveConstraint = Vector3.UnitY;
                if (Input.KeyboardCheckPressed(Keys.Z))
                    moveConstraint = -Vector3.UnitZ;

                //move
                float amount = 0;
                if (Input.KeyboardCheckDown(Keys.Up))
                    amount += 1;
                if (Input.KeyboardCheckDown(Keys.Down))
                    amount -= 1;

                //move with mouse
                if (moveConstraint!=Vector3.Zero && Input.MouseCheckDown(MouseButton.LeftButton))
                {
                    Vector3 transConstr = moveConstraint;//.Transform(World.Camera.ViewMatrix);
                    float[,] matrix = World.Camera.ViewMatrix.Transpose();
                    Vector3 mouseLastPos = VectorExtensions.LineLineIntersect(new Vector3(Input.MouseLastPosition(), 0).Transform(matrix), new Vector3(Input.MouseLastPosition(), 1).Transform(matrix), Vector3.Zero, moveConstraint);
                    Vector3 mousePos = VectorExtensions.LineLineIntersect(new Vector3(Input.MousePosition(), 0).Transform(matrix), new Vector3(Input.MousePosition(), 1).Transform(matrix), Vector3.Zero, moveConstraint);

                    float lastPoint = Vector3.Dot(mouseLastPos, transConstr);// Vector3.Dot(moveConstraint, moveConstraint);
                    float currentPoint = Vector3.Dot(mousePos, transConstr); // Vector3.Dot(moveConstraint, moveConstraint);
                    amount = (currentPoint - lastPoint) / (float)Math.Pow(World.Camera.Scale,2);
                }
                foreach (int i in selectedVertices)
                    myMesh.vertices[i] = myMesh.vertices[i] + moveConstraint * amount;
            }

            //make faces
            if (Input.KeyboardCheckPressed(Keys.F))
                MakeFaces(selectedVertices, true);

            //remove vertices
            if (Input.KeyboardCheckPressed(Keys.Delete) && selectedVertices.Count > 0)
            {
                AddHistoryEntry();
                for (int i = 0; i < selectedVertices.Count; i++)
                    RemoveVertex(selectedVertices[i]);
                selectedVertices.Clear();
            }

            //extrude
            if (Input.KeyboardCheckPressed(Keys.E))
                Extrude();
        }

        public override void Draw()
        {
            //base.Draw();
            if (!wireframe)
                myMesh.Draw(Drawing, World.Camera.ViewMatrix);
            if (drawDebug)
                myMesh.DrawDebug(Drawing, World.Camera.ViewMatrix);

            Vector3 middle = Vector3.Zero;
            foreach (int i in selectedVertices)
            {
                Drawing.DrawPoint(myMesh.vertices[i], 2, 1, Color.Yellow);
                middle += myMesh.vertices[i];
            }

            //draw move around tool
            if (showMoveTool && selectedVertices.Count > 0 && tool == "select")
            {
                middle /= selectedVertices.Count;
                moveToolPosition = middle;
                float size = moveToolSize / World.Camera.Scale;
                Vector3 x = Vector3.UnitX * size ;
                Vector3 y = Vector3.UnitY * size;
                Vector3 z = Vector3.UnitZ * size;
                Drawing.DrawLine(middle, middle + x, 2, Color.Red);
                Drawing.DrawPoint(middle + x, 4, Color.Red);
                Drawing.DrawLine(middle, middle + y, 2, Color.Green);
                Drawing.DrawPoint(middle + y, 4, Color.Green);
                Drawing.DrawLine(middle, middle + z, 2, Color.Blue);
                Drawing.DrawPoint(middle + z, 4, Color.Blue);

                //in faces
                //Drawing.DrawQuad(middle, middle + x / 2, middle + y / 2, middle + x / 2 + y / 2, new Color(255, 255, 0), false);
                //Drawing.DrawQuad(middle, middle + y / 2, middle + z / 2, middle + y / 2 + z / 2, new Color(0, 255, 255), false);
                //Drawing.DrawQuad(middle, middle + z / 2, middle + x / 2, middle + z / 2 + x / 2, new Color(255, 0, 255), false);
            }

            Vector2 mp = Input.MousePosition();
            if (tool == "select")
            {
                Vector3 v1 = new Vector3(mp, 2);
                Vector3 v2 = new Vector3(mp + new Vector2(15, 90 + Input.MouseSpeed().X).ToCartesian(), 2);
                Vector3 v3 = new Vector3(mp + new Vector2(15, 45 + Input.MouseSpeed().X).ToCartesian(), 2);

                Drawing.DrawTriangle(v1, v2, v3, Color.White, Color.White, Matrix.GetIdentity());
                v1.Z--; v2.Z--; v3.Z--;
                Drawing.DrawLine(v1, v2, 2, Color.Black, Matrix.GetIdentity());
                Drawing.DrawLine(v1, v3, 2, Color.Black, Matrix.GetIdentity());
                Drawing.DrawLine(v2, v3, 2, Color.Black, Matrix.GetIdentity());
            }
            if (tool == "color")
            {
                Drawing.DrawPoint(new Vector3(mp, 2), 3, Color.Black, Matrix.GetIdentity());
                Drawing.DrawPoint(new Vector3(mp, 1) - Vector3.UnitX, 2, colors[selectedColorLeft], Matrix.GetIdentity());
                Drawing.DrawPoint(new Vector3(mp, 1) + Vector3.UnitX, 2, colors[selectedColorRight], Matrix.GetIdentity());
            }

            //draw colors
            for (int c = 0; c < colors.Length; c++)
            {
                Color color = colors[c];
                if (color!= Color.Transparent)
                    Drawing.DrawPoint(new Vector3(paletteSize / 2 + paletteSize * c, 20, 2), paletteSize / 2, color, Matrix.GetIdentity());
            }

            //draw models
            float[,] matrix = Matrix.GetIdentity();
            matrix = matrix.Translate(paletteSize / 2, paletteSize * 2, 200);
            foreach (Mesh mesh in World.AllAssets.models.Values)
            {
                mesh.Draw(Drawing, matrix.Rotate(0, 135, 45));
                matrix = matrix.Translate(paletteSize, 0, 0);
            }
        }

        void Extrude()
        {
            int originalCount = myMesh.vertices.Count;
            if (originalCount == 0)
                return;
            AddHistoryEntry();
            for (int i = 0; i < selectedVertices.Count; i++)
            AddVertex(myMesh.vertices[selectedVertices[i]]);
            List<int> previousVerts = new List<int>(selectedVertices);
            selectedVertices.Clear();
            for (int i = originalCount; i < myMesh.vertices.Count; i++)
                selectedVertices.Add(i);

            //make faces if 2 of the previous verts are in the same one and only one triangle
            List<Triangle> previousTriangles = new List<Triangle>(myMesh.triangles);
            List<Point> edges = new List<Point>();
            foreach (int i in previousVerts)
            {
                List<Triangle> connectedTriangles = new List<Triangle>();
                foreach (Triangle tri in previousTriangles)
                {
                    if (tri.v0 == i || tri.v1 == i || tri.v2 == i)
                        connectedTriangles.Add(tri);
                }

                foreach (int j in previousVerts)
                {
                    if (i == j)
                        continue;
                    int countSharedTriangles = 0;
                    foreach (Triangle tri in connectedTriangles)
                    {
                        if (tri.v0 == j || tri.v1 == j || tri.v2 == j)
                            countSharedTriangles++;
                    }

                    if (countSharedTriangles == 1)
                    {
                        bool duplicate = false;
                        foreach (Point p in edges)
                            if ((p.X == i && p.Y == j) || (p.Y == i && p.X == j))
                                duplicate = true;
                        if (!duplicate)
                        {
                            edges.Add(new Point(i, j));
                            int otherJ = selectedVertices[previousVerts.FindIndex(x => x == j)];
                            int otherI = selectedVertices[previousVerts.FindIndex(x => x == i)];
                            MakeFaces(new List<int> { i, j, otherJ }, false);
                            MakeFaces(new List<int> { i, otherJ, otherI }, false);
                        }
                    }
                }
            }
            Console.WriteLine("edges: " + edges.Count);
        }

        void AddHistoryEntry()
        {
            if (historyIndex < history.Count)
                history.RemoveRange(historyIndex, history.Count - historyIndex);

            history.Add(myMesh.Copy());
            historyIndex++;
            Console.WriteLine(historyIndex + ", " + history.Count);
        }

        void Undo()
        {
            if (historyIndex > 0)
            {
                historyIndex--;
                myMesh = history[historyIndex].Copy();
            }
            Console.WriteLine(historyIndex + ", " + history.Count);
        }

        void Redo()
        {
            if (historyIndex < history.Count -1)
            {
                historyIndex++;
                myMesh = history[historyIndex].Copy();
            }
            Console.WriteLine(historyIndex + ", " + history.Count);
        }

        Mesh LoadCurrentModel()
        {

            if (World.AllAssets.models.ContainsKey(currentModel))
                return World.AllAssets.models[currentModel];
            else
            {
                Mesh mesh = new Mesh();
                mesh.MakeCube(Color.LightGray, Color.Transparent);
                return mesh;
            }
        }
        void SaveModel()
        {
            if (World.AllAssets.models.ContainsKey(currentModel))
                World.AllAssets.models[currentModel] = myMesh;
            else
            World.AllAssets.models.Add(currentModel, myMesh);
            World.AllAssets.SaveAllModels();
        }

        int AddVertex(Vector3 position)
        {
            myMesh.vertices.Add(position);
            return myMesh.vertices.Count;
        }

        void RemoveVertex(int index)
        {
            myMesh.vertices.RemoveAt(index);
            List<Triangle> removeTriangles = new List<Triangle>();
            foreach (Triangle tri in myMesh.triangles)
            {
                if (tri.v0 == index || tri.v1 == index || tri.v2 == index)
                    removeTriangles.Add(tri);

                if (tri.v0 > index)
                    tri.v0--;
                if (tri.v1 > index)
                    tri.v1--;
                if (tri.v2 > index)
                    tri.v2--;
            }
            foreach (Triangle tri in removeTriangles)
                myMesh.triangles.Remove(tri);

            for (int i = 0; i < selectedVertices.Count; i++)
                if (selectedVertices[i] > index)
                    selectedVertices[i]--;
        }

        void HandleLeftClick()
        {
            //select colors ?
            Vector2 mp = Input.MousePosition();
            if (mp.Y < paletteSize && mp.X < paletteSize * colors.Length)
            {
                selectedColorLeft = (int)(mp.X / paletteSize);
                tool = "color";
            }
            else
            {
                if (tool == "select")
                {
                    if (showMoveTool && selectedVertices.Count > 0)
                    {
                        Vector3 constr = SelectMoveConstraint();
                        if (constr != Vector3.Zero)
                        {
                            moveConstraint = constr;
                            AddHistoryEntry();
                        }
                    }
                }
                if (tool == "color")
                {
                    int v = SelectTriangle();
                    if (v != -1)
                    {
                        AddHistoryEntry();
                        if (myMesh.triangles[v].GetNormal(myMesh.vertices, World.Camera.ViewMatrix).Z < 0)
                            myMesh.triangles[v].FrontFaceColor = colors[selectedColorLeft];
                        else
                            myMesh.triangles[v].BackFaceColor = colors[selectedColorLeft];
                    }
                }
            }
        }

        void HandleRightClick()
        {
            //select colors ?
            Vector2 mp = Input.MousePosition();
            if (mp.Y < paletteSize && mp.X < paletteSize * colors.Length)
            {
                selectedColorRight = (int)(mp.X / paletteSize);
                tool = "color";
            }
            else
            if (tool == "select")
            {
                int v = SelectVertex();
                //selected a vertex
                if (v != -1)
                {
                    if (!Input.KeyboardCheckDown(Keys.LeftShift))
                        selectedVertices.Clear();

                    if (selectedVertices.Contains(v))
                        selectedVertices.Remove(v);
                    else
                        selectedVertices.Add(v);
                }
                else
                if (!Input.KeyboardCheckDown(Keys.LeftShift))
                    selectedVertices.Clear();
            }
            if (tool == "color")
            {
                int v = SelectTriangle();
                if (v != -1)
                {
                    AddHistoryEntry();
                    if (myMesh.triangles[v].GetNormal(myMesh.vertices, World.Camera.ViewMatrix).Z < 0)
                        myMesh.triangles[v].FrontFaceColor = colors[selectedColorRight];
                    else
                        myMesh.triangles[v].BackFaceColor = colors[selectedColorRight];
                }
            }
        }

        int SelectVertex()
        {
            int selected = -1;
            float selectZBuffer = Drawing.ZFar;

            //check for vertices
            for (int i = 0; i < myMesh.vertices.Count; i++)
            {
                Vector3 v = myMesh.vertices[i].Transform(World.Camera.ViewMatrix);
                if (PointClicked(v) && v.Z < selectZBuffer)
                {
                    selected = i;
                    selectZBuffer = v.Z;
                }
            }

            return selected;
        }

        int SelectTriangle()
        {
            float selectZBuffer = Drawing.ZFar;
            int selected = -1;
            //check for triangles
            for (int i = 0; i < myMesh.triangles.Count; i++)
            {
                Triangle tri = myMesh.triangles[i];
                Vector3 v0 = myMesh.vertices[tri.v0].Transform(World.Camera.ViewMatrix);
                Vector3 v1 = myMesh.vertices[tri.v1].Transform(World.Camera.ViewMatrix);
                Vector3 v2 = myMesh.vertices[tri.v2].Transform(World.Camera.ViewMatrix);
                float z = (v0.Z + v1.Z + v2.Z) / 3;
                if (VectorExtensions.PointInsideTriangle(Input.MousePosition(), v0.ToVector2(), v1.ToVector2(), v2.ToVector2()) && z < selectZBuffer && z > Drawing.ZNear)
                {
                    selectZBuffer = z;
                    selected = i;
                }
            }
            return selected;
        }

        Vector3 SelectMoveConstraint()
        {
            float selectZBuffer = Drawing.ZFar;
            Vector3 constraint = Vector3.Zero;
            Vector2 p = Input.MousePosition();

            float size = moveToolSize / World.Camera.Scale;
            Vector3 x = Vector3.UnitX * size;
            Vector3 y = Vector3.UnitY * size;
            Vector3 z = Vector3.UnitZ * size;

            //x
            Vector3 pos = (moveToolPosition + x).Transform(World.Camera.ViewMatrix);
            if (PointClicked(pos) && pos.Z < selectZBuffer)
            {
                constraint = Vector3.UnitX;
                selectZBuffer = pos.Z;
            }

            //y
            pos = (moveToolPosition + y).Transform(World.Camera.ViewMatrix);
            if (PointClicked(pos) && pos.Z < selectZBuffer)
            {
                constraint = Vector3.UnitY;
                selectZBuffer = pos.Z;
            }

            //z
            pos = (moveToolPosition + z).Transform(World.Camera.ViewMatrix);
            if (PointClicked(pos) && pos.Z < selectZBuffer)
            {
                constraint = -Vector3.UnitZ;
                selectZBuffer = pos.Z;
            }

            return constraint;
        }

        bool PointClicked(Vector3 point)
        {
            return PointClicked(point, selectionRadius);
        }

        bool PointClicked(Vector3 point, float radius)
        {
            return (point.ToVector2() - Input.MousePosition()).Length() < radius && point.Z > Drawing.ZNear;
        }

        void MakeFaces(List<int>verts, bool removeDuplicate)
        {
            if (verts.Count == 3)
            {
                //check if verts in triangle are all unique
                if (verts[0] == verts[1] || verts[1] == verts[2] || verts[2] == verts[0])
                    return;
                //check if triangle does not already exist. if so, remove that tri
                foreach (Triangle tri in myMesh.triangles)
                    if (verts.Contains(tri.v0) && verts.Contains(tri.v1) && verts.Contains(tri.v2))
                    {
                        if (removeDuplicate)
                        {
                            myMesh.triangles.Remove(tri);
                            AddHistoryEntry();
                        }
                        return;
                    }
                MakeTriangle(verts[0], verts[1], verts[2]);
                AddHistoryEntry();
            }
            Console.WriteLine(myMesh.triangles.Count);
        }

        Triangle MakeTriangle(int v0, int v1, int v2)
        {
            Triangle tri = new Triangle(v0, v1, v2, Color.Gray, Color.Gray);
            myMesh.triangles.Add(tri);
            AddHistoryEntry();
            return tri;
        }
    }
}