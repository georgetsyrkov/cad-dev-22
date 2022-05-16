using System;

namespace CadDev
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            bool fScreen = false;
            if (args.Length > 0 && args[0].Trim().ToLower() == "full")
            {
                fScreen = true;
            }

            OpenTK.Windowing.Desktop.NativeWindowSettings sets = new OpenTK.Windowing.Desktop.NativeWindowSettings();
            sets.Size = new OpenTK.Mathematics.Vector2i(640, 480);
            sets.Location = new OpenTK.Mathematics.Vector2i(100, 100);

            if (fScreen)
            {
                sets = new OpenTK.Windowing.Desktop.NativeWindowSettings();
                sets.IsFullscreen = true;
                sets.WindowState = OpenTK.Windowing.Common.WindowState.Fullscreen;
            }

            CadWindow wnd = new CadWindow(new OpenTK.Windowing.Desktop.GameWindowSettings()
                        {
                            RenderFrequency = 60,
                            UpdateFrequency = 60
                        },
                        sets
                    );

            wnd.Run();
        }
    }
}