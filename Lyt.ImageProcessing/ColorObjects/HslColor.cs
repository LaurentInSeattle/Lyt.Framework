namespace Lyt.ImageProcessing.ColorObjects;


public sealed class HslColor
{
    /// <summary> Hue Angle in degrees </summary>
    [JsonRequired]
    public double H { get; set; } = 0.0;

    [JsonRequired]
    public double S { get; set; } = 0.5;

    [JsonRequired]
    public double L { get; set; } = 0.5;

    public HslColor() { }

    public HslColor(double h, double s, double l)
    {
        this.H = h;
        this.S = s;
        this.L = l;
    }

    public HslColor(HslColor hsl)
    {
        this.H = hsl.H;
        this.S = hsl.S;
        this.L = hsl.L;
    }

    public HslColor(RgbColor rgb)
    {
        HslColor hsl = rgb.ToHsl();
        this.H = hsl.H;
        this.S = hsl.S;
        this.L = hsl.L;
    }

    public HslColor(uint bgra)
    {
        byte b = (byte)((bgra & 0xFF000000) >> 24);
        byte g = (byte)((bgra & 0x00FF0000) >> 16);
        byte r = (byte)((bgra & 0x0000FF00) >> 8);
        var rgb = new RgbColor(r, g, b);
        HslColor hsl = rgb.ToHsl();
        this.H = hsl.H;
        this.S = hsl.S;
        this.L = hsl.L;
    }

    public void Set(double h, double s, double l)
    {
        this.H = h;
        this.S = s;
        this.L = l;
    }

    public RgbColor ToRgb() => HslColor.ToRgb(this.H, this.S, this.L);

    public static RgbColor ToRgb(double hue, double saturation, double brightness)
    {
        ToRgb(hue, saturation, brightness, out byte red, out byte green, out byte blue);
        return new RgbColor(red, green, blue);
    }

    public static void ToRgb(
        double hue, double saturation, double brightness,
        out byte red, out byte green, out byte blue)
    {
        if (saturation == 0)
        {
            // No saturation: gray 
            double x = Math.Round(brightness * 255.0);
            red = (byte)x;
            green = (byte)x;
            blue = (byte)x;
            return;
        }

        // Helper function to convert a hue value to an RGB channel value
        static double HueToRgb(double m1, double m2, double h)
        {
            // Ensure the hue value is within the range [0, 1]
            if (h < 0)
            {
                h += 1;
            }

            if (h > 1)
            {
                h -= 1;
            }

            if (h * 6 < 1)
            {
                return m1 + (m2 - m1) * h * 6;
            }

            if (h * 2 < 1)
            {
                return m2;
            }

            if (h * 3 < 2)
            {
                return m1 + (m2 - m1) * (2.0 / 3.0 - h) * 6;
            }

            return m1;
        }

        double h = hue / 360.0;
        double s = saturation;
        double l = brightness;

        double m2 = l <= 0.5 ? l * (1.0 + s) : l + s - l * s;
        double m1 = 2.0 * l - m2;

        double r = HueToRgb(m1, m2, h + 1.0 / 3.0);
        double g = HueToRgb(m1, m2, h);
        double b = HueToRgb(m1, m2, h - 1.0 / 3.0);
        
        red = (byte)Math.Round(r * 255.0);
        green = (byte)Math.Round(g * 255.0);
        blue = (byte)Math.Round(b * 255.0);
    }

    public HsvColor ToHsv()
    {
        double h = this.H;
        double l = this.L;
        double s_hsl = this.S;

        double v = l + s_hsl * Math.Min(l, 1 - l);
        double s_hsv = 0;

        if (v != 0)
        {
            s_hsv = 2 * (1 - l / v);
        }

        return new HsvColor { H = h, S = s_hsv, V = v };
    }

    public override string ToString()
        => string.Format("Hue: {0:F1}  Sat: {1:F1}  Li: {2:F1}", this.H, this.S, this.L);
}
