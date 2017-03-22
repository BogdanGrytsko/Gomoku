using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public abstract class TwoCellAnalyzer : AnalyzerBase
    {
        protected TwoCellAnalyzer(Line line) : base(line)
        {
        }

        public override LineType Dead()
        {
            return LineType.DeadTwo;
        }
    }
}