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
using BH.Engine.Adapters.Revit;
using BH.oM.Adapters.Revit;
using BH.oM.Adapters.Revit.Settings;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Revit.Engine.Core
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static RevitMaterialTakeOff MaterialTakeoff(this Element element, RevitSettings settings, Dictionary<string, List<IBHoMObject>> refObjects = null)
        {
            settings = settings.DefaultIfNull();

            Dictionary<BH.oM.Physical.Materials.Material, double> takeoff = new Dictionary<BH.oM.Physical.Materials.Material, double>();
            double totalVolume = 0;
            string grade = element.MaterialGrade(settings);
            foreach (ElementId materialId in element.GetMaterialIds(false))
            {
                double volume = element.GetMaterialVolume(materialId).ToSI(UnitType.UT_Volume);
                if (volume <= settings.DistanceTolerance)
                    continue;

                Material material = (Material)element.Document.GetElement(materialId);
                BH.oM.Physical.Materials.Material bHoMMaterial = material.MaterialFromRevit(grade, settings, refObjects);
                if (takeoff.ContainsKey(bHoMMaterial))
                    takeoff[bHoMMaterial] += volume;
                else
                    takeoff[bHoMMaterial] = volume;

                totalVolume += volume;
            }

            if (takeoff.Count != 0)
                return new RevitMaterialTakeOff(totalVolume, new oM.Physical.Materials.MaterialComposition(takeoff.Keys, takeoff.Values.Select(x => x / totalVolume)));
            else
                return null;
        }

        /***************************************************/
    }
}
