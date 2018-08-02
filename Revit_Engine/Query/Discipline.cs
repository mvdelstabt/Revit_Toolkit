﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Adapters.Revit;

using BH.oM.Base;

using Autodesk.Revit.DB;

namespace BH.Engine.Revit
{
    /// <summary>
    /// BHoM Revit Engine Query Methods
    /// </summary>
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        /// <summary>
        /// Returs discipline of BHoM type. Default value: Revit.Discipline.Environmental
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns name="ElementId">Revit ElementId</returns>
        /// <search>
        /// Query, BHoM, discipline, Discipline, BHoMObject
        /// </search>
        public static Discipline Discipline(this Type type)
        {
            if (type == null)
                return oM.Adapters.Revit.Discipline.Environmental;

            if(type.Namespace.StartsWith("BH.oM.Structural"))
                return oM.Adapters.Revit.Discipline.Structural;

            if (type.Namespace.StartsWith("BH.oM.Environment"))
                return oM.Adapters.Revit.Discipline.Environmental;

            if (type.Namespace.StartsWith("BH.oM.Architecture"))
                return oM.Adapters.Revit.Discipline.Architecture;

            return oM.Adapters.Revit.Discipline.Environmental;
        }

        /***************************************************/
    }
}