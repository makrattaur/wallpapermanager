using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WallpaperManager
{
    class WallpaperDrawing
    {
        enum RectCalcMode
        {
            Fit, Fill
        }
        static Size calcRectSizeToFillOrFitRect(RectCalcMode mode, Size r1, Size r2)
        {
            float hScale = (float)r2.Height / (float)r1.Height;
            float wScale = (float)r2.Width / (float)r1.Width;

            float scale;
            if (mode == RectCalcMode.Fit)
                scale = Math.Min(hScale, wScale);
            else if (mode == RectCalcMode.Fill)
                scale = Math.Max(hScale, wScale);
            else
                scale = 1;

            return new Size() { Width = (int)Math.Round(r1.Width * scale), Height = (int)Math.Round(r1.Height * scale) };
        }


        static void DrawWholeImageFixedWrap(Graphics g, Image img, Rectangle dstRect)
        {
            using (ImageAttributes wrapMode = new ImageAttributes())
            {
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                g.DrawImage(img,
                    dstRect,
                    0,
                    0,
                    img.Width,
                    img.Height,
                    GraphicsUnit.Pixel,
                    wrapMode
                );
            }
        }

        static Rectangle CenterSizeOnRectangle(Size size, Rectangle targetRectangle)
        {
            return new Rectangle(
                new Point(
                    targetRectangle.X + (targetRectangle.Width / 2 - size.Width / 2),
                    targetRectangle.Y + (targetRectangle.Height / 2 - size.Height / 2)
                ),
                size
            );
        }

        static Size CalculateRectSizeForMode(WallpaperDisposition disp, Size imageSize, Size monitorSize)
        {
            switch (disp)
            {
                case WallpaperDisposition.Fit:
                {
                    return calcRectSizeToFillOrFitRect(
                        RectCalcMode.Fit,
                        imageSize,
                        monitorSize
                    );
                }
                case WallpaperDisposition.Fill:
                {
                    return calcRectSizeToFillOrFitRect(
                        RectCalcMode.Fill,
                        imageSize,
                        monitorSize
                    );
                }
                case WallpaperDisposition.MatchWidth:
                {
                    return new Size(
                        monitorSize.Width,
                        (int)Math.Round(monitorSize.Width * imageSize.Height / (double)monitorSize.Height)
                    );
                }
                case WallpaperDisposition.MatchHeight:
                {
                    return new Size(
                        (int)Math.Round(monitorSize.Height * imageSize.Width / (double)monitorSize.Height),
                        monitorSize.Height
                    );
                }
                case WallpaperDisposition.Center:
                {
                    return imageSize;
                }
                case WallpaperDisposition.Stretch:
                case WallpaperDisposition.Tile:
                default:
                {
                    return monitorSize;
                }
            }
        }

        public static Image GenerateWallpaper(Monitor[] monitors, WallpaperGenSetttings[] settings)
        {
            var xMax = monitors.SelectMany(m => new[] { m.Rect.Left, m.Rect.Right }).Max();
            var xMin = monitors.SelectMany(m => new[] { m.Rect.Left, m.Rect.Right }).Min();
            var yMax = monitors.SelectMany(m => new[] { m.Rect.Top, m.Rect.Bottom }).Max();
            var yMin = monitors.SelectMany(m => new[] { m.Rect.Top, m.Rect.Bottom }).Min();

            var w = xMax - xMin;
            var h = yMax - yMin;

            var bitmap = new Bitmap(w, h);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                for (int i = 0; i < monitors.Length; i++)
                {
                    var mon = monitors[i];

                    var drawRect = new Rectangle(
                        new Point(
                            mon.Rect.X >= 0 ? mon.Rect.X : w + mon.Rect.X,
                            mon.Rect.Y >= 0 ? mon.Rect.Y : h + mon.Rect.Y
                        ),
                        mon.Rect.Size
                    );

                    g.SetClip(drawRect, System.Drawing.Drawing2D.CombineMode.Replace);

                    using (var b = new SolidBrush(settings[i].BackgroundColor))
                    {
                        g.FillRectangle(b, drawRect);
                    }


                    var curSettings = settings[i];
                    var curDisp = curSettings.Disposition;
                    var curImage = curSettings.Image;

                    switch (curDisp)
                    {
                        case WallpaperDisposition.Fit:
                        case WallpaperDisposition.Fill:
                        case WallpaperDisposition.Stretch:
                        case WallpaperDisposition.MatchWidth:
                        case WallpaperDisposition.MatchHeight:
                        case WallpaperDisposition.Center:
                        {
                            Size wpRect = CalculateRectSizeForMode(curDisp, curImage.Size, drawRect.Size);

                            DrawWholeImageFixedWrap(g,
                                curImage,
                                CenterSizeOnRectangle(wpRect, drawRect)
                            );

                            break;
                        }
                        case WallpaperDisposition.Tile:
                        {
                            var tile = curImage;
                            int xTiles = (int)Math.Ceiling(drawRect.Size.Width / (double)tile.Width);
                            int yTiles = (int)Math.Ceiling(drawRect.Size.Height / (double)tile.Height);

                            for (int y = 0; y < yTiles; y++)
                            {
                                for (int x = 0; x < xTiles; x++)
                                {
                                    g.DrawImageUnscaled(tile,
                                        new Point(
                                            drawRect.X + x * tile.Width,
                                            drawRect.Y + y * tile.Height
                                        )
                                    );
                                }
                            }

                            break;
                        }
                        default:
                        {
                            DrawWholeImageFixedWrap(g,
                                curImage,
                                drawRect
                            );

                            break;
                        }
                    }

                    g.ResetClip();
                }
            }

            return bitmap;
        }
    }
}
