using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace CadDev
{
    public class CadWindow : OpenTK.Windowing.Desktop.GameWindow
    {
        public CadWindow(GameWindowSettings gameWindowSettings, 
                         NativeWindowSettings nativeWindowSettings) : 
                         base(gameWindowSettings, nativeWindowSettings)
        {

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

        Matrix4 model;
        Matrix4 view;
        Matrix4 projection;

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


            Vector3 cameraPos = new Vector3(0.0f, 0.0f, 3.0f);
            Vector3 cameraTarget = Vector3.Zero;
            Vector3 cameraDirection = Vector3.Normalize(cameraPos - cameraTarget);

            Vector3 up = Vector3.UnitY;
            Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraDirection));
            Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);


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

            model = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-55.0f));
            view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)((float)Width / (float)Height), 0.1f, 100.0f);

            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            shader.Use();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();
            base.OnRenderFrame(e);

            var errorCode = GL.GetError();
            if (errorCode != ErrorCode.NoError)
            {
                System.Console.WriteLine($"errorCode: {errorCode}");
            }
        }
    }
}