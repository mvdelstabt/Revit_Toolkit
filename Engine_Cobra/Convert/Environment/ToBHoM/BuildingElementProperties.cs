﻿using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using BH.Engine.Environment;
using BH.oM.Base;
using BH.oM.Environment.Elements;
using BH.oM.Environment.Properties;
using BH.oM.Adapters.Revit.Settings;

namespace BH.UI.Cobra.Engine
{
    public static partial class Convert
    {
        /***************************************************/
        /****             Internal methods              ****/
        /***************************************************/

        internal static BuildingElementProperties ToBHoMBuildingElementProperties(this ElementType elementType, PullSettings pullSettings = null)
        {
            //TODO: dynamic does not work. ToBHoM for WallType not recognized
            //aBuildingElementProperties = (elementType as dynamic).ToBHoM(discipline, copyCustomData, convertUnits) as BuildingElementProperties;

            BuildingElementProperties aBuildingElementProperties = null;

            if (elementType is WallType)
                aBuildingElementProperties = (elementType as WallType).ToBHoMBuildingElementProperties(pullSettings);
            else if (elementType is FloorType)
                aBuildingElementProperties = (elementType as FloorType).ToBHoMBuildingElementProperties(pullSettings);
            else if (elementType is CeilingType)
                aBuildingElementProperties = (elementType as CeilingType).ToBHoMBuildingElementProperties(pullSettings);
            else if (elementType is RoofType)
                aBuildingElementProperties = (elementType as RoofType).ToBHoMBuildingElementProperties(pullSettings);
            else if (elementType is FamilySymbol)
                aBuildingElementProperties = (elementType as FamilySymbol).ToBHoMBuildingElementProperties(pullSettings);

            if (aBuildingElementProperties == null)
                aBuildingElementProperties = new BuildingElementProperties();

            return aBuildingElementProperties;
        }

        /***************************************************/

        internal static BuildingElementProperties ToBHoMBuildingElementProperties(this WallType wallType, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            BuildingElementProperties aBuildingElementProperties = pullSettings.FindRefObject(wallType.Id.IntegerValue) as BuildingElementProperties;
            if (aBuildingElementProperties != null)
                return aBuildingElementProperties;

            aBuildingElementProperties = Create.BuildingElementProperties(wallType.Name, BuildingElementType.Wall);

            aBuildingElementProperties = Modify.SetIdentifiers(aBuildingElementProperties, wallType) as BuildingElementProperties;
            if (pullSettings.CopyCustomData)
            {
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, wallType, pullSettings.ConvertUnits) as BuildingElementProperties;
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, wallType, BuiltInParameter.ALL_MODEL_FAMILY_NAME, pullSettings.ConvertUnits) as BuildingElementProperties;
            }

            pullSettings.RefObjects = pullSettings.RefObjects.AppendRefObjects(aBuildingElementProperties);

            return aBuildingElementProperties;
        }

        /***************************************************/

        internal static BuildingElementProperties ToBHoMBuildingElementProperties(this FloorType floorType, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            BuildingElementProperties aBuildingElementProperties = pullSettings.FindRefObject(floorType.Id.IntegerValue) as BuildingElementProperties;
            if (aBuildingElementProperties != null)
                return aBuildingElementProperties;

            aBuildingElementProperties = Create.BuildingElementProperties(floorType.Name, BuildingElementType.Floor);

            aBuildingElementProperties = Modify.SetIdentifiers(aBuildingElementProperties, floorType) as BuildingElementProperties;
            if (pullSettings.CopyCustomData)
            {
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, floorType, pullSettings.ConvertUnits) as BuildingElementProperties;
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, floorType, BuiltInParameter.ALL_MODEL_FAMILY_NAME, pullSettings.ConvertUnits) as BuildingElementProperties;
            }

            pullSettings.RefObjects = pullSettings.RefObjects.AppendRefObjects(aBuildingElementProperties);

            return aBuildingElementProperties;
        }

        /***************************************************/

        internal static BuildingElementProperties ToBHoMBuildingElementProperties(this CeilingType ceilingType, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            BuildingElementProperties aBuildingElementProperties = pullSettings.FindRefObject(ceilingType.Id.IntegerValue) as BuildingElementProperties;
            if (aBuildingElementProperties != null)
                return aBuildingElementProperties;

            aBuildingElementProperties = Create.BuildingElementProperties(ceilingType.Name, BuildingElementType.Ceiling);

            aBuildingElementProperties = Modify.SetIdentifiers(aBuildingElementProperties, ceilingType) as BuildingElementProperties;
            if (pullSettings.CopyCustomData)
            {
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, ceilingType, pullSettings.ConvertUnits) as BuildingElementProperties;
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, ceilingType, BuiltInParameter.ALL_MODEL_FAMILY_NAME, pullSettings.ConvertUnits) as BuildingElementProperties;
            }

            pullSettings.RefObjects = pullSettings.RefObjects.AppendRefObjects(aBuildingElementProperties);

            return aBuildingElementProperties;
        }

        /***************************************************/

        internal static BuildingElementProperties ToBHoMBuildingElementProperties(this RoofType roofType, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            BuildingElementProperties aBuildingElementProperties = pullSettings.FindRefObject(roofType.Id.IntegerValue) as BuildingElementProperties;
            if (aBuildingElementProperties != null)
                return aBuildingElementProperties;

            aBuildingElementProperties = Create.BuildingElementProperties(roofType.Name, BuildingElementType.Roof);

            aBuildingElementProperties = Modify.SetIdentifiers(aBuildingElementProperties, roofType) as BuildingElementProperties;
            if (pullSettings.CopyCustomData)
            {
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, roofType, pullSettings.ConvertUnits) as BuildingElementProperties;
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, roofType, BuiltInParameter.ALL_MODEL_FAMILY_NAME, pullSettings.ConvertUnits) as BuildingElementProperties;
            }

            pullSettings.RefObjects = pullSettings.RefObjects.AppendRefObjects(aBuildingElementProperties);

            return aBuildingElementProperties;
        }

        /***************************************************/

        internal static BuildingElementProperties ToBHoMBuildingElementProperties(this FamilySymbol familySymbol, PullSettings pullSettings = null)
        {
            pullSettings = pullSettings.DefaultIfNull();

            BuildingElementProperties aBuildingElementProperties = pullSettings.FindRefObject(familySymbol.Id.IntegerValue) as BuildingElementProperties;
            if (aBuildingElementProperties != null)
                return aBuildingElementProperties;

            BuildingElementType? aBuildingElementType = Query.BuildingElementType(familySymbol.Category);
            if (!aBuildingElementType.HasValue)
                aBuildingElementType = BuildingElementType.Undefined;

            aBuildingElementProperties = Create.BuildingElementProperties(familySymbol.Name, aBuildingElementType.Value);
            aBuildingElementProperties = Modify.SetIdentifiers(aBuildingElementProperties, familySymbol) as BuildingElementProperties;
            if (pullSettings.CopyCustomData)
            {
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, familySymbol, pullSettings.ConvertUnits) as BuildingElementProperties;
                aBuildingElementProperties = Modify.SetCustomData(aBuildingElementProperties, familySymbol, BuiltInParameter.ALL_MODEL_FAMILY_NAME, pullSettings.ConvertUnits) as BuildingElementProperties;
            }

            pullSettings.RefObjects = pullSettings.RefObjects.AppendRefObjects(aBuildingElementProperties);

            return aBuildingElementProperties;
        }

        /***************************************************/
    }
}