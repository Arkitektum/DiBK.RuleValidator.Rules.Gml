using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningeneTilEnFlateSkalNøstesRiktig : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.3";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var polygonElements = document.GetFeatureElements().GetElements("//gml:Polygon | //gml:PolygonPatch");

            foreach (var element in polygonElements)
            {
                var exteriorElement = element.GetElement("*:exterior");

                try
                {
                    var exteriorPoints = GeometryHelper.GetCoordinates(exteriorElement);

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
                        var ringPoints = GeometryHelper.GetCoordinates(ringElement);

                        if (!GeometryHelper.PointsAreClockWise(ringPoints))
                        {
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
    }
}
