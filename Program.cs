using System;

namespace CadDev
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Trim().ToLower() == "ru")
            {
                Console.WriteLine("Привет мир");
            }
            else
            {
                Console.WriteLine("Hello world");
            }

            CadWindow wnd = new CadWindow(new OpenTK.Windowing.Desktop.GameWindowSettings()
                        {
                            RenderFrequency = 60,
                            UpdateFrequency = 60
                        },
                        new OpenTK.Windowing.Desktop.NativeWindowSettings()
                        {
                            IsFullscreen = true,
                            //API = OpenTK.Windowing.Common.ContextAPI.OpenGL,
                            //APIVersion = new Version(1, 1),
                            Profile = OpenTK.Windowing.Common.ContextProfile.Core,
                            Size = new OpenTK.Mathematics.Vector2i(640, 480),
                            Location = new OpenTK.Mathematics.Vector2i(100, 100),
                            IsEventDriven = false
                        }
                    );
            

            wnd.Run();

        }
    }
}