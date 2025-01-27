using StardewModdingAPI;

namespace CustomScheduleKeys.Helpers;

public class Utils
{
    public static IModInfo? TryGetModFromString(string? id)
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
        
        if (string.IsNullOrWhiteSpace(id)) return null;
        string[] parts = id.Split('_');
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