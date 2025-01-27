using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using CustomScheduleKeys.Helpers;
using StardewValley.Extensions;

namespace CustomScheduleKeys
{
    // See ScheduleData.cs
    public partial class ScheduleData
    {
        public string? Id { get; set; }
        public string? ScheduleKey { get; set; }
        public string? Priority { get; set; }
        public string? Condition { get; set; }
        private string? ModId => Utils.TryGetModFromString(ScheduleKey)?.Manifest.UniqueID;

        public bool HasValidKey()
        {
            if (ModId is not null) return true;
            
            Log.Error($"ScheduleData with Id '{Id}' has an invalid ScheduleKey value '{ScheduleKey}'. It must be prefixed with the mods UniqueId.");
            return false;

        }
    }
    
    internal sealed class ModEntry : Mod
    {
        internal static IModHelper ModHelper { get; private set; } = null!;
        internal static IMonitor ModMonitor { get; private set; } = null!;
        private static Harmony Harmony { get; set; } = null!;
        
        internal static List<ScheduleData>? _schedules { get; private set; } = null;
        internal static List<ScheduleData> Schedules
        {
            get
            {
                return _schedules ??= Game1.content.Load<List<ScheduleData>>($"Spiderbuttons.CustomScheduleKeys/Schedules");
            }
        }
        internal static List<ScheduleData> SortedSchedules
        {
            get
            {
                return Schedules.Where(sched => sched.HasValidKey()).OrderBy(sched => ModHelper.ModRegistry.GetAll().ToList().IndexOf(Utils.TryGetModFromString(sched.ScheduleKey)!)).ThenBy(x => x.GetPriority()).ToList();
            }
        }

        public override void Entry(IModHelper helper)
        {
            ModHelper = helper;
            ModMonitor = Monitor;
            Harmony = new Harmony(ModManifest.UniqueID);

            Harmony.PatchAll();

            Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            Helper.Events.Content.AssetRequested += this.OnAssetRequested;
            Helper.Events.Content.AssetsInvalidated += this.OnAssetsInvalidated;
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo($"Spiderbuttons.CustomScheduleKeys/Schedules"))
            {
                Log.Debug("Loading...");
                e.LoadFrom(() => new List<ScheduleData>(), AssetLoadPriority.Medium);
            }
        }

        private void OnAssetsInvalidated(object? sender, AssetsInvalidatedEventArgs e)
        {
            if (e.NamesWithoutLocale.Any(name => name.IsEquivalentTo($"Spiderbuttons.CustomScheduleKeys/Schedules")))
            {
                Log.Debug("Invalidating...");
                _schedules = null;
            }
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            // if (!Context.IsWorldReady)
            //     return;

            if (e.Button is SButton.F2)
            {
                Helper.GameContent.InvalidateCache("Spiderbuttons.CustomScheduleKeys/Schedules");
                Log.Debug(Schedules.Count);
                foreach (var schedule in Schedules)
                {
                    // log all the fields
                    Log.Debug($"Id: {schedule.Id}");
                    Log.Debug($"ScheduleKey: {schedule.ScheduleKey}");
                    Log.Debug($"Priority: {schedule.Priority} (Parsed: {schedule.GetPriority()})");
                    Log.Debug($"Condition: {schedule.Condition}");
                    Log.Debug("--------------------");
                }
            }
        }
    }
}