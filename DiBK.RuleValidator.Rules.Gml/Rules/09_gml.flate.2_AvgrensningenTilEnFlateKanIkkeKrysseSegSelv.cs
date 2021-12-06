using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningenTilEnFlateKanIkkeKrysseSegSelv : Rule<IGmlValidationData>
    {
        private readonly HashSet<string> _xPaths = new();

        public override void Create()
        {
            Id = "gml.flate.2";
            Name = "Avgrensningen til en flate kan ikke krysse seg selv";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var surfaceElements = document.GetFeatures().GetElements("*/gml:MultiSurface | */gml:Surface | */gml:Polygon");

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

            SetData(string.Format(DataKey.Selvkryss, document.Id), _xPaths);
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
                        var gmlId = GmlHelper.GetClosestGmlId(element);
                        var xPath = element.GetXPath();

                        this.AddMessage(
                            $"{GmlHelper.GetNameAndId(element)}: Avgrensningen krysser seg selv.",
                            document.FileName,
                            new[] { xPath },
                            new[] { GmlHelper.GetFeatureGmlId(element) },
                            pointWkt                            
                        );

                        _xPaths.Add(xPath);
                    }
                }
            }

            lineSegmentsList.ForEach(lineSegments => lineSegments.Dispose());
        }
    }
}
