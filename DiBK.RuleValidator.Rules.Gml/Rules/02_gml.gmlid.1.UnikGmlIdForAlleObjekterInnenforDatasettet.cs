using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    [Translation("gml.gmlid.1")]
    public class UnikGmlIdForAlleObjekterInnenforDatasettet : Rule<IGmlValidationData>
    {
        private static readonly XNamespace _gmlNs = "http://www.opengis.net/gml/3.2";

        public override void Create()
        {
            Id = "gml.gmlid.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            var duplicates = GetGmlIds(data)
                .GroupBy(tuple => new { tuple.FileName, tuple.Attribute.Value })
                .Where(grouping => grouping.Count() > 1);

            foreach (var grouping in duplicates)
            {
                foreach (var (FileName, Attribute) in grouping)
                {
                    var element = Attribute.Parent;

                    this.AddMessage(
                        Translate("Message", Attribute.Value, element.GetName()),
                        FileName,
                        new[] { element.GetXPath() }
                    );
                }
            }
        }

        private static List<(string FileName, XAttribute Attribute)> GetGmlIds(IGmlValidationData data)
        {
            var gmlIds = new List<(string FileName, XAttribute Attribute)>();

            void AddGmlIds(GmlDocument document)
            {
                if (document == null)
                    return;

                var attributes = document.Document.Descendants().Attributes(_gmlNs + "id")
                    .Select(attribute => (document.FileName, attribute))
                    .ToArray();

                gmlIds.AddRange(attributes);
            }

            var documents = data.Surfaces.Concat(data.Solids);

            foreach (var document in documents)
                AddGmlIds(document);

            return gmlIds;
        }
    }
}


