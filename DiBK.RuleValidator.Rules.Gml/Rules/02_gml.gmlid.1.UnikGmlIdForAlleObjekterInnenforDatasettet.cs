using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class UnikGmlIdForAlleObjekterInnenforDatasettet : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.gmlid.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
            data.Solids.ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            var duplicates = document.GetGmlElements()
                .Where(elements => elements.Count() > 1)
                .ToList();

            foreach (var grouping in duplicates)
            {
                foreach (var element in grouping)
                {
                    this.AddMessage(
                        Translate("Message", grouping.Key, element.GetName()),
                        document.FileName,
                        new[] { element.GetXPath() }
                    );
                }
            }
        }
    }
}


