﻿using DiBK.RuleValidator;
using DiBK.RuleValidator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class BueKanIkkeHaDobbeltpunkter : Rule<IGmlValidationData>
    {
        public override void Create()
        {
            Id = "gml.bue.1";
            Name = "Sirkelbuer kan ikke inneholde dobbeltpunkter";
        }

        protected override Status Validate(IGmlValidationData data)
        {
            if (!data.Surfaces.Any())
                return Status.NOT_EXECUTED;

            data.Surfaces.ForEach(Validate);

            return HasMessages ? Status.FAILED : Status.PASSED;
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
                        this.AddMessage("En bue kan kun inneholde tre punkter.", document.FileName, new[] { element.GetXPath() });
                        continue;
                    }
                }
                catch (Exception exception)
                {
                    this.AddMessage(exception.Message, document.FileName, new[] { element.GetXPath() });
                    continue;
                }

                var hasDoublePoint = coordinatePairs
                    .GroupBy(coordinatePair => new { x = coordinatePair[0], y = coordinatePair[1] })
                    .Where(coordinatePair => coordinatePair.Skip(1).Any())
                    .Any();

                if (hasDoublePoint)
                    this.AddMessage("En bue beregnes med tre punkter, og sirkelbuen kan ikke ha dobbeltpunkter.", document.FileName, new[] { element.GetXPath() });
            }
        }
    }
}
