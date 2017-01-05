using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MissionPlanner.Utilities;
using System.Globalization;
using GMap.NET;
using System.Diagnostics;

namespace MissionPlanner.GCSViews
{
    public class GroundTruthImporter
    {
        double mDistanceFromOriginToMarker = double.NaN;
        public double DistanceFromOriginToMarker
        {
            get;
            set;
        }

        double mDistanceFromOtherRefToMarker = double.NaN;
        public double DistanceFromOtherRefToMarker
        {
            get;
            set;
        }


        GMapMarker mOriginMarker = null;
        public GMapMarker OriginMarker
        {
            get { return mOriginMarker; }
            set
            {
                mOriginMarker = value;
            }
        }

        GMapMarker mOtherReferencePoint = null;
        public GMapMarker OtherReferencePoint
        {
            get { return mOtherReferencePoint; }
            set
            {
                mOtherReferencePoint = value;
            }
        }

        private double distanceFromOriginToRef()
        {
            double lRet = mProjection.GetDistance(OriginMarker.Position, OtherReferencePoint.Position);
            Debug.WriteLine("distance between points = {0}", lRet);
            return lRet;
        }

        PureProjection mProjection;

        public GroundTruthImporter(PureProjection aProjection)
        {
            mProjection = aProjection;
            OriginMarker = null;
            OtherReferencePoint = null;
        }

        // law of cosines. Get the unknown angle opposite side c

        public static double getGamma( double a, double b, double c )
        {
            double middleTerm = Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2);
            middleTerm = middleTerm / (2 * a * b);
            double gamma = Math.Acos(middleTerm);

            return gamma;
        }

        public double getBearing()
        {
            double bearing = getGamma(DistanceFromOriginToMarker, distanceFromOriginToRef(), DistanceFromOtherRefToMarker);
            return bearing;
        }
    }
}
