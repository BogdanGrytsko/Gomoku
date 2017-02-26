namespace Gomoku2
{
    public enum LineType
    {
        FiveInRow = 10000,
        StraightFour = 3000,
        DoubleThreat = 1000,
        DoubleBrokenThree = 35,
        FourInRow = 30,
        BrokenFourInRow = 23,
        ThreeInRow = 20,
        BrokenThree = 15,
        LongBrokenThree = 13,
        TwoInRow = 8,
        DeadFour = 7,
        BlokedThree = 6,
        LongBlockedThree = 5,
        BlockedTwo = 4,
        DeadThree = 3,
        DeadTwo = 2,
        SingleMark = 1,
        Useless = 0
    }
}