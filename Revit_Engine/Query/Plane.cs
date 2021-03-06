/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Adapters.Revit
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [ToBeRemoved("4.0", "BH.Engine.Adapters.Revit.Query.Plane method is not used any more.")]
        [Description("Returns a plane in which the PolyCurve lies. Null is returned if the curve is not planar.")]
        [Input("polyCurve", "PolyCurve to be queried.")]
        [Input("tolerance", "Geometrical tolerance to be applied to planarity check.")]
        [Output("plane")]
        public static Plane Plane(this PolyCurve polyCurve, double tolerance = Tolerance.Distance)
        {
            if (polyCurve == null)
                return null;

            List<Point> points = Geometry.Query.ControlPoints(polyCurve);

            if (points == null || points.Count < 2)
                return null;

            if (!Geometry.Query.IsCoplanar(points, tolerance))
                return null;

            return Plane(points);
        }

        /***************************************************/

        [ToBeRemoved("4.0", "BH.Engine.Adapters.Revit.Query.Plane method is not used any more.")]
        [Description("Returns a plane in which the Polyline lies. Null is returned if the curve is not planar.")]
        [Input("polyline", "Polyline to be queried.")]
        [Input("tolerance", "Geometrical tolerance to be applied to planarity check.")]
        [Output("plane")]
        public static Plane Plane(this Polyline polyline, double tolerance = Tolerance.Distance)
        {
            if (polyline == null)
                return null;

            if (polyline.ControlPoints == null || polyline.ControlPoints.Count < 2)
                return null;

            if (!Geometry.Query.IsCoplanar(polyline.ControlPoints, tolerance))
                return null;

            return Plane(polyline.ControlPoints);
        }

        /***************************************************/

        [ToBeRemoved("4.0", "BH.Engine.Adapters.Revit.Query.Plane method is not used any more.")]
        [Description("Returns a plane spanned on a collection of points. Null is returned if the point set is not planar.")]
        [Input("points", "Set of points to be queried.")]
        [Output("plane")]
        public static Plane Plane(this IEnumerable<Point> points)
        {
            if (points == null || points.Count() < 2)
                return null;

            Point point1 = points.ElementAt(0);
            Point point2 = null;
            Point point3 = null;

            for (int i = 1; i < points.Count() - 1; i++)
            {
                point2 = points.ElementAt(i);
                for (int j = 1; j < points.Count() - 1; j++)
                {
                    point3 = points.ElementAt(j);

                    Vector vector = Geometry.Query.CrossProduct(point2 - point1, point3 - point1);
                    if (Geometry.Query.Length(vector) > Tolerance.Distance)
                        return Geometry.Create.Plane(point1, point2, point3);
                }
            }

            return null;
        }

        /***************************************************/
    }
}

