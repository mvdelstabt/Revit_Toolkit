/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.Collections.Generic;

using Autodesk.Revit.DB;

using BH.Engine.Geometry;
using BH.oM.Adapters.Revit.Settings;
using BH.oM.Geometry;
using System.Linq;

namespace BH.UI.Revit.Engine
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Polyline(this FamilyInstance familyInstance, PullSettings pullSettings = null)
        {
            if (familyInstance == null)
                return null;

            HostObject aHostObject = familyInstance.Host as HostObject;
            if (aHostObject == null)
                return null;

            List<PolyCurve> aPolyCurveList = Profiles(aHostObject, pullSettings);
            if (aPolyCurveList == null || aPolyCurveList.Count == 0)
                return null;

            List<oM.Geometry.Plane> aPlaneList = new List<oM.Geometry.Plane>(); 
            foreach(PolyCurve aPolyCurve in aPolyCurveList)
            {
                oM.Geometry.Plane aPlane_Temp = BH.Engine.Adapters.Revit.Query.Plane(aPolyCurve);
                if (aPlane_Temp != null)
                    aPlaneList.Add(aPlane_Temp);
            }

            List<ICurve> aCurveList = Query.Curves(familyInstance, new Options(), pullSettings);
            if (aCurveList == null || aCurveList.Count == 0)
                return null;

            double aMin = double.MaxValue;
            oM.Geometry.Plane aPlane = null;
            foreach (ICurve aICurve in aCurveList)
            {
                List<oM.Geometry.Point> aPointList = BH.Engine.Geometry.Query.IControlPoints(aICurve);
                if (aPointList == null || aPointList.Count == 0)
                    continue;

                foreach(oM.Geometry.Plane aPlane_Temp in aPlaneList)
                {
                    double aMin_Temp = aPointList.ConvertAll(x => BH.Engine.Geometry.Query.Distance(x, aPlane_Temp)).Min();
                    if(aMin_Temp < aMin)
                    {
                        aPlane = aPlane_Temp;
                        aMin = aMin_Temp;
                    }
                }
            }

            BoundingBox aBoundingBox = null;

            for (int i = 0; i < aCurveList.Count; i++)
            {
                ICurve aICurve = BH.Engine.Geometry.Modify.IProject(aCurveList[i], aPlane);

                if (aBoundingBox == null)
                    aBoundingBox = BH.Engine.Geometry.Query.IBounds(aICurve);
                else
                    aBoundingBox += BH.Engine.Geometry.Query.IBounds(aICurve);

                aCurveList[i] = aICurve;
            }

            oM.Geometry.Point aCenter = BH.Engine.Geometry.Query.Centre(aBoundingBox);

            XYZ aHandOrientation = familyInstance.HandOrientation;

            XYZ aFacingOrientation = familyInstance.FacingOrientation;

            XYZ aUpOrientation = familyInstance.FacingOrientation.CrossProduct(familyInstance.HandOrientation);
            aUpOrientation = aUpOrientation.Normalize();

            Vector aExtent = BH.Engine.Geometry.Query.Extents(aBoundingBox);

            Vector aUpDirection = Create.Vector(aUpOrientation.X * aExtent.X / 2, aUpOrientation.Y * aExtent.Y / 2, aUpOrientation.Z * aExtent.Z / 2);
            Vector aHandDirection = Create.Vector(aHandOrientation.X * aExtent.X / 2, aHandOrientation.Y * aExtent.Y / 2, aHandOrientation.Z * aExtent.Z / 2);

            Vector aHandDirection_Invert = Create.Vector(-aHandDirection.X, -aHandDirection.Y, -aHandDirection.Z);

            oM.Geometry.Point aPoint_1 = BH.Engine.Geometry.Modify.Translate(aCenter, aUpDirection);
            aPoint_1 = BH.Engine.Geometry.Modify.Translate(aPoint_1, aHandDirection_Invert);

            oM.Geometry.Point aPoint_2 = BH.Engine.Geometry.Modify.Translate(aCenter, aUpDirection);
            aPoint_2 = BH.Engine.Geometry.Modify.Translate(aPoint_2, aHandDirection);

            Vector aUpDirection_Invert = Create.Vector(-aUpDirection.X, -aUpDirection.Y, -aUpDirection.Z);

            oM.Geometry.Point aPoint_3 = BH.Engine.Geometry.Modify.Translate(aCenter, aUpDirection_Invert);
            aPoint_3 = BH.Engine.Geometry.Modify.Translate(aPoint_3, aHandDirection);

            oM.Geometry.Point aPoint_4 = BH.Engine.Geometry.Modify.Translate(aCenter, aUpDirection_Invert);
            aPoint_4 = BH.Engine.Geometry.Modify.Translate(aPoint_4, aHandDirection_Invert);


            return Create.Polyline(new oM.Geometry.Point[] {aPoint_1, aPoint_2, aPoint_3, aPoint_4, aPoint_1 });
        }

        /***************************************************/

    }
}