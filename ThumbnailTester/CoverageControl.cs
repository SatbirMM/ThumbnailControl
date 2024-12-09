using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

public class CustomCircleControl : UserControl
{
    // Properties for segment colors (outermost to innermost)
    [Category("Appearance")]
    public Color Circle1Segment1 { get; set; } = Color.Red;
    [Category("Appearance")]
    public Color Circle1Segment2 { get; set; } = Color.Green;
    [Category("Appearance")]
    public Color Circle1Segment3 { get; set; } = Color.Blue;
    [Category("Appearance")]
    public Color Circle1Segment4 { get; set; } = Color.Yellow;

    [Category("Appearance")]
    public Color Circle2Segment1 { get; set; } = Color.Orange;
    [Category("Appearance")]
    public Color Circle2Segment2 { get; set; } = Color.Purple;
    [Category("Appearance")]
    public Color Circle2Segment3 { get; set; } = Color.Cyan;
    [Category("Appearance")]
    public Color Circle2Segment4 { get; set; } = Color.Magenta;

    [Category("Appearance")]
    public Color Circle3Segment1 { get; set; } = Color.Pink;
    [Category("Appearance")]
    public Color Circle3Segment2 { get; set; } = Color.Brown;
    [Category("Appearance")]
    public Color Circle3Segment3 { get; set; } = Color.Lime;
    [Category("Appearance")]
    public Color Circle3Segment4 { get; set; } = Color.Teal;

    [Category("Appearance")]
    public Color Circle4Segment1 { get; set; } = Color.Gray;
    [Category("Appearance")]
    public Color Circle4Segment2 { get; set; } = Color.Maroon;
    [Category("Appearance")]
    public Color Circle4Segment3 { get; set; } = Color.Navy;
    [Category("Appearance")]
    public Color Circle4Segment4 { get; set; } = Color.Olive;

    [Category("Appearance")]
    public Color InnerCircleColor { get; set; } = Color.Black;
    private int mQuadrantSelected = -1;



    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Define the colors for each ring
        Color[,] segmentColors = {
        { Circle1Segment1, Circle1Segment2, Circle1Segment3, Circle1Segment4 }, // Outermost circle
        { Circle2Segment1, Circle2Segment2, Circle2Segment3, Circle2Segment4 },
        { Circle3Segment1, Circle3Segment2, Circle3Segment3, Circle3Segment4 },
        { Circle4Segment1, Circle4Segment2, Circle4Segment3, Circle4Segment4 }
    };

        // Graphics setup
        Graphics g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Center and radius
        int centerX = Width / 2;
        int centerY = Height / 2;
        int radius = Math.Min(Width, Height) / 2;
        int segmentRadiusStep = radius / 5;

        // Outer to inner rings
        for (int i = 3; i >= 0; i--)
        {
            int outerRadius = segmentRadiusStep * (i + 1);
            int innerRadius = segmentRadiusStep * i;

            // Draw segments
            for (int j = 0; j < 4; j++)
            {
                using (SolidBrush brush = new SolidBrush(segmentColors[i, j]))
                {
                    g.FillPie(brush,
                              centerX - outerRadius, centerY - outerRadius,
                              outerRadius * 2, outerRadius * 2,
                              (j * 90) + 45, 90);
                }

                // Draw border for the selected quadrant
                if (mQuadrantSelected == j)
                {
                    using (Pen borderPen = new Pen(Color.Black, 4))
                    {
                        g.DrawPie(borderPen,
                                  centerX - outerRadius, centerY - outerRadius,
                                  outerRadius * 2, outerRadius * 2,
                                  (j * 90) + 45, 90);
                    }
                }
            }

            // Clear inner part if not innermost
            if (i > 0)
            {
                using (SolidBrush clearBrush = new SolidBrush(BackColor)) // Clear with background color
                {
                    g.FillEllipse(clearBrush,
                                  centerX - innerRadius, centerY - innerRadius,
                                  innerRadius * 2, innerRadius * 2);
                }
            }
        }

        // Draw inner solid circle
        using (SolidBrush innerBrush = new SolidBrush(InnerCircleColor))
        {
            g.FillEllipse(innerBrush, centerX - segmentRadiusStep, centerY - segmentRadiusStep, segmentRadiusStep * 2, segmentRadiusStep * 2);
        }

        // Draw outlines
        using (Pen outlinePen = new Pen(Color.Black))
        {
            for (int i = 1; i <= 4; i++)
            {
                int outlineRadius = segmentRadiusStep * i;
                g.DrawEllipse(outlinePen, centerX - outlineRadius, centerY - outlineRadius, outlineRadius * 2, outlineRadius * 2);
            }
        }
    }

    // Lets just add a utility method to set colors of a circle
    // Take first argument as circle number and next 4 as colors of segments
    public void SetCircleColors(int circleNumber, Color segment1, Color segment2, Color segment3, Color segment4)
    {
        if (circleNumber == 1)
        {
            Circle1Segment1 = segment1;
            Circle1Segment2 = segment2;
            Circle1Segment3 = segment3;
            Circle1Segment4 = segment4;
        }
        else if (circleNumber == 2)
        {
            Circle2Segment1 = segment1;
            Circle2Segment2 = segment2;
            Circle2Segment3 = segment3;
            Circle2Segment4 = segment4;
        }
        else if (circleNumber == 3)
        {
            Circle3Segment1 = segment1;
            Circle3Segment2 = segment2;
            Circle3Segment3 = segment3;
            Circle3Segment4 = segment4;
        }
        else if (circleNumber == 4)
        {
            Circle4Segment1 = segment1;
            Circle4Segment2 = segment2;
            Circle4Segment3 = segment3;
            Circle4Segment4 = segment4;
        }
        Invalidate();
    }
    // Now lets detect where mouse clicked and highlight quadrant
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        // Calculate the center of the control
        int centerX = Width / 2;
        int centerY = Height / 2;

        // Get the relative coordinates of the click
        int dx = e.X - centerX;
        int dy = centerY - e.Y; // Invert Y because screen coordinates increase downward

        // Calculate the angle in degrees
        double angle = -1 *  Math.Atan2(dy, dx) * (180 / Math.PI); // Convert radians to degrees
        
        if (angle < 0)
            angle += 360; // Normalize the angle to be in the range [0, 360]

        // Determine the quadrant based on the angle
        if (angle >= 45 && angle < 135)
            mQuadrantSelected = 0; // Top-left (45° to 135°)
        else if (angle >= 135 && angle < 225)
            mQuadrantSelected = 1; // Bottom-left (135° to 225°)
        else if (angle >= 225 && angle < 315)
            mQuadrantSelected = 2; // Bottom-right (225° to 315°)
        else
            mQuadrantSelected = 3; // Top-right (315° to 45°)

        // Trigger a repaint to update the control
        Invalidate();
    }

}
