﻿using Autodesk.Revit.DB;
using BH.oM.Structural.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Revit
{
    public static partial class Convert
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        public static FamilyInstance ToRevit(this FramingElement framingElement, Document document, bool copyCustomData = true, bool convertUnits = true)
        {
            return framingElement.ToRevitColumn(document, copyCustomData, convertUnits);
        }

        public static FamilyInstance ToRevitColumn(this FramingElement framingElement, Document document, bool copyCustomData = true, bool convertUnits = true)
        {
            if (framingElement == null || document == null)
                return null;

            object aCustomDataValue = null;

            Curve aCurve = framingElement.LocationCurve.ToRevit();
            Level aLevel = null;

            aCustomDataValue = Query.ICustomData(framingElement, "Reference Level");
            if (aCustomDataValue != null && aCustomDataValue is int)
            {
                ElementId aElementId = new ElementId((int)aCustomDataValue);
                aLevel = document.GetElement(aElementId) as Level;
            }

            if (aLevel == null)
                aLevel = Query.BottomLevel(framingElement.LocationCurve, document);

            FamilySymbol aFamilySymbol = ToRevitFamilySymbol(framingElement.Property, document, copyCustomData, convertUnits);

            if (aFamilySymbol == null)
            {
                List<FamilySymbol> aFamilySymbolList = new FilteredElementCollector(document).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_StructuralFraming).Cast<FamilySymbol>().ToList();

                aCustomDataValue = Query.ICustomData(framingElement, "Type");
                if (aCustomDataValue != null && aCustomDataValue is int)
                {
                    ElementId aElementId = new ElementId((int)aCustomDataValue);
                    aFamilySymbol = aFamilySymbolList.Find(x => x.Id == aElementId);
                }

                if (aFamilySymbol == null)
                    aFamilySymbolList.Find(x => x.Name == framingElement.Name);

                if (aFamilySymbol == null)
                    aFamilySymbol = aFamilySymbolList.First();
            }

            FamilyInstance aFamilyInstance = document.Create.NewFamilyInstance(aCurve, aFamilySymbol, aLevel, Autodesk.Revit.DB.Structure.StructuralType.Beam);

            if (copyCustomData)
                Modify.SetParameters(aFamilyInstance, framingElement, new BuiltInParameter[] { BuiltInParameter.STRUCTURAL_BEAM_END0_ELEVATION, BuiltInParameter.STRUCTURAL_BEAM_END1_ELEVATION }, convertUnits);

            return aFamilyInstance;
        }

        /***************************************************/
    }
}