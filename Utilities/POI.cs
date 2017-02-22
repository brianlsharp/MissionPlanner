using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public class POI
    {
        /// <summary>
        /// Store points of interest
        /// </summary>
        static ObservableCollection<PointLatLngAlt> POIs = new ObservableCollection<PointLatLngAlt>();
        
        public static ObservableCollection<PointLatLngAlt> pois()
        {
            return POIs;
        }

        public static event EventHandler POIModified;

        public static void POIDeleteAllContacts()
        {
            for(int i = POIs.Count() - 1; i >= 0; i-- )
            {
                if(!POIs[ i ].groundTruth)
                    POIs.RemoveAt( i );
            }

            if (POIModified != null)
                POIModified(null, null);
        }

        public static void POIAdd(PointLatLngAlt Point, string tag, bool aSuspectReading )
        {
            // local copy
            PointLatLngAlt pnt = Point;
            pnt.color = Point.color;
            pnt.Tag = tag + "\n" + pnt.ToString();
            pnt.suspectReading = aSuspectReading;

            POI.POIs.Add(pnt);

            if (POIModified != null)
                POIModified(null, null);
        }


        public static void POIAdd(PointLatLngAlt Point, string tag)
        {
            POIAdd( Point, tag, false );
        }

        public static void POIAdd(PointLatLngAlt Point)
        {
            if (Point == null)
                return;

            PointLatLngAlt pnt = Point;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            POIAdd(Point, output);
        }

        public static bool POIDelete(PointLatLngAlt Point)
        {
            if (Point == null)
                return false;

            for (int a = 0; a < POI.POIs.Count; a++)
            {
                if (POI.POIs[a].Point() == Point)
                {
                    POI.POIs.RemoveAt(a);
                    if (POIModified != null)
                        POIModified(null, null);
                    return true;
                }
            }

            return false;
        }

        public static void POIEdit(PointLatLngAlt Point)
        {
            if (Point == null)
                return;

            string output = "";

            if (DialogResult.OK != InputBox.Show("POI", "Enter ID", ref output))
                return;

            for (int a = 0; a < POI.POIs.Count; a++)
            {
                if (POI.POIs[a].Point() == Point)
                {
                    POI.POIs[a].Tag = output + "\n" + Point.ToString();
                    if (POIModified != null)
                        POIModified(null, null);
                    return;
                }
            }
        }

        public static void toggleExportStatus(PointLatLngAlt Point)
        {
            if (Point == null)
                return;

            for (int a = 0; a < POI.POIs.Count; a++)
            {
                if (POI.POIs[a].Point() == Point)
                {
                    POI.POIs[a].export = !POI.POIs[a].export;
                    if (POIModified != null)
                        POIModified(null, null);
                    return;
                }
            }
        }


        public static void POISave()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Poi File|*.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (Stream file = sfd.OpenFile())
                    {
                        foreach (var item in POI.POIs)
                        {
                            string line = item.Lat.ToString(CultureInfo.InvariantCulture) + "\t" +
                                          item.Lng.ToString(CultureInfo.InvariantCulture) + "\t" + item.Tag + "\r\n";
                            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                            file.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }

        public static void UpdateOverlay(GMap.NET.WindowsForms.GMapOverlay poioverlay)
        {
            if (poioverlay == null)
                return;

            poioverlay.Clear();

            foreach (var pnt in POIs)
            {
                GMarkerGoogle marker = null;
                if (pnt.export)
                    marker = new GMarkerGoogle(pnt, GMarkerGoogleType.red_small);
                else // not exporting this poi
                {
                    if ( pnt.groundTruth )
                        marker = new GMarkerGoogle(pnt, GMarkerGoogleType.green_dot);
                    else if( pnt.suspectReading )
                        marker = new GMarkerGoogle( pnt, GMarkerGoogleType.blue_small );
                    else 
                        marker = new GMarkerGoogle(pnt, GMarkerGoogleType.yellow_small );
                }

                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                marker.ToolTipText = pnt.Tag;
                poioverlay.Markers.Add(marker);                              
            }
        }


        internal static void setPOIs( ObservableCollection<PointLatLngAlt> lFused )
        {
            POIDeleteAllContacts();
            POIs = lFused;
            if(POIModified != null)
                POIModified( null, null );
        }
    }
}