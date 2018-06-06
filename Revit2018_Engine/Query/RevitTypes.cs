﻿using System.Collections.Generic;

using Autodesk.Revit.DB;

using BH.oM.Environmental.Elements;
using BH.oM.Structural.Elements;
using BH.oM.Base;

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
        /// Get Revit class types from BHoM class type.
        /// </summary>
        /// <param name="type">BHoM class Type</param>
        /// <returns name="Types">Revit class Types</returns>
        /// <search>
        /// Query, RevitTypes, Revit, Get Revit Types, Type, BHoM Type
        /// </search>
        public static IEnumerable<System.Type> RevitTypes(System.Type type)
        {
            if (type == null)
                return null;

            if (!Query.IsAssignableFromByFullName(type, typeof(BHoMObject)))
                return null;

            List<System.Type> aResult = new List<System.Type>();
            if (type == typeof(BuildingElement))
            {
                aResult.Add(typeof(Floor));
                aResult.Add(typeof(Wall));
                aResult.Add(typeof(Ceiling));
                aResult.Add(typeof(RoofBase));
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if (type == typeof(PanelPlanar))
            {
                aResult.Add(typeof(Floor));
                aResult.Add(typeof(Wall));
                return aResult;
            }
            
            if (type == typeof(FramingElement))
            {
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if (type == typeof(Building))
            {
                aResult.Add(typeof(Document));
                return aResult;
            }

            if (type == typeof(oM.Architecture.Elements.Level))
            {
                aResult.Add(typeof(Level));
                return aResult;
            }

            if (type == typeof(Space))
            {
                aResult.Add(typeof(SpatialElement));
                return aResult;
            }

            if (type == typeof(oM.Architecture.Elements.Grid))
            {
                aResult.Add(typeof(Grid));
                return aResult;
            }

            return null;
        }

        /***************************************************/
    }
}
