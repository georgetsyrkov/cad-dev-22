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

        protected override void OnLoad()
        {
            base.OnLoad();
            
            //GL.Enable(EnableCap.DepthTest);
            //GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            //GL.PatchParameter(PatchParameterInt.PatchVertices, 3);

            GL.ClearColor(0.3f, 0.3f, 0.5f, 1.0f);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            var Width = ClientRectangle.Size.X;
            var Height = ClientRectangle.Size.Y;

            GL.Viewport(0, 0, ClientRectangle.Size.X, ClientRectangle.Size.Y);
    
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
    
            GL.MatrixMode(MatrixMode.Projection);
    
            GL.LoadMatrix(ref projection);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 modelview = Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            GL.LoadIdentity();

            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1.0f, 0.0f, 0.0f); 
            GL.Vertex3(-1.0f, -1.0f, 4.0f);
            GL.Color3(0.0f, 1.0f, 0.0f); 
            GL.Vertex3(1.0f, -1.0f, 4.0f);
            GL.Color3(0.0f, 0.0f, 1.0f); 
            GL.Vertex3(0.0f, 1.0f, 4.0f);
            GL.End();
            
            GL.Flush();
        
            Context.SwapBuffers();
        }
    }
}