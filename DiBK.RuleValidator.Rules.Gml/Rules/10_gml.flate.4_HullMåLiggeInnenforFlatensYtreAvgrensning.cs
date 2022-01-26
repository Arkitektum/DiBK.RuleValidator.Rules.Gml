using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
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
                var exteriorElement = element.GetElement("*:exterior/*");
                Geometry exterior = null;

                try
                {
                    using var exteriorRing = Geometry.CreateFromGML(exteriorElement.ToString());
                    exterior = GeometryHelper.CreatePolygonFromRing(exteriorRing);
                }
                catch
                {
                    continue;
                }

                var interiorElements = element.GetElements("*:interior/*");

                foreach (var interiorElement in interiorElements)
                {
                    try
                    {
                        using var interiorRing = Geometry.CreateFromGML(interiorElement.ToString());
                        using var interior = GeometryHelper.CreatePolygonFromRing(interiorRing);

                        if (!exterior.Contains(interior))
                        {
                            this.AddMessage(
                                Translate("Message", GmlHelper.GetNameAndId(element)),
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
