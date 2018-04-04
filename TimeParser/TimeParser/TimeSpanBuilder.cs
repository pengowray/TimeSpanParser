using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil {
    class TimeSpanBuilder {
        private Units PrevUnit = Units.None;
        private HashSet<Units> DoneUnits = new HashSet<Units>(); //TODO: only create if needed

        //private bool ColonNumberBuilder = false;
        public int RemainingTimeSpans = int.MaxValue;

        TimeSpanParserOptions Options;
        public TimeSpanBuilder(TimeSpanParserOptions options = null) {
            if (options == null)
                options = new TimeSpanParserOptions();

            this.Options = options;
        }

        public TimeSpan TimeSpan { get => timeSpan; }

        // the builder has recieved no inputs. Might be considered null rather than 00:00:00
        //public bool IsNull { get => PrevUnit == Units.None; }
        public bool IsNull { get => isNull; }
        private bool isNull = true;

        private TimeSpan timeSpan = TimeSpan.Zero;

        // did the first item have a negative value. If so, do the same for subsequent time units
        private bool initialNegative = false;
        private bool currentlyParsingAColonBlock = false;
        private bool currentlyParsingZeroOnly = false;
        private bool finishedParsingZeroOnly = false;
        public TimeSpan? CompleteTimeSpan;

        public void StartParsingColonishNumber() {
            currentlyParsingAColonBlock = true;
        }

        public void EndParsingColonishNumber() {
            currentlyParsingAColonBlock = false;
            if (currentlyParsingZeroOnly) {
                currentlyParsingZeroOnly = false;
                finishedParsingZeroOnly = true;
            }
        }

        public bool NoMore() {
            return RemainingTimeSpans <= 0;
        }

        protected TimeSpanBuilder NewBuilder() {
            if (NoMore()) {
                // todo: how to send back a timespan now?
            }

            var newBuilder = new TimeSpanBuilder(Options);
            newBuilder.RemainingTimeSpans = RemainingTimeSpans - 1;
            if (currentlyParsingAColonBlock) newBuilder.StartParsingColonishNumber();
            return newBuilder;
        }


        /// <summary>
        /// e.g. for "10:30", if PrevUnit was minutes (10), then the "NextColonUnit" (30) will be seconds
        /// </summary>
        /// <returns></returns>
        public Units NextColonUnit() {
            if (PrevUnit == Units.None)
                return Units.None; // any

            if (PrevUnit >= Units.Days && PrevUnit <= Units.Minutes)
                return PrevUnit + 1;

            if (PrevUnit == Units.Seconds)
                return Units.ErrorTooManyUnits;

            if (PrevUnit == Units.ZeroOnly && currentlyParsingZeroOnly)
                return Units.ZeroOnly;

            return Units.Error;
        }

        public TimeSpan? Final() {
            if (IsNull)
                return null;

            return TimeSpan;
        }

        public Units GetUnitOrDefaultOption(Units originalUnit) {
            var unit = originalUnit;
            if (unit == Units.None) {
                if (IsNull) {
                    // resort to default
                    unit = currentlyParsingAColonBlock ? Options.ColonedDefault : Options.UncolonedDefault;

                } else if (currentlyParsingAColonBlock) {
                    unit = NextColonUnit();
                }
            }

            //Note: doesn't ever return Units.ZeroOnly

            return unit;
        }

        // Returns false if fails StrictBigToSmall or DisallowRepeatUnit. Can be treated as failure, or as a signal to the parser that a new TimeSpan has begun
        // Can throw: ArgumentException, OverflowException
        // noDeeper = don't go deeper (used internally only to avoid infinite recursion)
        // retrieve the just-completed TimeSpan in CompleteTimeSpan
        public TimeSpanBuilder AddUnit(decimal time, Units originalUnit) {
            return AddUnit(time, originalUnit, false);
        }

        private TimeSpanBuilder AddUnit(decimal time, Units originalUnit, bool noDeeper = false) {

            // work out what unit we're using. either originalUnit, a default, or a continuation from a colon-number

            var unit = GetUnitOrDefaultOption(originalUnit);

            // if no default unit, allow a zero anyway (must be at the start, unless 
            if ((unit == Units.None || (unit == Units.ErrorTooManyUnits && timeSpan == TimeSpan.Zero)) && Options.AllowUnitlessZero && time == 0) {
                // will require more to pass later (see: notTheFirstZeroOnly)
                if (unit == Units.ErrorTooManyUnits) Console.WriteLine($"parsing a zero-only:{currentlyParsingZeroOnly}");

                unit = Units.ZeroOnly;
                if (currentlyParsingAColonBlock) {
                    currentlyParsingZeroOnly = true;
                    //return this;

                } else {

                    finishedParsingZeroOnly = true;

                    //meh, just follow the rules of repeated units and stuff for now...
                }

            }  else if (unit == Units.ZeroOnly && time != 0) {
                throw new ArgumentException("failed zero only.");
            }


            // Done working out what unit is now.

            Units prevUnit = PrevUnit;
            bool repeatedUnit = DoneUnits.Contains(unit);
            PrevUnit = unit;
            DoneUnits.Add(unit);
            if (!noDeeper) CompleteTimeSpan = null; 
            //currentlyParsingZeroOnly = false;
            bool nowNull = isNull;
            isNull = false;

            //-==[ DON'T USE:  PrevUnit, DoneUnits, or isNull after this point ]==-

            Console.WriteLine($"actual unit: {unit}");

            if (unit == Units.Error || unit == Units.ErrorTooManyUnits || unit == Units.ErrorAmbiguous)
                throw new ArgumentException("Bad unit exception or no default: " + unit);

            if (nowNull && time < 0) {
                initialNegative = true;
            } else if (initialNegative) {
                time *= -1;
            }

            bool noUnitsBut = (unit == Units.None && !noDeeper && !nowNull); // no units but maybe next time...
            bool repeatedUnitViolation = Options.DisallowRepeatedUnit && repeatedUnit && !currentlyParsingZeroOnly;
            bool bigToSmallViolation = Options.StrictBigToSmall && unit <= prevUnit && !currentlyParsingZeroOnly;
            bool notTheFirstZeroOnly = !( // not: (i.e. it's fine if...)
                    unit != Units.ZeroOnly ||  // we're not looking at a ZeroOnly
                    nowNull ||  // we're at the start anyway
                    (currentlyParsingZeroOnly && !finishedParsingZeroOnly) || // we're in the middle of a coloned number and we haven't finished one yet
                    (!Options.StrictBigToSmall && !Options.DisallowRepeatedUnit));  // both StrictBigToSmall and DisallowRepeatedUnit are false (which means we allow multiple ZeroOnlys

            if (repeatedUnitViolation || bigToSmallViolation || notTheFirstZeroOnly || noUnitsBut) {
                Console.WriteLine("!! violation !!");

                if (noDeeper) {
                    Console.WriteLine("something went wrong");
                    throw new ArgumentException("something went wrong");
                }
                var next = NewBuilder();
                next.CompleteTimeSpan = TimeSpan;
                next.AddUnit(time, originalUnit, true);
                return next;
            }
            
            // Weeks, Days, Hours, Minutes, Seconds, Milliseconds

            if (unit == Units.Weeks) {
                timeSpan += TimeSpan.FromDays((double)time * 7);

            } else if (unit == Units.Days) {
                timeSpan += TimeSpan.FromDays((double)time);

            } else if (unit == Units.Hours) {
                timeSpan += TimeSpan.FromHours((double)time);

            } else if (unit == Units.Minutes) {
                timeSpan += TimeSpan.FromMinutes((double)time);

            } else if (unit == Units.Seconds) {
                //timeSpan += TimeSpan.FromSeconds(time); // not very accurate, use ticks instead.

                long ticks = (long) (time * 10_000_000);
                timeSpan += TimeSpan.FromTicks(ticks);

                if (Options.DecimalSecondsCountsAsMilliseconds) {
                    var timeTrunc = Math.Truncate(time);
                    if (time != timeTrunc) {
                        DoneUnits.Add(Units.Milliseconds);
                    }
                }

            } else if (unit == Units.Milliseconds) {
                timeSpan += TimeSpan.FromMilliseconds((double)time);

            } else if (unit == Units.ZeroOnly) {
                // do nothing

            }

            return this;
        }
    }
}

