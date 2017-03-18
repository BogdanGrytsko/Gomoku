namespace Gomoku2.LineCore
{
    public enum LineType
    {
        FiveInRow = 10000,
        StraightFour = 3000,
        DoubleThreat = 1000,
        DoubleBrokenThree = 40,
        FourInRow = 35,
        BrokenFourInRow = 28,
        ThreeInRow = 25,
        //todo set to ThreeInRow
        BrokenThree = 20,
        LongBrokenThree = 16,
        TwoInRow = 13,
        DeadFour = 12,
        LongBrokenTwoInRow = 8,
        BlokedThree = 6,
        LongBlockedThree = 5,
        BlockedTwo = 4,
        DeadThree = 3,
        DeadTwo = 2,
        SingleMark = 1,
        Useless = 0
    }
}