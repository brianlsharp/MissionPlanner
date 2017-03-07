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


        /* A little bit about this calculation... We are assuming the following:
         * that we know the relative distance between points (as defined by thier lat/longs)
         * to a high degree of accuracy. The user has provided us with an "origin" point that we
         * shall refer to as A. The user has provided another reference point, B. Using these two 
         * points, we construct a coordinate system with A at the orign and B somewhere along the x
         * axis. That distance A and B is Bx. 
         */
        public void export( )
        {
            DialogResult lResult = MessageBox.Show("Do you wish to export ALL contacts? Clicking 'No' will export only the contacts that were marked for export.", "Export", MessageBoxButtons.YesNoCancel);
            if (lResult == DialogResult.Cancel)
                return;

            bool exportAll = (lResult == DialogResult.Yes);

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File|*.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (Stream file = sfd.OpenFile())
                    {
                        string titleLine = "Report generated from file: \t" + MainV2.LogFilename;
                        titleLine += "\r\n";

                        double Bx = distanceFromOriginToRef();
                        //titleLine += "Origin Marker: " + OriginMarker.Tag.ToString();//.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).First().Trim(':');
                        titleLine += "\r\n";

                        //titleLine += "Other reference Marker: " + OtherReferencePoint.Tag.ToString();//.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).First().Trim(':');
                        titleLine += "\r\n";

                        titleLine += "Scaling Factor" + "\t" + "1.0";
                        titleLine += "\r\n";

                        // a blank line
                        titleLine += "\r\n";

                        titleLine += "Marker name" + "\t" + "x value (ft)" + "\t" + "y value (ft)" + "\t" + "lat"+ "\t" + "lon" + "\t" + "\t" + "scaled x" + "\t" + "scaled y";
                        titleLine += "\r\n";

                        byte[] titlebuffer = ASCIIEncoding.ASCII.GetBytes(titleLine);
                        file.Write(titlebuffer, 0, titlebuffer.Length);

                        int firstLineOfActualOutput = 7; 
                        int i = 0;
                        foreach (var item in POI.pois())
                        {
                            if (exportAll || item.export)
                            {
                                // grab the first part of the tag (aka label). The rest of the tag is lat\lon which we don't want to display in the first cell
                                string line = item.Tag.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).First() + "\t";

                                /*Trilateration will be used to solve for the coordinates of a given point (called item in this case).
                                 * Knowing A and B, we use the map's distance formula to calculate the distance between A and 
                                 * item. This number is Ar. Think of it as defining a circle with radius Ar, that is centered at A.
                                 * Do the same also for Br. Knowing these two distances and making use of the old fashioned distance formula
                                 * (this means we're assuming a flat earth) then we can solve for the coordinates of item in our 
                                 * previously defined coordinate system. Keep in mind that with only this info, there are 2 possible points
                                 * for Cy. 
                                 */

                                double Br = mProjection.GetDistance(OtherReferencePoint.Position, item);
                                double Ar = mProjection.GetDistance(mOriginMarker.Position, item);

                                double Cx = (Math.Pow(Bx, 2) - Math.Pow(Br, 2) + Math.Pow(Ar, 2)) / (2 * Bx);
                                double Cy = Math.Sqrt(Math.Pow(Ar, 2) - Math.Pow(Cx, 2));

                                // we had been working in kilometers
                                Cx = Cx * 1000 * 3.28084;
                                Cy = Cy * 1000 * 3.28084;

                                line += Cx.ToString() + "\t";
                                line += Cy.ToString() + "\t";

                                line += item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                        item.Lng.ToString(CultureInfo.InvariantCulture) + "\t";

                                line += "\t";
                                line += "=B" + ( i + firstLineOfActualOutput ).ToString() + "*(1/$B$4)";

                                line += "\t";
                                line += "=C" + ( i + firstLineOfActualOutput ).ToString() + "*(1/$B$4)";

                                // new line and write out
                                line += "\r\n";
                                byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                                file.Write(buffer, 0, buffer.Length);

                                i++;
                            }
                        }
                    }
                }
            }
        }


    }
}

