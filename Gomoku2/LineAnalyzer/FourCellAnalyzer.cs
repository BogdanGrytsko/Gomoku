using Gomoku2.LineCore;

namespace Gomoku2.LineAnalyzer
{
    public abstract class FourCellAnalyzer : AnalyzerBase
    {
        protected FourCellAnalyzer(Line line) : base(line)
        {
        }

        public override int NextAnalyzeLength => 1;
    }
}