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

using Autodesk.Revit.DB;

using BH.oM.Adapters.Revit.Elements;
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BH.oM.Structure.Elements;


namespace BH.UI.Revit.Engine
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        public static IEnumerable<System.Type> RevitTypes(System.Type type)
        {
            if (type == null)
                return null;

            if (!typeof(IBHoMObject).IsAssignableFrom(type))//BH.Engine.Adapters.Revit.Query.IsAssignableFromByFullName(type, typeof(BHoMObject)))
                return null;

            List<System.Type> aResult = new List<System.Type>();
            if (type == typeof(oM.Environment.Elements.Panel))
            {
                aResult.Add(typeof(Floor));
                aResult.Add(typeof(Wall));
                aResult.Add(typeof(Ceiling));
                aResult.Add(typeof(RoofBase));
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if (type == typeof(oM.Physical.Elements.Wall))
            {
                aResult.Add(typeof(Wall));
                return aResult;
            }

            if (type == typeof(oM.Physical.Elements.Floor))
            {
                aResult.Add(typeof(Floor));
                return aResult;
            }

            if (type == typeof(oM.Physical.Elements.Roof))
            {
                aResult.Add(typeof(RoofBase));
                return aResult;
            }


            if (type == typeof(oM.Physical.Elements.Window))
            {
                aResult.Add(typeof(FamilyInstance));
                //aResult.Add(typeof(Wall));
                //DO NOT ADD: Autodesk.Revit.DB.Panel -> does not work with FilteredElementCollector
                //aResult.Add(typeof(Autodesk.Revit.DB.Panel));
                return aResult;
            }


            if (type == typeof(oM.Physical.Elements.Door))
            {
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if(typeof(oM.Physical.Elements.ISurface).IsAssignableFrom(type))
            {
                aResult.Add(typeof(Wall));
                aResult.Add(typeof(Floor));
                aResult.Add(typeof(RoofBase));
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if (type == typeof(oM.Structure.Elements.Panel))
            {
                aResult.Add(typeof(Floor));
                aResult.Add(typeof(Wall));
                return aResult;
            }

            if (type == typeof(oM.Structure.Elements.Bar))
            {
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if (type == typeof(BH.oM.Physical.Elements.Cable) || type == typeof(BH.oM.Physical.Elements.Pile))
                return aResult;

            if (typeof(BH.oM.Physical.Elements.IFramingElement).IsAssignableFrom(type))
            {
                aResult.Add(typeof(FamilyInstance));
                return aResult;
            }

            if (type == typeof(Building))
            {
                aResult.Add(typeof(ProjectInfo));
                return aResult;
            }

            if (type == typeof(oM.Geometry.SettingOut.Level))
            {
                aResult.Add(typeof(Level));
                return aResult;
            }

            if (type == typeof(Space))
            {
                aResult.Add(typeof(SpatialElement));
                return aResult;
            }

            if (type == typeof(oM.Architecture.Elements.Room))
            {
                aResult.Add(typeof(SpatialElement));
                return aResult;
            }

            if (type == typeof(oM.Geometry.SettingOut.Grid))
            {
                aResult.Add(typeof(Grid));
                return aResult;
            }

            if (type == typeof(Sheet))
            {
                aResult.Add(typeof(ViewSheet));
                return aResult;
            }

            if (type == typeof(oM.Adapters.Revit.Elements.ViewPlan))
            {
                aResult.Add(typeof(Autodesk.Revit.DB.ViewPlan));
                return aResult;
            }

            if (type == typeof(DraftingInstance))
            {
                aResult.Add(typeof(FamilyInstance));
                aResult.Add(typeof(CurveElement));
                return aResult;
            }

            if (type == typeof(ModelInstance))
            {
                aResult.Add(typeof(FamilyInstance));
                aResult.Add(typeof(CurveElement));
                return aResult;
            }

            if (type == typeof(oM.Adapters.Revit.Properties.InstanceProperties))
            {
                aResult.Add(typeof(ElementType));
                aResult.Add(typeof(GraphicsStyle));
                return aResult;
            }

            if (type == typeof(oM.Adapters.Revit.Elements.Viewport))
            {
                aResult.Add(typeof(Autodesk.Revit.DB.Viewport));
                return aResult;
            }

            if (type == typeof(oM.Adapters.Revit.Generic.RevitFilePreview))
            {
                aResult.Add(typeof(Autodesk.Revit.DB.Family));
                return aResult;
            }

            if (type == typeof(oM.Architecture.Elements.Ceiling))
            {
                aResult.Add(typeof(Ceiling));
                return aResult;
            }

            if (type == typeof(oM.Geometry.ShapeProfiles.ISectionProfile))
            {
                aResult.Add(typeof(FamilySymbol));
                return aResult;
            }

            return null;
        }

        /***************************************************/
    }
}
