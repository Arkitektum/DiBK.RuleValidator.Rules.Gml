using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Linq;
using System.Xml.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class SirkelbuerKanKunInneholdeTrePunkter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.bue.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any() && !data.Solids.Any())
                SkipRule();

            data.Surfaces.ForEach(document => Validate(document, 2));
            data.Solids.ForEach(document => Validate(document, 3));
        }

        private void Validate(GmlDocument document, int dimensions)
        {
            var elements = document.GetFeatureElements().Descendants(GmlHelper.GmlNs + "Arc");
            
            foreach (var element in elements)
            {
                try
                {
                    var coordinatePairs = GeometryHelper.GetCoordinates(element, dimensions);

                    if (coordinatePairs.Count != 3)
                    {
                        this.AddMessage(
                            Translate("Message"),
                            document.FileName, 
                            new[] { element.GetXPath() },
                            new[] { GmlHelper.GetFeatureGmlId(element) }
                        );
                        
                        continue;
                    }
                }
                catch (Exception exception)
                {
                    this.AddMessage(
                        exception.Message, 
                        document.FileName, 
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                    
                    continue;
                }
            }
        }
    }
}
