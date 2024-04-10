using CCvLibrary;
using OpenCvSharp;


namespace WarpPerspective
{
    public partial class Form2 : Form
    {
        private readonly string ttl = "Processed Result";
        private Mat mDst;
        private CCvFunc mCcvfunc = null;

        public Form2()
        {
            InitializeComponent();

            Text = ttl;
            panel1.Dock = DockStyle.Fill;
            panel1.AutoScroll = true;
            pBox.Location = new System.Drawing.Point(0, 0);
        }
        private void AdjustWinSize(Image image)
        {
            pBox.Size = image.Size;
            ClientSize = new System.Drawing.Size(image.Width, image.Height + menuStrip1.Height);
        }

        private void bmpShow(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                pBox.Image = bitmap;
                AdjustWinSize(pBox.Image);
                Show();
            }
        }
        public void getPerspectiveShow(CCvFunc cCvFunc, string size)
        {
            mCcvfunc = cCvFunc;
            Bitmap bmp = mCcvfunc.getPerspective(size);
            bmpShow(bmp);
        }
        public void detectCornersShow(CCvFunc cCvFunc)
        {
            mCcvfunc = cCvFunc;
            Bitmap bmp = mCcvfunc.detectCorners();
            bmpShow(bmp);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mCcvfunc.SaveAS();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
