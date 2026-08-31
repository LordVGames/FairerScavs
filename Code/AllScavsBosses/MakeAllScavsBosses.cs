using MonoDetour;
using MonoDetour.HookGen;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
namespace FairerScavs.AllScavsBosses;


[MonoDetourTargets]
internal static class MakeAllScavsBosses
{
    private static readonly AssetReferenceT<GameObject> _bossCombatSquadReference = new(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Core.BossCombatSquad_prefab);
    private static GameObject _bossCombatSquad;


    [MonoDetourHookInitialize]
    private static void Setup()
    {
        AssetAsyncReferenceManager<GameObject>.LoadAsset(_bossCombatSquadReference).Completed += (handle) =>
        {
            _bossCombatSquad = handle.Result;
        };
        CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
    }


    private static void CharacterBody_onBodyStartGlobal(CharacterBody body)
    {
        if (
            !ConfigOptions.AllScavsAreBosses.MakeAllScavsBosses.Value
            || body == null
            || body.isPlayerControlled
            || !Main.IsBodyAScavenger(body)
        )
        {
            return;
        }


        // ty ss2 for a reference on how to do this
        CombatSquad squad = GameObject.Instantiate(_bossCombatSquad)?.GetComponent<CombatSquad>();
        if (squad)
        {
            squad.AddMember(body.master);
        }
        NetworkServer.Spawn(squad.gameObject);
    }
}