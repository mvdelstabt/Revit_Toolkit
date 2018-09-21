﻿using System.Collections.Generic;

using BH.oM.Base;
using BH.oM.DataManipulation.Queries;

namespace BH.Engine.Adapters.Revit
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IEnumerable<FilterQuery> FilterQueries(this FilterQuery filterQuery)
        {
            if (filterQuery == null)
                return null;

            if (!filterQuery.Equalities.ContainsKey(Convert.FilterQuery.FilterQueries))
                return null;

            return filterQuery.Equalities[Convert.FilterQuery.FilterQueries] as IEnumerable<FilterQuery>;
        }

        /***************************************************/
    }
}