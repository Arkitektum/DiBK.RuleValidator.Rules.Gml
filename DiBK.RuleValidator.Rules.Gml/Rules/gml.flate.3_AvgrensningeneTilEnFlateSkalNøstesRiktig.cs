using DiBK.RuleValidator.Models;
using DiBK.RuleValidator.Extensions;
using System;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class AvgrensningeneTilEnFlateSkalNøstesRiktig : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.3";
            Name = "Avgrensningene til en flate skal nøstes i riktig retning";
            Description = "Ytre flateavgrensning skal nøstes i retning mot klokken, og indre avgrensing i retning med klokken.";
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
            var polygonElements = document.GetFeatures().GetElements("//gml:Polygon | //gml:PolygonPatch");

            foreach (var element in polygonElements)
            {
                var exteriorElement = element.GetElement("*:exterior");

                try
                {
                    var exteriorPoints = GeometryHelper.GetCoordinates(exteriorElement);

                    if (GeometryHelper.PointsAreClockWise(exteriorPoints))
                    {
                        this.AddMessage(
                            $"{element.GetName()} '{element.GetAttribute("gml:id")}' er ugyldig: Ytre avgrensning går i retning med klokka, men skal gå i motsatt retning.",
                            document.FileName,
                            new[] { exteriorElement.GetXPath() }
                        );
                    }
                }
                catch (Exception exception)
                {
                    this.AddMessage(exception.Message, document.FileName, new[] { exteriorElement.GetXPath() });
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
                                $"{element.GetName()} '{element.GetAttribute("gml:id")}' er ugyldig: Indre avgrensning går i retning mot klokka, men skal gå med klokka.",
                                document.FileName,
                                new[] { interiorElement.GetXPath() }
                            );
                        }
                    }
                    catch (Exception exception)
                    {
                        this.AddMessage(exception.Message, document.FileName, new[] { interiorElement.GetXPath() });
                    }
                }
            }
        }
    }
}
