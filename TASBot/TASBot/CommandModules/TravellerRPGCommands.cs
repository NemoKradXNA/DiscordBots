using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;

using TASLibBot.BaseClasses;
using TASLibBot.Utilities;
using TASLibBot.Services;
using TASLibBot.Enums;
using TASLibBot.DataModels;
using TASLibBot.Configuration;

using Discord.WebSocket;

using IronPdf;
using IronPdf.Forms;

namespace TASLibBot.CommandModules
{
    /// <summary>
    /// This command module is designed to be used for RPG elements.
    /// </summary>
    public class TravellerRPGCommands : CommandModuleBase
    {
        public TravellerRPGCommands() : base() { defaultColor = new Color(0,0,0); }
        protected TravellerService mapService { get { return (TravellerService)Services.GetService(typeof(TravellerService)); } }
        protected string rootCharactersFolder
        {
            get
            {
                return $"{Context.Guild.Name}/Characters";
            }
        }

        protected string thisUserCharacterFolder { get { return $"{rootCharactersFolder }/{Context.User.Username}{Context.User.Discriminator}"; } }

        protected LibraryData dataAM;
        protected LibraryData dataNZ;

        Dictionary<SocketUser, int> UsersLastRollResult = new Dictionary<SocketUser, int>();

        static Dictionary<SocketUser, string> UsersCharacterUse = new Dictionary<SocketUser, string>();

        static List<string> characteristic = new List<string>()
        {
            "Strength",
            "Dexterity",
            "Endurance",
            "Intellect",
            "Education",
            "Social",
            "Psi",
        };

        static List<string> characteristicShort = new List<string>()
        {
            "Str",
            "Dex",
            "End",
            "Int",
            "Edu",
            "Soc",
            "Psi",
        };

        static Dictionary<int, string> skillIndexMap = new Dictionary<int, string>()
        {
            {0 , "Skill 1" },
            {9   , "Skill 10" },
            {10  , "Skill 11" },
            {11  , "Skill 12" },
            {12  , "Skill 13" },
            {31  , "Skill 132" },
            {32  , "Skill 133" },
            {33  , "Skill 134" },
            {34  , "Skill 135" },
            {35  , "Skill 136" },
            {36  , "Skill 137" },
            {37  , "Skill 138" },
            {38  , "Skill 139" },
            {13  , "Skill 14" },
            {39  , "Skill 140" },
            {40  , "Skill 141" },
            {41  , "Skill 142" },
            {42  , "Skill 143" },
            {43  , "Skill 144" },
            {44  , "Skill 145" },
            {45  , "Skill 146" },
            {46  , "Skill 147" },
            {47  , "Skill 148" },
            {48  , "Skill 149" },
            {14  , "Skill 15" },
            {49  , "Skill 150" },
            {50  , "Skill 151" },
            {51  , "Skill 152" },
            {52  , "Skill 154" },
            {53  , "Skill 155" },
            {54  , "Skill 156" },
            {55  , "Skill 157" },
            {56  , "Skill 158" },
            {57  , "Skill 159" },
            {15  , "Skill 16" },
            {58  , "Skill 160" },
            {59  , "Skill 161" },
            {60  , "Skill 162" },
            {61  , "Skill 163" },
            {62  , "Skill 164" },
            {63  , "Skill 165" },
            {64  , "Skill 166" },
            {65  , "Skill 167" },
            {66  , "Skill 169" },
            {16  , "Skill 17" },
            {17  , "Skill 18" },
            {18  , "Skill 19" },
            {1   , "Skill 2" },
            {19  , "Skill 20" },
            {20  , "Skill 21" },
            {21  , "Skill 22" },
            {22  , "Skill 23" },
            {23  , "Skill 24" },
            {24  , "Skill 25" },
            {25  , "Skill 26" },
            {26  , "Skill 27" },
            {27  , "Skill 28" },
            {28  , "Skill 29" },
            {2   , "Skill 3" },
            {30  , "Skill 30" },
            {3   , "Skill 4" },
            {4   , "Skill 5" },
            {5   , "Skill 6" },
            {6   , "Skill 7" },
            {7   , "Skill 8" },
            {8   , "Skill 9" },

        };
        static Dictionary<int, string> specialIndexMap = new Dictionary<int, string>()
        {
            { 2  ,"Specilism 1" },
            {16  ,"Specilism10" },
            {17  ,"Specilism11" },
            {18  ,"Specilism12" },
            {19  ,"Specilism13" },
            {20  ,"Specilism14" },
            {21  ,"Specilism15" },
            {22  ,"Specilism16" },
            {23  ,"Specilism17" },
            {25  ,"Specilism18" },
            {26  ,"Specilism19" },
            {3   ,"Specilism2" },
            {27  ,"Specilism20" },
            {29  ,"Specilism21" },
            {30  ,"Specilism22" },
            {31  ,"Specilism23" },
            {32  ,"Specilism24" },
            {33  ,"Specilism25" },
            {34  ,"Specilism26" },
            {35  ,"Specilism27" },
            {38  ,"Specilism28" },
            {39  ,"Specilism29" },
            {4   ,"Specilism3" },
            {40  ,"Specilism30" },
            {44  ,"Specilism31" },
            {45  ,"Specilism32" },
            {48  ,"Specilism33" },
            {49  ,"Specilism34" },
            {50  ,"Specilism35" },
            {51  ,"Specilism36" },
            {52  ,"Specilism37" },
            {53  ,"Specilism38" },
            {55  ,"Specilism39" },
            {5   ,"Specilism4" },
            {56  ,"Specilism40" },
            {57  ,"Specilism41" },
            {58  ,"Specilism42" },
            {59  ,"Specilism43" },
            {64  ,"Specilism44" },
            {65  ,"Specilism45" },
            {6   ,"Specilism5" },
            {7   ,"Specilism6" },
            {8   ,"Specilism7" },
            {9   ,"Specilism8" },
            {10  ,"Specilism9" },
        };

        static Dictionary<int, string> skillNameIndexMap = new Dictionary<int, string>()
        {
            { 0   ,"Admin"},
            {1    ,"Advocate"},
            {2    ,"Animals"},
            {3    ,"Animals"},
            {4    ,"Animals"},
            {5    ,"Athletics"},
            {6    ,"Athletics"},
            {7    ,"Athletics"},
            {8    ,"Art"},
            {9    ,"Art"},
            {10   ,"Art"},
            {11   ,"Astrogation"},
            {12   ,"Broker"},
            {13   ,"Carouse"},
            {14   ,"Deception"},
            {15   ,"Diplomat"},
            {16   ,"Drive"},
            {17   ,"Drive"},
            {18   ,"Electronics"},
            {19   ,"Electronics"},
            {20   ,"Electronics"},
            {21   ,"Engineer"},
            {22   ,"Engineer"},
            {23   ,"Engineer"},
            {24   ,"Explosives"},
            {25   ,"Flyer"},
            {26   ,"Flyer"},
            {27   ,"Flyer"},
            {28   ,"Gambler"},
            {29   ,"Gunner"},
            {30   ,"Gunner"},
            {31   ,"Gun Combat"},
            {32   ,"Gun Combat"},
            {33   ,"Gun Combat"},
            {34   ,"Heavy Weapons"},
            {35   ,"Heavy Weapons"},
            {36   ,"Investigate"},
            {37   ,"Jack of all Trades"},
            {38   ,"Language"},
            {39   ,"Language"},
            {40   ,"Language"},
            {41   ,"Leadership"},
            {42   ,"Mechanic"},
            {43   ,"Medic"},
            {44   ,"Melee"},
            {45   ,"Melee"},
            {46   ,"Navigation"},
            {47   ,"Persuade"},
            {48   ,"Pilot"},
            {49   ,"Pilot"},
            {50   ,"Pilot"},
            {51   ,"Profession"},
            {52   ,"Profession"},
            {53   ,"Profession"},
            {54   ,"Recon"},
            {55   ,"Science"},
            {56   ,"Science"},
            {57   ,"Science"},
            {58   ,"Seafarer"},
            {59   ,"Seafarer"},
            {60   ,"Stealth"},
            {61   ,"Steward"},
            {62   ,"Streetwise"},
            {63   ,"Survival"},
            {64   ,"Tactics"},
            {65   ,"Tactics"},
            {66   ,"Vacc Suit"},

        };
        static Dictionary<int, string> skillnameIndexMapShort = new Dictionary<int, string>()
        {
            { 0   ,"Adm"},
            {1    ,"Advo"},
            {2    ,"Anml"},
            {3    ,"Anml"},
            {4    ,"Anml"},
            {5    ,"Athl"},
            {6    ,"Athl"},
            {7    ,"Athl"},
            {8    ,"Art"},
            {9    ,"Art"},
            {10   ,"Art"},
            {11   ,"Astro"},
            {12   ,"Brkr"},
            {13   ,"Carouse"},
            {14   ,"Decept"},
            {15   ,"Diplmt"},
            {16   ,"Drive"},
            {17   ,"Drive"},
            {18   ,"Elec"},
            {19   ,"Elec"},
            {20   ,"Elec"},
            {21   ,"Eng"},
            {22   ,"Eng"},
            {23   ,"Eng"},
            {24   ,"Expl"},
            {25   ,"Fly"},
            {26   ,"Fly"},
            {27   ,"Fly"},
            {28   ,"Gmbl"},
            {29   ,"Gunner"},
            {30   ,"Gunner"},
            {31   ,"Gun"},
            {32   ,"Gun"},
            {33   ,"Gun"},
            {34   ,"HvyWeap"},
            {35   ,"HvyWeap"},
            {36   ,"Invstig"},
            {37   ,"JOT"},
            {38   ,"Lang"},
            {39   ,"Lang"},
            {40   ,"Lang"},
            {41   ,"Lang"},
            {42   ,"Mech"},
            {43   ,"Medic"},
            {44   ,"Melee"},
            {45   ,"Melee"},
            {46   ,"Nav"},
            {47   ,"Prsd"},
            {48   ,"Pilot"},
            {49   ,"Pilot"},
            {50   ,"Pilot"},
            {51   ,"Prof"},
            {52   ,"Prof"},
            {53   ,"Prof"},
            {54   ,"Recon"},
            {55   ,"Sci"},
            {56   ,"Sci"},
            {57   ,"Sci"},
            {58   ,"Sea"},
            {59   ,"Sea"},
            {60   ,"Stealth"},
            {61   ,"Steward"},
            {62   ,"Street"},
            {63   ,"Survival"},
            {64   ,"Tac"},
            {65   ,"Tac"},
            {66   ,"Vacc"},

        };

        [Command("rpg-commands")]
        [Alias(new string[] { "rpg-cmds" })]
        [Summary("This command lists ALL RPG related commands.")]
        public virtual async Task RPGCommands(string hlp = null)
        {
            if (hlp == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}rpg-commands", "This command lists ALL RPG related commands", $"{BotConfiguration.thisBotConfig.CommandIndicator}rpg-commands");
                return;
            }

            string txt = "Here is the definitive list of RPG Commands available to you:-";

            EmbedBuilder msg = GetMsg(defaultColor, $"RPG Commands List", txt);

            List<CommandInfo> commands = GetMyCommandsList();
            foreach (CommandInfo command in commands)
            {
                string aliases = "";
                foreach (string alias in command.Aliases)
                    aliases += $"{BotConfiguration.thisBotConfig.CommandIndicator}{alias}, ";

                if (aliases.Length > 0)
                    aliases = aliases.Substring(0, aliases.Length - 2);

                msg.AddField($"{BotConfiguration.thisBotConfig.CommandIndicator}{command.Name}", $"Summary:\n{command.Summary.Replace("[~]", $"{BotConfiguration.thisBotConfig.CommandIndicator}")}\n\nAliases:\n{aliases}", true);
            }

            await SendMessageAsync(msg);
        }

        [Command("check")]
        [Alias(new string[] { "chk" })]
        [Summary("Simply rolls 2D6, use v <difficulty> (simple, easy routine, average, difficult, verydifficult or formidable)")]
        public async Task Check(params string[] diceParams)
        {            
            if (diceParams.Length == 1 && diceParams[0] == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}check", "Simply rolls 2D6 with modifiers and/or difficulty (simple, easy routine, average, difficult, verydifficult or formidable)", $"{BotConfiguration.thisBotConfig.CommandIndicator}check or {BotConfiguration.thisBotConfig.CommandIndicator}check v hard ");
                return;
            }

            List<string> rollParams = new List<string>();

            List<string> dice = diceParams.ToList();

            bool ignoreH = dice.SingleOrDefault(d => d.ToLower() == "bane") != null;
            bool ignoreL = dice.SingleOrDefault(d => d.ToLower() == "boon") != null;

            bool hasDifficulty = dice.SingleOrDefault(d => d.ToLower() == "v") != null;

            TaskDifficultyTypeEnum dif = TaskDifficultyTypeEnum.average;

            if (hasDifficulty)
            {
                int idx = dice.IndexOf("v") + dice.IndexOf("V") + 2;
                Enum.TryParse(dice[idx].ToLower(), out dif);

                dice = dice.Where(d => d.ToLower() != "v" && d.ToLower() != dif.ToString().ToLower()).ToList();
            }


            if (ignoreH || ignoreL)
                rollParams.Add("3D6");
            else
                rollParams.Add("2D6");

            foreach (string d in dice)
            {
                // exclude all other dice.
                if (d.ToUpper().Contains("D4") ||
                    d.ToUpper().Contains("D6") ||
                    d.ToUpper().Contains("D8") ||
                    d.ToUpper().Contains("D10") ||
                    d.ToUpper().Contains("D12") ||
                    d.ToUpper().Contains("D20") ||
                    d.ToUpper().Contains("D100") ||
                    d.ToUpper().Contains("COIN"))
                    continue;

                rollParams.Add(d);
            }

            // Any character related stuff in there?
            if (UsersCharacterUse.ContainsKey(Context.User))
            {
                string file = GetCharacterSheetFile(UsersCharacterUse[Context.User], Context.User as IGuildUser);
                Dictionary<string, string> sheet = GetCharacterFields(file);

                sheet = sheet.OrderBy(s => s.Key).ToDictionary(x => x.Key, x => x.Value);


                // Skill map
                Dictionary<string, string> skills = sheet.Where(k => k.Key.Contains("Skill ")).OrderBy(s => s.Key).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<string, string> special = sheet.Where(k => k.Key.Contains("Specilism")).OrderBy(s => s.Key).ToDictionary(x => x.Key, x => x.Value);

               
                
                //using (StreamWriter sw = new StreamWriter("Sheet.csv", false))
                //{
                //    foreach (string key in sheet.Keys)
                //    {
                //        sw.WriteLine(string.Format("{0},{1}", key, sheet[key]));
                //    }
                //}

                //using (StreamWriter sw = new StreamWriter("Skills.csv", false))
                //{
                //    foreach (string key in skills.Keys)
                //    {
                //        sw.WriteLine(string.Format("{0},{1},{2}", key, skills[key], int.Parse(skills[key]) - 1));
                //    }
                //}

                //using (StreamWriter sw = new StreamWriter("Specials.csv",false))
                //{
                //    foreach (string key in special.Keys)
                //    {
                //        sw.WriteLine(string.Format("{0},{1}", key, special[key]));
                //    }
                //}

                // Is this a characteristic check or skill
                List<string> charList = dice.Where(d => characteristic.Any(c => c.ToLower() == d.ToLower()) || characteristicShort.Any(cs => cs.ToLower() == d.ToLower())).ToList();
                List<string> skilLst = dice.Where(d => special.Any(sp => sp.Value.ToLower() == d.ToLower()) || skillNameIndexMap.Any(s => s.Value.ToLower() == d.ToLower()) || skillnameIndexMapShort.Any(ss => ss.Value.ToLower() == d.ToLower())).ToList();

                int finalMod = 0;

                if (charList != null)
                {
                    foreach (string ch in charList)
                    {
                        int idx = -1;
                        string s = characteristic.SingleOrDefault(c => c.ToLower() == ch);
                        if(s == null)
                            s = characteristicShort.SingleOrDefault(c => c.ToLower() == ch);

                        if (s != null)
                        {
                            if (characteristic.Contains(s))
                                idx = characteristic.IndexOf(s);
                            else
                                idx = characteristicShort.IndexOf(s);

                            if (idx != -1)
                            {
                                if (idx == 6)
                                    idx--;
                                string DM = $"DM {idx+1}";
                                string v = GetSheetField(sheet, DM);
                                int thisM = 0;
                                int.TryParse(v, out thisM);
                                finalMod += thisM;

                                rollParams.Remove(ch);
                            }
                        }
                    }
                }

                if (skilLst != null)
                {
                    foreach (string sk in skilLst)
                    {
                        int idx = -1;
                        Dictionary<int,string> kvps = skillNameIndexMap.Where(i => i.Value.ToLower() == sk).ToDictionary(x => x.Key, x => x.Value); 
                        if(kvps == null || kvps.Count == 0)
                            kvps = skillnameIndexMapShort.Where(i => i.Value.ToLower() == sk).ToDictionary(x => x.Key, x => x.Value);

                        if (kvps == null || kvps.Count == 0) // could be a specialization..
                        {
                            KeyValuePair<string, string> spKvp = special.SingleOrDefault(s => s.Value.ToLower() == sk);
                            kvps = specialIndexMap.Where(sp => sp.Value.ToLower() == spKvp.Key.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                        }

                        if (kvps != null && kvps.Count != 0)
                        {
                            KeyValuePair<int, string> kvp = new KeyValuePair<int, string>();

                            if (kvps.Count > 1) // must have specializm.
                            {
                                kvp = kvps.First();
                                EmbedBuilder msg = GetWarningMsg($"The skill '{skillNameIndexMap[kvp.Key]}' has a specialism, please specify.", Context.User as IGuildUser);
                                await SendMessageAsync(msg);
                                return;
                            }
                            else
                            {
                                kvp = kvps.First();
                            }

                            idx = kvp.Key;
                            string thisSkill = skillIndexMap[idx];
                            string v = GetSheetField(sheet, thisSkill);
                            int thisM = 0;
                            int.TryParse(v, out thisM);
                            finalMod += thisM;

                            rollParams.Remove(sk);
                        }
                    }
                }

                if (finalMod > 0)
                {
                    rollParams.Add($"+{finalMod}");
                }
                else if (finalMod < 0)
                    rollParams.Add($"-{finalMod}");
            }
                        
            await Roll(rollParams.ToArray());

            if (hasDifficulty)
            {
                int v = UsersLastRollResult[Context.User];

                int t = (v + (int)dif) - 8;

                string effect = "";

                if (t == 0)
                    effect = "Marginal Success";
                else if (t >= 1 && t <= 5)
                    effect = "Average Success";
                else if (t >= 6)
                    effect = "Exceptional Success";
                else if (t <= -6)
                    effect = "Exceptional Failure";
                else if (t >= -5 && t <= -1)
                    effect = "Average Failure";
                else if (t == -1)
                    effect = "Marginal Failure";

                EmbedBuilder msg = GetMsg(defaultColor, effect, $"Roll {v}, difficulty {(int)dif}/{dif}, effect {t}");
                await SendMessageAsync(msg);
            }
        }        

        [Command("roll")]
        [Alias(new string[] { "dice" })]
        [Summary("With this command you can roll dice or flip coins (coins are 0-1), [~]roll 3D6 3 1D4 will roll 3 D6 and 1 D4\nUse the command with boom and/or bane to ignore Lowest/Highest")]
        public async Task Roll(params string[] diceParams)
        {
            try
            {
                if (diceParams.Length == 0 || diceParams[0] == "?")
                {
                    await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}roll 3D6 3 1D4 will roll 3 D6 and 1 D4", "With this command you can roll dice or flip coins (coins are 0-1)\nUse the command with boon and/or bane to ignore Lowest/Highest\nmake the last field + or - do give a mod, 3D6 +4, to roll the same set multiple times, use xN, so 2D6 x2 would roll it twice.", $"{BotConfiguration.thisBotConfig.CommandIndicator}roll 3D6 -1");
                    return;
                }

                // Ignore low and high flags as well as show average flag
                bool ignoreL = false;
                bool ignoreH = false;
                bool average = false;

                List<string> dice = diceParams.ToList();

                int numberofRolls = 1;

                // get first and only multiple.
                string firstMultiplier = dice.FirstOrDefault(d => d.ToLower().Contains("x"));

                // do we have a roll multiplier?
                if (firstMultiplier != null)
                {
                    int.TryParse(firstMultiplier.Substring(1), out numberofRolls);

                    // Remove all roll multipliers
                    dice = dice.Where(d => !d.ToLower().Contains("x")).ToList();
                }


                // get any dice mods +/-
                int mod = 0;
                List<string> mods = dice.Where(d => d.Contains("+") || d.Contains("-")).ToList();

                // Sum the mods
                foreach (string m in mods)
                {
                    int thisMod = 0;
                    if (m.Contains("-"))
                    {
                        string md = m.Substring(1);
                        int.TryParse(md, out thisMod);
                        thisMod *= -1;
                    }
                    if (m.Contains("+"))
                    {
                        string md = m.Substring(1);
                        int.TryParse(md, out thisMod);
                    }

                    mod += thisMod;
                }

                // do we need to ignore the higest roll?
                ignoreH = dice.SingleOrDefault(d => d.ToLower() == "bane") != null;
                // do we need to ignore the lowest roll?
                ignoreL = dice.SingleOrDefault(d => d.ToLower() == "boon") != null;
                // do we want the average?
                average = dice.SingleOrDefault(d => d.ToLower() == "avg") != null;

                // remove markers.
                dice = dice.Where(d => d.ToLower() != "boon" && d.ToLower() != "bane" && d.ToLower() != "avg" && !d.Contains("-") && !d.Contains("+")).ToList();

                for (int rollCnt = 0; rollCnt < numberofRolls; rollCnt++)
                {
                    // Values rolled
                    List<int> rolls = new List<int>();

                    // user output of the roll.
                    List<string> rollRecord = new List<string>();

                    // roll averages
                    List<float> averages = new List<float>();

                    // final roll value
                    int rollValue = 0;
                    // user outpur for bad dice.
                    string badDice = "";


                    for (int d = 0; d < dice.Count; d++)
                    {
                        // dice to roll
                        string thisDiceString = dice[d];

                        // How many dice?
                        string no = "1";
                        // What type of dice (D6 by default).
                        string thisD = "D6";

                        // Is it a coin?
                        if (thisDiceString.ToLower().Contains("coin"))
                        {
                            no = thisDiceString.Substring(0, thisDiceString.ToUpper().IndexOf("C"));
                            thisD = "Coin";
                        }
                        else
                        {
                            try
                            {
                                no = thisDiceString.Substring(0, thisDiceString.ToUpper().IndexOf("D"));
                                thisD = thisDiceString.Substring(no.Length).ToUpper();
                            }
                            catch (Exception ex)
                            {
                                badDice += thisDiceString + " & ";
                                continue;
                            }
                        }

                        // Dice enum
                        DiceTypeEnum thisDice = DiceTypeEnum.Coin;

                        // Parse thisDice into an enum
                        if (Enum.TryParse(thisD, out thisDice))
                        {
                            // actual dice count
                            int cnt = 1;

                            // if no is null, set to 1
                            if (string.IsNullOrEmpty(no))
                                no = "1";

                            // try and parse how many to roll.
                            int.TryParse(no, out cnt);

                            // add dice average for this dice type.
                            averages.Add((((int)thisDice + 1) / 2f) * cnt);

                            // make each roll now.
                            for (int dr = 0; dr < cnt; dr++)
                            {
                                // Roll the dice
                                int valueRolled = mapService.GetDiceRoll(thisDice, 1);

                                // record the outcome
                                rollRecord.Add($"Rolling a {thisDice} for {valueRolled}");

                                // add it to the list of rolls.
                                rolls.Add(valueRolled);
                            }

                        }
                        else // Failed to parse to must be a bad dice type :/
                        {                            
                            badDice += thisDiceString + " & ";
                        }
                    }

                    int l = int.MaxValue;
                    int h = int.MinValue;

                    // Find lowest and highest rolls
                    foreach (int r in rolls)
                    {
                        l = Math.Min(l, r);
                        h = Math.Max(h, r);
                    }

                    if (ignoreH)
                        rollRecord.Add($"Highest Roll was {h}");
                    if (ignoreL)
                        rollRecord.Add($"Lowest Roll was {l}");

                    int idx = 0;

                    // Markers, has the boon or bust already been applied?
                    bool ignoredH = false;
                    bool ignoredL = false;

                    // output string showing what#s been rolled and applied.
                    string simleOutput = "";

                    foreach (int r in rolls)
                    {
                        int thisV = r;

                        if (!ignoredH && ignoreH && thisV >= h)
                        {
                            ignoredH = true;
                            rollRecord[idx] = $"{rollRecord[idx]} Ignored";
                            thisV = 0;
                        }

                        if (!ignoredL && ignoreL && thisV <= l)
                        {
                            ignoredL = true;
                            rollRecord[idx] = $"{rollRecord[idx]} Ignored";
                            thisV = 0;
                        }

                        rollValue += thisV;

                        if (thisV > 0)
                            simleOutput += $"{thisV}+";

                        idx++;
                    }

                    if (simleOutput.Length > 0)
                        simleOutput = simleOutput.Substring(0, simleOutput.Length - 1);

                    if (badDice.Length != 0)
                    {
                        badDice = badDice.Substring(0, badDice.Length - 3);
                        badDice = $"\nBad dice/param ignored {badDice}";
                    }

                    rollValue += mod;

                    string txt = $"You rolled a total of {rollValue} {badDice}";

                    // Build output to server.
                    EmbedBuilder msg = GetMsg(defaultColor, $"Rolling {string.Join(' ', dice)}", txt);

                    foreach (string str in rollRecord)
                    {
                        if (str.ToLower().Contains("lowest"))
                            msg.AddField("Lowest", str);
                        else if (str.ToLower().Contains("highest"))
                            msg.AddField("Highest", str);
                        else
                            msg.AddField("Roll", str);
                    }

                    if (mod != 0)
                    {
                        if (mod > 0)
                        {
                            msg.AddField("Mod", $"+{mod}");
                            simleOutput = $"({simleOutput})+{mod}";
                        }
                        else
                        {
                            msg.AddField("Mod", $"{mod}");
                            simleOutput = $"({simleOutput}){mod}";
                        }
                    }

                    // simplify output..
                    msg = GetMsg(defaultColor, $"Rolling {string.Join(' ', dice)}", txt);
                    simleOutput += $"={rollValue}";

                    if (average)
                    {
                        int sumAverage = (int)averages.Sum();
                        msg.AddField($"Calculated Average", sumAverage);
                    }


                    string ignored = rollRecord.SingleOrDefault(rr => rr.Contains("Ignored"));
                    if (ignored != null)
                        msg.AddField("Ignored", ignored);

                    msg.AddField($"Result {rollValue}", simleOutput);

                    if (!UsersLastRollResult.ContainsKey(Context.User))
                        UsersLastRollResult.Add(Context.User, 0);

                    UsersLastRollResult[Context.User] = rollValue;

                    await SendMessageAsync(msg);
                }
            }
            catch (Exception ex)
            {
                await SendMessageAsync(GetErrorMsg(ex.Message, Context.User));
            }
        }

        [Command("library")]
        [Alias(new string[] { "lib" })]
        [Summary("This command will allow you to search the LBB library suppliments, e.g. [~]library ATV")]
        public async Task Library(params string[] grains)
        {
            if (grains.Length == 0 || grains[0] == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}library ATV", "This command will allow you to search the LBB library suppliments", $"{BotConfiguration.thisBotConfig.CommandIndicator}library atv");
                return;
            }

            string txt = $"Thank you for your library query on '{string.Join(' ', grains)}', this is what we have found:-";
            
            if (dataAM == null)
                dataAM = DeserializeJSON<LibraryData>(GetJSONFileData("Library/LibraryA-M.json"));
            if(dataNZ == null)
                dataNZ = DeserializeJSON<LibraryData>(GetJSONFileData("Library/LibraryN-Z.json"));

            List<LibraryEntry> searchResults = new List<LibraryEntry>();

            // Concat grains
            string term = string.Join(' ', grains);

            searchResults.AddRange(dataAM.Entries.Where(e => e.Name.ToLower().Contains(term.ToLower()) && !searchResults.Contains(e)));
            searchResults.AddRange(dataNZ.Entries.Where(e => e.Name.ToLower().Contains(term.ToLower()) && !searchResults.Contains(e)));

            if (searchResults.Count == 0)
            {
                foreach (string grain in grains)
                {
                    searchResults.AddRange(dataAM.Entries.Where(e => e.Name.ToLower().Contains(grain.ToLower()) && !searchResults.Contains(e)));
                    searchResults.AddRange(dataNZ.Entries.Where(e => e.Name.ToLower().Contains(grain.ToLower()) && !searchResults.Contains(e)));
                }
            }

            // Search content..
            if (searchResults.Count == 0)
            {
                foreach (string grain in grains)
                {
                    searchResults.AddRange(dataAM.Entries.Where(e => e.Data.ToLower().Contains(grain.ToLower()) && !searchResults.Contains(e)));
                    searchResults.AddRange(dataNZ.Entries.Where(e => e.Data.ToLower().Contains(grain.ToLower()) && !searchResults.Contains(e)));
                }
            }

            EmbedBuilder msg = GetMsg(defaultColor, $"Library A-Z found {searchResults.Count} entries", txt);

            if (searchResults.Count > 10)
            {
                msg.AddField("Too vague", "Your query returned too many results, please be more specific, and try again.", true);
                await SendMessageAsync(msg);
                return;
            }

            await SendMessageAsync(msg);



            foreach (LibraryEntry entry in searchResults)
            {
                List<string> words = entry.Data.Split(" ").ToList();

                string txtOut = string.Empty;

                foreach (string word in words)
                {
                    if (txtOut.Length + word.Length >= 2048)
                    {
                        txtOut = txtOut.Substring(0, txtOut.Length - 1);
                        msg = GetMsg(defaultColor, entry.Name, txtOut);
                        await SendMessageAsync(msg, null, "...cont");

                        await Task.Delay(100);
                        //await Task.Delay(15);
                        txtOut = word + " ";
                    }
                    else
                        txtOut += word + " ";

                }

                if (!string.IsNullOrEmpty(txtOut))
                {
                    msg = GetMsg(defaultColor, entry.Name, txtOut);
                    await SendMessageAsync(msg, null, "end");
                }
            }

            msg = GetMsg(defaultColor, $"Library A-Z found {searchResults.Count} entries", "End");
            await SendMessageAsync(msg);
        }

        [Command("add-character")]
        [Alias(new string[] { "add-chr"})]
        [Summary("This command allows you to upload a character to the bot")]        
        public async Task AddCharacter(string type = null)
        {
            if (string.IsNullOrEmpty(type) || type == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}add-character ct", "This command allows you to upload a character to the bot", $"{BotConfiguration.thisBotConfig.CommandIndicator}add-character ct [remember to add your file before sending the command.]");
                return;
            }

            EmbedBuilder msg = GetMsg("TODO", "This command is not yet implemented");

            if (Context.Message.Attachments.Count == 0)
            {
                msg.AddField("No File", "You need to attache a character sheet to the command.");
            }
            else
            {
                try
                {
                    string folder = thisUserCharacterFolder;

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    foreach (Attachment att in Context.Message.Attachments)
                    {
                        string file = GetResponseToFile(att.Url, $"{folder}/{att.Filename}");

                        try
                        {
                            if (ValidCharacterSheet(file, type))
                            {
                                msg.AddField("Received", att.Filename);
                            }
                            else
                            {
                                msg.AddField("Inalid", $"File {att.Filename} is not a valid character sheet format for {type}");
                                File.Delete(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            File.Delete(file);
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
                    
                }
            }            

            await SendMessageAsync(msg);
        }

        [Command("use-character")]
        [Alias(new string[] { "use-char" })]
        [Summary("This command allows you to put a character shet in use.")]
        public async Task UseCharacter(string filename = null)
        {
            if (filename == "?")
            {
                string current = "\n[When using a character, you will see it here.]";

                if (UsersCharacterUse.ContainsKey(Context.User))
                    current = $"\nYou are currently using {UsersCharacterUse[Context.User]}";

                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}use-character", 
                    $"This command allows you to put a character shet in use.{current}", 
                    $"{BotConfiguration.thisBotConfig.CommandIndicator}use-character filename");
                return;
            }

            EmbedBuilder msg = null;
            if (string.IsNullOrEmpty(filename))
            {
                // Stop using character.
                if (UsersCharacterUse.ContainsKey(Context.User))
                {
                    UsersCharacterUse.Remove(Context.User);
                    msg = GetMsg($"{filename}", $"This character sheet is no longer in use.");
                }
                else
                {
                    msg = GetMsg($"None in use...", $"You didn't have a character in use.");
                }
            }
            else
            {
                // Is it a valid character?
                string characterSheet = GetCharacterSheetFile(filename, Context.User as IGuildUser);
                if (!string.IsNullOrEmpty(characterSheet))
                {
                    characterSheet = characterSheet.Substring(characterSheet.LastIndexOf("\\") + 1);
                    UsersCharacterUse.Add(Context.User, characterSheet);
                    msg = GetMsg($"{characterSheet}", $"You are now using this character sheet.");
                }
                else
                {
                    msg = GetMsg($"{filename}", $"This character sheet could not be located, please try again.");
                }
            }

            await SendMessageAsync(msg);
        }

        [Command("get-character")]
        [Alias(new string[] { "get-chr" })]
        [Summary("This command allows you to download a character from the bot")]        
        public async Task GetCharacter(string fileRequested = null)
        {
            if (string.IsNullOrEmpty(fileRequested) || fileRequested == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}get-character ct", "This command allows you to download a character from the bot", $"{BotConfiguration.thisBotConfig.CommandIndicator}get-character ct filename");
                return;
            }

            EmbedBuilder msg = null;

            try
            {
                string file = GetCharacterSheetFile(fileRequested, Context.User as IGuildUser);

                if (!string.IsNullOrEmpty(file))
                {
                    msg = GetMsg(fileRequested, "Here is the character file you requested.");
                    await Context.Channel.SendFileAsync(file, Context.Message.Content, false, msg.Build());
                    return;
                }
                else
                {
                    msg = GetMsg("None Found", "No character sheet found with that name...");
                }

            }
            catch (Exception ex)
            {
                msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
            }

            await SendMessageAsync(msg);
        }

        [Command("get-character-sheet")]
        [Alias(new string[] { "get-chr-sheet" })]
        [Summary("This command allows you to get a quick view of your character sheet.")]
        public async Task GetCharacterSheet(string characterSheet = null)
        {
            if (string.IsNullOrEmpty(characterSheet) || characterSheet == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}get-character-sheet", "This command allows you to get a quick view of your character sheet.", $"{BotConfiguration.thisBotConfig.CommandIndicator}get-character-sheet filename");
                return;
            }

            EmbedBuilder msg = null;

            try
            {
                string file = GetCharacterSheetFile(characterSheet, Context.User as IGuildUser);

                if (!string.IsNullOrEmpty(file))
                {
                    Dictionary<string, string> sheet = GetCharacterFields(file);
                    msg = GetMsg(characterSheet, "Here is the character sheet you requested.");


                    msg.AddField("Name", GetSheetField(sheet,"Name","Unknown"), true);
                    msg.AddField("Age", GetSheetField(sheet, "Age", "Unknown"), true);
                    msg.AddField("Homeworld", GetSheetField(sheet, "Homeworld", "Unknown"), true);
                    msg.AddField("Pension", GetSheetField(sheet, "Pension", "0"), true);
                    msg.AddField("Debt", GetSheetField(sheet, "Debt", "0"), true);
                    msg.AddField("Cash On Hand", GetSheetField(sheet, "Cash on hand", "0"), true);

                    int idx = 0;
                    foreach (string ch in characteristic)
                    {
                        string v = GetSheetField(sheet, ch, "1");

                        if (idx == 6)
                            idx--;

                        string dm = GetSheetField(sheet, $"DM {idx+1}", "0");

                        if (!dm.Contains("-"))
                            dm = $"+{dm}";

                        msg.AddField(ch, $"{v} ({dm})", true);

                        idx++;
                    }
                    //msg.AddField("Strength", GetSheetField(sheet, "Strength", "1"), true);
                    //msg.AddField("Dexterity", GetSheetField(sheet, "Dexterity", "1"), true);
                    //msg.AddField("Endurance", GetSheetField(sheet, "Endurance", "1"), true);
                    //msg.AddField("Intellect", GetSheetField(sheet, "Intellect", "1"), true);
                    //msg.AddField("Education", GetSheetField(sheet, "Education", "1"), true);
                    //msg.AddField("Social", GetSheetField(sheet, "Social", "1"), true);
                    //msg.AddField("Psi", GetSheetField(sheet, "PSI", "0"), true);
                }
                else
                {
                    msg = GetMsg("None Found", "No character sheet found with that name...");
                }
            }
            catch (Exception ex)
            {
                msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
            }

            await SendMessageAsync(msg);
        }

        [Command("drop-character")]
        [Alias(new string[] { "drop-char" })]
        [Summary("This command allows you to delete a character sheet from the bot")]
        public async Task DropCharacterSheet(string characterSheet = null)
        {
            if (string.IsNullOrEmpty(characterSheet) || characterSheet == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}drop-character", "This command allows you to delete a character sheet from the bot", $"{BotConfiguration.thisBotConfig.CommandIndicator}drop-character filename");
                return;
            }

            EmbedBuilder msg = null;

            try
            {
                string file = GetCharacterSheetFile(characterSheet, Context.User as IGuildUser);

                if (!string.IsNullOrEmpty(file))
                {
                    File.Delete(file);
                    msg = GetMsg(characterSheet, "This character sheet has been removed.");
                }
                else
                {
                    msg = GetMsg("None Found", "No character sheet found with that name...");
                }
            }
            catch (Exception ex)
            {
                msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
            }

            await SendMessageAsync(msg);
        }

        [Command("show-my-characters")]
        [Alias(new string[] { "show-chars", "my-chars" })]
        [Summary("This command will show you all the character sheets you have uploaded")]
        public async Task ShowMyCharacters(string info = null)
        {
            if (info == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}show-my-caharacters", "This command will show you all the character sheets you have uploaded", $"{BotConfiguration.thisBotConfig.CommandIndicator}show-my-caharacters");
                return;
            }

            EmbedBuilder msg = null;
            string folder = thisUserCharacterFolder;

            try
            {
                List<string> files = Directory.GetFiles(folder).ToList();

                if (files == null || files.Count == 0)
                {
                    msg = GetMsg("None Found", $"You have no uploaded character sheets, to upload a shhet use the {BotConfiguration.thisBotConfig.CommandIndicator}add-character command");
                }
                else
                {
                    msg = GetMsg($"", "Here are the character sheets you currently have");

                    foreach (string file in files)
                    {
                        string fileName = file.Substring(file.Replace("\\","/").LastIndexOf("/")+1);

                        Dictionary<string, string> sheet = GetCharacterFields(file, new List<string>() { "Name" });
                        msg.AddField($"Character Name: { GetSheetField(sheet, "Name", "Unknown")}", $"{fileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
            }

            await SendMessageAsync(msg);
        }

        [Command("show-all-characters")]
        [Alias(new string[] { "show-all-chrs" })]
        [Summary("[Admin Only] Show a list of all the character sheets currently stored...")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task GetAllCharacterSheets(string player = null)
        {

            if (player == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}show-all-characters", "This command allows administrators to see all character sheets. Leave blank to see all folders with file counts, or give folder name to see file names.", $"{BotConfiguration.thisBotConfig.CommandIndicator}show-all-characters folder");
                return;
            }

            string folder = $"{rootCharactersFolder}";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            List<string> directories = Directory.GetDirectories(folder).ToList();

            EmbedBuilder msg = null;

            if (!string.IsNullOrEmpty(player))
            {
                folder = $"{folder}/{player}";
                if (Directory.Exists(folder))
                {
                    List<string> files = Directory.GetFiles(folder).ToList();

                    msg = GetMsg($"Character Sheets", $"Here are the {files.Count} character sheets found in {player}");


                    foreach (string file in files)
                    {
                        Dictionary<string, string> sheet = GetCharacterFields(file, new List<string>() { "Name" });

                        msg.AddField($"Character Name: {GetSheetField(sheet, "Name", "Unknown")}", file.Replace("\\","/"));
                    }
                }
                else
                {
                    msg = GetErrorMsg($"The folder for {player} does not exist, please try again.", Context.User);
                }
            }
            else
            {
                msg = GetMsg($"Character Sheets", $"A total of {directories.Count} have been found.");

                foreach (string dir in directories)
                {
                    List<string> files = Directory.GetFiles(dir).ToList();

                    msg.AddField(dir.Replace("\\", "/"), $"{files.Count} Found", true);
                    
                }
            }

            await SendMessageAsync(msg);
            
        }

        protected virtual string GetCharacterSheetFile(string fileRequested, IGuildUser gu)
        {
            string folder = string.Empty;
            List<string> files = null;
            string file = string.Empty;

            if (gu != null && gu.GuildPermissions.Administrator)
            {
                folder = $"{rootCharactersFolder}";
                List<string> directories = Directory.GetDirectories(folder).ToList();

                foreach (string dir in directories)
                {
                    files = Directory.GetFiles(dir).ToList();

                    file = files.SingleOrDefault(f => f.ToLower().Contains(fileRequested.ToLower()));

                    if (!string.IsNullOrEmpty(file))
                        break;
                }
            }
            else
            {
                folder = thisUserCharacterFolder;

                files = Directory.GetFiles(folder).ToList();

                file = files.SingleOrDefault(f => f.ToLower().Contains(fileRequested.ToLower()));
            }

            return file;
        }

        protected string GetSheetField(Dictionary<string, string> sheet, string field, string defaultValue = "0")
        {
            string retVal = string.Empty;

            if (sheet.Keys.Contains(field))
                retVal = sheet[field];

            if (string.IsNullOrEmpty(retVal))
                retVal = defaultValue;

            return retVal;
        }

        protected Dictionary<string, string> GetCharacterFields(string characterFile, List<string> fields = null)
        {
            Dictionary<string, string> characterFields = new Dictionary<string, string>();
            PdfDocument PDF = PdfDocument.FromFile(characterFile);

            List<string> fieldsName = PDF.Form.FieldNames.ToList();
            List<FormField> fieldData = PDF.Form.Fields.ToList();

            foreach (string name in fieldsName)
            {
                if (fields == null || fields.Contains(name))
                {
                    int idx = fieldsName.IndexOf(name);
                    characterFields.Add(name, fieldData[idx].Value);
                }
            }

            return characterFields;
        }

        protected virtual bool ValidCharacterSheet(string file, string type)
        {
            // Validate that this sheet is a valid character sheet.
            string ext = file.Substring(file.LastIndexOf(".") + 1);

            PdfDocument PDF = PdfDocument.FromFile(file);

            string text = PDF.ExtractAllText();

            List<string> fieldName = PDF.Form.FieldNames.ToList();
            List<FormField> fields = PDF.Form.Fields.ToList();

            if (ext.ToLower() != "pdf")
            {
                return false;
            }
            
            return true;
        }
    }
}
