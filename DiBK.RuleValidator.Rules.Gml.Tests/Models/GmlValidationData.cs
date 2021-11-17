using DiBK.RuleValidator.Extensions;
using System;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Model
{
    public class GmlValidationData : IGmlValidationData
    {
        public List<GmlDocument> Surfaces { get; } = new();
        public List<GmlDocument> Solids { get; } = new();

        private GmlValidationData(List<GmlDocument> surfaces, List<GmlDocument> solids)
        {
            Surfaces.AddRange(surfaces);
            Solids.AddRange(solids);
        }

        public static IGmlValidationData Create(List<GmlDocument> surfaces, List<GmlDocument> solids)
        {
            return new GmlValidationData(surfaces, solids);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                return;

            Surfaces.ForEach(surface => surface.Dispose());
            Solids.ForEach(solid => solid.Dispose());
        }
    }
}
