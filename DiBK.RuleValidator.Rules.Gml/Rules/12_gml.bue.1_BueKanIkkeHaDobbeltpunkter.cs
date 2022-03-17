﻿using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
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
            var elements = document.GetFeatureElements().GetElements("//gml:Arc");

            foreach (var element in elements)
            {
                var pointTuples = new List<(double[] PointA, double[] PointB)>();

                try
                {
                    var coordinatePairs = GeometryHelper.GetCoordinates(element);

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

                    for (var i = 1; i < coordinatePairs.Count; i++)
                        pointTuples.Add((coordinatePairs[i - 1], coordinatePairs[i]));
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

                var doublePoint = pointTuples
                    .FirstOrDefault(tuple => tuple.PointA[0] == tuple.PointB[0] && tuple.PointA[1] == tuple.PointB[1]);

                if (doublePoint != default)
                {
                    var x = doublePoint.PointA[0];
                    var y = doublePoint.PointA[1];
                    using var point = GeometryHelper.CreatePoint(x, y);

                    this.AddMessage(
                        Translate("Message2", x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture)), 
                        document.FileName, 
                        new[] { element.GetXPath() },
                        new[] { GmlHelper.GetFeatureGmlId(element) },
                        GeometryHelper.GetZoomToPoint(point)
                    );
                }
            }
        }
    }
}
