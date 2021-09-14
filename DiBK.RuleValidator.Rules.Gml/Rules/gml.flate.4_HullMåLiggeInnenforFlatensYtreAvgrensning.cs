using DiBK.RuleValidator;
using DiBK.RuleValidator.Extensions;
using OSGeo.OGR;
using System;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullMåLiggeInnenforFlatensYtreAvgrensning : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.flate.4";
            Name = "Hull i flate må ligge innenfor flatens ytre avgrensning";
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
                Geometry exterior;

                try
                {
                    exterior = CreateLineString(exteriorElement);
                }
                catch (Exception exception)
                {
                    this.AddMessage(exception.Message, document.FileName, new[] { exteriorElement.GetXPath() });
                    continue;
                }

                var interiorElements = element.GetElements("*:interior");

                foreach (var interiorElement in interiorElements)
                {
                    try
                    {
                        using var interior = CreateLineString(interiorElement);

                        if (interior.Intersects(exterior))
                        {
                            this.AddMessage(
                                $"{element.GetName()} '{element.GetAttribute("gml:id")}': Et hull i flaten ligger utenfor flatens ytre avgrensning.",
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

                exterior.Dispose();
            }
        }

        private static Geometry CreateLineString(XElement element)
        {
            var points = GeometryHelper.GetCoordinates(element);
            var lineString = new Geometry(wkbGeometryType.wkbLineString);

            foreach (var point in points)
                lineString.AddPoint(point[0], point[1], 0);

            return lineString;
        }
    }
}
