using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
namespace FairerScavs.FindItemsSkill;


[MonoDetourTargets(typeof(EntityStates.ScavMonster.GrantItem))]
internal static class MakeFoundItemsTemporary
{
    private static readonly AssetReferenceT<ExpansionDef> _dlc3AssetReference = new(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3.DLC3_asset);
    internal static ExpansionDef Dlc3;


    [MonoDetourHookInitialize]
    private static void Setup()
    {
        AssetAsyncReferenceManager<ExpansionDef>.LoadAsset(_dlc3AssetReference).Completed += (handle) =>
        {
            Dlc3 = handle.Result;
        };
        Mdh.EntityStates.ScavMonster.GrantItem.GrantPickupServer.ILHook(GiveTempInsteadOfPermanent);
    }


    private static bool CanItemsBecomeTemporary()
    {
        return ConfigOptions.FindItemSkill.MakeFoundItemsTemporary.Value && Run.instance.IsExpansionEnabled(Dlc3);
    }


    private static void GiveTempInsteadOfPermanent(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.DeclareVariable(typeof(Inventory), out var inventoryVariable);
        w.DeclareVariable(typeof(ItemIndex), out var itemIndexVariable);
        w.DeclareVariable(typeof(int), out var itemStackCountVariable);
        Instruction startOfSkip = null!;
        Instruction endOfSkip = null!;


        w.MatchRelaxed(
            x => x.MatchCallOrCallvirt<CharacterBody>("get_inventory") && w.SetCurrentTo(x),
            x => x.MatchLdloc(1), // itemIndex
            x => x.MatchLdarg(2), // countToGrant
            x => x.MatchCallOrCallvirt<Inventory>("GiveItemPermanent")
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Stloc, inventoryVariable),
            w.Create(OpCodes.Ldloc, inventoryVariable)
        )
        .CurrentToNext()
        .InsertAfterCurrent(
            w.Create(OpCodes.Stloc, itemIndexVariable),
            w.Create(OpCodes.Ldloc, itemIndexVariable)
        )
        .CurrentToNext()
        .InsertAfterCurrent(
            w.Create(OpCodes.Stloc, itemStackCountVariable),
            w.Create(OpCodes.Ldloc, itemStackCountVariable),
            w.CreateDelegateCall((Inventory inventory, ItemIndex itemIndex, int stacks) =>
            {
                if (CanItemsBecomeTemporary())
                {
                    for (int i = 0; i < stacks; i++)
                    {
                        inventory.GiveItemTemp(itemIndex);
                    }
                }
            }),
            w.Create(OpCodes.Ldloc, inventoryVariable),
            w.Create(OpCodes.Ldloc, itemIndexVariable),
            w.Create(OpCodes.Ldloc, itemStackCountVariable)
        )
        .MatchRelaxed(
            x => x.MatchLdloc(inventoryVariable.Index) && w.SetInstructionTo(ref startOfSkip, x),
            x => x.MatchLdloc(itemIndexVariable.Index),
            x => x.MatchLdloc(itemStackCountVariable.Index),
            x => x.MatchCallOrCallvirt<Inventory>("GiveItemPermanent") && w.SetInstructionTo(ref endOfSkip, x)
        ).ThrowIfFailure()
        .InsertBranchOverIfTrue(startOfSkip, endOfSkip, w.CreateDelegateCall(() =>
        {
            return CanItemsBecomeTemporary();
        }));
    }
}