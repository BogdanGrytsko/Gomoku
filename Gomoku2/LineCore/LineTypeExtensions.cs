namespace Gomoku2.LineCore
{
    public static class LineTypeExtensions
    {
        public static bool IsDeadTwo(this LineType lineType)
        {
            return lineType == LineType.DeadTwo;
        }

        public static bool IsTwoInRow(this LineType lineType)
        {
            return lineType == LineType.TwoInRow;
        }

        public static bool IsBrokenFourInRow(this LineType lineType)
        {
            return lineType == LineType.BrokenFour;
        }

        public static bool IsBlokedThree(this LineType lineType)
        {
            return lineType == LineType.BlokedThree;
        }

        public static bool IsBrokenThree(this LineType lineType)
        {
            return lineType == LineType.BrokenThree;
        }

        public static bool IsLongBrokenThree(this LineType lineType)
        {
            return lineType == LineType.LongBrokenThree;
        }

        public static bool IsLongBrokenTwo(this LineType lineType)
        {
            return lineType == LineType.LongBrokenTwo;
        }

        public static bool IsDeadThree(this LineType lineType)
        {
            return lineType == LineType.DeadThree;
        }

        public static bool ThreatOfFour(this LineType lineType)
        {
            return lineType == LineType.FourInRow || lineType == LineType.BrokenFour;
        }

        public static bool FourCellLine(this LineType lineType)
        {
            return ThreatOfFour(lineType) || lineType == LineType.StraightFour;
        }

        public static bool ThreatOfThree(this LineType lineType)
        {
            return lineType.IsThreeInRow() || lineType.IsBrokenThree();
        }

        public static bool IsThreeInRow(this LineType lineType)
        {
            return lineType == LineType.ThreeInRow;
        }

        public static bool IsStraightFour(this LineType lineType)
        {
            return lineType == LineType.StraightFour;
        }

        public static bool IsSingleMark(this LineType lineType)
        {
            return lineType == LineType.SingleMark;
        }
    }
}