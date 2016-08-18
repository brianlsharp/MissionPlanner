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
    public class GridExporter
    {
        GMapMarker mOriginMarker;

        public GMapMarker OriginMarker
        {
            get { return mOriginMarker; }
            set
            {
                mOriginMarker = value;
            }
        }

        GMapMarker mOtherReferencePoint;
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
            double lRet =  mProjection.GetDistance(OriginMarker.Position, OtherReferencePoint.Position);
            Debug.WriteLine("distance between points = {0}", lRet);
            return lRet;
        }

        PureProjection mProjection;

        public GridExporter(PureProjection aProjection)
        {
            OriginMarker = null;
            OtherReferencePoint = null;
            mProjection = aProjection;
        }

        public void export()
        {
            double Bx = distanceFromOriginToRef();
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Poi File|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (Stream file = sfd.OpenFile())
                    {
                        foreach (var item in POI.pois())
                        {
                            string line = item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                          item.Lng.ToString(CultureInfo.InvariantCulture) + "\t";
                            string lLabel = item.Tag.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).First() + "\r\n";
                            line += lLabel;


                            double Br = mProjection.GetDistance(OtherReferencePoint.Position, item);
                            double Ar = mProjection.GetDistance(mOriginMarker.Position, item);

                            double Cx = (Math.Pow(Bx, 2) - Math.Pow(Br, 2) + Math.Pow(Ar, 2)) / (2 * Bx);

                            
                            
                            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                            file.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
    }
}

