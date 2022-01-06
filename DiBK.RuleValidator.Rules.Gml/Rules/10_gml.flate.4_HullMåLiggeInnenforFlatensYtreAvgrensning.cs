using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Rules.Gml.Constants;
using OSGeo.OGR;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class HullMåLiggeInnenforFlatensYtreAvgrensning : Rule<IGmlValidationData>
    {
        private readonly HashSet<string> _xPaths = new();

        public override void Create()
        {
            Id = "gml.flate.4";
            Name = "Hull i flate må ligge innenfor flatens ytre avgrensning";
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
                Geometry exterior;

                try
                {
                    exterior = GeometryHelper.CreatePolygon(exteriorElement);
                }
                catch
                {
                    continue;
                }

                var interiorElements = element.GetElements("*:interior");

                foreach (var interiorElement in interiorElements)
                {
                    try
                    {
                        using var interior = GeometryHelper.CreatePolygon(interiorElement);
                        
                        if (!exterior.Contains(interior))
                        {
                            this.AddMessage(
                                $"{GmlHelper.GetNameAndId(element)}: Et hull i flaten ligger utenfor flatens ytre avgrensning.",
                                document.FileName,
                                new[] { interiorElement.GetXPath() },
                                new[] { GmlHelper.GetFeatureGmlId(element) }
                            );

                            _xPaths.Add(GmlHelper.GetBaseGmlElement(element).GetXPath());
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                exterior.Dispose();
            }

            SetData(string.Format(DataKey.HullUtenforYtreAvgrensning, document.Id), _xPaths);
        }
    }
}
