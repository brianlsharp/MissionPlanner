using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MissionPlanner.Utilities;
using GMap.NET;
using System.Diagnostics;

namespace MissionPlanner.GCSViews
{
    public class ContactFuser
    {
        PureProjection mProjection;
        double mEpsilon;
        public ContactFuser(PureProjection aProjection)
        {
            mProjection = aProjection;
            mEpsilon = 10.0F; // distance in cm
        }

        public ObservableCollection<PointLatLngAlt> fuse( ObservableCollection<PointLatLngAlt> aInputs )
        {
            ObservableCollection<PointLatLngAlt> lRet = new ObservableCollection<PointLatLngAlt>(  );

            for(int i = 0; i < aInputs.Count(); i++ )
            {
                PointLatLngAlt lOuterLoop = aInputs[ i ];
                int j = i +1;
                for( ; j < aInputs.Count(); j++ )
                {
                    PointLatLngAlt lInnerLoop = aInputs[ j ];
                    double lDistance = mProjection.GetDistance( lInnerLoop, lOuterLoop );
                    lDistance = lDistance * 100000; // convert km to cm

                    if(lDistance < mEpsilon)
                    {
                        Debug.WriteLine( "Element #{0} is {1} centimeters away from element {2}. Not being added to return list", j, lDistance, i );
                        break;  // no need to compare anymore, go back to the outer loop
                    }
                    else
                        continue; // compare to the next number
                }
                if( j == aInputs.Count() )
                {
                    Debug.WriteLine( "Adding Element {0} to return list.", i );
                    lRet.Add( lOuterLoop );
                }
            }

                return lRet;
//            return lRet;
        }
    }


}
