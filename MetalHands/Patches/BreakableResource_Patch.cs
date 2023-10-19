using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;
using UWE;
using System.Diagnostics;
using MetalHands.Items.Equipment;

namespace MetalHands.Patches
{
    [HarmonyPatch(typeof(BreakableResource))]
    [HarmonyPatch(nameof(BreakableResource.HitResource))]
    public static class BreakableResource_HitResource_Patch
    {
        [HarmonyPostfix]
        private static void Postfix(BreakableResource __instance)
        {
            HitResource_Patch_Postfix(__instance);
        }

        private static void HitResource_Patch_Postfix(BreakableResource __instance)
        {
            //as this is Postfix first check if player already hit it down so we do not call the break twice
            if (__instance.hitsToBreak > 0)
            {
                //check if user force fastbreak OR has one of the Glove's
                if (Plugin.Config.Config_fastbreak == true | (Plugin.Config.Config_ModEnable == true && ((Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK1Prefab.Info.TechType) | (Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK2Prefab.Info.TechType))))
                {
                    __instance.hitsToBreak = 0;
                    __instance.BreakIntoResources();
                }
            }
        }
    }

    [HarmonyPatch(typeof(BreakableResource))]
    [HarmonyPatch(nameof(BreakableResource.BreakIntoResources))]
    public static class BreakableResource_BreakIntoResources_Patch
    {

        [HarmonyPrefix]
        private static bool Prefix(BreakableResource __instance)
        {
            // Get call stack
            StackTrace stackTrace = new StackTrace();
            // Get calling method name
            string callingmethode = stackTrace.GetFrame(1).GetMethod().Name;

            //if(callingmethode == "PunchRock" | callingmethode == "OnImpact") //OnInpact does not interact with the methode anymore
            if (callingmethode == "PunchRock")
            {
                return true;
            }
            else
            {
                BreakIntoResources_Patch(__instance);
                return false;
            }
        }

        private static void BreakIntoResources_Patch(BreakableResource __instance)
        {
            __instance.SendMessage("OnBreakResource", null, SendMessageOptions.DontRequireReceiver);
            if (__instance.gameObject.GetComponent<VFXBurstModel>())
            {
                __instance.gameObject.BroadcastMessage("OnKill");
            }
            else
            {
                UnityEngine.Object.Destroy(__instance.gameObject);
            }
            if (__instance.customGoalText != "")
            {
                GoalManager.main.OnCustomGoalEvent(__instance.customGoalText);
            }
            bool flag = false;
            for (int i = 0; i < __instance.numChances; i++)
            {
                AssetReferenceGameObject assetReferenceGameObject = __instance.ChooseRandomResource();
                if (assetReferenceGameObject != null)
                {
                    if (Player.main.GetVehicle() is Exosuit exosuit)
                    {
                        var installedmodule = exosuit.modules.GetCount(MetalHandsClawModulePrefab.Info.TechType);
                        if (((installedmodule > 0) | (Plugin.Config.Config_fastcollect == true)) & exosuit.storageContainer.container.HasRoomFor(1, 1))
                        {
                            CoroutineHost.StartCoroutine(AddtoPrawn(__instance, exosuit, assetReferenceGameObject));
                        }
                        else
                        {
                            __instance.SpawnResourceFromPrefab(assetReferenceGameObject);
                        }
                    }
                    else
                    {
                        Inventory inventory = Inventory.Get();
                        if (((Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK2Prefab.Info.TechType) | (Plugin.Config.Config_fastcollect == true)) & inventory.HasRoomFor(1, 1))
                        {
                            CoroutineHost.StartCoroutine(AddbrokenRestoPlayerInv(__instance, assetReferenceGameObject));
                        }
                        else
                        {
                            __instance.SpawnResourceFromPrefab(assetReferenceGameObject);
                        }
                    }
                    flag = true;
                }
            }
            if (!flag)
            {
                if (Player.main.GetVehicle() is Exosuit exosuit)
                {
                    var installedmodule = exosuit.modules.GetCount(MetalHandsClawModulePrefab.Info.TechType);
                    if (((installedmodule > 0) | (Plugin.Config.Config_fastcollect == true)) && exosuit.storageContainer.container.HasRoomFor(1, 1))
                    {
                        CoroutineHost.StartCoroutine(AddtoPrawn(__instance, exosuit, __instance.defaultPrefabReference));
                    }
                    else
                    {
                        __instance.SpawnResourceFromPrefab(__instance.defaultPrefabReference);
                    }
                }
                else
                {
                    Inventory inventory = Inventory.Get();
                    if (((Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK2Prefab.Info.TechType) | (Plugin.Config.Config_fastcollect == true)) & inventory.HasRoomFor(1, 1))
                    {
                        CoroutineHost.StartCoroutine(AddbrokenRestoPlayerInv(__instance, __instance.defaultPrefabReference));
                    }
                    else
                    {
                        __instance.SpawnResourceFromPrefab(__instance.defaultPrefabReference);
                    }
                }

            }
            FMODUWE.PlayOneShot(__instance.breakSound, __instance.transform.position, 1f);
            if (__instance.hitFX)
            {
                global::Utils.PlayOneShotPS(__instance.breakFX, __instance.transform.position, Quaternion.Euler(new Vector3(270f, 0f, 0f)), null);
            }
        }

        private static IEnumerator AddtoPrawn(BreakableResource __instance, Exosuit exosuit, AssetReferenceGameObject gameObject)
        {
            CoroutineTask<GameObject> task = AddressablesUtility.InstantiateAsync(gameObject.RuntimeKey as string);
            yield return task;

            GameObject prefab = task.GetResult();
            var pickupable = prefab.GetComponent<Pickupable>();

            pickupable.Initialize();
            var item = new InventoryItem(pickupable);
            exosuit.storageContainer.container.UnsafeAdd(item);

            string name = Language.main.GetOrFallback(pickupable.GetTechType().AsString(), pickupable.GetTechType().AsString());
            ErrorMessage.AddMessage(Language.main.GetFormat("VehicleAddedToStorage", name));
            uGUI_IconNotifier.main.Play(pickupable.GetTechType(), uGUI_IconNotifier.AnimationType.From, null);
            pickupable.PlayPickupSound();

            yield break;
        }

        private static IEnumerator AddbrokenRestoPlayerInv(BreakableResource __instance, AssetReferenceGameObject gameObject)
        {
            CoroutineTask<GameObject> task = AddressablesUtility.InstantiateAsync(gameObject.RuntimeKey as string);
            yield return task;

            GameObject prefab = task.GetResult();
            var pickupable = prefab.GetComponent<Pickupable>();

            pickupable.Initialize();
            CraftData.AddToInventory(pickupable.GetTechType());

            yield break;
        }
    }
}