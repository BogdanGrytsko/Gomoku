namespace Gomoku2.LineCore
{
    public enum LineType
    {
        //"always" lethal
        FiveInRow = 10000,
        StraightFour = 4000,
        DoubleThreat = 1500,
        //lethal on my turn
        //1 continuation
        FourInRow = 100,
        BrokenFour = 99,
        //2 continuations
        ThreeInRow = 50,
        //1 continuation
        BrokenThree = 49,
        //non lethal - threat generating
        //2 continuations
        LongBrokenThree = 25,
        //4 continuations, broken has 3
        TwoInRow = 20,
        //2 continuations
        LongBrokenTwo = 16,
        //2 continuations
        BlokedThree = 15,
        //non threat generating 
        BlockedTwo = 6,
        DeadFour = 4,
        DeadThree = 3,
        DeadTwo = 2,
        SingleMark = 1,
        Useless = 0
    }
}