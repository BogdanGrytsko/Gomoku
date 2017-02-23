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
        LongBrokenThree = 12,
        TwoInRow = 6,
        DeadFour = 5,
        BlokedThree = 4,
        BlockedTwo = 3,
        DeadThree = 3,
        DeadTwo = 2,
        SingleMark = 1,
        Useless = 0
    }
}