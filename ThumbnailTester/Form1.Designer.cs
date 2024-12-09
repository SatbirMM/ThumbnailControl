namespace ThumbnailTester
{
    partial class ThumbnailViewerTesting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.thumbnailViewerControl1 = new ScleraThumbnailControl.ThumbnailViewerControl();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.previousButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.textBox = new System.Windows.Forms.TextBox();
            this.customCircleControl1 = new CustomCircleControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 56);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add a thumbnail";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // thumbnailViewerControl1
            // 
            this.thumbnailViewerControl1.Location = new System.Drawing.Point(12, 24);
            this.thumbnailViewerControl1.mBorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.thumbnailViewerControl1.Name = "thumbnailViewerControl1";
            this.thumbnailViewerControl1.SelectNewThumbnailOnAdd = true;
            this.thumbnailViewerControl1.Size = new System.Drawing.Size(189, 593);
            this.thumbnailViewerControl1.TabIndex = 0;
            this.thumbnailViewerControl1.Load += new System.EventHandler(this.thumbnailViewerControl1_Load);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.previousButton);
            this.flowLayoutPanel1.Controls.Add(this.nextButton);
            this.flowLayoutPanel1.Controls.Add(this.clearButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(398, 527);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(660, 116);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // previousButton
            // 
            this.previousButton.Location = new System.Drawing.Point(128, 3);
            this.previousButton.Name = "previousButton";
            this.previousButton.Size = new System.Drawing.Size(92, 56);
            this.previousButton.TabIndex = 2;
            this.previousButton.Text = "Prev";
            this.previousButton.UseVisualStyleBackColor = true;
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(226, 3);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(117, 56);
            this.nextButton.TabIndex = 3;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(349, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(112, 56);
            this.clearButton.TabIndex = 4;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.textBox);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(204, 34);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(993, 265);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(3, 3);
            this.textBox.MaximumSize = new System.Drawing.Size(471, 200);
            this.textBox.MinimumSize = new System.Drawing.Size(471, 200);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(471, 20);
            this.textBox.TabIndex = 4;
            // 
            // customCircleControl1
            // 
            this.customCircleControl1.Circle1Segment1 = System.Drawing.Color.Red;
            this.customCircleControl1.Circle1Segment2 = System.Drawing.Color.Green;
            this.customCircleControl1.Circle1Segment3 = System.Drawing.Color.Blue;
            this.customCircleControl1.Circle1Segment4 = System.Drawing.Color.Yellow;
            this.customCircleControl1.Circle2Segment1 = System.Drawing.Color.Orange;
            this.customCircleControl1.Circle2Segment2 = System.Drawing.Color.Purple;
            this.customCircleControl1.Circle2Segment3 = System.Drawing.Color.Cyan;
            this.customCircleControl1.Circle2Segment4 = System.Drawing.Color.Magenta;
            this.customCircleControl1.Circle3Segment1 = System.Drawing.Color.Pink;
            this.customCircleControl1.Circle3Segment2 = System.Drawing.Color.Brown;
            this.customCircleControl1.Circle3Segment3 = System.Drawing.Color.Lime;
            this.customCircleControl1.Circle3Segment4 = System.Drawing.Color.Teal;
            this.customCircleControl1.Circle4Segment1 = System.Drawing.Color.Gray;
            this.customCircleControl1.Circle4Segment2 = System.Drawing.Color.Maroon;
            this.customCircleControl1.Circle4Segment3 = System.Drawing.Color.Navy;
            this.customCircleControl1.Circle4Segment4 = System.Drawing.Color.Olive;
            this.customCircleControl1.InnerCircleColor = System.Drawing.Color.Black;
            this.customCircleControl1.Location = new System.Drawing.Point(1034, 359);
            this.customCircleControl1.Name = "customCircleControl1";
            this.customCircleControl1.Size = new System.Drawing.Size(150, 150);
            this.customCircleControl1.TabIndex = 0;
            this.customCircleControl1.Load += new System.EventHandler(this.customCircleControl1_Load);
            // 
            // ThumbnailViewerTesting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 787);
            this.Controls.Add(this.customCircleControl1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.thumbnailViewerControl1);
            this.Name = "ThumbnailViewerTesting";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.ThumbnailViewerTesting_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ScleraThumbnailControl.ThumbnailViewerControl thumbnailViewerControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button clearButton;
        private CustomCircleControl customCircleControl1;
    }
}

