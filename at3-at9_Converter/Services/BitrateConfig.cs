using System.Collections.Generic;

namespace at3_at9_Converter.Services
{
    public class BitrateConfig
    {
        public string[] ConsoleListAt3 { get; } = new string[] { "PSP", "PS3" };
        public string[] ConsoleListAt9 { get; } = new string[] { "PS4", "PSVita" };

        public string[] PspList { get; } = new string[] { "32", "48", "52", "64", "66", "96", "105", "128", "132", "160", "192", "256", "320", "352" };
        public string[] Ps3List { get; } = new string[] { "32", "48", "57", "64", "72", "96", "114", "128", "144", "160", "192", "256", "320", "384", "512", "768" };
        public string[] PsvitaList { get; } = new string[] { "36", "48", "60", "72", "84", "96", "120", "144", "168", "192" };
        public string[] Ps4List { get; } = new string[] { "36", "48", "60", "72", "84", "96", "120", "144", "168", "192", "240", "288", "300", "384", "336", "360", "384", "420", "480", "504", "672" };

        public Dictionary<string, int> At3ChannelMap { get; } = new Dictionary<string, int>
        {
            { "32", 1 }, { "48", 1 }, { "52", 1 }, { "64", 1 }, { "66", 1 },
            { "96", 2 }, { "105", 2 }, { "128", 2 }, { "132", 2 }, { "160", 2 },
            { "192", 2 }, { "256", 2 }, { "320", 2 }, { "352", 2 }
        };

        public Dictionary<string, int> At9ChannelMap { get; } = new Dictionary<string, int>
        {
            { "36", 1 }, { "48", 1 }, { "60", 1 }, { "72", 1 }, { "84", 1 },
            { "96", 2 }, { "120", 2 }, { "144", 2 }, { "168", 2 }, { "192", 2 },
            { "240", 2 }, { "288", 2 }, { "300", 2 }, { "336", 2 }, { "360", 2 },
            { "384", 2 }, { "420", 2 }, { "480", 2 }, { "504", 2 }, { "672", 2 }
        };

        public int GetAt3Channels(string bitrate)
        {
            return At3ChannelMap.ContainsKey(bitrate) ? At3ChannelMap[bitrate] : 2;
        }

        public int GetAt9Channels(string bitrate)
        {
            return At9ChannelMap.ContainsKey(bitrate) ? At9ChannelMap[bitrate] : 2;
        }
    }
}
