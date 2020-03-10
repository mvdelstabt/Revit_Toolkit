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
using BH.oM.Adapters.Revit.Settings;
using BH.oM.Base;
using BH.oM.Environment.Fragments;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.UI.Revit.Engine
{
    public static partial class Convert
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static List<oM.Physical.Elements.Roof> ToBHoMRoofs(this RoofBase roofBase, RevitSettings settings = null, Dictionary<string, List<IBHoMObject>> refObjects = null)
        {
            settings = settings.DefaultIfNull();

            List<oM.Physical.Elements.Roof> roofs = refObjects.GetValues<oM.Physical.Elements.Roof>(roofBase.Id);
            if (roofs != null && roofs.Count != 0)
                return roofs;

            HostObjAttributes hostObjAttributes = roofBase.Document.GetElement(roofBase.GetTypeId()) as HostObjAttributes;
            oM.Physical.Constructions.Construction construction = hostObjAttributes.ToBHoMConstruction(settings, refObjects);
            string materialGrade = roofBase.MaterialGrade();
            construction = construction.UpdateMaterialProperties(hostObjAttributes, materialGrade, settings, refObjects);

            Dictionary<PlanarSurface, List<oM.Physical.Elements.IOpening>> dictionary = roofBase.PlanarSurfaceDictionary(true, settings);
            if (dictionary == null)
                return null;

            roofs = new List<oM.Physical.Elements.Roof>();
            foreach (KeyValuePair<PlanarSurface, List<oM.Physical.Elements.IOpening>> kvp in dictionary)
            {
                oM.Physical.Elements.Roof bHoMRoof = BH.Engine.Physical.Create.Roof(construction, kvp.Key);

                if (kvp.Value != null)
                    bHoMRoof.Openings = kvp.Value;

                bHoMRoof.Name = roofBase.FamilyTypeFullName();

                //BEnv origin context fragment
                OriginContextFragment originContext = roofBase.OriginContext(settings);

                //Set identifiers & custom data
                bHoMRoof.SetIdentifiers(roofBase);
                bHoMRoof.SetCustomData(roofBase);
                bHoMRoof.UpdateValues(settings, roofBase);

                refObjects.AddOrReplace(roofBase.Id, bHoMRoof);
                roofs.Add(bHoMRoof);
            }

            return roofs;
        }

        /***************************************************/
    }
}
