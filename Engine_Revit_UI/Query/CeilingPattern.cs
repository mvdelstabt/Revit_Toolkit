﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using BH.Engine.Geometry;
using BH.oM.Geometry;

namespace BH.UI.Revit.Engine
{
    public static partial class Query
    {
        public static List<BH.oM.Geometry.Line> CeilingPattern(this Ceiling ceiling, PlanarSurface surface)
        {
            CeilingType ceilingType = ceiling.Document.GetElement(ceiling.GetTypeId()) as CeilingType;

            CompoundStructure comStruct = ceilingType.GetCompoundStructure();

            double lowestX = surface.ExternalBoundary.IControlPoints().Min(x => x.X);
            double lowestY = surface.ExternalBoundary.IControlPoints().Min(x => x.Y);
            double highestX = surface.ExternalBoundary.IControlPoints().Max(x => x.X);
            double highestY = surface.ExternalBoundary.IControlPoints().Max(x => x.Y);
            double z = surface.ExternalBoundary.IControlPoints().Max(x => x.Z);

            double yLength = highestY - lowestY;

            BH.oM.Geometry.Line leftLine = new oM.Geometry.Line
            {
                Start = new oM.Geometry.Point { X = lowestX, Y = lowestY, Z = z },
                End = new oM.Geometry.Point { X = lowestX, Y = highestY, Z = z },
            };

            BH.oM.Geometry.Line rightLine = new oM.Geometry.Line
            {
                Start = new oM.Geometry.Point { X = highestX, Y = lowestY, Z = z },
                End = new oM.Geometry.Point { X = highestX, Y = highestY, Z = z },
            };

            List<BH.oM.Geometry.Line> boundarySegments = surface.ExternalBoundary.ISplitAtPoints(surface.ExternalBoundary.IControlPoints()).SelectMany(x => (x as PolyCurve).Curves.Select(y => y as BH.oM.Geometry.Line)).ToList();

            List<BH.oM.Geometry.Line> patterns = new List<BH.oM.Geometry.Line>();

            List<ElementId> materialIds = ceiling.GetMaterialIds(false).ToList();

            foreach(ElementId e in materialIds)
            {
                Material revitMaterial = ceiling.Document.GetElement(e) as Material;
                FillPatternElement fillPatternElement = null;

#if REVIT2020
                fillPatternElement = revitMaterial.Document.GetElement(revitMaterial.SurfaceForegroundPatternId) as FillPatternElement;
#else
                fillPatternElement = revitMaterial.Document.GetElement(revitMaterial.SurfacePatternId) as FillPatternElement;
#endif

                if (fillPatternElement != null)
                {
                    FillPattern fillPattern = fillPatternElement.GetFillPattern();
                    if (fillPattern == null || fillPattern.IsSolidFill)
                        continue; //Skip solid filled patterns

                    IList<FillGrid> fillGridList = fillPattern.GetFillGrids();
                    foreach(FillGrid grid in fillGridList)
                    {
                        double offset = grid.Offset.ToSI(UnitType.UT_Length);

                        double currentY = lowestY - (yLength / 2);

                        while((currentY + offset) < (highestY + (yLength + 2)))
                        {
                            BH.oM.Geometry.Point pt = new oM.Geometry.Point { X = lowestX, Y = currentY + offset, Z = z };
                            BH.oM.Geometry.Point pt2 = new oM.Geometry.Point { X = highestX, Y = currentY + offset, Z = z };

                            BH.oM.Geometry.Line pline = new oM.Geometry.Line { Start = pt, End = pt2 };

                            if (grid.Angle > 0)
                            {
                                BH.oM.Geometry.Point rotatePt = new BH.oM.Geometry.Line { Start = pt, End = pt2 }.Centroid();
                                pline = pline.Rotate(rotatePt, Vector.ZAxis, grid.Angle.ToSI(UnitType.UT_Angle));

                                BH.oM.Geometry.Point intersect = pline.LineIntersections(leftLine, true).FirstOrDefault();
                                if(intersect != null)
                                    pline = pline.Extend(pline.Start.Distance(intersect));

                                intersect = pline.LineIntersections(rightLine, true).FirstOrDefault();
                                if (intersect != null)
                                    pline = pline.Extend(0, pline.End.Distance(intersect));
                            }

                            List<BH.oM.Geometry.Point> intersections = new List<oM.Geometry.Point>();
                            foreach(BH.oM.Geometry.Line l in boundarySegments)
                            {
                                BH.oM.Geometry.Point p = pline.LineIntersection(l);
                                if (p != null)
                                    intersections.Add(p);
                            }

                            if(intersections.Count > 0)
                            {
                                List<BH.oM.Geometry.Line> lines = pline.SplitAtPoints(intersections);
                                foreach(BH.oM.Geometry.Line l in lines)
                                {
                                    if (surface.ExternalBoundary.IIsContaining(l.IControlPoints(), true))
                                        patterns.Add(l);
                                }
                            }

                            currentY += offset;
                        }
                    }
                }
            }

            return patterns;
        }
    }
}
