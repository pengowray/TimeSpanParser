using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil
{
    class TimeSpanToken : ParserToken {
        public TimeSpan? timespan;
        public Units smallest = Units.None;
        public bool initialNegative = false;

        public override bool IsInitialNegative() {
            //return timespan < TimeSpan.Zero;
            return initialNegative;
        }

        public override bool IsNull() {
            return !timespan.HasValue;
        }

        public override bool IsZero() {
            return timespan == TimeSpan.Zero;
        }

        public override TimeSpan? ToTimeSpan() {
            return timespan;
        }

        public override bool UsesColonedDefault() {
            return false;
        }

        protected override Units SmallestUnit() {
            return smallest;
        }

        //delete me
        protected Units SmallestUnitv2() {
            if (!timespan.HasValue)
                return Units.None;

            TimeSpan t = (TimeSpan) timespan.Value;

            if (t.Ticks != 0 || t.Milliseconds != 0)
                return Units.Milliseconds;

            if (t.Seconds != 0)
                return Units.Seconds;

            if (t.Minutes != 0)
                return Units.Minutes;

            if (t.Hours != 0)
                return Units.Hours;

            if (t.Days != 0)
                return Units.Days;

            return Units.Years; // uh not quite right
        }
    }
}
