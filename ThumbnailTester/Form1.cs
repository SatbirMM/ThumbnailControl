using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThumbnailViewer;

namespace ThumbnailTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // add a text box to log text into it
            var textBox = new System.Windows.Forms.TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // Add a panel to the form to display the thumbnails
            // set panel size to fill the form horizontally and 100 pixels vertically
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200
            };

            // thumbnailViewer shall be fill the form 
            var thumbnailViewer = new ThumbnailViewerControl
            {
                Dock = DockStyle.Fill
            };

            // Set the thumbnail provider to a mock thumbnail provider when window is loaded

            Load += (sender, e) =>
            {
                thumbnailViewer.SetThumbnailProvider(new MockThumbnailProvider(ref textBox));
                // Text box shall be below the panel
                

                textBox.Text = "Window loaded\n";
                
            };

           
            thumbnailViewer.ThumbnailClicked += (sender, index) =>
            {
               textBox.Text += $"Thumbnail {index} clicked\n";
            };
            panel.Controls.Add( thumbnailViewer );

            this.Controls.Add(panel);

            // now add two buttons to the form to select previous and next thumbnail
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100
            };

            var textBoxPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 100
            };

            var previousButton = new Button
            {
                Text = "Previous",
                Dock = DockStyle.Left
            };

            previousButton.Click += (sender, e) =>
            {
                // add log line to the text box
                textBox.Text += "Previous button clicked\n";
                thumbnailViewer.SelectPreviousThumbnail();
            };
            buttonPanel.Controls.Add( previousButton );

            var nextButton = new Button
            {
                Text = "Next",
                Dock = DockStyle.Right
            };

            nextButton.Click += (sender, e) =>
            {
                // add log line to the text box
                textBox.Text += "Next button clicked\n";

                thumbnailViewer.SelectNextThumbnail();
            };

            buttonPanel.Controls.Add( nextButton );

            this.Controls.Add( buttonPanel );


            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.Dock = DockStyle.Fill;
            textBox.ScrollBars = ScrollBars.Vertical;



            textBoxPanel.Controls.Add(textBox);
            this.Controls.Add(textBoxPanel);
          



        }
    }
}
