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

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
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
                    var ringElement = exteriorElement.Elements().First();
                    using var ring = GeometryHelper.GeometryFromGML(ringElement);
                    var exteriorPoints = GeometryHelper.GetPoints(ring);
                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    if (GeometryHelper.PointsAreClockWise(exteriorPoints))
                    {
                        var gmlId = element.GetAttribute("gml:id");

                        this.AddMessage(
                            Translate("Message1", GmlHelper.GetNameAndId(element)),
                            document.FileName,
                            new[] { exteriorElement.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) },
                            LineNumber,
                            LinePosition
                        );
                    }
                }
                catch (Exception exception)
                {
                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    this.AddMessage(
                        exception.Message, 
                        document.FileName, 
                        new[] { exteriorElement.GetXPath() }, 
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        LineNumber,
                        LinePosition
                    );
                }

                var ringElements = element.GetElements("*:interior/*");

                foreach (var ringElement in ringElements)
                {
                    try
                    {
                        var dimension = GmlHelper.GetDimension(ringElement);

                        if (dimension == 3)
                            AddSrsDimensionAttribute(ringElement);

                        using var ring = GeometryHelper.GeometryFromGML(ringElement);
                        var ringPoints = GeometryHelper.GetPoints(ring);

                        if (!GeometryHelper.PointsAreClockWise(ringPoints))
                        {
                            using var polygon = GeometryHelper.CreatePolygonFromRing(ring);
                            using var point = polygon.PointOnSurface();
                            var (LineNumber, LinePosition) = ringElement.GetLineInfo();

                            this.AddMessage(
                                Translate("Message2", GmlHelper.GetNameAndId(element)),
                                document.FileName,
                                new[] { ringElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) },                                
                                LineNumber,
                                LinePosition,
                                GeometryHelper.GetZoomToPoint(point)
                            );
                        }
                    }
                    catch (Exception exception)
                    {
                        var (LineNumber, LinePosition) = ringElement.GetLineInfo();

                        this.AddMessage(
                            exception.Message, 
                            document.FileName, 
                            new[] { ringElement.GetXPath() }, 
                            new[] { GmlHelper.GetFeatureGmlId(element) },
                            LineNumber,
                            LinePosition
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
