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
using BH.oM.Adapters.Revit.Settings;
using System;
using System.Collections.Generic;

namespace BH.Revit.Engine.Core
{
    public static partial class Convert
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static FamilySymbol ToRevitElementType(this oM.Physical.FramingProperties.IFramingElementProperty framingElementProperty, Document document, IEnumerable<BuiltInCategory> categories = null, RevitSettings settings = null, Dictionary<Guid, List<int>> refObjects = null)
        {
            if (framingElementProperty == null || document == null)
                return null;

            FamilySymbol familySymbol = refObjects.GetValue<FamilySymbol>(document, framingElementProperty.BHoM_Guid);
            if (familySymbol != null)
                return familySymbol;

            settings = settings.DefaultIfNull();

            familySymbol = framingElementProperty.ElementType(document, categories, settings) as FamilySymbol;
            if (familySymbol == null)
                return null;

            // Copy parameters from BHoM object to Revit element
            familySymbol.CopyParameters(framingElementProperty, settings);

            refObjects.AddOrReplace(framingElementProperty, familySymbol);
            return familySymbol;
        }

        /***************************************************/

        public static HostObjAttributes ToRevitElementType(this oM.Physical.Constructions.IConstruction construction, Document document, IEnumerable<BuiltInCategory> categories = null, RevitSettings settings = null, Dictionary<Guid, List<int>> refObjects = null)
        {
            if (construction == null || document == null)
                return null;

            HostObjAttributes elementType = refObjects.GetValue<HostObjAttributes>(document, construction.BHoM_Guid);
            if (elementType != null)
                return elementType;

            settings = settings.DefaultIfNull();

            elementType = construction.ElementType(document, categories, settings) as HostObjAttributes;
            if (elementType == null)
                return null;

            // Copy parameters from BHoM object to Revit element
            elementType.CopyParameters(construction, settings);

            refObjects.AddOrReplace(construction, elementType);
            return elementType;
        }

        /***************************************************/
    }
}

