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


        public GridExporter()
        {
            OriginMarker = null;
            OtherReferencePoint = null;
        }

        public void export()
        {
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
                            
                            
                            byte[] buffer = ASCIIEncoding.ASCII.GetBytes(line);
                            file.Write(buffer, 0, buffer.Length);
                        }
                    }
                }
            }
        }
    }
}

