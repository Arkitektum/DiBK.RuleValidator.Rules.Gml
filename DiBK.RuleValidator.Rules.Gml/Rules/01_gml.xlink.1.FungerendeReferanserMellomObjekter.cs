using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static DiBK.RuleValidator.Extensions.Gml.Constants.Namespace;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class FungerendeReferanserMellomObjekter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.xlink.1";
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

        private void Validate(GmlDocument document, Dictionary<string, HashSet<string>> allGmlIds)
        {
            if (document == null)
                return;

            var xlinks = document.GetFeatureElements()
                .Descendants()
                .Where(element => element.Attribute(XLinkNs + "href") != null)
                .ToList();

            foreach (var element in xlinks)
            {
                var xlink = element.GetAttribute("xlink:href").Split("#");

                if (xlink.Length != 2)
                {
                    this.AddMessage(
                        Translate("Message1", GmlHelper.GetFeatureType(element), element.GetName(), xlink[0]),
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

                this.AddMessage(
                    Translate("Message2", GmlHelper.GetNameAndId(GmlHelper.GetFeatureElement(element)), element.GetName(), gmlId),
                    document.FileName,
                    new[] { element.GetXPath() },
                    new[] { GmlHelper.GetFeatureGmlId(element) }
                );
            }
        }

        private static Dictionary<string, HashSet<string>> GetGmlIds(IEnumerable<GmlDocument> documents)
        {
            var allGmlIds = new Dictionary<string, HashSet<string>>();

            foreach (var document in documents)
            {
                if (document == null)
                    continue;

                var gmlIds = document.GetFeatureElements()
                    .Attributes(GmlNs + "id")
                    .Select(attribute => attribute.Value)
                    .ToHashSet();

                allGmlIds.Add(document.FileName, gmlIds);
            }

            return allGmlIds;
        }
    }
}
