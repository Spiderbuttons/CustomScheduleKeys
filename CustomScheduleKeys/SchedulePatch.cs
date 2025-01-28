using System;
using CustomScheduleKeys.Helpers;
using StardewValley;

namespace CustomScheduleKeys;

public class SchedulePatch
{
    public static bool TryLoadSchedule_Prefix(NPC __instance, ref bool __result)
    {
        try
        {
            if (ModEntry.DebugScheduleKey is not null && __instance.TryLoadSchedule(ModEntry.DebugScheduleKey))
            {
                Log.Debug($"Using forced schedule key '{ModEntry.DebugScheduleKey}' for NPC '{__instance.Name}'");
                __result = true;
                return false;
            }
            
            foreach (var sched in ModEntry.SortedSchedules)
            {
                if (GameStateQuery.CheckConditions(sched.Condition, location: __instance.currentLocation) && __instance.TryLoadSchedule(sched.ScheduleKey))
                {
                    Log.Trace($"Custom schedule key '{sched.ScheduleKey}' applied to NPC '{__instance.Name}'");
                    __result = true;
                    return false;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Error loading custom schedule keys for NPC '{__instance.Name}', reverting to vanilla behaviour: {ex}");
            return true;
        }
    }

    public static void doMorningStuff_Postfix()
    {
        ModEntry.DebugScheduleKey = null;
    }
}