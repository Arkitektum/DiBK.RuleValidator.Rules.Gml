using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class FungerendeReferanserMellomObjekter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.xlink.1";
            Name = "Fungerende referanser mellom objekter";
            Documentation = "https://dibk.atlassian.net/wiki/spaces/FP/pages/1828814869/rpf.xlink.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            var documents = data.Surfaces.Concat(data.Solids);
            var allGmlIds = GetGmlIds(documents);

            foreach (var document in documents)
                Validate(document, allGmlIds);
        }

        private void Validate(GmlDocument document, Dictionary<string, IEnumerable<string>> allGmlIds)
        {
            if (document == null)
                return;

            var xlinks = document.GetFeatures().GetElements("//*[@xlink:href]");

            foreach (var element in xlinks)
            {
                var xlink = element.GetAttribute("xlink:href").Split("#");

                if (xlink.Length != 2)
                {
                    this.AddMessage(
                        $"Referansen fra {GmlHelper.GetFeatureType(element)} til {element.GetName()} '{xlink[0]}' har ugyldig format.",
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );

                    continue;
                }

                var fileName = string.IsNullOrWhiteSpace(xlink[0]) ? document.FileName : xlink[0];
                var gmlId = xlink[1];

                if (allGmlIds.TryGetValue(fileName, out var gmlIds) && gmlIds.Contains(gmlId))
                    continue;

                var feature = GmlHelper.GetFeatureElement(element);

                this.AddMessage(
                    $"Referansen fra {feature.GetName()} '{feature.GetAttribute("gml:id")}' til {element.GetName()} '{gmlId}' fungerer ikke.", 
                    document.FileName,
                    new[] { element.GetXPath() },
                    new[] { GmlHelper.GetFeatureGmlId(element) }
                );
            }
        }

        private static Dictionary<string, IEnumerable<string>> GetGmlIds(IEnumerable<GmlDocument> documents)
        {
            return documents.Where(document => document != null)
                .Select(document => new KeyValuePair<string, IEnumerable<string>>(document.FileName, document.GetFeatures().GetAttributes("gml:id")))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
