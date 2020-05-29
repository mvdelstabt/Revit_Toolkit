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
using System.Linq;

using Autodesk.Revit.DB;

using BH.oM.Base;
using BH.oM.Adapters.Revit.Settings;
using BH.oM.Reflection.Attributes;

namespace BH.Revit.Engine.Core
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [DeprecatedAttribute("3.0", "BH.UI.Revit.Engine.Query.NextLevel method is not supported any more")]
        public static Level NextLevel(this Level level)
        {
            if (level == null)
                return null;

            List<Level> levels = new FilteredElementCollector(level.Document).OfClass(typeof(Level)).Cast<Level>().ToList();
            if (levels == null || levels.Count < 2)
                return null;

            levels.Sort((x, y) => x.ProjectElevation.CompareTo(y.ProjectElevation));

            int index = levels.FindIndex(x => x.Id == level.Id);
            if (index == -1)
                return null;

            if (index == levels.Count - 1)
                return null;

            return levels[index + 1];
        }

        /***************************************************/
    }
}