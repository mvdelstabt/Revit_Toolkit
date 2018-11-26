﻿using Autodesk.Revit.DB;
using BH.oM.Adapters.Revit.Settings;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB.IFC;
using BH.oM.Geometry;

namespace BH.UI.Cobra.Engine
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        static public List<PolyCurve> Profiles(this Element element, PullSettings pullSettings = null)
        {
            if (element == null || element.Document == null)
                return null;

            Document aDocument = element.Document;

            IEnumerable<ElementId> aElementIds = null;
            using (Transaction aTransaction = new Transaction(aDocument, "Temp"))
            {
                aTransaction.Start();
                aElementIds = aDocument.Delete(element.Id);
                aTransaction.RollBack();
            }

            if (aElementIds == null || aElementIds.Count() == 0)
                return null;

            List<PolyCurve> aResult = new List<PolyCurve>();
            foreach (ElementId aElementId in aElementIds)
            {
                Element aElement = aDocument.GetElement(aElementId);
                if (aElement == null)
                    continue;

                if (aElement is Sketch)
                {
                    Sketch aSketch = (Sketch)aElement;

                    if (aSketch.Profile == null)
                        continue;

                    List<List<ICurve>> aCurveListList = Convert.ToBHoM(aSketch.Profile, pullSettings);
                    if (aCurveListList == null)
                        continue;

                    foreach (List<ICurve> aCureList in aCurveListList)
                        if (aCureList != null)
                            aResult.Add(BH.Engine.Geometry.Create.PolyCurve(aCureList));
                }
            }

            if (element is Wall && aResult.Count == 0)
                return Profiles_Wall((Wall)element, pullSettings);

            return aResult;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<PolyCurve> Profiles_Wall(this Wall wall, PullSettings pullSettings = null)
        {
            List<PolyCurve> aPolyCurveList = null;

            BoundingBoxXYZ aBoundingBoxXYZ = wall.get_BoundingBox(null);
            if (aBoundingBoxXYZ != null)
            {
                LocationCurve aLocationCurve = wall.Location as LocationCurve;
                if (aLocationCurve != null)
                {
                    ICurve aCurve = Convert.ToBHoM(aLocationCurve, pullSettings);
                    if (aCurve != null)
                    {
                        oM.Geometry.Plane aPlane = null;

                        double aMax = aBoundingBoxXYZ.Max.Z;
                        if (pullSettings != null && pullSettings.ConvertUnits)
                            aMax = UnitUtils.ConvertFromInternalUnits(aMax, DisplayUnitType.DUT_METERS);

                        aPlane = BH.Engine.Geometry.Create.Plane(BH.Engine.Geometry.Create.Point(0, 0, aMax), BH.Engine.Geometry.Create.Vector(0, 0, 1));
                        ICurve aCurve_Max = BH.Engine.Geometry.Modify.Project(aCurve as dynamic, aPlane);


                        double aMin = aBoundingBoxXYZ.Min.Z;
                        if (pullSettings != null && pullSettings.ConvertUnits)
                            aMin = UnitUtils.ConvertFromInternalUnits(aMin, DisplayUnitType.DUT_METERS);

                        aPlane = BH.Engine.Geometry.Create.Plane(BH.Engine.Geometry.Create.Point(0, 0, aMin), BH.Engine.Geometry.Create.Vector(0, 0, 1));
                        ICurve aCurve_Min = BH.Engine.Geometry.Modify.Project(aCurve as dynamic, aPlane);

                        oM.Geometry.Point aPoint_1;
                        oM.Geometry.Point aPoint_2;

                        aPoint_1 = BH.Engine.Geometry.Query.StartPoint(aCurve_Min as dynamic);
                        aPoint_2 = BH.Engine.Geometry.Query.StartPoint(aCurve_Max as dynamic);

                        oM.Geometry.Line aLine_1 = BH.Engine.Geometry.Create.Line(aPoint_1, aPoint_2);

                        aPoint_1 = BH.Engine.Geometry.Query.EndPoint(aCurve_Max as dynamic);
                        aPoint_2 = BH.Engine.Geometry.Query.EndPoint(aCurve_Min as dynamic);

                        oM.Geometry.Line aLine_2 = BH.Engine.Geometry.Create.Line(aPoint_1, aPoint_2);


                        aPolyCurveList = new List<PolyCurve>();
                        aPolyCurveList.Add(BH.Engine.Geometry.Create.PolyCurve(new ICurve[] {aCurve_Min, aLine_1, aCurve_Max, aLine_2}));
                        return aPolyCurveList;
                    }

                }
            }


            if (!ExporterIFCUtils.HasElevationProfile(wall))
                return null;

            IList<CurveLoop> aCurveLoopList = ExporterIFCUtils.GetElevationProfile(wall);
            if (aCurveLoopList == null)
                return null;

            aPolyCurveList = new List<PolyCurve>();
            foreach(CurveLoop aCurveLoop in aCurveLoopList)
            {
                PolyCurve aPolyCurve = Convert.ToBHoM(aCurveLoop, pullSettings);
                if (aPolyCurve != null)
                    aPolyCurveList.Add(aPolyCurve);
            }
            return aPolyCurveList;
        }

        /***************************************************/
    }
}
