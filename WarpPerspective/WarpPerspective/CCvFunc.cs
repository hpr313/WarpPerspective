using System.Drawing;
using OpenCvSharp;

namespace CCvLibrary
{
    public class CCvFunc : CCv
    {
        private List<List<OpenCvSharp.Point>> mPsrc; // perspective source
        private List<List<OpenCvSharp.Point>> mPdst; // perspective destination
        int mPersWidth, mPersHeight;
        public CCvFunc() : base()
        {
            mPsrc = new List<List<OpenCvSharp.Point>>();     // perspective source
            mPsrc.Add(new List<OpenCvSharp.Point>());        // perspective source
            mPdst = new List<List<OpenCvSharp.Point>>();     // perspective destination
            mPdst.Add(new List<OpenCvSharp.Point>());        // perspective destination
        }
        public (Bitmap, int) findRectangles(string size)
        {
            int detects = 0;
            char[] delimitter = { 'X', 'x', '*' };
            string[] resolutions = size.Split(delimitter);
            int width = Convert.ToInt32(resolutions[0]);
            int height = Convert.ToInt32(resolutions[1]);

            using (Mat gray = new Mat())
            {
                Cv2.CvtColor(mSrc, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.Threshold(gray, gray, 128, 255, ThresholdTypes.Binary);

                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchy;
                Cv2.FindContours(gray, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxTC89L1);
                mDst = mSrc.Clone();
                for (int i = 0; i < contours.Length; i++)
                {
                    double a = Cv2.ContourArea(contours[i], false);
                    if (a > width * height)
                    {
                        OpenCvSharp.Point[] approx;
                        approx = Cv2.ApproxPolyDP(contours[i], 0.01 * Cv2.ArcLength(contours[i], true), true);
                        if (approx.Length == 3)
                        {
                            detects += 1;
                            OpenCvSharp.Point[][] tmpContours = new OpenCvSharp.Point[][] { approx };

                            int maxLevel = 0;
                            Cv2.DrawContours(mDst, tmpContours, 0, Scalar.Black, 5, LineTypes.AntiAlias, hierarchy, maxLevel);
                        }
                    }
                }
            }
            Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mDst);
            return (bmp, detects);
        }
        public Bitmap detectCorners()
        {
            using (var gray = new Mat())
            {
                Cv2.CvtColor(mSrc, gray, ColorConversionCodes.BGR2GRAY);
                const int maxCorners = 50, blockSize = 3;
                const double qualityLevel = 0.01, minDistance = 20.0, k = 0.04;
                const bool useHarrisDetector = false;
                Point2f[] corners = Cv2.GoodFeaturesToTrack(gray, maxCorners, qualityLevel, minDistance, new Mat(), blockSize, useHarrisDetector, k);
                mDst = mSrc.Clone();
                foreach (Point2f it in corners)
                {
                    Cv2.Circle(mDst, (OpenCvSharp.Point)it, 4, Scalar.Blue, 2);
                }
            }
            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mDst);
        }
        public Bitmap getPerspective(string size)
        {
            char[] delimitter = { 'X', 'x', '*' };
            string[] resolutions = size.Split(delimitter);
            mPersWidth = Convert.ToInt32(resolutions[0]);
            mPersHeight = Convert.ToInt32(resolutions[1]);
            return doPerspective(mPersWidth, mPersHeight, mPsrc, mPdst);
        }

        private Bitmap doPerspective(int mPersWidth, int mPersHeight, List<List<OpenCvSharp.Point>> mPsrc, List<List<OpenCvSharp.Point>> mPdst)
        {
            mDst = new Mat(mPersHeight, mPersWidth, mSrc.Type());

            Point2f[] pSrc = new Point2f[4];    // perspective source
            for (int i = 0; i < mPsrc[0].Count; i++)
            {
                pSrc[i] = (Point2f)mPsrc[0][i];
            }

            Point2f[] pDst = new Point2f[] {  // perspective destination
                new Point2f(0.0f, 0.0f),
                new Point2f(0.0f, (float)(mPersHeight - 1)),
                new Point2f((float)(mPersWidth - 1), (float)(mPersHeight - 1)),
                new Point2f((float)(mPersWidth - 1), 0.0f)
                };

            Mat persMatrix = Cv2.GetPerspectiveTransform(pSrc, pDst);
            Cv2.WarpPerspective(mSrc, mDst, persMatrix, mDst.Size(), InterpolationFlags.Cubic);

            return OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mDst);
        }
        private void SortSrcPoints(List<OpenCvSharp.Point> points)
        {
            points.Sort((a, b) => a.X - b.X);   //sort by X

            if (points[0].Y > points[1].Y) { (points[0], points[1]) = (points[1], points[0]); }
            if (points[3].Y > points[2].Y) { (points[2], points[3]) = (points[3], points[2]); }
        }
        public Bitmap mouseDown(System.Drawing.Point point)
        {
            if (mPsrc[0].Count < 4)
            {
                mPsrc[0].Add(new OpenCvSharp.Point(point.X, point.Y));
            }
            else
            {
                for (int i = 0; i < mPsrc[0].Count; i++)
                {
                    if (Math.Abs(mPsrc[0][i].X - point.X) < 100 && Math.Abs(mPsrc[0][i].Y - point.Y) < 100)
                    {
                        mPsrc[0][i] = new OpenCvSharp.Point(point.X, point.Y);
                        break;
                    }
                }
            }
            return GetRectsOnBmp(mSrc);
        }

        public Bitmap Clear()
        {
            //reset
            mPsrc[0].Clear();
            return GetRectsOnBmp(mSrc);
        }

        private Bitmap GetRectsOnBmp(Mat img)
        {
            Bitmap bmp;
            using (Mat dst = img.Clone())
            {
                if (mPsrc[0].Count > 0)
                {
                    foreach (var point in mPsrc.First())
                    {
                        Cv2.Circle(dst, point, 3, Scalar.Red, -1);
                    }
                    if (mPsrc[0].Count == 4)
                    {
                        SortSrcPoints(mPsrc[0]);
                        Cv2.Polylines(dst, mPsrc, true, Scalar.Red);
                    }
                }
                bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            }
            return bmp;
        }
        public Bitmap rotated()
        {
            (mPsrc[0][0], mPsrc[0][1], mPsrc[0][2], mPsrc[0][3]) = (mPsrc[0][1], mPsrc[0][2], mPsrc[0][3], mPsrc[0][0]);
            return doPerspective(mPersWidth, mPersHeight, mPsrc, mPdst);
        }
    }
}
