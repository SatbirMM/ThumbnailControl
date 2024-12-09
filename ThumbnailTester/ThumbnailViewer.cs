using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScleraThumbnailControl;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        private readonly FlowLayoutPanel mThumbnailPanel;
        private IThumbnailProvider _thumbnailProvider;
        private readonly Dictionary<int, PictureBox> mThumbnailCache;
        private readonly System.Windows.Forms.ScrollBar mScrollBar;
        private int mTotalThumbnails;
        private int mFirstVisibleThumbnail = 0;
        private int mLastVisibleThumbnail = 0;
        private int mCurrentSelectedThumbnail = 0;
        private bool mAdjustCurrentSelected = false;

        /// <summary>
        ///Thumbnail width and height, set it as per your requirements
        /// </summary>
        public const int mThumbnailSize = 100;
        /// <summary>
        /// Set Margin as per your requirements
        /// </summary>
        public const int mMarginSize = 1;
        /// <summary>
        /// Set this if you want to select new thumbnail when it is added
        /// </summary>
        public bool SelectNewThumbnailOnAdd { get; set; } = true;
        /// <summary>
        /// Event when a thumbnail is clicked
        /// </summary>
        public event EventHandler<int> ThumbnailClicked;
        public BorderStyle mBorderStyle { get; set; }
        /// <summary>
        /// temporary debug feature
        /// </summary>
        public System.Windows.Forms.TextBox mLogger = null;


        public ThumbnailViewerControl()
        {
            mBorderStyle = BorderStyle.FixedSingle;
            mCurrentSelectedThumbnail = 0;
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
            mScrollBar.Minimum = 0;
            mScrollBar.Maximum = 1;


            Controls.Add(mThumbnailPanel);
            Controls.Add(mScrollBar);


            mThumbnailCache = new Dictionary<int, PictureBox>();

            // When user click on a thumbnail draw a border around it
            ThumbnailClicked += (sender, index) =>
            {
                Log("Value: " + mScrollBar.Value.ToString());
                Log("Maximum: " + mScrollBar.Maximum.ToString());
                Log("Minimum: " + mScrollBar.Minimum.ToString());
                SetBorderStyle(index);
                mScrollBar.Value = index;
                Log("Value: " + mScrollBar.Value.ToString());
                Log("Maximum: " + mScrollBar.Maximum.ToString());
                Log("Minimum: " + mScrollBar.Minimum.ToString());

            };
        }

        private void SetBorderStyle(int index)
        {
            foreach (var pictureBox in mThumbnailCache.Values)
            {
                pictureBox.BorderStyle = BorderStyle.None;
                pictureBox.BackColor = Color.Transparent;
            }

            if (mThumbnailCache.ContainsKey(index))
            {
                mThumbnailCache[index].BorderStyle = mBorderStyle;
                mThumbnailCache[index].Padding = new Padding(1);
                mThumbnailCache[index].BackColor = Color.Red;
                mCurrentSelectedThumbnail = index;
            }
        }

        // Get index of selected thumbnail
        public int GetSelectedThumbnailIndex()
        {
            foreach (var pictureBox in mThumbnailCache.Values)
            {
                if (pictureBox.BorderStyle == mBorderStyle)
                {
                    return (int)pictureBox.Tag;
                }
            }
            return -1;
        }
        private void Log(string message)
        {
            if (mLogger != null)
            {
                mLogger.AppendText(message);
                mLogger.AppendText(Environment.NewLine);
            }
        }

        // select next thumbnail
        public void SelectNextThumbnail()
        {
            Log("SelectNextThumbnail");
            mAdjustCurrentSelected = false;

            int selectedIndex = GetSelectedThumbnailIndex();
            if (selectedIndex == -1 || selectedIndex == mTotalThumbnails - 1) return;

            int nextIndex = selectedIndex + 1;
            if (nextIndex <= mTotalThumbnails)
            {

                if (!mThumbnailCache.ContainsKey(nextIndex))
                {
                    CreateThumbnail(nextIndex);
                }
                // just adjust mFrstVisibleThumbnail, _lastVisibleThumbnail
                // only increase one count at a time
                if (nextIndex >= mLastVisibleThumbnail)
                {
                    mLastVisibleThumbnail = nextIndex;
                    mFirstVisibleThumbnail = Math.Max(0, mLastVisibleThumbnail - GetThumbnailsPerRow());
                    mScrollBar.Value = mFirstVisibleThumbnail / GetThumbnailsPerRow();
                }
                mCurrentSelectedThumbnail = nextIndex;

                LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);
            }
        }

        // select previous thumbnail
        public void SelectPreviousThumbnail()
        {
            mAdjustCurrentSelected = true;
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
                    mLastVisibleThumbnail = Math.Min(mTotalThumbnails - 1, mFirstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow());
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
                if (SelectNewThumbnailOnAdd)
                {
                    mAdjustCurrentSelected = false;
                    mCurrentSelectedThumbnail = index;
                    ThumbnailClicked?.Invoke(this, index);
                }
                LoadThumbnails();

            };
            LoadThumbnails();
        }

        public void LoadThumbnails()
        {
            if (_thumbnailProvider == null) return;

            mTotalThumbnails = _thumbnailProvider.GetTotalThumbnails();
            if (mTotalThumbnails == 0)
            {
                mThumbnailPanel.Controls.Clear();
                mThumbnailCache.Clear();
                return;
            }

            mScrollBar.Maximum = Math.Max(0, mTotalThumbnails * mThumbnailSize);
            mLastVisibleThumbnail = mCurrentSelectedThumbnail;

            mFirstVisibleThumbnail = Math.Max(0, mLastVisibleThumbnail - GetVisibleRowCount() * GetThumbnailsPerRow());

            LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);
            mScrollBar.Value = mLastVisibleThumbnail;
            Log(mScrollBar.Value.ToString());
        }

        private void CreateThumbnail(int index)
        {
            var pictureBox = new PictureBox
            {
                Size = new Size(mThumbnailSize, mThumbnailSize),
                Margin = new Padding(mMarginSize),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Tag = index
            };
            pictureBox.Click += Thumbnail_Click;
            mThumbnailPanel.Controls.Add(pictureBox);
            mThumbnailCache[index] = pictureBox;
        }

        private void LoadVisibleThumbnails(int startIndex, int endIndex)
        {

            if (mTotalThumbnails == 0)
            {
                Log("No thumbnails to load");
                return;
            }
            mThumbnailPanel.Controls.Clear();
            mThumbnailCache.Clear();

            for (int i = startIndex; i <= endIndex; i++)
            {

                // check if we already have in cache
                if (!mThumbnailCache.ContainsKey(i))
                {
                    Log($"Creating thumbnail {i}");
                    CreateThumbnail(i);
                }
                else
                {
                    Log($"Thumbnail {i} already exists");
                }
                LoadThumbnailImage(i);
            }
            if (mAdjustCurrentSelected)
            {
                if (mCurrentSelectedThumbnail < mFirstVisibleThumbnail)
                {

                    mCurrentSelectedThumbnail = mFirstVisibleThumbnail;


                }
                else if (mCurrentSelectedThumbnail >= mLastVisibleThumbnail)
                {

                    mCurrentSelectedThumbnail = mLastVisibleThumbnail - 1;
                }
            }
            SetBorderStyle(mCurrentSelectedThumbnail);
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
            mAdjustCurrentSelected = true;
            mFirstVisibleThumbnail = mScrollBar.Value * GetThumbnailsPerRow();
            //  ensure that we don't go beyond total thumbnails
            mFirstVisibleThumbnail = Math.Min(mFirstVisibleThumbnail, mTotalThumbnails - 1);

            mLastVisibleThumbnail = Math.Min(mFirstVisibleThumbnail + GetVisibleRowCount() * GetThumbnailsPerRow(), mTotalThumbnails - 1);
            LoadVisibleThumbnails(mFirstVisibleThumbnail, mLastVisibleThumbnail);

            Log("Value: " + mScrollBar.Value.ToString());
            Log("Maximum: " + mScrollBar.Maximum.ToString());
            Log("Minimum: " + mScrollBar.Minimum.ToString());
        }

        private int GetThumbnailsPerRow()
        {
            return Math.Max(1, mThumbnailPanel.Width / (mThumbnailSize + mMarginSize * 2));
        }

        private int GetVisibleRowCount()
        {
            return Math.Max(1, Height / (mThumbnailSize + mMarginSize * 2));
        }
    }
}

public class ThumbnailProvider : IThumbnailProvider
{

    /// <summary>
    /// Store bitmap data here
    /// </summary>
    private List<Bitmap> mBitmaps = new List<Bitmap>();
    /// <summary>
    /// 
    /// </summary>
    public System.Windows.Forms.TextBox textBox = null;

    public event EventHandler<int> ThumbnailAdded;

    // return total number of thumbnails
    public int GetTotalThumbnails() => mBitmaps.Count;

    // log to text box, GetThumbnail method is called
    public ThumbnailProvider()
    {
    }
    private void Log(string message)
    {
        if (textBox != null)
        {
            textBox.AppendText(message);
            textBox.AppendText(Environment.NewLine);

        }
    }
    public void SetBitmap(Bitmap bitmap)
    {
        Log("SetBitmap\n");
        mBitmaps.Add(bitmap);
        // fire event when new thumbnail is added
        ThumbnailAdded?.Invoke(this, mBitmaps.Count - 1);
    }


    public Image GetThumbnail(int index)
    {
        Log($"GetThumbnail {index}\n");
        return mBitmaps[index];
    }

    void IThumbnailProvider.ThumbnailClicked(int index)
    {
        Log($"ThumbnailClicked {index}\n");
    }

    public void ClearThumbnails()
    {

        Log("ClearThumbnails");
        mBitmaps.Clear();
        ThumbnailAdded?.Invoke(this, 0);

    }
}

