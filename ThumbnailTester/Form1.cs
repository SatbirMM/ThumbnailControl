using ScleraThumbnailControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThumbnailTester;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ThumbnailTester
{
    public partial class ThumbnailViewerTesting : Form
    {
        ThumbnailProvider mMockThumbnailProvider = new ThumbnailProvider();
        private int mMockThumbnailCount = 0;
        public ThumbnailViewerTesting()
        {
            InitializeComponent();
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Size = new Size(400, 400);
            mMockThumbnailProvider.textBox = textBox;
            thumbnailViewerControl1.mLogger = textBox;

            thumbnailViewerControl1.ThumbnailClicked += (sender, index) =>
            {
                Log($"Thumbnail {index} clicked\n");
            };
        }

        private void thumbnailViewerControl1_Load(object sender, EventArgs e)
        {
            thumbnailViewerControl1.SetThumbnailProvider(mMockThumbnailProvider);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // add a dummy thumbnail with a random color  in mMockThumbnailProvider
            // Create a bitmap with random color
            Bitmap bitmap = new Bitmap(100, 100);
            Random random = new Random();
            Color randomColor = Color.Black;
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Draw a count on bitmap
                RectangleF rect = new RectangleF(0, 0, 50, 50);

                g.Clear(randomColor);
                g.DrawString(mMockThumbnailCount.ToString(), new Font("Tahoma", 8), Brushes.White, rect);
                mMockThumbnailCount++;
            };

            mMockThumbnailProvider.SetBitmap(bitmap);

        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void ThumbnailViewerTesting_Load(object sender, EventArgs e)
        {

        }

        private void textBox_TextChanged_2(object sender, EventArgs e)
        {
            // change size to fill the panel
            textBox.Size = new Size(400, 400);
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            // add log line to the text box
            Log("Previous button clicked\n");
            thumbnailViewerControl1.SelectPreviousThumbnail();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            // add log line to the text box
            Log("Next button clicked\n");

            thumbnailViewerControl1.SelectNextThumbnail();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            mMockThumbnailProvider.ClearThumbnails();
        }
        private void Log(string message)
        {
            textBox.AppendText(message);
            textBox.AppendText(Environment.NewLine);
        }

        private void customCircleControl1_Load(object sender, EventArgs e)
        {

            var r = Color.FromArgb(184, 44, 0);
            var g = Color.FromArgb(59, 184, 0);
            var y = Color.FromArgb(194, 157, 23);
            var b = Color.Black;

            customCircleControl1.InnerCircleColor = g;
            customCircleControl1.SetCircleColors(1, r, g, g, g);
            customCircleControl1.SetCircleColors(2, r, g, y, g);
            customCircleControl1.SetCircleColors(3, r, g, y, g);
            customCircleControl1.SetCircleColors(4, r, g, y, g);
        }
    }
}
