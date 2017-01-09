using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using GMap.NET;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews
{
    public class GroundTruthImporter
    {
        public double DistanceFromOriginToMarker
        {
            get;
            set;
        }

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

        // find the angle of the
        public double getInternalAngleOfTriangle()
        {
            double bearing = getGamma(DistanceFromOriginToMarker, distanceFromOriginToRef(), DistanceFromOtherRefToMarker);
            return bearing;
        }

        public double getDoubleFromMessageBox( string aPrompt )
        {
            // define distance from origin to marker
            string output = "";
            double parsed = double.NaN;
            if (DialogResult.OK == InputBox.Show("Ground Truth Importer", aPrompt, ref output))
                double.TryParse(output, out parsed);

            return parsed;
        }

        public bool promptAndGetDistances()
        {
            // they will enter the number in Meters, but we want it in km
            DistanceFromOriginToMarker = 0.0003048 * getDoubleFromMessageBox("Enter distance (in decimal feet) from origin to marker ");
            if (double.IsNaN(DistanceFromOriginToMarker))
                return false;

            DistanceFromOtherRefToMarker = 0.0003048 * getDoubleFromMessageBox("Enter distance (in decimal feet) from other ref point to marker ");
            if (double.IsNaN(DistanceFromOtherRefToMarker))
                return false;

            return true;
        }

        public List<PointLatLngAlt> mineLocations()
        {
            List<PointLatLngAlt> lRet = new List<PointLatLngAlt>();

            double bearingFromOriginToOtherRef = new PointLatLngAlt(OriginMarker.Position).GetBearing(OtherReferencePoint.Position);
            double internalAngleOfTriangle = (getInternalAngleOfTriangle()) * FlightData.rad2deg;

            if ( double.IsNaN( bearingFromOriginToOtherRef ) || double.IsNaN( internalAngleOfTriangle ))
                MessageBox.Show("invalid bearing ");
            else
            {
                PointLatLngAlt lNewPoint = OriginMarker.Position;
                lNewPoint = lNewPoint.newpos(bearingFromOriginToOtherRef - internalAngleOfTriangle, DistanceFromOriginToMarker * 1000);
                lRet.Add(lNewPoint);

                PointLatLngAlt lNewPoint2 = OriginMarker.Position;
                lNewPoint2 = lNewPoint2.newpos(bearingFromOriginToOtherRef + internalAngleOfTriangle, DistanceFromOriginToMarker * 1000);
                lRet.Add(lNewPoint2);

            }

            return lRet;
        }
    }
}
