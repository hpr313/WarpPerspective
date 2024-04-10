using CCvLibrary;
using OpenCvSharp;

namespace WarpPerspective
{
    public partial class Form1 : Form
    {
        private Mat mSrc;
        private readonly Form2 mForm2 = null;
        private CCvFunc ccvfunc = null;
        private System.Drawing.Point mStartPoint;
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.FileName = "";
            toolStripStatusLabel1.Text = "Status";
            panel1.Dock = DockStyle.Fill;
            panel1.AutoScroll = true;
            pictureBox1.Location = new System.Drawing.Point(0, 0);
            tSTextBox.TextBoxTextAlign = HorizontalAlignment.Center;
            tSTextBox.Text = "500*600";

            AllowDrop = true;
            DragEnter += new DragEventHandler(Form1_DragEnter);
            DragDrop += new DragEventHandler(Form1_DragDrop);

            mForm2 = new Form2();
            ccvfunc = new CCvFunc();
        }
        private void OpenFile(string fname = null)
        {
            Bitmap bmp = null;
            string newfname = fname;

            (newfname, bmp) = ccvfunc.OpenFileCv(fname);

            if (bmp == null) { return; }
            mSrc = Cv2.ImRead(newfname);
            bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mSrc);

            //pictureBox1.Image = bmp;
            pictureBox1.Image = ccvfunc.Clear();
            pictureBox1.Size = pictureBox1.Image.Size;
            AdjustWinSize(pictureBox1.Image);

            toolStripStatusLabel1.Text = Path.GetFileName(fname);
            detectCornerToolStripMenuItem_Click(null, null);
            if (mForm2.Validate())
            {
                mForm2.Hide();
            }
            //mCsarea.SetNewImage(bmp);
        }

        private void detectCornerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Image == null) { return; }
                Cursor = Cursors.WaitCursor;
                mForm2.detectCornersShow(ccvfunc);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { Cursor = Cursors.Default; }
        }
        private void AdjustWinSize(Image image)
        {
            pictureBox1.Size = image.Size;
            ClientSize = new System.Drawing.Size(image.Width, image.Height + menuStrip1.Height + toolStrip1.Height + statusStrip1.Height);
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { OpenFile(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] fname = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                OpenFile(fname[0]);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (pictureBox1.Image == null)
                    return;

                if (e.Button == MouseButtons.Right)
                {
                    pictureBox1.Image = ccvfunc.Clear();
                }
                else
                {
                    if (e.Button != MouseButtons.Left)
                        return;
                    System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                    pictureBox1.Image = ccvfunc.mouseDown(p);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void warpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (pictureBox1.Image == null) { return; }
                Cursor = Cursors.WaitCursor;
                mForm2.getPerspectiveShow(ccvfunc, tSTextBox.Text);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { Cursor = Cursors.Default; }
        }
    }
}
