using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using static CraftData;

namespace MetalHands.Items.Equipment
{
    public static class MetalHandsClawModulePrefab
    {
        public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("MetalHandsClawModule",
            "Grav Hand Plugin",
            "This Plugin intregates a Gravitations function to the PRAWN Hands and allow a shortrange Magnetic Ressource Collection.")
        .WithIcon(GetItemSprite())
        .WithSizeInInventory(new Vector2int(1, 1));

        public static string IconFileName => "MetalHandsClawModule.png";

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetRecipe(GetBlueprintRecipe())
                .WithFabricatorType(CraftTree.Type.SeamothUpgrades)
                .WithStepsToFabricatorTab("ExosuitModules")
                .WithCraftingTime(3f);
            customPrefab.SetEquipment(EquipmentType.ExosuitModule);
            customPrefab.SetUnlock(MetalHandsMK1Prefab.Info.TechType);
            customPrefab.SetPdaGroupCategory(TechGroup.VehicleUpgrades, TechCategory.VehicleUpgrades);
            customPrefab.Register();
        }

        public static Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), IconFileName));
        }

        public static RecipeData GetBlueprintRecipe()
        {
            if (Plugin.ICMConfig.Config_Hardcore == false)
            {
                return new RecipeData()
                {
                    craftAmount = 1,
                    Ingredients =
                    {
                        new Ingredient(TechType.Titanium, 2),
                        new Ingredient(TechType.Diamond, 1),
                        new Ingredient(TechType.CopperWire, 1),
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 1),
                        new Ingredient(TechType.WiringKit, 1)
                    }
                };
            }
            else
            {
                return new RecipeData()
                {
                    craftAmount = 1,
                    Ingredients =
                    {
                        new Ingredient(TechType.TitaniumIngot, 1),
                        new Ingredient(TechType.Diamond, 2),
                        new Ingredient(TechType.CopperWire, 4),
                        new Ingredient(TechType.Magnetite, 4),
                        new Ingredient(TechType.AdvancedWiringKit, 2)
                    }
                };
            }
        }
    }
}
