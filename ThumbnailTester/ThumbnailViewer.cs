using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScleraThumbnailControl;

namespace ScleraThumbnailControl
{
    public interface IThumbnailProvider
    {
        int GetTotalThumbnails(); // Fetch total number of thumbnails
        Image GetThumbnail(int index); // Fetch thumbnail image by index
        void ThumbnailClicked(int index); // Event when a thumbnail is clicked
        void ClearThumbnails(); // clear all thumbnails
        // fire event when new thumbnail is added
        event EventHandler<int> ThumbnailAdded;

    }

    public class ThumbnailViewerControl : UserControl
    {
        //https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/flowlayoutpanel-control-overview?view=netframeworkdesktop-4.8
        private readonly FlowLayoutPanel mThumbnailPanel;
        private IThumbnailProvider _thumbnailProvider;
        private readonly Dictionary<int, PictureBox> mThumbnailCache;
        private readonly System.Windows.Forms.ScrollBar mScrollBar;

        private const int ThumbnailSize = 100; // Thumbnail width and height
        private const int MarginSize = 1; // Margin around thumbnails
        private int _totalThumbnails;
        int mFirstVisibleThumbnail = 0;
        int mLastVisibleThumbnail = 0;
        int mCurrentSelectedThumbnail = 0;
        // Set this if you want to select new thumbnail when it is added
        public bool SelectNewThumbnailOnAdd { get; set; } = true;

        public event EventHandler<int> ThumbnailClicked; // Event when a thumbnail is clicked


        public ThumbnailViewerControl()
        {
            mThumbnailPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = false,
                WrapContents = true,
                FlowDirection = FlowDirection.LeftToRight
            };
            bool isHorizontal = false;
            if (isHorizontal)
            {
                // add Horizontal scrollbar
                mScrollBar = new HScrollBar
                {
                    Dock = DockStyle.Bottom,
                    Width = 20
                };
            }
            else
            {
                mScrollBar = new VScrollBar
                {
                    Dock = DockStyle.Right,
                    Width = 20
                };

            }
            // instead of calling it every time during scroll we call when scrolling stops

            mScrollBar.Scroll += ScrollBar_Scroll;
            mScrollBar.Width = 20;


            Controls.Add(mThumbnailPanel);
            Controls.Add(mScrollBar);


            mThumbnailCache = new Dictionary<int, PictureBox>();

            // When user click on a thumbnail draw a border around it
            ThumbnailClicked += (sender, index) =>
            {
                foreach (var pictureBox in mThumbnailCache.Values)
                {
                    pictureBox.BorderStyle = BorderStyle.None;
                }

                if (mThumbnailCache.ContainsKey(index))
                {
                    mThumbnailCache[index].BorderStyle = BorderStyle.FixedSingle;
                    mThumbnailCache[index].Padding = new Padding(20);
                    mThumbnailCache[index].BackColor = Color.Transparent;
                    mCurrentSelectedThumbnail = index;
                }

            };
        }

        // Get index of selected thumbnail
        public int GetSelectedThumbnailIndex()
        {
            foreach (var pictureBox in mThumbnailCache.Values)
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
            Console.WriteLine("SelectNextThumbnail");

            int selectedIndex = GetSelectedThumbnailIndex();
            if (selectedIndex == -1 || selectedIndex == _totalThumbnails) return;

            int nextIndex = selectedIndex + 1;
            if (nextIndex <= _totalThumbnails)
            {

                if (!mThumbnailCache.ContainsKey(nextIndex))
                {
                    CreateThumbnail(nextIndex);
                }
                // just adjust mFrstVisibleThumbnail, _lastVisibleThumbnail
                // only increase one count at a time
                if (nextIndex >= mLastVisibleThumbnail)
                {
                    mFirstVisibleThumbnail = mLastVisibleThumbnail;
                    mLastVisibleThumbnail = Math.Min(_totalThumbnails, mFirstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow());
                    mScrollBar.Value = mFirstVisibleThumbnail / GetThumbnailsPerRow();
                }
                mCurrentSelectedThumbnail = nextIndex;

                LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);
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
                if (!mThumbnailCache.ContainsKey(previousIndex))
                {
                    CreateThumbnail(previousIndex);
                }
                // just adjust mFrstVisibleThumbnail, _lastVisibleThumbnail
                // only increase one count at a time
                if (previousIndex < mFirstVisibleThumbnail)
                {
                    mFirstVisibleThumbnail = previousIndex;
                    mLastVisibleThumbnail = Math.Min(_totalThumbnails, mFirstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow());
                    mScrollBar.Value = mFirstVisibleThumbnail / GetThumbnailsPerRow();
                }
                mCurrentSelectedThumbnail = previousIndex;
                LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);
            }

        }
        public void SetThumbnailProvider(IThumbnailProvider provider)
        {
            _thumbnailProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _thumbnailProvider.ThumbnailAdded += (sender, index) =>
            {
                LoadThumbnails();
                if(SelectNewThumbnailOnAdd)
                {
                    mCurrentSelectedThumbnail = index;
                    ThumbnailClicked?.Invoke(this, index);
                }
            };
            LoadThumbnails();
        }

        public void LoadThumbnails()
        {
            if (_thumbnailProvider == null) return;

            _totalThumbnails = _thumbnailProvider.GetTotalThumbnails();
            if(_totalThumbnails == 0)
            {
                mThumbnailPanel.Controls.Clear();
                mThumbnailCache.Clear();
                return;
            }

            mScrollBar.Maximum = Math.Max(0, _totalThumbnails * GetThumbnailsPerRow());
            mScrollBar.Value = 0;
            // Set start index and last index
            mFirstVisibleThumbnail = 0;
            mLastVisibleThumbnail = Math.Min(_totalThumbnails, GetVisibleRowCount() * GetThumbnailsPerRow());

            LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);
        }

        private void CreateThumbnail(int index)
        {
            var pictureBox = new PictureBox
            {
                Size = new Size(ThumbnailSize, ThumbnailSize),
                Margin = new Padding(MarginSize),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = index
            };
            pictureBox.Click += Thumbnail_Click;
            mThumbnailPanel.Controls.Add(pictureBox);
            mThumbnailCache[index] = pictureBox;
        }

        private void LoadVisibleThumbnails(int startIndex, int endIndex)
        {
            if (_totalThumbnails == 0)
            {
                Console.WriteLine("No thumbnails to load");
                return;
            }
            

            for (int i = startIndex; i < endIndex; i++)
            {

                // check if we already have in cache
                if (!mThumbnailCache.ContainsKey(i))
                {
                    Console.WriteLine($"Creating thumbnail {i}");
                    CreateThumbnail(i);
                }
                else
                {
                    Console.WriteLine($"Thumbnail {i} already exists");
                }
                LoadThumbnailImage(i);
            }
            if (mCurrentSelectedThumbnail < mFirstVisibleThumbnail)
            {

                mCurrentSelectedThumbnail = mFirstVisibleThumbnail;


            }
            else if (mCurrentSelectedThumbnail >= mLastVisibleThumbnail)
            {

                mCurrentSelectedThumbnail = mLastVisibleThumbnail - 1;
            }
            // first check if current selected thumbnail is in cache
            mThumbnailCache[mCurrentSelectedThumbnail].BorderStyle = BorderStyle.Fixed3D;

        }

        private void LoadThumbnailImage(int index)
        {
            if (_thumbnailProvider == null || !mThumbnailCache.ContainsKey(index)) return;

            var image = _thumbnailProvider.GetThumbnail(index);
            mThumbnailCache[index].Image = image;
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
            mFirstVisibleThumbnail = mScrollBar.Value * GetThumbnailsPerRow();
            //  ensure that we don't go beyond total thumbnails
            mFirstVisibleThumbnail = Math.Min(mFirstVisibleThumbnail, _totalThumbnails - 1);

            mLastVisibleThumbnail = Math.Min(mFirstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow(), _totalThumbnails);
            LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);
        }

        private int GetThumbnailsPerRow()
        {
            return Math.Max(1, mThumbnailPanel.Width / (ThumbnailSize + MarginSize * 2));
        }

        private int GetVisibleRowCount()
        {
            return Math.Max(1, Height / (ThumbnailSize + MarginSize * 2));
        }
    }
}

public class ThumbnailProvider : IThumbnailProvider
{

    // store bitmap in list
    private List<Bitmap> mBitmaps = new List<Bitmap>();

    public System.Windows.Forms.TextBox textBox = null;

    public event EventHandler<int> ThumbnailAdded;

    // return total number of thumbnails
    public int GetTotalThumbnails() => mBitmaps.Count;

    // log to text box, GetThumbnail method is called
    public ThumbnailProvider()
    {
    }
    public void SetBitmap(Bitmap bitmap)
    {
        Console.WriteLine("SetBitmap");
        mBitmaps.Add(bitmap);
        // fire event when new thumbnail is added
        ThumbnailAdded?.Invoke(this, mBitmaps.Count - 1);
    }


    public Image GetThumbnail(int index)
    {
        Console.WriteLine($"GetThumbnail {index}");
        return mBitmaps[index];
    }

    void IThumbnailProvider.ThumbnailClicked(int index)
    {
        Console.WriteLine($"ThumbnailClicked {index}");
    }

    public void ClearThumbnails()
    {

       Console.WriteLine("ClearThumbnails");
        mBitmaps.Clear();
       ThumbnailAdded?.Invoke(this, 0);

    }
}

