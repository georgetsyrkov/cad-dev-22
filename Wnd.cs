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
        //Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
        Matrix4 model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(0f));
        Matrix4 view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
        Matrix4 projection;

        Vector3 position = new Vector3(0.0f, 0.0f,  3.0f);
        Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
        Vector3 up = new Vector3(0.0f, 1.0f,  0.0f);

        Vector2 lastPos;

        Shader? shader;

        public CadWindow(GameWindowSettings gameWindowSettings, 
                         NativeWindowSettings nativeWindowSettings) : 
                         base(gameWindowSettings, nativeWindowSettings)
        {

        }

        readonly float[] Points = new float[] {
            //  X,     Y,    Z, (W)
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
             0.5f, -0.5f, 0.0f, //Bottom-right vertex
             0.0f,  0.5f, 0.0f  //Top vertex
            };

        readonly float[] paralPoints = OtherPoints.GetParal(2, 2, 2);

        int VertexBufferObject;
        int VertexArrayObject;

        protected override void OnLoad()
        {   
            GL.ClearColor(0.15f, 0.15f, 0.15f, 0.0f);

            shader = new Shader("shader.vert", "shader.frag");

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            
            GL.BufferData(BufferTarget.ArrayBuffer, paralPoints.Length * sizeof(float), paralPoints, BufferUsageHint.StaticDraw);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3*sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.Enable(EnableCap.DepthTest);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferObject);

            if (shader != null) ((Shader)shader).Dispose();

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            if (shader != null) ((Shader)shader).Use();

            view = Matrix4.LookAt(position, position + front, up);

            if (shader != null) ((Shader)shader).SetMatrix4("model", model);
            if (shader != null) ((Shader)shader).SetMatrix4("view", view);
            if (shader != null) ((Shader)shader).SetMatrix4("projection", projection);

            GL.PointSize(6);

            GL.BindVertexArray(VertexArrayObject);
            //GL.DrawArrays(BeginMode.Quads, 0, paralPoints.Length);
            GL.DrawArrays(PrimitiveType.Triangles, 0, paralPoints.Length);

            Context.SwapBuffers();
            base.OnRenderFrame(e);

            OpenTK.Graphics.OpenGL.ErrorCode errorCode = GL.GetError();
            if (errorCode != OpenTK.Graphics.OpenGL.ErrorCode.NoError)
            {
                System.Console.WriteLine($"errorCode: {errorCode}");
            }
        }

        bool firstMove = true;
        float sensitivity = 0.10f;
        float speed = 5f;
        float yaw = -90;
        float pitch = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (!IsFocused) { return; }

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
            if (KeyboardState.IsKeyDown(Keys.C))
            {
                position -= up * speed * (float)e.Time; //Down
            }
            if (KeyboardState.IsKeyDown(Keys.R))
            {
                firstMove = true;
                yaw = -90;
                pitch = 0;
            }

            if (MouseState.IsButtonDown(MouseButton.Left))
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
            else
            {
                firstMove = true;
            }

            front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);
        }
    }
}