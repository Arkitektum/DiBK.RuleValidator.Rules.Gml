using DiBK.RuleValidator.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiBK.RuleValidator.Rules.Gml
{
    public class BueKanIkkeHaPunkterPåRettLinje : Rule<IGmlValidationData>
    {
        private const double MIN_SANGITTA = 0.02;

        public override void Create()
        {
            Id = "gml.bue.2";
            Name = "Punktene kan ikke ligge på rett linje for sirkelbue";
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
                var points = new List<double[]>();

                try
                {
                    points = GeometryHelper.GetCoordinates(element);
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

                var circle = GeometryHelper.PointsToCircle(points[0], points[1], points[2]);

                if (circle == null)
                    continue;

                using var firstPoint = GeometryHelper.CreatePoint(points[0][0], points[0][1]);
                using var lastPoint = GeometryHelper.CreatePoint(points[2][0], points[2][1]);

                var chordHalfLength = firstPoint.Distance(lastPoint) / 2;
                var sangitta = circle.Radius - Math.Sqrt(Math.Pow(circle.Radius, 2) - Math.Pow(chordHalfLength, 2));

                if (sangitta >= MIN_SANGITTA)
                    continue;

                this.AddMessage(
                    "Punktene som danner sirkelbuen ligger i rett linje og bør fremstilles som kurve.", 
                    document.FileName, 
                    new[] { element.GetXPath() },
                    new[] { GmlHelper.GetFeatureGmlId(element) }
                );
            }
        }
    }
}
