using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace CadDev
{
    public class CadWindow : OpenTK.Windowing.Desktop.GameWindow
    {
        Matrix4 model;
        Matrix4 view;
        Matrix4 projection;

        float speed = 1.5f;

        Vector3 position = new Vector3(0.0f, 0.0f,  3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f,  0.0f);

        Vector2 lastPos;

        public CadWindow(GameWindowSettings gameWindowSettings, 
                         NativeWindowSettings nativeWindowSettings) : 
                         base(gameWindowSettings, nativeWindowSettings)
        {
                CursorVisible = false;
                CursorGrabbed = true;

            model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
            view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        }

        // Points of a triangle in normalized device coordinates.
        readonly float[] Points = new float[] {
            //  X,     Y,    Z, (W)
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
             0.5f, -0.5f, 0.0f, //Bottom-right vertex
             0.0f,  0.5f, 0.0f  //Top vertex
            };

        int VertexBufferObject;
        int VertexArrayObject;

        Shader shader;        

        protected override void OnLoad()
        {   
            GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Length * sizeof(float), Points, BufferUsageHint.StaticDraw);

            shader = new Shader("shader.vert", "shader.frag");

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3*sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);
            shader.Dispose();

            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteVertexArray(VertexArrayObject);

            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            var Width = ClientRectangle.Size.X;
            var Height = ClientRectangle.Size.Y;

            GL.Viewport(0, 0, Width, Height);


            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)((float)Width / (float)Height), 0.1f, 100.0f);

            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();

            view = Matrix4.LookAt(position, position + front, up);

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(e);

            OpenTK.Graphics.OpenGL.ErrorCode errorCode = GL.GetError();
            if (errorCode != OpenTK.Graphics.OpenGL.ErrorCode.NoError)
            {
                System.Console.WriteLine($"errorCode: {errorCode}");
            }
        }


        bool firstMove = true;
        float sensitivity = 0.05f;
        float yaw = -90;
        float pitch = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused) // check to see if the window is focused
            {
                return;
            }

            if (KeyboardState.IsKeyDown(Keys.W))
            {
                position += front * speed * (float)e.Time; //Forward 
            }

            if (KeyboardState.IsKeyDown(Keys.S))
            {
                position -= front * speed * (float)e.Time; //Backwards
            }

            if (KeyboardState.IsKeyDown(Keys.A))
            {
                position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Left
            }

            if (KeyboardState.IsKeyDown(Keys.D))
            {
                position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)e.Time; //Right
            }

            if (KeyboardState.IsKeyDown(Keys.Space))
            {
                position += up * speed * (float)e.Time; //Up 
            }

            if (KeyboardState.IsKeyDown(Keys.LeftShift))
            {
                position -= up * speed * (float)e.Time; //Down
            }

            //if (MouseState.WasButtonDown(MouseButton.Left))
            //{
            //    firstMove = true;
            //}

            if (KeyboardState.IsKeyDown(Keys.C))
            {
                firstMove = true;
                yaw = -90;
                pitch = 0;

                view = Matrix4.LookAt(new Vector3(0,0,-3), new Vector3(0,0,0), up);
            }


            if (MouseState.IsButtonDown(MouseButton.Left))// && KeyboardState.IsKeyDown(Keys.E))
            {
                if (firstMove)
                {
                    lastPos = new Vector2(MousePosition.X, MousePosition.Y);
                    firstMove = false;
                }
                else
                {

                    float deltaX = MousePosition.X - lastPos.X;
                    float deltaY = MousePosition.Y - lastPos.Y;

                    lastPos = new Vector2(MousePosition.X, MousePosition.Y);

                    yaw += deltaX * sensitivity;
                    if (pitch > 89.0f)
                    {
                        pitch = 89.0f;
                    }
                    else if (pitch < -89.0f)
                    {
                        pitch = -89.0f;
                    }
                    else
                    {
                        pitch -= deltaY * sensitivity;
                    }
                }
            }

            if (KeyboardState.IsKeyReleased(Keys.E))
            {
                //CursorVisible = true;
                //CursorGrabbed = false;
                //firstMove = true;
            }

            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);


        }
    }
}