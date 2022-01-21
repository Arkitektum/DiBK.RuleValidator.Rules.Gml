using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    [Translation("gml.flate.3")]
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
            var polygonElements = document.GetFeatures().GetElements("//gml:Polygon | //gml:PolygonPatch");

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

                var interiorElements = element.GetElements("*:interior");

                foreach (var interiorElement in interiorElements)
                {
                    try
                    {
                        var interiorPoints = GeometryHelper.GetCoordinates(interiorElement);

                        if (!GeometryHelper.PointsAreClockWise(interiorPoints))
                        {
                            this.AddMessage(
                                Translate("Message2", GmlHelper.GetNameAndId(element)),
                                document.FileName,
                                new[] { interiorElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) }
                            );
                        }
                    }
                    catch (Exception exception)
                    {
                        this.AddMessage(
                            exception.Message, 
                            document.FileName, 
                            new[] { interiorElement.GetXPath() }, 
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                    }
                }
            }
        }
    }
}
