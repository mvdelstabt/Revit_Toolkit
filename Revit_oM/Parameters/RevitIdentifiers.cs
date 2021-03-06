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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.oM.Adapters.Revit.Parameters
{
    [Description("An entity containing identifiers of Revit element correspondent to the BHoM object that carries it.")]
    public class RevitIdentifiers : IObject, IFragment, IImmutable
    {
        /***************************************************/
        /****             Public Properties             ****/
        /***************************************************/

        [Description("UniqueId of the Revit element correspondent to the BHoM object that carries this fragment.")]
        public virtual string UniqueId { get; } = "";

        [Description("ElementId of the Revit element correspondent to the BHoM object that carries this fragment.")]
        public virtual int ElementId { get; } = -1;

        [Description("Category of the Revit element correspondent to the BHoM object that carries this fragment.")]
        public virtual string CategoryName { get; } = "";

        [Description("Family of the Revit element correspondent to the BHoM object that carries this fragment.")]
        public virtual string FamilyName { get; } = "";

        [Description("Family type of the Revit element correspondent to the BHoM object that carries this fragment.")]
        public virtual string FamilyTypeName { get; } = "";

        [Description("ElementId of family type of the Revit element correspondent to the BHoM object that carries this fragment.")]
        public virtual int FamilyTypeId { get; } = -1;

        [Description("ElementId of view that owns the Revit element correspondent to the BHoM object that carries this fragment. -1 if the Revit element is not view-dependent.")]
        public virtual int OwnerViewId { get; } = -1;

        [Description("ElementId of the parent element of the Revit element correspondent to the BHoM object that carries this fragment. -1 if the Revit element is not a nested element.")]
        public virtual int ParentElementId { get; } = -1;


        /***************************************************/
        /****            Public Constructors            ****/
        /***************************************************/

        public RevitIdentifiers(string uniqueId = "", int elementId = -1, string categoryName = "", string familyName = "", string familyTypeName = "", int familyTypeId = -1, int ownerViewId = -1, int parentElementId = -1)
        {
            UniqueId = uniqueId;
            ElementId = elementId;
            CategoryName = categoryName;
            FamilyName = familyName;
            FamilyTypeName = familyTypeName;
            FamilyTypeId = familyTypeId;
            OwnerViewId = ownerViewId;
            ParentElementId = parentElementId;
        }

        /***************************************************/
    }
}


