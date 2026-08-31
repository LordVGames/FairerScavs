using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using MonoDetour.HookGen;
using MonoDetour;
using RoR2.CharacterAI;
namespace FairerScavs.FindItemsSkill;


[MonoDetourTargets]
internal static class ScavSkillEdit
{
    private static readonly AssetReferenceT<SkillDef> _sitSkillDefReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Scav.Sit_asset);


    [MonoDetourHookInitialize]
    internal static void MakeFindItemSkillFairer()
    {
        if (ConfigOptions.FindItemSkill.StockCount.Value < 0)
        {
            return;
        }


        AssetAsyncReferenceManager<SkillDef>.LoadAsset(_sitSkillDefReference).Completed += (handle) =>
        {
            SkillDef sitSkillDef = handle.Result;
            int stockCount = (int)ConfigOptions.FindItemSkill.StockCount.Value;
            if (stockCount > -1)
            {
                sitSkillDef.baseMaxStock = stockCount;
            }
            if (ConfigOptions.FindItemSkill.RemoveRecharge.Value)
            {
                sitSkillDef.baseRechargeInterval = -1;
                sitSkillDef.rechargeStock = 0;
            }
        };
        CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
    }


    private static void CharacterMaster_onStartGlobal(CharacterMaster characterMaster)
    {
        if (
            characterMaster.name != "ScavMaster(Clone)"
            || !characterMaster.TryGetComponent<BaseAI>(out var scavAI)
            || scavAI.skillDrivers.Length < 1
        )
        {
            return;
        }


        foreach (AISkillDriver skillDriver in scavAI.skillDrivers)
        {
            if (skillDriver.customName != "Sit")
            {
                continue;
            }


            skillDriver.maxUserHealthFraction = ConfigOptions.FindItemSkill.MaxHealthUsagePercent.Value;
        }
    }
}