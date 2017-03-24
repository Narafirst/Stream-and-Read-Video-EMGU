using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Point = System.Drawing.Point;

namespace ReadVideoFile01
{
    class Utils
    {
        public static Mat CreateMaskFromPoints(Point[] points, out int x, out int y, out int width, out int height)
        {
            int minX = points.Min(p => p.X);
            int maxX = points.Max(p => p.X);
            int minY = points.Min(p => p.Y);
            int maxY = points.Max(p => p.Y);

            x = minX;
            y = minY;
            width = maxX - minX;
            height = maxY - minY;

            Mat blackMat = new Mat(maxY, maxX, DepthType.Cv8U, 1);
            blackMat.SetTo(new MCvScalar(0));
            CvInvoke.FillConvexPoly(blackMat, new VectorOfPoint(points.ToArray()), new MCvScalar(255));

            Mat resultMask = new Mat(blackMat, new Rectangle(x, y, width, height));

            return resultMask;
        }
    }
}
