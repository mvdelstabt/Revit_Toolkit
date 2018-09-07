﻿using System;
using System.Collections.Generic;

using Autodesk.Revit.DB;

using BH.Engine.Environment;
using BH.oM.Environment.Elements;
using BH.oM.Adapters.Revit.Settings;

namespace BH.UI.Cobra.Engine
{
    public static partial class Convert
    {
        /***************************************************/
        /****             Internal methods              ****/
        /***************************************************/

        internal static List<BuildingElementPanel> ToBHoMBuildingElementPanels(this PlanarFace planarFace, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            List<BuildingElementPanel> aResult = new List<BuildingElementPanel>();
            
            EdgeArrayArray aEdgeArrayArray = planarFace.EdgeLoops;
            if (aEdgeArrayArray == null && aEdgeArrayArray.Size == 0)
                return aResult;
            
            for (int i = 0; i < aEdgeArrayArray.Size; i++)
            {
                EdgeArray aEdgeArray = aEdgeArrayArray.get_Item(i);
                List<oM.Geometry.ICurve> aCurveList = new List<oM.Geometry.ICurve>();
                foreach (Edge aEdge in aEdgeArray)
                {
                    Curve aCurve = aEdge.AsCurve();
                    if (aCurve != null)
                        aCurveList.Add(aCurve.ToBHoM(pullSettings));
                }

                if (aCurveList != null && aCurveList.Count > 0)
                {
                    BuildingElementPanel aBuildingElementPanel = new BuildingElementPanel();
                    aBuildingElementPanel = aBuildingElementPanel.SetGeometry(BH.Engine.Geometry.Create.PolyCurve(aCurveList));
                    aResult.Add(aBuildingElementPanel);
                }
            }
            return aResult;
        }

        /***************************************************/

        internal static List<BuildingElementPanel> ToBHoMBuildingElementPanels(this Element element, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            return ToBHoMBuildingElementPanels(element.get_Geometry(new Options()), pullSettings);
        }

        /***************************************************/

        internal static List<BuildingElementPanel> ToBHoMBuildingElementPanels(this RoofBase roofBase, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            return ToBHoMBuildingElementPanels(roofBase.get_Geometry(new Options()), pullSettings);
        }

        /***************************************************/

        internal static List<BuildingElementPanel> ToBHoMBuildingElementPanels(this FamilyInstance familyInstance, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            List<BuildingElementPanel> aResult = new List<BuildingElementPanel>();

            //TODO: Get more accurate shape. Currently, Windows and doors goes as rectangular panel
            BoundingBoxXYZ aBoundingBoxXYZ = familyInstance.get_BoundingBox(null);

            XYZ aVector = aBoundingBoxXYZ.Max - aBoundingBoxXYZ.Min;

            double aWidth = Math.Abs(aVector.Y);
            double aHeight = Math.Abs(aVector.Z);

            XYZ aVector_Y = (aBoundingBoxXYZ.Transform.BasisY * aWidth) / 2;
            XYZ aVector_Z = (aBoundingBoxXYZ.Transform.BasisZ * aHeight) / 2;

            XYZ aMiddle = (aBoundingBoxXYZ.Max + aBoundingBoxXYZ.Min) / 2;

            XYZ aXYZ_1 = aMiddle + aVector_Z - aVector_Y;
            XYZ aXYZ_2 = aMiddle + aVector_Z + aVector_Y;
            XYZ aXYZ_3 = aMiddle - aVector_Z + aVector_Y;
            XYZ aXYZ_4 = aMiddle - aVector_Z - aVector_Y;

            List<oM.Geometry.Point> aPointList = new List<oM.Geometry.Point>();
            aPointList.Add(aXYZ_1.ToBHoM(pullSettings));
            aPointList.Add(aXYZ_2.ToBHoM(pullSettings));
            aPointList.Add(aXYZ_3.ToBHoM(pullSettings));
            aPointList.Add(aXYZ_4.ToBHoM(pullSettings));
            aPointList.Add(aXYZ_1.ToBHoM(pullSettings));

            BuildingElementPanel aBuildingElementPanel = Create.BuildingElementPanel(new oM.Geometry.Polyline[] { BH.Engine.Geometry.Create.Polyline(aPointList) });
            if (aBuildingElementPanel != null)
                aResult.Add(aBuildingElementPanel);

            return aResult;
        }

        /***************************************************/

        internal static List<BuildingElementPanel> ToBHoMBuildingElementPanels(this GeometryElement geometryElement, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            List<BuildingElementPanel> aResult = new List<BuildingElementPanel>();
            foreach (GeometryObject aGeometryObject in geometryElement)
            {
                Solid aSolid = aGeometryObject as Solid;
                if (aSolid == null)
                    continue;

                PlanarFace aPlanarFace = Query.Top(aSolid);
                if (aPlanarFace == null)
                    continue;

                List<BuildingElementPanel> aBuildingElementPanelList = aPlanarFace.ToBHoMBuildingElementPanels(pullSettings);
                if (aBuildingElementPanelList == null || aBuildingElementPanelList.Count < 1)
                    continue;

                aResult.AddRange(aBuildingElementPanelList);
            }

            return aResult;

        }

        /***************************************************/
    }
}