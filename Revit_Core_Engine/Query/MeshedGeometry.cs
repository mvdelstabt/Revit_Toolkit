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

using Autodesk.Revit.DB;
using BH.oM.Adapters.Revit.Settings;
using System.Collections.Generic;
using System.Linq;

namespace BH.Revit.Engine.Core
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static List<oM.Geometry.Mesh> MeshedGeometry(this Element element, Options options, RevitSettings settings = null)
        {
            if (element == null)
                return null;

            List<oM.Geometry.Mesh> result = element.Faces(options, settings).Select(x => x.Triangulate(options.DetailLevel.FaceTriangulationFactor()).MeshFromRevit()).ToList();
            result.AddRange(element.GeometryPrimitives(options, settings).Where(x => x is Mesh).Cast<Mesh>().Select(x => x.MeshFromRevit()));
            result.AddRange(element.Curves(options, settings, false).Select(x => x.MeshFromRevit()));
            return result;
        }

        /***************************************************/
    }
}


