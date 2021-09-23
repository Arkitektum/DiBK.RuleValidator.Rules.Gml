using DiBK.RuleValidator.Extensions;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningenTilEnFlateKanIkkeKrysseSegSelv : Rule<IGmlValidationData>
    {
        private readonly List<string> _gmlIds = new();

        public override void Create()
        {
            Id = "gml.flate.2";
            Name = "Avgrensningen til en flate kan ikke krysse seg selv";
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                return Status.NOT_EXECUTED;

            data.Surfaces.ForEach(Validate);

            return HasMessages ? Status.FAILED : Status.PASSED;
        }

        private void Validate(GmlDocument document)
        {
            var surfaceElements = document.GetFeatures().GetElements("*/gml:MultiSurface | */gml:Surface | */gml:Polygon | */gml:PolygonPatch");

            foreach (var element in surfaceElements)
            {
                using var geometry = document.GetOrCreateGeometry(element, out var errorMessage);

                if (geometry == null)
                    continue;

                using var polygon = geometry.GetGeometryType() == wkbGeometryType.wkbPolygon ? geometry : Ogr.ForceToPolygon(geometry);

                if (polygon.IsSimple())
                    continue;

                DetectSelfIntersections(document, element, polygon);
            }

            SetData($"SelfIntersections_{document.Id}", _gmlIds);
        }

        private void DetectSelfIntersections(GmlDocument document, XElement element, Geometry polygon)
        {
            var lineSegmentsList = GeometryHelper.GetLineSegmentsOfPolygon(polygon);

            foreach (var lineSegments in lineSegmentsList)
            {
                for (var i = 0; i < lineSegments.Count; i++)
                {
                    var lineSegment = lineSegments[i];
                    var otherLineSegments = lineSegments.Skip(i + 1).ToList();

                    for (int j = 0; j < otherLineSegments.Count; j++)
                    {
                        var otherLineSegment = otherLineSegments[j];

                        if (GeometryHelper.LineSegmentsAreConnected(lineSegment, otherLineSegment))
                            continue;

                        using var intersection = lineSegment.Intersection(otherLineSegment);

                        if (intersection.IsEmpty())
                            continue;

                        var point = intersection.GetPoint(0);
                        var pointWkt = $"POINT ({point[0]} {point[1]})";
                        var gmlId = element.GetAttribute("gml:id");

                        this.AddMessage(
                            $"{element.GetName()} '{gmlId}': Avgrensningen krysser seg selv.",
                            document.FileName,
                            new[] { element.GetXPath() },
                            new[] { gmlId },
                            pointWkt                            
                        );

                        _gmlIds.Add(gmlId);
                    }
                }
            }

            lineSegmentsList.ForEach(lineSegments => lineSegments.Dispose());
        }
    }
}
