﻿using System;
using CustomScheduleKeys.Helpers;
using StardewModdingAPI.Events;
using StardewValley;

namespace CustomScheduleKeys;

public partial class ScheduleData
{
    public int GetPriority()
    {
        /* ---------------------------------------------------------------------------

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

        --------------------------------------------------------------------------- */
        
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
        Log.Error($"ScheduleData with Id '{this.Id}' has an invalid priority value '{this.Priority}.' It must be one of [{string.Join(", ", Enum.GetNames(typeof(AssetEditPriority)))}] or a priority with an optional sign and offset (e.g. 'Late + 10').");
        return 0;
    }
}