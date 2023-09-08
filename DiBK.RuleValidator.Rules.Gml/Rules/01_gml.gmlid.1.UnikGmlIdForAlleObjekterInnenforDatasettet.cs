using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class UnikGmlIdForAlleObjekterInnenforDatasettet : Rule<IGmlValidationInputV1>
    {
        public override void Create()
        {
            Id = "gml.gmlid.1";
        }

        protected override void Validate(IGmlValidationInputV1 input)
        {
            if (!input.Documents.Any())
                SkipRule();

            input.Documents.ForEach(Validate);
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
                    var (LineNumber, LinePosition) = element.GetLineInfo();

                    this.AddMessage(
                        Translate("Message", grouping.Key, element.GetName()),
                        document.FileName,
                        new[] { element.GetXPath() },
                        Array.Empty<string>(),
                        LineNumber,
                        LinePosition
                    );
                }
            }
        }
    }
}


