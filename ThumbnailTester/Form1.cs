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
        public ThumbnailViewerTesting()
        {
            InitializeComponent();
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Size = new Size(400, 400);




            thumbnailViewerControl1.ThumbnailClicked += (sender, index) =>
            {
                textBox.Text += $"Thumbnail {index} clicked\n";
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
                g.Clear(randomColor);
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
            textBox.Text = "Previous button clicked\n";
            thumbnailViewerControl1.SelectPreviousThumbnail();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            // add log line to the text box
            textBox.Text += "Next button clicked\n";

            thumbnailViewerControl1.SelectNextThumbnail();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            mMockThumbnailProvider.ClearThumbnails();
        }
    }
}
