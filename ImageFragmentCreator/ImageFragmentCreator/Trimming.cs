using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ToyBox;

namespace ImageFragmentCreator
{
    public static class Trimming
    {
        private static Color backColor = Color.FromArgb(0, 0, 0, 0);

        public static Point centerLast;

        public static Bitmap Takumin(Bitmap satou, Size yosida)
        {
            Bitmap tanaken = new Bitmap(yosida.Width, yosida.Height);
            Graphics g = Graphics.FromImage(tanaken), gr = Graphics.FromImage(satou);
            Point sanagi = new Point();

            g.Clear(Color.White);

            while (true)
            {
                sanagi.X = (int)MyRandom.Next((uint)satou.Width);
                sanagi.Y = (int)MyRandom.Next((uint)satou.Height);

                if (satou.GetPixel(sanagi.X, sanagi.Y).A >= 255)
                {
                    Console.WriteLine(satou.GetPixel(sanagi.X, sanagi.Y).A + ", " + satou.GetPixel(sanagi.X, sanagi.Y).R + "," + satou.GetPixel(sanagi.X, sanagi.Y).G + "," + satou.GetPixel(sanagi.X, sanagi.Y).B);
                    break;
                }
                Console.WriteLine(satou.GetPixel(sanagi.X, sanagi.Y).A + ", " + satou.GetPixel(sanagi.X, sanagi.Y).R + "," + satou.GetPixel(sanagi.X, sanagi.Y).G + "," + satou.GetPixel(sanagi.X, sanagi.Y).B);
            }

            gr.FillEllipse(Brushes.Red, sanagi.X - 5, sanagi.Y - 5, 10, 10);
            gr.Dispose();
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(satou, new Rectangle(0, 0, yosida.Width, yosida.Height), new Rectangle(sanagi.X - yosida.Width / 2, sanagi.Y - yosida.Height / 2, yosida.Width, yosida.Height), GraphicsUnit.Pixel);

            g.Dispose();

            return tanaken;
        }

        public static Bitmap TrimmingBitmap(Element element, Bitmap origin, Size size)
        {
            Bitmap ret = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage(ret), gr = Graphics.FromImage(origin);

            Point center = RandomTrimmingCenter(element, origin);
            centerLast = center;

            //gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //gr.TranslateTransform(center.X, center.Y);
            //gr.RotateTransform(element.angle);

            //gr.FillEllipse(Brushes.Red, -5, -5, 10, 10);
            //gr.Dispose();

            g.Clear(backColor);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g.DrawImage(origin, new Rectangle(0, 0, size.Width, size.Height), new Rectangle(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height), GraphicsUnit.Pixel);

            g.Dispose();

            return ret;
        }

        public static Point RandomTrimmingCenter(Element element, Bitmap img)
        {
            double angle = element.angle * Math.PI / 180.0; 


            Point ret = new Point();
            Size ellipseSize = new Size((int)(Math.Abs(element.xr * Math.Cos(angle)) + Math.Abs(element.yr * Math.Sin(angle))), 
                (int)(Math.Abs(element.xr * Math.Sin(angle)) + Math.Abs(element.yr * Math.Cos(angle))));
            Bitmap ellipseBound = new Bitmap(ellipseSize.Width, ellipseSize.Height);
            Graphics eg = Graphics.FromImage(ellipseBound);

            eg.Clear(backColor);
            eg.InterpolationMode = InterpolationMode.HighQualityBicubic;
            eg.PixelOffsetMode = PixelOffsetMode.HighQuality;

            eg.TranslateTransform(ellipseSize.Width / 2, ellipseSize.Height / 2);
            eg.RotateTransform(element.angle);
            eg.FillEllipse(Brushes.White, new Rectangle(-element.xr / 2, -element.yr / 2, element.xr, element.yr));

            eg.Dispose();
            //ellipseBound.Save("ellipse.png");

            BitmapData data = ellipseBound.LockBits(
                new Rectangle(0, 0, ellipseSize.Width, ellipseSize.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            byte[] buf = new byte[ellipseSize.Width * ellipseSize.Height * 4];
            Marshal.Copy(data.Scan0, buf, 0, buf.Length);
            ellipseBound.UnlockBits(data);

            List<int> points = new List<int>();
            
            for (int i = 0; i < ellipseSize.Width * ellipseSize.Height; i++)
            {
                if (255 <= buf[i * 4 + 3])
                {
                    points.Add(i);
                }
            }

            int[] pointArray = points.ToArray();

            while (true)
            {
                int index = pointArray[MyRandom.Next((uint)pointArray.Length)];

                Point p = new Point((index % ellipseSize.Width - ellipseSize.Width / 2), (index / ellipseSize.Width - ellipseSize.Height / 2));
                ret.X = p.X + element.pos.X;
                ret.Y = p.Y + element.pos.Y;

                try
                {
                    if (img.GetPixel(ret.X, ret.Y).A >= 255)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("GetPixelメソッド参照範囲外");
                    continue;
                }
                
            }

            return ret;
        }
    }
}
