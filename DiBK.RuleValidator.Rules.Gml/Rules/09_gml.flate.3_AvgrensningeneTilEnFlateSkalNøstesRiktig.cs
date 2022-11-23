using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningeneTilEnFlateSkalNøstesRiktig : Rule<IGmlValidationInputV1>
    {
        public override void Create()
        {
            Id = "gml.flate.3";
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
            var polygonElements = document.GetFeatureGeometryElements(GmlGeometry.MultiSurface, GmlGeometry.Surface, GmlGeometry.Polygon)
                .SelectMany(element =>
                {
                    return element.DescendantsAndSelf()
                        .Where(element => element.Name.LocalName == GmlGeometry.Polygon || element.Name.LocalName == GmlGeometry.PolygonPatch);
                })
                .ToList();

            foreach (var element in polygonElements)
            {
                var exteriorElement = element.GetElement("*:exterior");

                try
                {
                    var exteriorPoints = GeometryHelper.GetCoordinates(exteriorElement, dimensions);

                    if (GeometryHelper.PointsAreClockWise(exteriorPoints))
                    {
                        var gmlId = element.GetAttribute("gml:id");

                        this.AddMessage(
                            Translate("Message1", GmlHelper.GetNameAndId(element)),
                            document.FileName,
                            new[] { exteriorElement.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                    }
                }
                catch (Exception exception)
                {
                    this.AddMessage(
                        exception.Message, 
                        document.FileName, 
                        new[] { exteriorElement.GetXPath() }, 
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }

                var ringElements = element.GetElements("*:interior/*");

                foreach (var ringElement in ringElements)
                {
                    try
                    {
                        var ringPoints = GeometryHelper.GetCoordinates(ringElement, dimensions);

                        if (!GeometryHelper.PointsAreClockWise(ringPoints))
                        {
                            if (dimensions == 3)
                                AddSrsDimensionAttribute(ringElement);

                            using var ring = GeometryHelper.GeometryFromGML(ringElement);
                            using var polygon = GeometryHelper.CreatePolygonFromRing(ring);
                            using var point = polygon.PointOnSurface();

                            this.AddMessage(
                                Translate("Message2", GmlHelper.GetNameAndId(element)),
                                document.FileName,
                                new[] { ringElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) },
                                GeometryHelper.GetZoomToPoint(point)
                            );
                        }
                    }
                    catch (Exception exception)
                    {
                        this.AddMessage(
                            exception.Message, 
                            document.FileName, 
                            new[] { ringElement.GetXPath() }, 
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                    }
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
