

using OpenCvSharp;

namespace CCvLibrary
{
    public class CCv
    {
        protected Mat mSrc, mDst;

        public CCv() { }
        private string GetReadFile(
            string filter = "Image File(*.jpg,*.bmp,*.png)|*.jpg;*.bmp;*.png|"
                                            + "All Files(*.*)|*.*")
        {
            string fname = null;

            using (OpenFileDialog openDlg = new OpenFileDialog())
            {
                openDlg.CheckFileExists = true;
                openDlg.Filter = filter;
                openDlg.FilterIndex = 1;
                if (openDlg.ShowDialog() == DialogResult.OK)
                    fname = openDlg.FileName;
            }
            return fname;
        }

        private string GetWriteFile(
            string filter = "Image File(*.jpg,*.bmp,*.png)|*.jpg;*.bmp;*.png|"
                                            + "All Files(*.*)|*.*")
        {
            string fname = null;

            using (SaveFileDialog svDlg = new SaveFileDialog())
            {
                svDlg.Filter = filter;
                svDlg.FilterIndex = 1;
                if (svDlg.ShowDialog() == DialogResult.OK)
                    fname = svDlg.FileName;
            }
            return fname;
        }

        public (string, Bitmap) OpenFileCv(string fname)
        {
            Bitmap bmp = null;
            string newfname = fname;

            if (fname == null)
            {
                newfname = GetReadFile();
            }

            if (newfname != null)
            {
                Mat img = Cv2.ImRead(newfname);
                if (!img.Empty())
                {
                    mSrc = img;
                    bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mSrc);
                }
            }
            return (newfname, bmp);
        }


        public void SaveAS()
        {
            string fname = GetWriteFile();
            if (fname != null)
            {
                Cv2.ImWrite(fname, mDst);    // OpenCV
            }
        }
    }
}
