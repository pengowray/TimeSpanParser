using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TimeSpanParserUtil
{
    class OneUnitToken : ParserToken
    {
        public decimal? uncolonedValue;

        public override bool UsesColonedDefault() {
            return false;
        }

        public override bool IsNull() {
            return uncolonedValue == null;
        }

        public override bool IsZero() {
            if (IsNull()) return false; // technically not zero

            return (uncolonedValue == 0); // already null checked
        }

        public override bool IsInitialNegative() {
            return uncolonedValue.HasValue && uncolonedValue < 0;
        }

        public override TimeSpan? ToTimeSpan() {
            if (IsNull())
                return null;

            return GetValue(uncolonedValue, GivenOrDefaultOrZeroUnits());
        }

    }
}
