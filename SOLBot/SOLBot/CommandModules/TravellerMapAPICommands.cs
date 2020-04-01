using System.Threading.Tasks;
using Discord;
using Discord.Commands;

using SOLBot.BaseClasses;

namespace SOLBot.CommandModules
{
    public class TravellerMapAPICommands : CommandModuleBase
    {
        public TravellerMapAPICommands() : base() { defaultColor = new Color(0, 71, 171); }

        [Command("travellermap")]
        [Alias(new string[] { "tm", "map" })]
        [Summary("This command lists ALL Traveller Map API commands")]
        public async Task TravellerMap()
        {
            string hlp = "Here is the definitive list of Traveller Map Command available to you:-";

            EmbedBuilder Msg = GetMsg(defaultColor, $"Traveller Map API Commands List", hlp);

            Msg.AddField("Universe", "Get a lsit of ALL sectors by using\n~universe", true);

            await Context.Channel.SendMessageAsync("", false, Msg.Build());
        }
    }
}
