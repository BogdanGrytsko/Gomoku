using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public abstract class ThreeCellAnalyzer : AnalyzerBase
    {
        protected ThreeCellAnalyzer(Line line) : base(line)
        {
        }

        public override LineType Dead()
        {
            return LineType.DeadThree;
        }

        public override int NextAnalyzeLength => 2;
    }
}