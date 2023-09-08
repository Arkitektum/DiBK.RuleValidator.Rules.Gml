using DiBK.RuleValidator.Extensions.Gml;
using System;
using System.Collections.Generic;

namespace DiBK.RuleValidator.Rules.Gml.Tests.Model
{
    public class GmlValidationInputV1 : IGmlValidationInputV1
    {
        private bool _disposed = false;
        public List<GmlDocument> Documents { get; } = new();

        private GmlValidationInputV1(List<GmlDocument> documents)
        {
            Documents.AddRange(documents);
        }

        public static IGmlValidationInputV1 Create(List<GmlDocument> documents)
        {
            return new GmlValidationInputV1(documents);
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
                    Documents.ForEach(document => document.Dispose());

                _disposed = true;
            }
        }
    }
}
