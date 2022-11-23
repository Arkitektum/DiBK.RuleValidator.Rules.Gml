using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Model
{
    public class GmlValidationInputV1 : IGmlValidationInputV1
    {
        private bool _disposed = false;
        public List<GmlDocument> Surfaces { get; } = new();
        public List<GmlDocument> Solids { get; } = new();

        private GmlValidationInputV1(List<GmlDocument> surfaces, List<GmlDocument> solids)
        {
            Surfaces.AddRange(surfaces);
            Solids.AddRange(solids);
        }

        public static IGmlValidationInputV1 Create(List<GmlDocument> surfaces, List<GmlDocument> solids)
        {
            return new GmlValidationInputV1(surfaces, solids);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Surfaces.ForEach(surface => surface.Dispose());
                    Solids.ForEach(solid => solid.Dispose());
                }

                _disposed = true;
            }
        }
    }
}
