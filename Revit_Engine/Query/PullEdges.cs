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
using System.ComponentModel;

using BH.oM.Data.Requests;
using BH.oM.Reflection.Attributes;
using System.Linq;

namespace BH.Engine.Adapters.Revit
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        //[Description("Returns true if FilterRequest should pull edges from Revit Element")]
        //[Input("filterRequest", "FilterRequest")]
        //[Output("PullEdges")]
        //public static bool PullEdges(this FilterRequest filterRequest)
        //{
        //    if (filterRequest == null)
        //        return false;

        //    if (filterRequest.Equalities.ContainsKey(Convert.FilterRequest.PullEdges))
        //    {
        //        object obj = filterRequest.Equalities[Convert.FilterRequest.PullEdges];
        //        if (obj is bool)
        //            return (bool)obj;
        //    }

        //    return false;
        //}

        /***************************************************/

        //[Description("Returns true if at least one FilterRequest on list should pull edges from Revit Element")]
        //[Input("filterRequests", "FilterRequests")]
        //[Output("PullEdges")]
        //public static bool PullEdges(this IEnumerable<FilterRequest> filterRequests)
        //{
        //    if (filterRequests == null)
        //        return false;

        //    return filterRequests.ToList().Any(x => x.PullEdges());
        //}

        /***************************************************/
    }
}
