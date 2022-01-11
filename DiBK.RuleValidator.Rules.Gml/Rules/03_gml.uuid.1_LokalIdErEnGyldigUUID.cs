using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class LokalIdErEnGyldigUUID : Rule<IGmlValidationData>
    {
        private static readonly Regex _uuidRegex = 
            new("^[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public override void Create()
        {
            Id = "gml.uuid.1";
            Name = "LokalId er en gyldig UUID";
            Description = "Egenskapen 'LokalId' må være en gyldig UUID (ref 2.1)";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.Concat(data.Solids)
                .ToList()
                .ForEach(Validate);
        }

        private void Validate(GmlDocument document)
        {
            if (document == null)
                return;

            var uuids = new List<string>(25000);
            var elements = document.GetFeatures().GetElements("*:identifikasjon//*:lokalId").ToList();

            foreach (var element in elements)
            {
                var lokalId = element.Value;

                if (!_uuidRegex.IsMatch(lokalId))
                {
                    this.AddMessage(
                        $"{GmlHelper.GetNameAndId(GmlHelper.GetFeatureElement(element))}: Egenskapen 'LokalId' må være en gyldig UUID.",
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
                else if (uuids.Any(id => id == lokalId))
                {
                    this.AddMessage(
                        $"{GmlHelper.GetNameAndId(GmlHelper.GetFeatureElement(element))}: Det kan ikke finnes flere like 'LokalId'. 'LokalId' må være unik.",
                        document.FileName,
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
                else
                {
                    uuids.Add(lokalId);
                }
            }
        }
    }
}
