using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using CustomScheduleKeys.Helpers;

namespace CustomScheduleKeys
{
    // See ScheduleKeyData.cs
    public partial class ScheduleKeyData
    {
        public string? Id { get; set; }
        public string? ScheduleKey { get; set; }
        public string? Priority { get; set; }
        public string? Condition { get; set; }
        private string? ModId => TryGetModFromKey()?.Manifest.UniqueID;

        public bool HasValidKey()
        {
            if (ModId is not null) return true;
            
            Log.Error($"ScheduleKeyData with Id '{Id}' has an invalid ScheduleKey value '{ScheduleKey}'. It must be prefixed with the mod's UniqueId.");
            return false;

        }
    }
    
    internal sealed class ModEntry : Mod
    {
        internal static IModHelper ModHelper { get; private set; } = null!;
        internal static IMonitor ModMonitor { get; private set; } = null!;
        private static Harmony Harmony { get; set; } = null!;
        
        public static string? DebugScheduleKey { get; set; }

        private static List<ScheduleKeyData>? _schedules { get; set; }
        private static List<ScheduleKeyData>? _sortedSchedules { get; set; }

        private static List<ScheduleKeyData> Schedules
        {
            get
            {
                return _schedules ??= Game1.content.Load<List<ScheduleKeyData>>($"Spiderbuttons.CustomScheduleKeys/ScheduleKeys");
            }
        }
        internal static List<ScheduleKeyData> SortedSchedules
        {
            get
            {
                return _sortedSchedules ??= Schedules.Where(sched => sched.HasValidKey()).OrderBy(x => x.GetPriority()).ThenBy(sched => ModHelper.ModRegistry.GetAll().ToList().IndexOf(sched.TryGetModFromKey()!)).ToList();
            }
        }

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);

            Harmony.Patch(original: AccessTools.Method(typeof(NPC), nameof(NPC.TryLoadSchedule), []), prefix: new HarmonyMethod(typeof(SchedulePatch), nameof(SchedulePatch.TryLoadSchedule_Prefix)));
            Harmony.Patch(original: AccessTools.Method(typeof(Game1), nameof(Game1.doMorningStuff)), postfix: new HarmonyMethod(typeof(SchedulePatch), nameof(SchedulePatch.doMorningStuff_Postfix)));
            
            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.Content.AssetsInvalidated += OnAssetsInvalidated;

            Helper.ConsoleCommands.Add("csk_force",
                "Force all NPCs to use a specific schedule tomorrow, if they have it.\n\nUsage: csk_force <schedule key>\n- Example: csk_force spring_Mon",
                (_, args) =>
                {
                    if (args.Length < 1)
                    {
                        Log.Error("You must specify a schedule key.");
                        return;
                    }

                    DebugScheduleKey = args[0];
                    Log.Info($"All NPCs will attempt to use schedule key '{DebugScheduleKey}' tomorrow.");
                });
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"Spiderbuttons.CustomScheduleKeys/ScheduleKeys"))
            {
                Log.Trace("Loading custom schedule keys...");
                e.LoadFrom(() => new List<ScheduleKeyData>(), AssetLoadPriority.Medium);
            }
        }

        private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"Spiderbuttons.CustomScheduleKeys/ScheduleKeys")))
            {
                Log.Trace("Invalidating custom schedule keys...");
                _schedules = null;
                _sortedSchedules = null;
            }
        }
    }
}