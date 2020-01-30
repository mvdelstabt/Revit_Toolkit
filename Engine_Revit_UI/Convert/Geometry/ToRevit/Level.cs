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

using Autodesk.Revit.DB;
using BH.oM.Adapters.Revit.Settings;
using System.Linq;

namespace BH.UI.Revit.Engine
{
    public static partial class Convert
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static Level ToRevitLevel(this oM.Geometry.SettingOut.Level level, Document document, PushSettings pushSettings = null)
        {
            Level revitLevel = pushSettings.FindRefObject<Level>(document, level.BHoM_Guid);
            if (revitLevel != null)
                return revitLevel;

            pushSettings.DefaultIfNull();

            ElementId elementID = level.ElementId();

            if (elementID != null && elementID != ElementId.InvalidElementId)
                revitLevel = document.GetElement(elementID) as Level;

            if (revitLevel == null)
                revitLevel = new FilteredElementCollector(document).OfClass(typeof(Level)).Cast<Level>().ToList().Find(x => x.Name == level.Name);

            if (revitLevel == null)
            {
                double elevation = level.Elevation.FromSI(UnitType.UT_Length);

                revitLevel = Level.Create(document, elevation);
                revitLevel.Name = level.Name;
            }

            revitLevel.CheckIfNullPush(level);
            if (revitLevel == null)
                return null;

            if (pushSettings.CopyCustomData)
                Modify.SetParameters(revitLevel, level, new BuiltInParameter[] { BuiltInParameter.DATUM_TEXT, BuiltInParameter.LEVEL_ELEV });

            pushSettings.RefObjects = pushSettings.RefObjects.AppendRefObjects(level, revitLevel);

            return revitLevel;
        }

        /***************************************************/
    }
}
