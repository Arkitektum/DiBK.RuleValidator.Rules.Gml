using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class MålemetodeMåVæreIHenholdTilKodeliste : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.kod.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() || !data.Målemetoder.Any())
                SkipRule();

            data.Surfaces.ForEach(document => Validate(document, data.Målemetoder));
        }

        private void Validate(GmlDocument document, List<CodelistItem> målemetoder)
        {
            var featureElements = document.GetFeatureElements();

            foreach (var featureElement in featureElements)
            {
                var målemetodeElement = featureElement.GetElement("*:kvalitet/*:Posisjonskvalitet/*:målemetode");

                if (målemetodeElement == null)
                    continue;

                var målemetode = målemetodeElement.Value;

                if (!målemetoder.Any(metode => metode.Value == målemetode))
                {
                    this.AddMessage(
                        Translate("Message", GmlHelper.GetNameAndId(featureElement), målemetode),
                        document.FileName,
                        new[] { målemetodeElement.GetXPath() },
                        new[] { featureElement.GetAttribute("gml:id") }
                    );
                }
            }
        }
    }
}
