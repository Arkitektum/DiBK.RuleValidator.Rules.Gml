using DiBK.RuleValidator.Extensions;
using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Model
{
    public class GmlValidationData : IGmlValidationData
    {
        private bool _disposed = false;
        public List<GmlDocument> Surfaces { get; } = new();
        public List<GmlDocument> Solids { get; } = new();
        public List<CodelistItem> Målemetoder { get; } = new();

        private GmlValidationData(List<GmlDocument> surfaces, List<GmlDocument> solids, List<CodelistItem> målemetoder)
        {
            Surfaces.AddRange(surfaces);
            Solids.AddRange(solids);
            Målemetoder.AddRange(målemetoder);
        }

        public static IGmlValidationData Create(List<GmlDocument> surfaces, List<GmlDocument> solids, List<CodelistItem> målemetoder)
        {
            return new GmlValidationData(surfaces, solids, målemetoder);
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
