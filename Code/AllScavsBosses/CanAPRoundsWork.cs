using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
namespace FairerScavs.AllScavsBosses;


[MonoDetourTargets(typeof(HealthComponent))]
internal static class CanAPRoundsWork
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.HealthComponent.TakeDamageProcess.ILHook(CheckForBossScavs);
    }


    private static void CheckForBossScavs(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        ILLabel skipItemEffect = w.DefineLabel();


        w.MatchRelaxed(
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<HealthComponent>("body"),
            x => x.MatchCallOrCallvirt<CharacterBody>("get_isBoss"),
            x => x.MatchBrfalse(out skipItemEffect) && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((HealthComponent healthComponent) =>
            {
                if (healthComponent == null || healthComponent.body == null || !Main.IsBodyAScavenger(healthComponent.body))
                {
                    return true;
                }


                return ConfigOptions.AllScavsAreBosses.LetAPRoundsWorkOnScavengers.Value;
            }),
            w.Create(OpCodes.Brfalse, skipItemEffect)
        );
    }
}