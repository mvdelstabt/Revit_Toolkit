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

using System.Collections.Generic;
using System.Linq;

using BH.oM.Adapters.Revit.Settings;
using BH.oM.Base;
using System;
using Autodesk.Revit.DB;

namespace BH.UI.Revit.Engine
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static T FindRefObject<T>(this PullSettings pullSettings, int elementId) where T : IBHoMObject
        {
            if (pullSettings.RefObjects == null)
                return default(T);

            List<T> aTList = FindRefObjects<T>(pullSettings, elementId);
            if (aTList != null && aTList.Count > 0)
                return aTList.First();

            return default(T);
        }

        /***************************************************/

        public static T FindRefObject<T>(this PushSettings pushSettings, Document Document, Guid guid) where T : Element
        {
            if (pushSettings.RefObjects == null)
                return null;

            List<T> aTList = FindRefObjects<T>(pushSettings, Document, guid);
            if (aTList != null && aTList.Count > 0)
                return aTList.First();

            return default(T);
        }

        /***************************************************/
    }
}