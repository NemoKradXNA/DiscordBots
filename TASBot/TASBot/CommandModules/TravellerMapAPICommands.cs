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
using TASLibBot.DataModels;
using TASLibBot.Configuration;

namespace TASLibBot.CommandModules
{
    /// <summary>
    /// This Command Module is used to interface with the most excellent https://travellermap.com API
    /// </summary>
    public class TravellerMapAPICommands : CommandModuleBase
    {
        public TravellerMapAPICommands() : base() { defaultColor = new Color(0, 71, 171); }
        protected TravellerService mapService { get { return (TravellerService)Services.GetService(typeof(TravellerService)); } }

        [Command("travellermap")]
        [Alias(new string[] { "tm", "map" })]
        [Summary("This command lists ALL Traveller Map API commands")]        
        public virtual async Task TravellerMap(string help = null)
        {

            if (help == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}travellermap", "This command lists ALL Traveller Map API commands", $"{BotConfiguration.thisBotConfig.CommandIndicator}travellermap");
                return;
            }

            string txt = "Here is the definitive list of Traveller Map Commands available to you:-";

            EmbedBuilder msg = GetMsg(defaultColor, $"Traveller Map API Commands List", txt);

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

        //[Command("universe")]
        //[Summary("This will get a list of ALL sectors, there are a lot, so it may take some time...")]
        //public virtual async Task TM_Universe()
        //{
        //    string hlp = "Here is the definitive list of Traveller Map Command available to you:-";

        //    EmbedBuilder Msg = null;
            
            

        //    if (mapService != null)
        //    {
        //        string last = "";
        //        try
        //        {
        //            string json = GetResponse(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Universe]);

        //            Universe universe = DeserializeJSON<Universe>(json);

        //            Msg = GetMsg(defaultColor, $"The universe contains {universe.Sectors.Count} Sectors", hlp);

        //            Msg.AddField("Sectors", "[Abbreviation] - [Coordinates] - [Names]", false);
        //            int tcnt = 0;
        //            int cnt = 0;
        //            foreach (Sector sector in universe.Sectors)
        //            {
        //                last = sector.Abbreviation;

        //                if (string.IsNullOrEmpty(last))
        //                {
        //                    last = "???";
        //                }
                       
        //                Msg.AddField($"{last}", $"({sector.X},{sector.Y})\n{sector.GetNames()}", true);
        //                cnt++;
        //                tcnt++;

        //                if (cnt == 20)
        //                {
        //                    Msg.AddField($"{tcnt}/{universe.Sectors.Count}", $"Continued...", true);
        //                    cnt = 0;
        //                    await Context.Channel.SendMessageAsync("", false, Msg.Build());

        //                    Msg = GetMsg(defaultColor, $"...", "Continued");

        //                }
        //            }
                    
        //        }
        //        catch (Exception ex)
        //        {
        //            Msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
        //        }
        //    }
        //    else
        //    {
        //        Msg = GetErrorMsg("Service does not use Traveller API", Context.User);
        //    }

        //    await Context.Channel.SendMessageAsync("", false, Msg.Build());
        //}

        [Command("sector")]
        [Summary("This command will return a sectors information, e.g. [~]sector spin")]
        public virtual async Task TM_Sector(string sector = null)
        {
            EmbedBuilder msg = null;

            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}sector spin", "This command will return a sectors information", $"{BotConfiguration.thisBotConfig.CommandIndicator}sector <sector>");
                return;
            }

            string txt = $"Here is the data for the {sector} sector:-";
            

            if (mapService != null)
            {
                try
                {
                    string data = GetResponse(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Sector].Replace("/sector", $"/{sector}"));

                    SectorSurvey sectorSurvey = new SectorSurvey(data);

                    msg = GetMsg(defaultColor, $"{sectorSurvey.Title} Survey", txt);
                    msg.AddField("Coords", sectorSurvey.Coords, true);
                    msg.AddField("Names", sectorSurvey.GetNames());
                    msg.AddField("Subsectors", sectorSurvey.GetSubSectors());
                    msg.AddField("Allegancies", sectorSurvey.GetAlegancies());
                    msg.AddField("Worlds", sectorSurvey.Worlds.Count, true);

                }
                catch (Exception ex)
                {
                    msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
                }
            }
            else
            {
                msg = GetErrorMsg("Service does not use Traveller API", Context.User);
            }

            await SendMessageAsync(msg);            
        }

        [Command("sector-worlds")]
        [Summary("This command will return a sectors worlds information, e.g. [~]sector-worlds spin")]
        public virtual async Task TM_SectorWorlds(string sector = null)
        {
            EmbedBuilder msg = null;

            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}sector-worlds spin", "This command will return a sectors worlds information", $"{BotConfiguration.thisBotConfig.CommandIndicator}sector-worlds <sector>");
                return;
            }

            string txt = $"Here is the world data for the {sector} sector:-";

            
            try
            {
                string data = GetResponse(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Sector].Replace("/sector", $"/{sector}"));

                SectorSurvey sectorSurvey = new SectorSurvey(data);

                msg = GetMsg(defaultColor, $"{sectorSurvey.Title} Worlds", txt);
                msg.AddField("Coords", sectorSurvey.Coords, true);


                int cnt = 0;
                int tcnt = 0;
                foreach (WorldSurvey world in sectorSurvey.Worlds)
                {
                    msg.AddField($"{world.Name}", $"{world.Hex} - {world.UWP} - {world.Z}\n", true);
                    cnt++;
                    tcnt++;

                    if (cnt == 20)
                    {
                        cnt = 0;
                        await SendMessageAsync(msg,null, $"{tcnt}/{sectorSurvey.Worlds.Count} continued...");

                        msg = GetMsg(defaultColor, $"{sectorSurvey.Title} Worlds...",null,null,"...continued");

                    }
                }
            }
            catch (Exception ex)
            {
                msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
            }


            await SendMessageAsync(msg);            
        }

        [Command("subsector")]
        [Summary("Shows a sectors information, ~subsector sector subsector, e.g. [~]subsector spin Regina")]
        public virtual async Task TM_SubSector(string sector = null, string subsector = null)
        {
            EmbedBuilder msg = null;

            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}subsector spin Regina", "Shows a sectors information, ~subsector sector subsector", $"{BotConfiguration.thisBotConfig.CommandIndicator}subsector <sector> <subsector>");
                return;
            }

            string txt = $"Here is the world data for the {subsector} subsector:-";

            if (mapService != null)
            {
                try
                {
                    string data = GetResponse(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Subsector].Replace("/sector", $"/{sector}").Replace("/subsector",$"/{subsector}"));

                    SubSectorSurvey sub = new SubSectorSurvey(data);
                    msg = GetMsg(defaultColor, $"Worlds", txt);

                    int cnt = 0;
                    int tcnt = 0;
                    foreach (WorldSurvey world in sub.Worlds)
                    {
                        msg.AddField($"{world.Name}", $"Hex: {world.Hex}\nUWP: {world.UWP}\nRemarks: {world.Remarks}\nIx: {world.Ix}\nEx: {world.Ex}\nCx: {world.Cx}\nN: {world.N}\nB: {world.B}\nZ: {world.Z}\nPBG: {world.PBG}\nW: {world.W}\nA: {world.A}\nStellar: {world.Stellar}\n", true);
                        cnt++;
                        tcnt++;

                        if (cnt == 20)
                        {
                            cnt = 0;
                            await SendMessageAsync(msg,null, $"{tcnt}/{sub.Worlds.Count} continued...");

                            msg = GetMsg(defaultColor, $"More Worlds...", null, null, "...continued");

                        }
                    }

                }
                catch (Exception ex)
                {
                    msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
                }
            }
            else
            {
                msg = GetErrorMsg("Service does not use Traveller API", Context.User);
            }

            await SendMessageAsync(msg);
            
        }

        [Command("sector-map")]
        [Summary("Get the map for the given sector, e,g, [~]sector-map spin")]
        public virtual async Task TM_SectorMap(string sector = null)
        {
            EmbedBuilder msg = null;

            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}sector-map spin", "Get the map for the given sector", $"{BotConfiguration.thisBotConfig.CommandIndicator}sector-map <sector>");
                return;
            }

            string hlp = $"Here is the map you requested:-";
            
            
            string file = GetResponseImage(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Sector_Image].Replace("/sector", $"/{sector}"), sector);

            msg = GetMsg(defaultColor, "Sector Map", hlp, Context.User);
            await Context.Channel.SendFileAsync(file, Context.Message.Content,false, msg.Build());

            File.Delete(file);
        }

        [Command("sector-quad-map")]
        [Summary("Get the map for the given sector's quadrant Alpha, Beta, Gamma or Delta. e.g. [~]sector-quad-map spin alpha")]
        public virtual async Task TM_SectorQuadMap(string sector = null, string quadrant = null)
        {
            EmbedBuilder msg = null;

            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}sector-quad-map spin alpha", "Get the map for the given sector's quadrant Alpha, Beta, Gamma or Delta.", $"{BotConfiguration.thisBotConfig.CommandIndicator}sector-quad-map <sector> <quadrant>");
                return;
            }

            string hlp = "Here is the map you requested:-";

            
            string file = GetResponseImage(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Quadrant_Image].Replace("/sector", $"/{sector}").Replace("/quadrant",$"/{quadrant}"), $"{sector}_{quadrant}");

            msg = GetMsg(defaultColor, "Quadrant Map", hlp, Context.User);
            await Context.Channel.SendFileAsync(file, Context.Message.Content,false,msg.Build());

            File.Delete(file);
        }

        [Command("subsector-map")]
        [Summary("Get the map for the given sector's subsector, e.g. [~]subsector-map spin regina")]
        public virtual async Task TM_SubsectorMap(string sector = null, string subsector = null)
        {

            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}subsector-map spin regina", "Get the map for the given sector's subsector.", $"{BotConfiguration.thisBotConfig.CommandIndicator}subsector-map <sector> <subsector>");
                return;
            }
            string txt = "Here is the map you requested:-";

            
            string file = GetResponseImage(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Subsector_Image].Replace("/sector", $"/{sector}").Replace("/subsector", $"/{subsector}"), $"{sector}_{subsector}");

            EmbedBuilder msg = GetMsg(defaultColor, "Subsector Map", txt, Context.User);
            await Context.Channel.SendFileAsync(file,Context.Message.Content,false,msg.Build());
            File.Delete(file);
        }

        [Command("sector-booklet")]
        [Summary("Get the booklet for the given sector, e.g. [~]sector-booklet spin")]
        public virtual async Task TM_SectorBooklet(string sector = null)
        {
            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}sector-booklet spin", "Get the booklet for the given sector", $"{BotConfiguration.thisBotConfig.CommandIndicator}sector-booklet <sector>");
                return;
            }

            string txt = "Here is the booklet you requested:-";

            

            EmbedBuilder msg = GetMsg("Booklet", txt);
            msg.Url = mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Sector_Booklet].Replace("/sector", $"/{sector}");

            await SendMessageAsync(msg);
        }

        [Command("world-hex")]
        [Summary("Get world data from a given hex in a sector, e.g. [~]world-hex 1910")]
        public virtual async Task TM_WorldHex(string sector = null, string hex = null, bool verbose = true)
        {
            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}world-hex 1910", "Get world data from a given hex in a sector", $"{BotConfiguration.thisBotConfig.CommandIndicator}world-hex <sector> <hex>");
                return;
            }

            EmbedBuilder msg = null;
            

            if (mapService != null)
            {
                try
                {
                    string json = GetResponse(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.World_Data].Replace("/sector", $"/{sector}").Replace("/hex", $"/{hex}"));

                    WorldsHex worlds = DeserializeJSON<WorldsHex>(json);

                    if (worlds.Worlds != null && worlds.Worlds.Count > 0)
                    {
                        if (!verbose)
                        {
                            msg = GetMsg(defaultColor, $"World Found", $"{hex}");
                        }

                        foreach (World world in worlds.Worlds)
                        {
                            if (verbose)
                            {
                                msg = GetWorldMessage(world);

                                await SendMessageAsync(msg);
                            }
                            else
                                GetWorldMessage(world, msg);
                        }

                        if (!verbose)
                            await SendMessageAsync(msg);
                    }                     
                    else                  
                    {                     
                        msg = GetMsg(defaultColor, $"{sector} {hex}", "No wrolds found here..");
                        await SendMessageAsync(msg);
                    }

                }
                catch (Exception ex)
                {
                    msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
                    await SendMessageAsync(msg);
                }
            }
            else
            {
                msg = GetErrorMsg("Service does not use Traveller API", Context.User);
                await SendMessageAsync(msg);
            }

            
        }

        [Command("world-jump")]
        [Summary("Get worlds in a given jump range of a sector hex, e.g. [~]world-jump spin 1910 4\nThis can me made verbose if you put true on the end, e.g. [~]world-jump spin 1910 4 true")]
        public virtual async Task TM_JumpRange(string sector = null, string hex = null, string jump = null, bool verbose = false)
        {
            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}world-jump spin 1910 4", "Get worlds in a given jump range of a sector hex\nThis can me made verbose if you put true on the end, e.g. [~]world-jump spin 1910 4 true" , $"{BotConfiguration.thisBotConfig.CommandIndicator}world-jump <sector> <hex> <range> <verbose>");
                return;
            }


            EmbedBuilder msg = null;
            

            if (mapService != null)
            {
                try
                {
                    string json = GetResponse(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Jump_Range].Replace("/sector", $"/{sector}").Replace("/hex", $"/{hex}").Replace("/range",$"/{jump}"));

                    WorldsHex worlds = DeserializeJSON<WorldsHex>(json);

                    if (worlds.Worlds != null && worlds.Worlds.Count > 0)
                    {
                        if (!verbose)
                        {
                            msg = GetMsg(defaultColor, $"Jump Range of {jump}", $"{worlds.Worlds.Count} Worlds Found");
                        }


                        int cnt = 0;
                        int tcnt = 0;
                        foreach (World world in worlds.Worlds)
                        {
                            if (verbose)
                            {
                                msg = GetWorldMessage(world);

                                await SendMessageAsync(msg);
                            }
                            else
                            {
                                GetWorldMessage(world, msg);
                                cnt++;
                                tcnt++;

                                if (cnt == 5)
                                {
                                    cnt = 0;

                                    await SendMessageAsync(msg, null, $"{tcnt}/{worlds.Worlds.Count} continued...");

                                    msg = GetMsg(defaultColor, $"...", null, null, "...continued");
                                }
                            }
                        }

                        if (!verbose)
                            await SendMessageAsync(msg);
                    }
                    else
                    {
                        msg = GetMsg(defaultColor, $"{sector} {hex}", "No wrolds found in range..");
                        await SendMessageAsync(msg);
                    }

                }
                catch (Exception ex)
                {
                    msg = GetErrorMsg($"Error:\n{ex.Message}", Context.User);
                    await SendMessageAsync(msg);
                }
            }
            else
            {
                msg = GetErrorMsg("Service does not use Traveller API", Context.User);
                await SendMessageAsync(msg);
            }


        }

        [Command("world-jump-map")]
        [Alias(new string[] { "wjm" })]
        [Summary("Get the ranged jump map for the given sector's hex, e.g. [~]world-jump-map spin 1910 4")]
        public virtual async Task TM_JumpMap(string sector = null, string hex = null, string jump = null)
        {
            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}world-jump-map spin 1910 4", "Get the ranged jump map for the given sector's hex", $"{BotConfiguration.thisBotConfig.CommandIndicator}world-jump-map <sector> <hex> <range>");
                return;
            }
            string txt = "Here is the jump map you requested:-";

            
            string file = GetResponseImage(mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.Jump_Image].Replace("/sector", $"/{sector}").Replace("/hex", $"/{hex}").Replace("/range",$"/{jump}"), $"{sector}_{hex}_{jump}");

            EmbedBuilder msg = GetMsg(defaultColor, "Subsector Map", txt, Context.User);
            await Context.Channel.SendFileAsync(file, Context.Message.Content, false, msg.Build());
            File.Delete(file);
        }

        [Command("world-sheet")]
        [Summary("Get the sheet for a world at given hex in a given sector, e.g. [~]world-sheet spin 1910")]
        public virtual async Task TM_WorldSheet(string sector = null, string hex = null)
        {
            if (string.IsNullOrEmpty(sector) || sector == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}world-sheet spin 1910", "Get the sheet for a world at given hex in a given sector", $"{BotConfiguration.thisBotConfig.CommandIndicator}world-sheet <sector> <hex>");
                return;
            }

            string hlp = "Here is the world sheet you requested:-";

            

            EmbedBuilder msg = GetMsg("World Sheet", hlp);
            msg.Url = mapService.TravellerMapAPI[Enums.TravellerAPIMapEnums.World_Sheet].Replace("/sector", $"/{sector}").Replace("/hex", $"/{hex}");

            await SendMessageAsync(msg);
        }

        protected virtual void GetWorldMessage(World world, EmbedBuilder msg)
        {
            msg.AddField("Location", $"[{world.Sector} - {world.Hex}]", true);
            msg.AddField("Name", $"[{world.Name}]", true);
            msg.AddField("Data", $"[{world.UWP} - {world.Zone}]", true);
        }

        protected virtual EmbedBuilder GetWorldMessage(World world)
        {
            EmbedBuilder msg = GetMsg(defaultColor, $"{world.Sector} - {world.Name} - {world.Hex}", world.UWP);

            msg.AddField("PBG", $"[{world.PBG}]", true);
            msg.AddField("Zone", $"[{world.Zone}]", true);
            msg.AddField("Bases", $"[{world.Bases}]", true);
            msg.AddField("Allegiance", $"[{world.Allegiance}]", true);
            msg.AddField("Stellar", $"[{world.Stellar}]", true);
            msg.AddField("SS", $"[{world.SS}]", true);
            msg.AddField("Ix", $"[{world.Ix}]", true);
            msg.AddField("CalculatedImportance", $"[{world.CalculatedImportance}]", true);
            msg.AddField("Ex", $"[{world.Ex}]", true);
            msg.AddField("Cx", $"[{world.Cx}]", true);
            msg.AddField("Nobility", $"[{world.Nobility}]", true);
            msg.AddField("Worlds", $"[{world.Worlds}]", true);
            msg.AddField("ResourceUnits", $"[{world.ResourceUnits}]", true);
            msg.AddField("Subsector", $"[{world.Subsector}]", true);
            msg.AddField("Quadrant", $"[{world.Quadrant}]", true);
            msg.AddField("WorldX", $"[{world.WorldX}]", true);
            msg.AddField("WorldY", $"[{world.WorldY}]", true);
            msg.AddField("Remarks", $"[{world.Remarks}]", true);
            msg.AddField("LegacyBaseCode", $"[{world.LegacyBaseCode}]", true);
            msg.AddField("SubsectorName", $"[{world.SubsectorName}]", true);
            msg.AddField("SectorAbbreviation", $"[{world.SectorAbbreviation}]", true);
            msg.AddField("CxAllegianceName", $"[{world.AllegianceName}]", true);

            return msg;
        }

       

    }
}
