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

            using (CadWindow wnd = new CadWindow(new OpenTK.Windowing.Desktop.GameWindowSettings()
                        {
                            RenderFrequency = 60
                        },
                        new OpenTK.Windowing.Desktop.NativeWindowSettings()
                        {
                            IsFullscreen = true,
                            API = OpenTK.Windowing.Common.ContextAPI.OpenGL,
                            APIVersion = new Version(3, 2)
                        }
                    ))
            {
                wnd.Run();
            }

        }
    }
}