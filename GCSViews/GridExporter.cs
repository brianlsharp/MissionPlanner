using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            mOriginMarker = null;
            OtherReferencePoint = null;
        }
    }
}

