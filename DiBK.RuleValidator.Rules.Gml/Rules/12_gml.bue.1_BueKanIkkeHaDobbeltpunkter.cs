using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    [Translation("gml.bue.1")]
    public class BueKanIkkeHaDobbeltpunkter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.bue.1";
        }

        protected override void Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                SkipRule();

            data.Surfaces.ForEach(Validate);
        }


        private void Validate(GmlDocument document)
        {
            var elements = document.GetFeatures().GetElements("//gml:Arc");

            foreach (var element in elements)
            {
                var coordinatePairs = new List<double[]>();

                try
                {
                    coordinatePairs = GeometryHelper.GetCoordinates(element);

                    if (coordinatePairs.Count != 3)
                    {
                        this.AddMessage(
                            Translate("Message1"),
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

                var hasDoublePoint = coordinatePairs
                    .GroupBy(coordinatePair => new { x = coordinatePair[0], y = coordinatePair[1] })
                    .Where(coordinatePair => coordinatePair.Skip(1).Any())
                    .Any();

                if (hasDoublePoint)
                {
                    this.AddMessage(
                        Translate("Message2"), 
                        document.FileName, 
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) }
                    );
                }
            }
        }
    }
}
