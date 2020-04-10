using System;
using System.Collections.Generic;
using System.Text;

using TASBot.Enums;

namespace TASBot.Interfaces
{
    public interface IRPGService
    {
        int GetDiceRoll(DiceTypeEnum dieType, int numberOfDice = 1);
    }
}
