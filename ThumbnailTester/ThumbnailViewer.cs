using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ThumbnailViewer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ThumbnailViewer
{
    public interface IThumbnailProvider
    {
        int GetTotalThumbnails(); // Fetch total number of thumbnails
        Image GetThumbnail(int index); // Fetch thumbnail image by index
        void ThumbnailClicked(int index); // Event when a thumbnail is clicked

    }

    public class ThumbnailViewerControl : UserControl
    {
        //https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/flowlayoutpanel-control-overview?view=netframeworkdesktop-4.8
        private readonly FlowLayoutPanel _thumbnailPanel;
        private IThumbnailProvider _thumbnailProvider;
        private readonly Dictionary<int, PictureBox> _thumbnailCache;
        private readonly VScrollBar _scrollBar;
        private const int ThumbnailSize = 100; // Thumbnail width and height
        private const int MarginSize = 10; // Margin around thumbnails
        private int _totalThumbnails;

        public event EventHandler<int> ThumbnailClicked; // Event when a thumbnail is clicked
        
        // Control allow user to set width & height
        // Also let user to select horizontal or vertical layout

        public ThumbnailViewerControl()
        {
            _thumbnailPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };

            _scrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Width = 20
            };

            _scrollBar.Scroll += ScrollBar_Scroll;

            Controls.Add(_thumbnailPanel);
            Controls.Add(_scrollBar);

            _thumbnailCache = new Dictionary<int, PictureBox>();

            // When user click on a thumbnail draw a border around it
            ThumbnailClicked += (sender, index) =>
            {
                foreach (var pictureBox in _thumbnailCache.Values)
                {
                    pictureBox.BorderStyle = BorderStyle.None;
                }

                if (_thumbnailCache.ContainsKey(index))
                {
                    _thumbnailCache[index].BorderStyle = BorderStyle.FixedSingle;
                }
                // Now make sure adjust scroll bar so that selected thumbnail is visible
                int rowIndex = index / GetThumbnailsPerRow();
                _scrollBar.Value = Math.Max(0, rowIndex - GetVisibleRowCount() / 2);


            };
           
        }
        
        // Get index of selected thumbnail
        public int GetSelectedThumbnailIndex()
        {
            foreach (var pictureBox in _thumbnailCache.Values)
            {
                if (pictureBox.BorderStyle == BorderStyle.FixedSingle)
                {
                    return (int)pictureBox.Tag;
                }
            }
            return -1;
        }

        // select next thumbnail
        public void SelectNextThumbnail()
        {
            int selectedIndex = GetSelectedThumbnailIndex();
            if (selectedIndex == -1) return;

            int nextIndex = selectedIndex + 1;
            if (nextIndex < _totalThumbnails)
            {
                ThumbnailClicked?.Invoke(this, nextIndex); 
            }
        }

        // select previous thumbnail
        public void SelectPreviousThumbnail()
        {
            int selectedIndex = GetSelectedThumbnailIndex();
            if (selectedIndex == -1) return;

            int previousIndex = selectedIndex - 1;
            if (previousIndex >= 0)
            {
                ThumbnailClicked?.Invoke(this, previousIndex);
            }
        }
        public void SetThumbnailProvider(IThumbnailProvider provider)
        {
            _thumbnailProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            LoadThumbnails();
        }

        private void LoadThumbnails()
        {
            if (_thumbnailProvider == null) return;

            _totalThumbnails = _thumbnailProvider.GetTotalThumbnails();
            _scrollBar.Maximum = Math.Max(0, _totalThumbnails - GetVisibleRowCount() * GetThumbnailsPerRow());
            _scrollBar.Value = 0;

            LoadVisibleThumbnails();
        }

        private void LoadVisibleThumbnails()
        {
            _thumbnailPanel.Controls.Clear();
            _thumbnailCache.Clear();

            int startIndex = _scrollBar.Value * GetThumbnailsPerRow();
            int endIndex = Math.Min(startIndex + GetVisibleRowCount() * GetThumbnailsPerRow(), _totalThumbnails);

            for (int i = startIndex; i < endIndex; i++)
            {
                var pictureBox = new PictureBox
                {
                    Size = new Size(ThumbnailSize, ThumbnailSize),
                    Margin = new Padding(MarginSize),
                    BackColor = Color.Gray, // Placeholder color
                    Tag = i
                };

                pictureBox.Click += Thumbnail_Click;

                _thumbnailPanel.Controls.Add(pictureBox);
                _thumbnailCache[i] = pictureBox;

                // Load the thumbnail image lazily
                LoadThumbnailImage(i);
            }
        }

        private void LoadThumbnailImage(int index)
        {
            if (_thumbnailProvider == null || !_thumbnailCache.ContainsKey(index)) return;

            var image = _thumbnailProvider.GetThumbnail(index);
            _thumbnailCache[index].Image = image;
        }

        private void Thumbnail_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox && pictureBox.Tag is int index)
            {
                ThumbnailClicked?.Invoke(this, index);
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            LoadVisibleThumbnails();
        }

        private int GetThumbnailsPerRow()
        {
            return Math.Max(1, _thumbnailPanel.Width / (ThumbnailSize + MarginSize * 2));
        }

        private int GetVisibleRowCount()
        {
            return Math.Max(1, Height / (ThumbnailSize + MarginSize * 2));
        }
    }
}

public class MockThumbnailProvider : IThumbnailProvider
{       
    public System.Windows.Forms.TextBox textBox = null;

    public int GetTotalThumbnails() => 100;

    // log to text box, GetThumbnail method is called
    public MockThumbnailProvider(ref System.Windows.Forms.TextBox textbox)
    {
       textBox = textbox;
    }


    public Image GetThumbnail(int index)
    {
        textBox.Text += $"GetThumbnail({index})\n";
        // Return a placeholder image for demonstration
        var bitmap = new Bitmap(100, 100);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.AliceBlue);
            g.DrawString(index.ToString(), new Font("Arial", 16), Brushes.Black, 10, 40);
        }
        return bitmap;
    }

    void IThumbnailProvider.ThumbnailClicked(int index)
    {
        
    }
}
