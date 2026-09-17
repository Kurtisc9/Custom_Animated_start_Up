using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CASU.Themes.NeuralCore
{
    public sealed class NeuralCoreRenderer
    {
        private readonly Random random;
        private readonly float[] starX;
        private readonly float[] starY;
        private readonly float[] starDepth;

        public NeuralCoreRenderer(int seed)
        {
            random = new Random(seed);

            const int starCount = 150;

            starX = new float[starCount];
            starY = new float[starCount];
            starDepth = new float[starCount];

            for (int i = 0; i < starCount; i++)
            {
                starX[i] = (float)random.NextDouble();
                starY[i] = (float)random.NextDouble();
                starDepth[i] =
                    0.25f + ((float)random.NextDouble() * 0.75f);
            }
        }

        public void Render(
            Graphics graphics,
            Rectangle bounds,
            double time,
            bool focalDisplay,
            string identity,
            string stage)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality =
                CompositingQuality.HighQuality;

            using (var background =
                new LinearGradientBrush(
                    bounds,
                    Color.FromArgb(255, 1, 3, 12),
                    Color.FromArgb(255, 8, 2, 22),
                    45f))
            {
                graphics.FillRectangle(background, bounds);
            }

            RenderStars(graphics, bounds, time);
            RenderArchitecture(graphics, bounds, time);

            if (focalDisplay)
            {
                RenderCore(graphics, bounds, time);
                RenderIdentity(
                    graphics,
                    bounds,
                    identity,
                    stage,
                    time);
            }
            else
            {
                RenderExtendedEnvironment(
                    graphics,
                    bounds,
                    time);
            }

            RenderHud(
                graphics,
                bounds,
                stage,
                focalDisplay,
                time);
        }

        private void RenderStars(
            Graphics g,
            Rectangle b,
            double time)
        {
            for (int i = 0; i < starX.Length; i++)
            {
                float drift =
                    (float)((time * starDepth[i] * 10.0) %
                    Math.Max(1, b.Width));

                float x =
                    ((starX[i] * b.Width) + drift) % b.Width;

                float y = starY[i] * b.Height;

                int alpha =
                    40 + (int)(150 * starDepth[i]);

                float size =
                    1f + (2.5f * starDepth[i]);

                using (var brush =
                    new SolidBrush(
                        Color.FromArgb(
                            Math.Min(220, alpha),
                            160,
                            220,
                            255)))
                {
                    g.FillEllipse(
                        brush,
                        x,
                        y,
                        size,
                        size);
                }
            }
        }

        private void RenderArchitecture(
            Graphics g,
            Rectangle b,
            double time)
        {
            int horizon =
                b.Top + (int)(b.Height * 0.72);

            using (var pen =
                new Pen(
                    Color.FromArgb(70, 50, 160, 255),
                    1f))
            {
                for (int i = -8; i <= 8; i++)
                {
                    int centerX =
                        b.Left + (b.Width / 2);

                    int bottomX =
                        centerX + (i * b.Width / 8);

                    g.DrawLine(
                        pen,
                        centerX,
                        horizon,
                        bottomX,
                        b.Bottom);
                }

                for (int y = horizon;
                     y < b.Bottom;
                     y += Math.Max(12, b.Height / 20))
                {
                    g.DrawLine(
                        pen,
                        b.Left,
                        y,
                        b.Right,
                        y);
                }
            }
        }

        private void RenderCore(
            Graphics g,
            Rectangle b,
            double time)
        {
            float pulse =
                1.0f +
                (0.06f *
                (float)Math.Sin(time * 2.2));

            float baseSize =
                Math.Min(b.Width, b.Height) * 0.34f;

            float size = baseSize * pulse;

            float cx = b.Left + (b.Width / 2f);
            float cy = b.Top + (b.Height * 0.43f);

            RectangleF outer =
                new RectangleF(
                    cx - size / 2f,
                    cy - size / 2f,
                    size,
                    size);

            using (var path = new GraphicsPath())
            {
                path.AddEllipse(outer);

                using (var gradient =
                    new PathGradientBrush(path))
                {
                    gradient.CenterColor =
                        Color.FromArgb(
                            255,
                            220,
                            250,
                            255);

                    gradient.SurroundColors =
                        new[]
                        {
                            Color.FromArgb(
                                30,
                                20,
                                40,
                                120)
                        };

                    g.FillPath(gradient, path);
                }
            }

            for (int ring = 0; ring < 4; ring++)
            {
                float ringScale =
                    1.20f + (ring * 0.16f);

                float ringW = size * ringScale;
                float ringH =
                    size *
                    (0.25f + ring * 0.045f);

                RectangleF orbit =
                    new RectangleF(
                        cx - ringW / 2f,
                        cy - ringH / 2f,
                        ringW,
                        ringH);

                int alpha = 150 - (ring * 22);

                using (var pen =
                    new Pen(
                        Color.FromArgb(
                            alpha,
                            ring % 2 == 0 ? 80 : 150,
                            ring % 2 == 0 ? 220 : 80,
                            255),
                        2f))
                {
                    g.DrawEllipse(pen, orbit);
                }
            }

            float energyAngle =
                (float)(time * 1.7);

            for (int i = 0; i < 24; i++)
            {
                double angle =
                    ((Math.PI * 2.0) / 24.0 * i) +
                    energyAngle;

                float radius =
                    size *
                    (0.55f +
                    (0.08f *
                    (float)Math.Sin(time * 2 + i)));

                float x =
                    cx + ((float)Math.Cos(angle) * radius);

                float y =
                    cy + ((float)Math.Sin(angle) *
                    radius * 0.45f);

                using (var brush =
                    new SolidBrush(
                        Color.FromArgb(
                            150,
                            i % 3 == 0 ? 170 : 70,
                            i % 3 == 0 ? 70 : 210,
                            255)))
                {
                    g.FillEllipse(
                        brush,
                        x - 3,
                        y - 3,
                        6,
                        6);
                }
            }
        }

        private void RenderIdentity(
            Graphics g,
            Rectangle b,
            string identity,
            string stage,
            double time)
        {
            float scan =
                (float)((Math.Sin(time * 2.0) + 1.0) / 2.0);

            float fontSize =
                Math.Max(24f, b.Height * 0.065f);

            using (var font =
                new Font(
                    "Segoe UI",
                    fontSize,
                    FontStyle.Bold,
                    GraphicsUnit.Pixel))
            using (var format =
                new StringFormat())
            {
                format.Alignment =
                    StringAlignment.Center;

                format.LineAlignment =
                    StringAlignment.Center;

                RectangleF area =
                    new RectangleF(
                        b.Left,
                        b.Top + b.Height * 0.67f,
                        b.Width,
                        b.Height * 0.12f);

                using (var shadow =
                    new SolidBrush(
                        Color.FromArgb(
                            140,
                            0,
                            0,
                            0)))
                {
                    RectangleF shadowArea = area;
                    shadowArea.Offset(3, 4);

                    g.DrawString(
                        identity,
                        font,
                        shadow,
                        shadowArea,
                        format);
                }

                int blue =
                    180 + (int)(75 * scan);

                using (var identityBrush =
                    new SolidBrush(
                        Color.FromArgb(
                            255,
                            210,
                            blue,
                            255)))
                {
                    g.DrawString(
                        identity,
                        font,
                        identityBrush,
                        area,
                        format);
                }

                float scanX =
                    area.Left +
                    (area.Width * scan);

                using (var scanPen =
                    new Pen(
                        Color.FromArgb(
                            180,
                            230,
                            255,
                            255),
                        2f))
                {
                    g.DrawLine(
                        scanPen,
                        scanX,
                        area.Top,
                        scanX,
                        area.Bottom);
                }
            }
        }

        private void RenderExtendedEnvironment(
            Graphics g,
            Rectangle b,
            double time)
        {
            float cx =
                b.Left + (b.Width / 2f);

            float cy =
                b.Top + (b.Height * 0.45f);

            for (int i = 0; i < 6; i++)
            {
                float width =
                    b.Width * (0.25f + i * 0.09f);

                float height =
                    b.Height * (0.05f + i * 0.015f);

                using (var pen =
                    new Pen(
                        Color.FromArgb(
                            40 + i * 12,
                            i % 2 == 0 ? 50 : 150,
                            100,
                            255),
                        1.5f))
                {
                    g.DrawEllipse(
                        pen,
                        cx - width / 2f,
                        cy - height / 2f,
                        width,
                        height);
                }
            }
        }

        private void RenderHud(
            Graphics g,
            Rectangle b,
            string stage,
            bool focal,
            double time)
        {
            using (var font =
                new Font(
                    "Consolas",
                    Math.Max(11f, b.Height * 0.018f),
                    FontStyle.Regular,
                    GraphicsUnit.Pixel))
            using (var brush =
                new SolidBrush(
                    Color.FromArgb(
                        150,
                        130,
                        220,
                        255)))
            {
                string status =
                    focal
                    ? "NEURAL CORE // " + stage
                    : "SYSTEM LINK // SYNCHRONIZED";

                g.DrawString(
                    status,
                    font,
                    brush,
                    b.Left + 24,
                    b.Top + 24);

                string telemetry =
                    "CASU // " +
                    DateTime.Now.ToString("HH:mm:ss") +
                    " // ENERGY " +
                    (92 +
                    (int)(Math.Abs(
                        Math.Sin(time)) * 7)) +
                    "%";

                g.DrawString(
                    telemetry,
                    font,
                    brush,
                    b.Left + 24,
                    b.Bottom - 48);
            }
        }
    }
}
