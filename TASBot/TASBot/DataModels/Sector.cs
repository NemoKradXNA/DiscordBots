using System;
using System.Collections.Generic;
using System.Text;

namespace TASLibBot.DataModels
{
    public class Sector
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Milieu { get; set; }
        public string Abbreviation { get; set; }
        public string Tags { get; set; }
        public List<Name> Names { get; set; }

        public string GetNames()
        {
            string names = "";

            foreach (Name name in Names)
            {
                names += string.Format("[{0}] - {1}", name.Lang == null  ? "?": name.Lang, name.Text);
            }

            return names;
        }
    }

    public class SectorSurvey
    {
        public string GeneratedBy { get; set; }
        public string DateTime { get; set; }
        public string Title { get; set; }
        public string Coords { get; set; }
        public List<Name> Names { get; set; }

        public string Milieu { get; set; }
        public string Abbreviation { get; set; }
        public List<SubSectorSurvey> SubSectors { get; set; }

        public List<string> Alegences { get; set; }
        public List<WorldSurvey> Worlds { get; set; }

        public SectorSurvey(string data)
        {
            

            string[] lines = data.Split("\r\n");

            GeneratedBy = lines[0].Substring(15);
            DateTime = lines[1].Substring(2);
            Title = lines[3].Substring(2);
            Coords= $"({lines[4].Substring(2)})";

            Names = new List<Name>();
            SubSectors = new List<SubSectorSurvey>();
            Alegences = new List<string>();
            Worlds = new List<WorldSurvey>();

            string key = "";

            bool inWorlds = false;
            foreach (string line in lines)
            {try
                {
                    if (!inWorlds)
                    {
                        if (line.Contains("# Milieu: "))
                        {
                            Milieu = line.Substring(10);
                        }

                        if (line.Contains("# Name: "))
                        {
                            Name thisName = new Name();

                            if (line.Contains("(")) // we have a language
                            {
                                int start = line.IndexOf("(") + 1;
                                int len = line.IndexOf(")", start) - start;

                                thisName.Lang = line.Substring(start, len);
                                thisName.Text = line.Substring(8, start - 8).Trim();
                            }
                            else
                                thisName.Text = line.Substring(8).Trim();

                            Names.Add(thisName);
                        }

                        if (line.Contains("# Subsector "))
                        {
                            SubSectors.Add(new SubSectorSurvey(line));
                        }

                        if (line.Contains("# Alleg: "))
                        {
                            Alegences.Add(line.Substring(9));
                        }
                                          
                        if (line.Contains("Hex  Name"))
                        {
                            inWorlds = true;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (line.Substring(0, 4) == "----")
                                key = line;

                            if (line.Substring(0, 4) != "----")
                                Worlds.Add(new WorldSurvey(line, key));
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public string GetNames()
        {
            string names = "";

            foreach (Name name in Names)
            {
                names += string.Format("[{0}] - {1}", name.Lang == null ? "?" : name.Lang, name.Text);
            }

            return names;
        }

        public string GetAlegancies()
        {
            string ret = "";

            foreach (string al in Alegences)
            {
                ret += $"{al}\n";
            }

            if (string.IsNullOrEmpty(ret))
                ret = "None.";

            return ret;
        }

        public string GetSubSectors()
        {
            string subs = "";

            foreach (SubSectorSurvey sub in SubSectors)
            {
                if (!string.IsNullOrEmpty(sub.Name))
                    subs += $"Subsector {sub.Letter} - {sub.Name}\n";
                else
                    subs += $"Subsector {sub.Letter} - \n";
            }

            return subs;
        }

        public string GetWorlds()
        {
            string ret = "";

            foreach (WorldSurvey world in Worlds)
            {
                ret += $"{world.Hex} - {world.Name} - {world.UWP}\n";
            }

            return ret;
        }
    }

    public class SubSectorSurvey
    {
        public string Letter { get; set; }
        public string Name { get; set; }

        public List<WorldSurvey> Worlds { get; set; }

        public SubSectorSurvey(string data)
        {

            if (data.Contains("# Subsector"))
            {
                Letter = data.Substring(12, 1);

                if (data.Length > 15)
                    Name = data.Substring(15);
            }
            else
            {
                string[] lines = data.Split("\r\n");
                string key = lines[1];

                Worlds = new List<WorldSurvey>();

                for (int l = 2; l < lines.Length; l++)
                {
                    if(!string.IsNullOrEmpty(lines[l]))
                        Worlds.Add(new WorldSurvey(lines[l], key));
                }
            }
        }
    }

    public class WorldSurvey
    {
        public string Hex { get; set; }
        public string Name { get; set; }
        public string UWP { get; set; }
        public string Remarks { get; set; }
        public string Ix { get; set; }
        public string Ex { get; set; }
        public string Cx { get; set; }
        public string N { get; set; }
        public string B { get; set; }
        public string Z { get; set; }
        public string PBG { get; set; }
        public string W { get; set; }
        public string A { get; set; }
        public string Stellar { get; set; }

        public WorldSurvey(string data, string key)
        {
            try
            {
                //0101 Zeycude              C430698-9 De Na Ni Po                              { -1 } (C53-1) [6559] -     -  - 613 8  ZhIN K9 V  
                //---- -------------------- --------- ---------------------------------------- ------ ------- ------ ----- -- - --- -- ---- --------------

                string[] lengths = key.Split(" ");

                int start = 0;
                int len = lengths[0].Length;

                Hex = data.Substring(start, len);

                start += len + 1;
                len = lengths[1].Length;
                Name = data.Substring(start, len).Trim();

                start += len + 1;
                len = lengths[2].Length;
                UWP = data.Substring(start, len);

                start += len + 1;
                len = lengths[3].Length;
                Remarks = data.Substring(start, len).Trim();

                start += len + 1;
                len = lengths[4].Length;
                Ix = data.Substring(start, len);

                start += len + 1;
                len = lengths[5].Length;
                Ex = data.Substring(start, len);

                start += len + 1;
                len = lengths[6].Length;
                Cx = data.Substring(start, len);

                start += len + 1;
                len = lengths[7].Length;
                N = data.Substring(start, len).Trim();

                start += len + 1;
                len = lengths[8].Length;
                B = data.Substring(start, len).Trim();

                start += len + 1;
                len = lengths[9].Length;
                Z = data.Substring(start, len);

                start += len + 1;
                len = lengths[10].Length;
                PBG = data.Substring(start, len);

                start += len + 1;
                len = lengths[11].Length;
                W = data.Substring(start, len).Trim();

                start += len + 1;
                len = lengths[12].Length;
                A = data.Substring(start, len);

                start += len + 1;
                len = lengths[13].Length;
                Stellar = data.Substring(start).Trim();
            }
            catch
            {

            }
        }
    }

    public class WorldsHex
    {
        public List<World> Worlds { get; set; }

        public WorldsHex() { }
    }

    public class World
    {
        public string Name { get; set; }
        public string Hex { get; set; }
        public string UWP { get; set; }
        public string PBG { get; set; }
        public string Zone { get; set; }
        public string Bases { get; set; }
        public string Allegiance { get; set; }
        public string Stellar { get; set; }
        public string SS { get; set; }
        public string Ix { get; set; }
        public int CalculatedImportance { get; set; }
        public string Ex { get; set; }
        public string Cx { get; set; }
        public string Nobility { get; set; }
        public int Worlds { get; set; }
        public int ResourceUnits { get; set; }
        public int Subsector { get; set; }
        public int Quadrant { get; set; }
        public int WorldX { get; set; }
        public int WorldY { get; set; }
        public string Remarks { get; set; }
        public string LegacyBaseCode { get; set; }
        public string Sector { get; set; }
        public string SubsectorName { get; set; }
        public string SectorAbbreviation { get; set; }
        public string AllegianceName { get; set; }

        public World() { }
    }
}
