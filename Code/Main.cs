using Mono.Cecil.Cil;
using MonoDetour.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace FairerScavs;


internal static class Main
{
    internal static void LogILInstructions(this ILWeaver w)
    {
        foreach (Instruction instruction in w.Instructions)
        {
            Log.Warning(instruction);
        }
    }


    internal static bool IsBodyAScavenger(CharacterBody body)
    {
        // would use a switch statement if i could but bodyIndexes aren't constant values!!!!!!!!!!!!!
        return body.bodyIndex == RoR2Content.BodyPrefabs.ScavBody.bodyIndex
            || body.bodyIndex == RoR2Content.BodyPrefabs.ScavLunar1Body.bodyIndex
            || body.bodyIndex == RoR2Content.BodyPrefabs.ScavLunar2Body.bodyIndex
            || body.bodyIndex == RoR2Content.BodyPrefabs.ScavLunar3Body.bodyIndex
            || body.bodyIndex == RoR2Content.BodyPrefabs.ScavLunar4Body.bodyIndex;
    }
}