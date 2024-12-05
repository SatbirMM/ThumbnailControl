using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
        private readonly System.Windows.Forms.ScrollBar _scrollBar;

        private const int ThumbnailSize = 100; // Thumbnail width and height
        private const int MarginSize = 10; // Margin around thumbnails
        private int _totalThumbnails;
        int _firstVisibleThumbnail = 0;
        int _lastVisibleThumbnail = 0;
        int _currentSelectedThumbnail = 0;

        public event EventHandler<int> ThumbnailClicked; // Event when a thumbnail is clicked

        // Control allow user to set width & height
        // Also let user to select horizontal or vertical layout

        public ThumbnailViewerControl(bool isHorizontal = true)
        {
            _thumbnailPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };
            if (isHorizontal)
            {
                // add Horizontal scrollbar
                _scrollBar = new HScrollBar
                {
                    Dock = DockStyle.Bottom,
                    Width = 20
                };
            }
            else
            {
                _scrollBar = new VScrollBar
                {
                    Dock = DockStyle.Right,
                    Width = 20
                };

            }
            // instead of calling it every time during scroll we call when scrolling stops

            _scrollBar.Scroll += ScrollBar_Scroll;
            _scrollBar.Width = 20;


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
                    _currentSelectedThumbnail = index;
                }

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
            if (selectedIndex == -1 || selectedIndex== _totalThumbnails) return;

            int nextIndex = selectedIndex + 1;
            if (nextIndex <= _totalThumbnails)
            {

                if (!_thumbnailCache.ContainsKey(nextIndex))
                {
                    CreateThumbnail(nextIndex);
                }
                // just adjust _firstVisibleThumbnail, _lastVisibleThumbnail
                // only increase one count at a time
                if (nextIndex >= _lastVisibleThumbnail)
                {
                    _firstVisibleThumbnail = _lastVisibleThumbnail;
                    _lastVisibleThumbnail = Math.Min(_totalThumbnails, _firstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow());
                    _scrollBar.Value = _firstVisibleThumbnail / GetThumbnailsPerRow();
                }
                _currentSelectedThumbnail = nextIndex;

                LoadVisibleThumbnails(_firstVisibleThumbnail, _lastVisibleThumbnail);
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
                if (!_thumbnailCache.ContainsKey(previousIndex))
                {
                    CreateThumbnail(previousIndex);
                }
                // just adjust _firstVisibleThumbnail, _lastVisibleThumbnail
                // only increase one count at a time
                if (previousIndex < _firstVisibleThumbnail)
                {
                    _firstVisibleThumbnail = previousIndex;
                    _lastVisibleThumbnail = Math.Min(_totalThumbnails, _firstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow());
                    _scrollBar.Value = _firstVisibleThumbnail / GetThumbnailsPerRow();
                }
                _currentSelectedThumbnail = previousIndex;
                LoadVisibleThumbnails(_firstVisibleThumbnail, _lastVisibleThumbnail);
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
            // Set start index and last index
            _firstVisibleThumbnail = 0;
            _lastVisibleThumbnail = Math.Min(_totalThumbnails, GetVisibleRowCount() * GetThumbnailsPerRow());

            LoadVisibleThumbnails(_firstVisibleThumbnail, _lastVisibleThumbnail);
        }

        private void CreateThumbnail(int index)
        {
            var pictureBox = new PictureBox
            {
                Size = new Size(ThumbnailSize, ThumbnailSize),
                Margin = new Padding(MarginSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                Tag = index
            };
            pictureBox.Click += Thumbnail_Click;
            _thumbnailPanel.Controls.Add(pictureBox);
            _thumbnailCache[index] = pictureBox;
        }

        private void LoadVisibleThumbnails(int startIndex, int endIndex)
        {
            _thumbnailPanel.Controls.Clear();
            _thumbnailCache.Clear();

            for (int i = startIndex; i < endIndex; i++)
            {

                // check if we already have in cache
                if (!_thumbnailCache.ContainsKey(i))
                {
                    CreateThumbnail(i);
                }
                // Only load if image was not already loaded

                LoadThumbnailImage(i);
            }
            if (_currentSelectedThumbnail < _firstVisibleThumbnail)
            {
                
                _currentSelectedThumbnail = _firstVisibleThumbnail;


            }
            else if (_currentSelectedThumbnail >= _lastVisibleThumbnail)
            {
          
                _currentSelectedThumbnail = _lastVisibleThumbnail - 1;
            }
            // first check if current selected thumbnail is in cache
            _thumbnailCache[_currentSelectedThumbnail].BorderStyle = BorderStyle.FixedSingle;

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
            _firstVisibleThumbnail = _scrollBar.Value * GetThumbnailsPerRow();
            //  ensure that we don't go beyond total thumbnails
            _firstVisibleThumbnail = Math.Min(_firstVisibleThumbnail, _totalThumbnails -1);

            _lastVisibleThumbnail = Math.Min(_firstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow(), _totalThumbnails);
            LoadVisibleThumbnails(_firstVisibleThumbnail, _lastVisibleThumbnail);
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
