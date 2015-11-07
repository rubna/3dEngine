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
        float selectionRadius = 12;
        Vector3 moveConstraint = Vector3.Zero;
        Vector3 moveToolPosition = Vector3.Zero;
        float moveToolSize = 40;
        float paletteSize = 40;
        int selectedColor = 0;
        Color[] colors = { new Color(165, 177, 63), Color.Gray, Color.Transparent, Color.Green, new Color(82, 149, 204) };

        string tool = "select";

        public ModelEditor()
        {
            myMesh = new Mesh();
            myMesh.MakeCube(Color.LightGray, Color.Transparent);
        }

        public override void Create()
        {
            base.Create();
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
            //if (selectedVertices.Count == 0)
            //showMoveTool = false;

            //toggle shading
            if (Input.KeyboardCheckPressed(Keys.S))
                Drawing.EnableShading = !Drawing.EnableShading;

            //click on stuff
            if (Input.MouseCheckPressed(MouseButton.LeftButton))
            {
                //select colors ?
                Vector2 mp = Input.MousePosition();
                if (mp.Y < paletteSize && mp.X < paletteSize * colors.Length)
                {
                    selectedColor = (int)(mp.X / paletteSize);
                    tool = "color";
                }
                else
                {
                    if (tool == "select")
                    {
                        bool selectedConstraint = false;
                        int v = SelectVertex();
                        if (showMoveTool && selectedVertices.Count > 0)
                        {
                            Vector3 constr = SelectMoveConstraint();
                            if (constr != Vector3.Zero)
                            {
                                moveConstraint = constr;
                                selectedConstraint = true;
                            }
                        }

                        //selected a vertex
                        if (v != -1 && !selectedConstraint)
                        {
                            if (!Input.KeyboardCheckDown(Keys.LeftShift))
                                selectedVertices.Clear();

                            if (selectedVertices.Contains(v))
                                selectedVertices.Remove(v);
                            else
                                selectedVertices.Add(v);
                        }
                        else
                        if (!selectedConstraint && !Input.KeyboardCheckDown(Keys.LeftShift))
                            selectedVertices.Clear();
                    }
                    if (tool == "color")
                    {
                        int v = SelectVertex();
                        v = SelectTriangle();
                        if (v != -1)
                        {
                            if (myMesh.triangles[v].GetNormal(myMesh.vertices, World.Camera.ViewMatrix).Z < 0)
                                myMesh.triangles[v].FrontFaceColor = colors[selectedColor];
                            else
                                myMesh.triangles[v].BackFaceColor = colors[selectedColor];
                        }
                        else
                            tool = "select";
                    }
                }
            }

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
            {
                MakeFaces();
            }

            //remove vertices
            if (Input.KeyboardCheckPressed(Keys.Delete))
            {
                foreach (int i in selectedVertices)
                    RemoveVertex(i);
                selectedVertices.Clear();
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (drawDebug)
                myMesh.DrawDebug(Drawing, Transform);

            Vector3 middle = Vector3.Zero;
            foreach (int i in selectedVertices)
            {
                Drawing.DrawPoint(myMesh.vertices[i], 2, 1, Color.Yellow);
                middle += myMesh.vertices[i];
            }

            //draw move around tool
            if (showMoveTool && selectedVertices.Count > 0)
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
                Drawing.DrawPoint(new Vector3(mp, 1), 2, colors[selectedColor], Matrix.GetIdentity());
            }

            //draw colors
            for (int c = 0; c < colors.Length; c++)
            {
                Color color = colors[c];
                if (color!= Color.Transparent)
                    Drawing.DrawPoint(new Vector3(paletteSize / 2 + paletteSize * c, 20, 2), paletteSize / 2, color, Matrix.GetIdentity());
            }
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

        void MakeFaces()
        {
            if (selectedVertices.Count == 3)
            {
                //check if triangle does not already exist. if so, remove that tri
                foreach (Triangle tri in myMesh.triangles)
                    if (selectedVertices.Contains(tri.v0) && selectedVertices.Contains(tri.v1) && selectedVertices.Contains(tri.v2))
                    {
                        myMesh.triangles.Remove(tri);
                        return;
                    }

                myMesh.triangles.Add(new Triangle(selectedVertices[0], selectedVertices[1], selectedVertices[2], Color.Gray, Color.Gray));
            }
            Console.WriteLine(myMesh.triangles.Count);
        }
    }
}