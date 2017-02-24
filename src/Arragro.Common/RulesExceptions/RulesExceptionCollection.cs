using System;
using System.Collections.Generic;

namespace Arragro.Common.RulesExceptions
{
    public class RulesExceptionCollection : Exception
    {
        public List<RulesException> RulesExceptions { get; protected set; }

        public RulesExceptionCollection() : base()
        {
            RulesExceptions = new List<RulesException>();
        }

        public RulesExceptionCollection(IEnumerable<RulesException> rulesExceptions) : this()
        {
            RulesExceptions.AddRange(rulesExceptions);
        }

        public RulesExceptionDto GetRulesExceptionDto()
        {
            return new RulesExceptionDto(RulesExceptions);
        }
    }
}
