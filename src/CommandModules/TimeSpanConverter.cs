using System.Globalization;
using System.Text.RegularExpressions;
using DSharpPlus.Entities;

namespace StickBot.CommandModules;

public static class TimeSpanConverter
{
    // Adapted from the internal DSharpPlus library, licensed under MIT
    // 
    // Copyright (c) 2015 Mike Santiago
    // Copyright (c) 2016-2022 DSharpPlus Development Team
    
    private static Regex TimeSpanRegex { get; set; }

    static TimeSpanConverter()
    {
        TimeSpanRegex = new Regex(@"^(?<days>\d+d)?\s*(?<hours>\d{1,2}h)?\s*(?<minutes>\d{1,2}m)?\s*(?<seconds>\d{1,2}s)?\s*$", 
            RegexOptions.ECMAScript | RegexOptions.Compiled);
    }

    public static TimeSpan? Convert(string value)
    {
        if (value == "0")
            return TimeSpan.Zero;

        if (int.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out _))
            return null;

        value = value.ToLowerInvariant();

        if (TimeSpan.TryParse(value, CultureInfo.CurrentCulture, out var result))
            return result;

        var gps = new string[] { "days", "hours", "minutes", "seconds" };
        var mtc = TimeSpanRegex.Match(value);
        if (!mtc.Success)
            return null;

        var d = 0;
        var h = 0;
        var m = 0;
        var s = 0;
        foreach (var gp in gps)
        {
            var gpc = mtc.Groups[gp].Value;
            if (string.IsNullOrWhiteSpace(gpc))
                continue;

            var gpt = gpc[gpc.Length - 1];
            int.TryParse(gpc.Substring(0, gpc.Length - 1), NumberStyles.Integer, CultureInfo.CurrentCulture, out var val);
            switch (gpt)
            {
                case 'd':
                    d = val;
                    break;

                case 'h':
                    h = val;
                    break;

                case 'm':
                    m = val;
                    break;

                case 's':
                    s = val;
                    break;
            }
        }
        result = new TimeSpan(d, h, m, s);
        return result;
    }
}