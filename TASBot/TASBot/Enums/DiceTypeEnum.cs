using System;
using System.Collections.Generic;
using System.Text;

namespace TASLibBot.Enums
{
    public enum DiceTypeEnum : int
    {
        Coin = 1,
        D4 = 4,
        D6 = 6,
        D8 = 8,
        D10 = 10,
        D12 = 12,
        D20 = 20,
        D100 = 100,
    }

    public enum TaskDifficultyTypeEnum : int
    {
        simple = 6,
        easy = 4,
        routine = 2,
        average = 0,
        difficult = -2,
        verydifficult = -4,
        formidable = -6
    }
}
