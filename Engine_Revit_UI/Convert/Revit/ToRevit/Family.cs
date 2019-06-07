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

using Autodesk.Revit.DB;

using BH.oM.Adapters.Revit.Properties;
using BH.oM.Adapters.Revit.Settings;

namespace BH.UI.Revit.Engine
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Internal Methods                          ****/
        /***************************************************/

        internal static Family ToRevitFamily(this oM.Adapters.Revit.Elements.Family family, Document document, PushSettings pushSettings = null)
        {
            Family aFamily = pushSettings.FindRefObject<Family>(document, family.BHoM_Guid);
            if (aFamily != null)
                return aFamily;

            pushSettings.DefaultIfNull();

            if (family.PropertiesList != null && family.PropertiesList.Count > 0)
            {
                foreach(InstanceProperties aInstanceProperties in family.PropertiesList)
                    ToRevitElementType(aInstanceProperties, document, pushSettings);
            }

            BuiltInCategory aBuiltInCategory = family.BuiltInCategory(document, pushSettings.FamilyLoadSettings);

            aFamily = family.Family(document, aBuiltInCategory, pushSettings.FamilyLoadSettings);

            aFamily.CheckIfNullPush(family);
            if (aFamily == null)
                return null;

            if (pushSettings.CopyCustomData)
                Modify.SetParameters(aFamily, family, null, pushSettings.ConvertUnits);

            pushSettings.RefObjects = pushSettings.RefObjects.AppendRefObjects(family, aFamily);

            return aFamily;
        }

        /***************************************************/
    }
}