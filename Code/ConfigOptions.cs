using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using MiscFixes.Modules;
namespace FairerScavs;


public static class ConfigOptions
{
    internal static class FindItemSkill
    {
        private const string _sectionName = "Find item skill";
        public static ConfigEntry<float> StockCount;
        public static ConfigEntry<bool> RemoveRecharge;
        public static ConfigEntry<float> MaxHealthUsagePercent;
        public static ConfigEntry<bool> MakeFoundItemsTemporary;


        internal static void BindConfigOptions(ConfigFile config)
        {
            // have to bind with this being a float to set the minimum as -1
            // ugh
            StockCount = config.BindOptionSteppedSlider(
                _sectionName,
                "Stock count",
                "The max amount of times a scavenger can find items in it's bag. Set to -1 for no change.",
                3,
                1,
                -1, 100,
                Extensions.ConfigFlags.RestartRequired
            );
            RemoveRecharge = config.BindOption(
                _sectionName,
                "No skill recharge",
                "Should the skill be unable to recharge?",
                true,
                Extensions.ConfigFlags.RestartRequired
            );
            MaxHealthUsagePercent = config.BindOptionSteppedSlider(
                _sectionName,
                "Usable below this percent hp",
                "Should the Scavengers find items skill only be usable below a certain health percentage? Vanilla is at 50%.",
                0.5f,
                0.01f,
                0.01f, 100
            );
            MakeFoundItemsTemporary = config.BindOption(
                _sectionName,
                "Make found items temporary",
                "Should the items obtained from the Scavengers find items skill all be temporary?",
                true
            );
        }
    }


    internal static class AllScavsAreBosses
    {
        private const string _sectionName = "Boss health bars";
        public static ConfigEntry<bool> MakeAllScavsBosses;
        public static ConfigEntry<bool> LetAPRoundsWorkOnScavengers;


        internal static void BindConfigOptions(ConfigFile config)
        {
            MakeAllScavsBosses = config.BindOption(
                _sectionName,
                "Each scav has a boss bar",
                "Should every scavenger have their own boss bar when spawned? This is meant to be used alongside the ShowBossInventory mod so you can see every scavengers inventory.",
                true
            );
            LetAPRoundsWorkOnScavengers = config.BindOption(
                _sectionName,
                "Should AP rounds work",
                "Every scavenger having a boss bar would mean they all take the damage increase from armor piercing rounds. Disable this if you want armor piercing rounds to not work on scavengers (includes ones from the teleporter!)",
                true
            );
        }
    }


    internal static void BindAllConfigOptions(ConfigFile config)
    {
        AllScavsAreBosses.BindConfigOptions(config);
        FindItemSkill.BindConfigOptions(config);
    }
}
