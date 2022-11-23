using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullKanIkkeOverlappeAndreHullISammeFlate : Rule<IGmlValidationInputV1>
    {
        private readonly ConcurrentBag<XElement> _invalidElements = new();

        public override void Create()
        {
            Id = "gml.flate.5";
        }

        protected override void Validate(IGmlValidationInputV1 data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.ForEach(document => Validate(document, 2));
            data.Solids.ForEach(document => Validate(document, 3));
        }

        private void Validate(GmlDocument document, int dimensions)
        {
            SetData(DataKey.OverlappingHoles + document.Id, _invalidElements);

            var polygonElements = document.GetFeatureGeometryElements(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon)
                .SelectMany(element =>
                {
                    return element.DescendantsAndSelf()
                        .Where(element => element.Name.LocalName == GmlGeometry.Polygon || element.Name.LocalName == GmlGeometry.PolygonPatch);
                })
                .ToList();

            foreach (var element in polygonElements)
            {
                var interiorRingElements = element.GetElements("*:interior/*");

                if (interiorRingElements.Count() < 2)
                    continue;

                var interiors = new List<(XElement, Geometry)>();

                foreach (var interiorRingElement in interiorRingElements)
                {
                    try
                    {
                        if (dimensions == 3)
                            AddSrsDimensionAttribute(interiorRingElement);

                        using var interiorRing = GeometryHelper.GeometryFromGML(interiorRingElement);

                        if (interiorRing != null)
                        {
                            interiors.Add((interiorRingElement, GeometryHelper.CreatePolygonFromRing(interiorRing)));
                        }
                    }
                    catch
                    {
                    }
                }

                if (interiors.Count < 2)
                    continue;

                for (int i = 0; i < interiors.Count - 1; i++)
                {
                    var (geoElement, geometry) = interiors[i];

                    Parallel.For(i + 1, interiors.Count, index =>
                    {
                        var (otherGeoElement, otherGeometry) = interiors[index];

                        if (geometry.Overlaps(otherGeometry))
                        {
                            using var intersection = geometry.Intersection(otherGeometry);
                            intersection.ExportToWkt(out var intersectionWkt);

                            this.AddMessage(
                                Translate("Message", GmlHelper.GetNameAndId(element)),
                                document.FileName,
                                new[] { geoElement.GetXPath(), otherGeoElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) },
                                intersectionWkt
                            );

                            _invalidElements.Add(GmlHelper.GetBaseGmlElement(element));
                        }
                    });
                }
            }
        }

        private static void AddSrsDimensionAttribute(XElement geoElement)
        {
            if (geoElement.Attribute("srsDimension") == null)
                geoElement.Add(new XAttribute("srsDimension", 3));
        }
    }
}

