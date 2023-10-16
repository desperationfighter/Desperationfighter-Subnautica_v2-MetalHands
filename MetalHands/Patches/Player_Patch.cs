using HarmonyLib;
using MetalHands.Items.Equipment;

namespace MetalHands.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.UpdateReinforcedSuit))]
    public static class Player_UpdateReinforcedSuit_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            //additional Protection for the MK2
            if (Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK2Prefab.Info.TechType)
            {
                __instance.temperatureDamage.minDamageTemperature += 2f;
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch(nameof(Player.HasReinforcedGloves))]
    public static class Player_HasReinforcedGloves
    {
        public static void Postfix(Player __instance, ref bool __result)
        {
            if (Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK1Prefab.Info.TechType | Inventory.main.equipment.GetTechTypeInSlot("Gloves") == MetalHandsMK2Prefab.Info.TechType)
            {
                __result = true;
            }
        }
    }
}