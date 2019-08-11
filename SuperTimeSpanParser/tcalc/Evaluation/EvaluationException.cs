using System;

namespace tcalc.Evaluation
{
    public class EvaluationException : Exception
    {
        public EvaluationException(string message)
            : base(message)
        {
        }
    }
}
