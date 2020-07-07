﻿/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using Autodesk.Revit.DB;
using BH.Engine.Adapters.Revit;
using BH.Engine.Geometry;
using BH.oM.Adapters.Revit.Settings;
using BH.oM.Geometry;
using BH.oM.Physical.Elements;
using BH.oM.Reflection;
using System;
using System.Linq;

namespace BH.Revit.Engine.Core
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static ICurve LocationCurve(this FamilyInstance familyInstance, RevitSettings settings = null)
        {
            settings = settings.DefaultIfNull();
            ICurve curve = null;

            if (typeof(Column).BuiltInCategories().Contains((BuiltInCategory)familyInstance.Category.Id.IntegerValue))
                curve = familyInstance.LocationCurveColumn(settings);
            else if (typeof(IFramingElement).BuiltInCategories().Contains((BuiltInCategory)familyInstance.Category.Id.IntegerValue))
                curve = familyInstance.LocationCurveFraming(settings);

            return curve;
        }

        /***************************************************/
        
        public static ICurve LocationCurveFraming(this FamilyInstance familyInstance, RevitSettings settings = null)
        {
            settings = settings.DefaultIfNull();

            ICurve curve = (familyInstance.Location as LocationCurve)?.Curve?.IFromRevit();
            if (curve == null || (!(curve is NurbsCurve) && curve.ILength() <= settings.DistanceTolerance))
            {
                familyInstance.FramingCurveNotFoundWarning();
                return null;
            }

            BH.oM.Geometry.Line line = curve as BH.oM.Geometry.Line;
            if (line == null)
            {
                familyInstance.NonLinearFramingOffsetWarning();
                return curve;
            }

            Transform transform = familyInstance.GetTotalTransform();
            Vector dir = line.Direction();
            BH.oM.Geometry.Plane startPlane = new oM.Geometry.Plane { Origin = line.Start, Normal = dir };
            BH.oM.Geometry.Plane endPlane = new oM.Geometry.Plane { Origin = line.End, Normal = dir };
            BH.oM.Geometry.Line transformedLine = BH.Engine.Geometry.Create.Line(transform.Origin.PointFromRevit(), transform.BasisX.VectorFromRevit());
            return new BH.oM.Geometry.Line { Start = transformedLine.PlaneIntersection(startPlane, true), End = transformedLine.PlaneIntersection(endPlane, true) };
        }

        /***************************************************/

        public static BH.oM.Geometry.Line LocationCurveColumn(this FamilyInstance familyInstance, RevitSettings settings = null)
        {
            settings = settings.DefaultIfNull();

            BH.oM.Geometry.Line curve;

            if (familyInstance.IsSlantedColumn)
                curve = (familyInstance.Location as LocationCurve).Curve.IFromRevit() as BH.oM.Geometry.Line;
            else
            {
                XYZ loc = (familyInstance.Location as LocationPoint).Point;
                Parameter baseLevelParam = familyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
                Parameter topLevelParam = familyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
                Parameter baseOffsetParam = familyInstance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
                Parameter topOffsetParam = familyInstance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

                double baseLevel = (familyInstance.Document.GetElement(baseLevelParam.AsElementId()) as Level).ProjectElevation;
                double topLevel = (familyInstance.Document.GetElement(topLevelParam.AsElementId()) as Level).ProjectElevation;
                double baseOffset = baseOffsetParam.AsDouble();
                double topOffset = topOffsetParam.AsDouble();

                XYZ baseNode = new XYZ(loc.X, loc.Y, baseLevel + baseOffset);
                XYZ topNode = new XYZ(loc.X, loc.Y, topLevel + topOffset);
                curve = new oM.Geometry.Line { Start = baseNode.PointFromRevit(), End = topNode.PointFromRevit() };
            }

            Output<double, double> extensions = familyInstance.ColumnExtensions(settings);
            double startExtension = extensions.Item1;
            double endExtension = extensions.Item2;

            if (Math.Abs(startExtension) > settings.DistanceTolerance || Math.Abs(endExtension) > settings.DistanceTolerance)
            {
                Vector direction = curve.Direction();
                curve = new oM.Geometry.Line { Start = curve.Start - direction * startExtension, End = curve.End + direction * endExtension };
            }

            if (curve.Length() <= settings.DistanceTolerance)
            {
                familyInstance.FramingCurveNotFoundWarning();
                return null;
            }

            return curve;
        }

        /***************************************************/
    }
}
