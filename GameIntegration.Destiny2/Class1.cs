using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using SimpleLed;

namespace GameIntegration.Destiny2PC
{
    public class Destiny2 : ISimpleLed
    {
        public class Area
        {
            public int StartX { get; set; }
            public int StartY { get; set; }
            public int EndX { get; set; }
            public int EndY { get; set; }

            public Area(int sx, int sy, int ex, int ey)
            {
                StartX = sx;
                StartY = sy;
                EndX = ex;
                EndY = ey;
            }
        }

        private Area grenadeArea = new Area(125, 935, 125, 990);
        private Area superArea = new Area(125, 919,557,901);
        private ControlDevice.LedUnit[] grenade = new ControlDevice.LedUnit[30];
        private DateTime lastScreenshot = DateTime.MinValue;
        public void Dispose()
        {


        }

        public void Configure(DriverDetails driverDetails)
        {

        }

        public List<ControlDevice> GetDevices()
        {
            for (int i = 0; i < grenade.Length; i++)
            {
                grenade[i] = new ControlDevice.LedUnit()
                {
                    Color = new LEDColor(0,0,0),
                    Data = new ControlDevice.LEDData()
                    {
                        LEDNumber = i
                    },
                    LEDName = i+" LED"
                };
            }

            return new List<ControlDevice>
            {
                new ControlDevice
                {
                    LEDs = grenade,
                    Name = "Grenade",
                    DeviceType = DeviceTypes.GameIntegration,
                    Driver=this
                }
            };
        }

        public void Push(ControlDevice controlDevice)
        {

        }

        public void Pull(ControlDevice controlDevice)
        {
            if ((DateTime.Now - lastScreenshot) > TimeSpan.FromSeconds(1))
            {
                var bmp=CaptureApplication("destiny2");
                lastScreenshot=DateTime.Now;

                try
                {
                    int cp = 0;
                    if (bmp != null)
                    {
                        Color prv= Color.Black;
                        
                        bool deffoNotYellow = false;
                        for (int i = 0; i < grenade.Length; i++)
                        {
                            float perc = i / (float) grenade.Length;

                            var r = Vector2.Lerp(new Vector2(superArea.StartX, superArea.StartY),
                                new Vector2(superArea.EndX, superArea.EndY), perc);

                            Color p = bmp.GetPixel((int) r.X, (int) r.Y);

                            //if (p.R > 128 && p.G > 128 && p.B > 128)
                            //{
                            //    grenade[i].Color = new LEDColor(255,255,255);
                            //}
                            //else
                            //{
                            //    grenade[i].Color = new LEDColor(0,0,0);
                            //}

                            bool possiblyYellow =
                                p.R > 160 && p.G > 145 && p.B > 58 && p.R < 205 && p.G < 190 && p.B < 85;

                            if (!possiblyYellow) deffoNotYellow = true;

                          
                            if (i > 0)
                            {
                                var rr = Math.Abs(prv.R - p.R);
                                var gg = Math.Abs(prv.G - p.G);
                                var bb = Math.Abs(prv.B - p.B);

                                if (rr + gg + bb > 80 && cp==0)
                                {
                                    cp = i;
                                }
                                Debug.WriteLine(i+":"+rr+"-"+gg+"-"+bb+"-"+(rr+gg+bb));

                             
                            }
                            prv = p;
                        }

                        for (int i = 0; i < grenade.Length; i++)
                        {
                            if (i >= cp)
                            {
                                grenade[i].Color = new LEDColor(0, 0, 0);
                            }
                            else
                            {
                                grenade[i].Color = new LEDColor(255, 255, 255);
                            }
                        }

                        if (!deffoNotYellow)
                        {
                            for (int i = 0; i < grenade.Length; i++)
                            {
                                grenade[i].Color = new LEDColor(205, 190, 85);
                            }
                        }
                    }
                }
                catch
                {
                }

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

        private int screenshotCount = 0;
        public Bitmap CaptureApplication(string procName)
        {
            try
            {
                screenshotCount++;
                var proc = Process.GetProcessesByName(procName)[0];
                var rect = new User32.Rect();
                User32.GetWindowRect(proc.MainWindowHandle, ref rect);

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height),
                        CopyPixelOperation.SourceCopy);
                }

                return bmp;
            }
            catch
            {
                return null;
            }
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
