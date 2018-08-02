﻿using System;
using System.Collections.Generic;
using System.Linq;

using Autodesk.Revit.DB;

using BH.oM.Base;
using BH.oM.Environment.Properties;
using BH.oM.Environment.Elements;
using BH.oM.Structural.Elements;

using BH.Engine.Revit;

namespace BH.UI.Revit.Adapter
{
    public partial class CobraAdapter
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        /// <summary>
        /// Delete BHoMObjects from Revit Document. BHoMObjects have to be linked to Revit elements by CustomData parameter called by Utilis.AdapterId const. Use Utilis.BHoM.CopyIdentifiers to include necessary information in BHoMObject
        /// </summary>
        /// <param name="bHoMObjects">BHoMObjects</param>
        /// <returns name="Succeeded">Delete succeeded</returns>
        /// <search>
        /// Delete, BHoMObject, BHoMObjects, Revit, Document
        /// </search>
        public bool Delete(IEnumerable<BHoMObject> bHoMObjects)
        {
            if (m_Document == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because Revit Document is null.");
                return false;
            }

            if (bHoMObjects == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM objects are null.");
                return false;
            }

            if (bHoMObjects.Count() < 1)
                return false;

            List<ElementId> aElementIdList = Query.ElementIds(m_Document, Query.UniqueIds(bHoMObjects, true), true);

            if (aElementIdList == null || aElementIdList.Count < 1)
                return false;

            bool aResult = false;
            using (Transaction aTransaction = new Transaction(m_Document, "Create"))
            {
                aTransaction.Start();
                aResult = Delete(aElementIdList);
                aTransaction.Commit();
            }
            return aResult;
        }

        /***************************************************/

        /// <summary>
        /// Delete BHoMObject from Revit Document. BHoMObject has to be linked to Revit element by CustomData parameter called by Utilis.AdapterId const. Use Utilis.BHoM.CopyIdentifiers to include necessary information in BHoMObject
        /// </summary>
        /// <param name="bHoMObject">BHoMObjects</param>
        /// <returns name="Succeeded">Delete succeeded</returns>
        /// <search>
        /// Delete, BHoMObject, Revit, Document
        /// </search>
        public bool Delete(BHoMObject bHoMObject)
        {
            if (m_Document == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because Revit Document is null.");
                return false;
            }

            if (bHoMObject == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM object is null.");
                return false;
            }

            bool aResult = false;
            using (Transaction aTransaction = new Transaction(m_Document, "Create"))
            {
                aTransaction.Start();
                aResult = DeleteByUniqueId(bHoMObject);
                aTransaction.Commit();
            }
            return aResult;
        }


        /***************************************************/

        /// <summary>
        /// Delete ElementType from Revit Document. If DeleteByName set to false then BuildingElementProperties has to be linked to Revit ElementType by CustomData parameter called by Utilis.AdapterId const. Use Utilis.BHoM.CopyIdentifiers to include UniqueId in BHoMObject.
        /// </summary>
        /// <param name="buildingElementProperties">BHoM BuildingElementProperties</param>
        /// <param name="deleteByName">Use Storey Name to match with Revit Level</param>
        /// <returns name="Succeeded">Delete succeeded</returns>
        /// <search>
        /// Delete, BHoMObject, BuildingElementProperties, Revit, Document
        /// </search>
        public bool Delete(BuildingElementProperties buildingElementProperties, bool deleteByName)
        {
            if (m_Document == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because Revit Document is null.");
                return false;
            }

            if (buildingElementProperties == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM buildingElementProperties are null.");
                return false;
            }

            bool aResult = false;
            using (Transaction aTransaction = new Transaction(m_Document, "Create"))
            {
                aTransaction.Start();
                if (deleteByName)
                    aResult = this.DeleteByName(typeof(Level), buildingElementProperties);
                else
                    aResult = DeleteByUniqueId(buildingElementProperties);
                aTransaction.Commit();
            }
            return aResult;
        }

        /***************************************************/

        /// <summary>
        /// Delete ElementTypes from Revit Document. If DeleteByName set to false then BuildingElementProperties have to be linked to Revit ElementTypes by CustomData parameter called by Utilis.AdapterId const. Use Utilis.BHoM.CopyIdentifiers to include UniqueId in BHoMObject.
        /// </summary>
        /// <param name="buildingElementProperties">BHoM BuildingElementProperties collection</param>
        /// <param name="deleteByName">Use Storey Name to match with Revit Level</param>
        /// <returns name="Succeeded">Delete succeeded</returns>
        /// <search>
        /// Delete, BHoMObject, BuildingElementProperties, Revit, Document
        /// </search>
        public bool Delete(IEnumerable<BuildingElementProperties> buildingElementProperties, bool deleteByName)
        {
            if (m_Document == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because Revit Document is null.");
                return false;
            }

            if (buildingElementProperties == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM buildingElementProperties are null.");
                return false;
            }

            if (buildingElementProperties.Count() < 1)
                return false;

            bool aResult = false;
            using (Transaction aTransaction = new Transaction(m_Document, "Create"))
            {
                aTransaction.Start();
                if (deleteByName)
                {
                    List<BuildingElementProperties> aBuildingElementPropertiesList = buildingElementProperties.ToList();
                    foreach(BuildingElementType aBuildingElementType in aBuildingElementPropertiesList.ConvertAll(x => x.BuildingElementType).Distinct())
                    {
                        Type aType = Query.RevitType(aBuildingElementType);
                        this.DeleteByName(aType, aBuildingElementPropertiesList.FindAll(x => x.BuildingElementType == aBuildingElementType));
                    }
                } 
                else
                {
                    return Delete(buildingElementProperties.Cast<BHoMObject>());
                }
                    
                aTransaction.Commit();
            }
            return aResult;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private bool DeleteByUniqueId(BHoMObject bHoMObject)
        {
            if(bHoMObject == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM object is null.");
                return false;
            }

            string aUniqueId = Query.UniqueId(bHoMObject);
            if (aUniqueId != null)
            {
                Element aElement = m_Document.GetElement(aUniqueId);
                return Delete(aElement);
            }

            return false;
        }

        /***************************************************/

        private bool DeleteByName(Type type, BHoMObject bHoMObject)
        {
            if (bHoMObject == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM object is null.");
                return false;
            }

            if (type == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because provided type is null.");
                return false;
            }

            List<Element> aElementList = new FilteredElementCollector(m_Document).OfClass(type).ToList();
            if (aElementList == null || aElementList.Count < 1)
                return false;

            Element aElement = aElementList.Find(x => x.Name == bHoMObject.Name);
            return Delete(aElement);
        }

        /***************************************************/

        private bool DeleteByName(Type type, IEnumerable<BHoMObject> bHoMObjects)
        {
            if (bHoMObjects == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because BHoM objects are null.");
                return false;
            }

            if (type == null)
            {
                Engine.Reflection.Compute.RecordError("Revit objects could not be deleted because provided type is null.");
                return false;
            }

            if (bHoMObjects.Count() < 1)
                return false;

            List<Element> aElementList = new FilteredElementCollector(m_Document).OfClass(type).ToList();
            if (aElementList == null || aElementList.Count < 1)
                return false;

            List<ElementId> aElementIdList = new List<ElementId>();
            foreach(BHoMObject aBHoMObject in bHoMObjects)
            {
                foreach(Element aElement in aElementList)
                {
                    if(aBHoMObject.Name == aElement.Name)
                    {
                        aElementIdList.Add(aElement.Id);
                        break;
                    }
                }
            }

            return Delete(aElementIdList);
        }

        /***************************************************/

        private bool Delete(Element element)
        {
            if(element == null)
            {
                Engine.Reflection.Compute.RecordError("Revit element could not be deleted because is null.");
                return false;
            }

            if (!Query.AllowElement(RevitSettings, element))
                return false;

            ICollection<ElementId> aElementIds = m_Document.Delete(element.Id);
            if (aElementIds != null && aElementIds.Count > 0)
                return true;

            return false;
        }

        /***************************************************/

        private bool Delete(ICollection<ElementId> elementIds)
        {
            if (elementIds == null)
            {
                Engine.Reflection.Compute.RecordError("Revit elements could not be deleted because element Ids are null.");
                return false;
            }

            if (elementIds.Count() < 1)
                return false;

            List<ElementId> aElementIdList = new List<ElementId>();

            foreach (ElementId aElementId in elementIds)
                if (Query.AllowElement(RevitSettings, m_Document, aElementId))
                    aElementIdList.Add(aElementId);

            ICollection<ElementId> aElementIds = m_Document.Delete(aElementIdList);
            if (aElementIds != null && aElementIds.Count > 0)
                return true;

            return false;
        }

        /***************************************************/
    }
}