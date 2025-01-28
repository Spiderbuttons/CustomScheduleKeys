using System;
using CustomScheduleKeys.Helpers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomScheduleKeys;

public partial class ScheduleKeyData
{
    /* -----------------------------------------------------------------------------------

        THE FOLLOWING LICENSE APPLIES TO THE CODE IN THIS FILE:

        The MIT License (MIT)

        Copyright 2016–2021 Jesse Plamondon-Willard (Pathoschild)

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.

    ----------------------------------------------------------------------------------- */
    
    public int GetPriority()
    {
        if (string.IsNullOrWhiteSpace(this.Priority)) return 0;
        if (Utility.TryParseEnum(this.Priority, out AssetEditPriority parsed))
        {
            return (int)parsed;
        }
        int splitAt = this.Priority.IndexOfAny(['-', '+']);
        if (splitAt > 0)
        {
            string rawPriority = this.Priority.Substring(0, splitAt);
            char rawSign = this.Priority[splitAt];
            string priority = this.Priority;
            int num = splitAt + 1;
            string rawOffset = priority.Substring(num, priority.Length - num);
            if (Utility.TryParseEnum(rawPriority, out parsed) && int.TryParse(rawOffset, out var offset))
            {
                if (rawSign == '-') offset *= -1;
                return ((int)parsed + offset);
            }
        }
        Log.Error($"ScheduleKeyData with Id '{Id}' has an invalid priority value '{Priority}.' It must be one of [{string.Join(", ", Enum.GetNames(typeof(AssetEditPriority)))}] or a priority with an optional sign and offset (e.g. 'Late + 10').");
        return 0;
    }
    
    public IModInfo? TryGetModFromKey()
    {
        if (string.IsNullOrWhiteSpace(ScheduleKey)) return null;
        string[] parts = ScheduleKey.Split('_');
        if (parts.Length == 1) return null;

        string modId = parts[0];
        int idIndex = parts.Length - 1;
        for (int i = 0; i < idIndex; i++)
        {
            if (i != 0) modId += '_' + parts[i];

            IModInfo? mod = ModEntry.ModHelper.ModRegistry.Get(modId);
            if (mod != null) return mod;
        }

        return null;
    }
}