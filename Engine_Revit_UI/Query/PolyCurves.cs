/*
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

using System.Collections.Generic;

using Autodesk.Revit.DB;

using BH.Engine.Geometry;
using BH.oM.Adapters.Revit.Settings;
using BH.oM.Geometry;

namespace BH.UI.Revit.Engine
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static List<PolyCurve> PolyCurves(this Autodesk.Revit.DB.Face face, Transform transform = null, PullSettings pullSettings = null)
        {
            if (face == null)
                return null;

            List<PolyCurve> result = new List<PolyCurve>();

            foreach (CurveLoop curveLoop in face.GetEdgesAsCurveLoops())
            {
                List<ICurve> curves = new List<ICurve>();
                foreach (Curve curve in curveLoop)
                {
                    ICurve crv = null;

                    if (transform != null)
                        crv = curve.CreateTransformed(transform).IToBHoM();
                    else
                        crv = curve.IToBHoM();

                    curves.Add(crv);
                }
                result.Add(new oM.Geometry.PolyCurve { Curves = curves });
            }

            return result;
        }

        /***************************************************/

        public static List<PolyCurve> PolyCurves(this Element element, IEnumerable<Reference> references, PullSettings pullSettings = null)
        {
            List<PolyCurve> polycurves = new List<PolyCurve>();

            foreach (Reference reference in references)
            {
                Autodesk.Revit.DB.Face face = element.GetGeometryObjectFromReference(reference) as Autodesk.Revit.DB.Face;
                if (face == null)
                    continue;

                List<PolyCurve> tempCurves = null;

                if (face is PlanarFace)
                    tempCurves = ((PlanarFace)face).PolyCurves(null, pullSettings);
                else
                    tempCurves = face.Triangulate().PolyCurves(pullSettings);

                if (tempCurves == null || tempCurves.Count == 0)
                    continue;

                polycurves.AddRange(tempCurves);
            }

            return polycurves;
        }

        /***************************************************/

        public static List<PolyCurve> PolyCurves(this Autodesk.Revit.DB.Mesh mesh, PullSettings pullSettings = null)
        {
            if (mesh == null)
                return null;

            List<PolyCurve> result = new List<PolyCurve>();
            for (int i=0; i < mesh.NumTriangles; i++)
            {
                PolyCurve pcurve = mesh.get_Triangle(i).PolyCurve(pullSettings);
                if (pcurve != null)
                    result.Add(pcurve);
            }
            return result;
        }

        /***************************************************/
    }
}
