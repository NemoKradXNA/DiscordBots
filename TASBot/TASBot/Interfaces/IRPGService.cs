using System;
using System.Collections.Generic;
using System.Text;

using TASLibBot.Enums;

namespace TASLibBot.Interfaces
{
    public interface IRPGService
    {
        int GetDiceRoll(DiceTypeEnum dieType, int numberOfDice = 1);
    }
}
