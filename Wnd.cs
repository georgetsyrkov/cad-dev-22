using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

// git@github.com:georgetsyrkov/cad-dev-22.git


namespace CadDev
{
    public class CadWindow : OpenTK.Windowing.Desktop.GameWindow
    {
        public CadWindow(GameWindowSettings gameWindowSettings, 
                         NativeWindowSettings nativeWindowSettings) : 
                         base(gameWindowSettings, nativeWindowSettings)
        {

        }

        private Matrix4 _view;
        private Matrix4 _projection;

        // A simple vertex shader possible. Just passes through the position vector.
        const string VertexShaderSource = @"
            #version 330
            
            layout(location = 0) in vec4 position;
            
            uniform mat4 view;
            uniform mat4 projection;
            
            void main(void)
            {
                gl_Position = position;
                gl_Position = projection * view * position;
            }
        ";

        // A simple fragment shader. Just a constant red color.
        const string FragmentShaderSource = @"
            #version 330
            out vec4 outputColor;
            void main(void)
            {
                outputColor = vec4(1.0, 0.0, 0.0, 1.0);
            }
        ";

        // Points of a triangle in normalized device coordinates.
        readonly float[] Points = new float[] {
            // X, Y, Z, W
            -50f, 0.0f, -10.0f, 1.0f,
            50f, 0.0f, -10.0f, 1.0f,
            0.0f, 50f, -10.0f, 1.0f };

        int VertexShader;
        int FragmentShader;
        int ShaderProgram;
        int VertexBufferObject;
        int VertexArrayObject;


        private bool _isFirstMouseMove = true;
        private Vector2 _previousMousePosition;
        private float _mouseSensitivity = 0.004f;
        private float _maxRotationSpeed = 0.1f;

        public Vector3 UpDirection => _pivot.YAxisPositiveDirection;
        public Vector3 RightDirection => _pivot.XAxisPositiveDirection;

        public void ProcessMouseMovement(MouseState mouseState)
        {
            if (_isFirstMouseMove)
            {
                _previousMousePosition = new Vector2(mouseState.X, mouseState.Y);
                _isFirstMouseMove = false;
            }
            else
            {
                var (x, y) = (mouseState.Position - _previousMousePosition) * -_mouseSensitivity;

                x = MathHelper.Clamp(x, -_maxRotationSpeed, _maxRotationSpeed);
                y = MathHelper.Clamp(y, -_maxRotationSpeed, _maxRotationSpeed);
                _previousMousePosition = mouseState.Position;
                RotateViewDirection(x, UpDirection);
                RotateViewDirection(y, RightDirection);
            }
        }

        private readonly Pivot _pivot = new Pivot();
        public void RotateViewDirection(float angle, Vector3 vector)
        {
            _pivot.LocalRotate(angle, vector);
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            _view = Matrix4.LookAt(_pivot.Position, _pivot.Position + _pivot.ZAxisPositiveDirection * -1,
                    _pivot.YAxisPositiveDirection);
        }

        private float _fov = MathHelper.PiOver3;    // Угол поля зрения в направлении оси OY (в радианах).
        private float _distanceToTheNearClipPlane = 0.01f;
        private float _distanceToTheFarClipPlane = 100.0f;
        private float _viewportAspectRatio;
        private void UpdateProjectionMatrix()
        {
            _projection = Matrix4.CreatePerspectiveFieldOfView(_fov, _viewportAspectRatio, 
                    _distanceToTheNearClipPlane, _distanceToTheFarClipPlane);
        }

        private void UpdateViewportAspectRatio()
        {
            var Width = ClientRectangle.Size.X;
            var Height = ClientRectangle.Size.Y;

            _viewportAspectRatio = Width / (float)Height;
        }

        protected override void OnLoad()
        {
            // Load the source of the vertex shader and compile it.
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);

            // Load the source of the fragment shader and compile it.
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            // Create the shader program, attach the vertex and fragment shaders and link the program.
            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);

            // Create the vertex buffer object (VBO) for the vertex data.
            VertexBufferObject = GL.GenBuffer();
            // Bind the VBO and copy the vertex data into it.
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Length * sizeof(float), Points, BufferUsageHint.StaticDraw);

            // Retrive the position location from the program.
            var positionLocation = GL.GetAttribLocation(ShaderProgram, "position");

            // Create the vertex array object (VAO) for the program.
            VertexArrayObject = GL.GenVertexArray();
            // Bind the VAO and setup the position attribute.
            GL.BindVertexArray(VertexArrayObject);
            GL.VertexAttribPointer(positionLocation, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(positionLocation);

            // Set the clear color to blue
            GL.ClearColor(0.0f, 0.0f, 1.0f, 0.0f);

            this.UpdateViewportAspectRatio();
            this.UpdateProjectionMatrix();
            this.UpdateViewMatrix();

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteProgram(ShaderProgram);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            var Width = ClientRectangle.Size.X;
            var Height = ClientRectangle.Size.Y;

            // Resize the viewport to match the window size.
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(0.3f, 0.3f, 0.5f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

             // Bind the VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            // Bind the VAO
            GL.BindVertexArray(VertexArrayObject);
            // Use/Bind the program
            GL.UseProgram(ShaderProgram);

            var viewLocation = GL.GetAttribLocation(ShaderProgram, "view");
            GL.UniformMatrix4(viewLocation, false, ref _view);

            var projectionLocation = GL.GetAttribLocation(ShaderProgram, "projection");
            GL.UniformMatrix4(projectionLocation, false, ref _projection);

            // This draws the triangle.
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}