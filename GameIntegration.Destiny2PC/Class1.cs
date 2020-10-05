using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using SimpleLed;

namespace GameIntegration.Destiny2PC
{
    public class Destiny2 : ISimpleLed
    {
        private DateTime lastScreenshot = DateTime.MinValue;
        public void Dispose()
        {
            

        }

        public void Configure(DriverDetails driverDetails)
        {
            
        }

        public List<ControlDevice> GetDevices()
        {
            throw new NotImplementedException();
        }

        public void Push(ControlDevice controlDevice)
        {
            
        }

        public void Pull(ControlDevice controlDevice)
        {
            if ((DateTime.Now - lastScreenshot) > TimeSpan.FromSeconds(1))
            {

            }
            
        }

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                Author = "mad ninja",
                Blurb = "Get live feedback from various Destiny 2 values in your RGB!",
                GitHubLink = "https://github.com/SimpleLed/GameIntegration.Destiny2PC",
                CurrentVersion = new ReleaseNumber(0, 0, 0, 1000),
                Id = Guid.Parse("b3a7c0c3-5db0-4e1f-8cd5-595f74cec559"),
                IsPublicRelease = false,
                IsSource = true,
                SupportsPull = true
            };
        }

        public T GetConfig<T>() where T : SLSConfigData
        {
            return null;
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
            
        }

        public string Name() => "Destiny 2";

        public event EventHandler DeviceRescanRequired;

        public void CaptureApplication(string procName)
        {
            var proc = Process.GetProcessesByName(procName)[0];
            var rect = new User32.Rect();
            User32.GetWindowRect(proc.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }

            bmp.Save("c:\\tmp\\test.png", ImageFormat.Png);
        }

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        }
    }
}
