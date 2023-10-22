using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using static CraftData;

namespace MetalHands.Items.Equipment
{
    public static class MetalHandsMK2Prefab
    {
        public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("MetalHandsMK2",
            "Metal Improved Grav Gloves",
            "This Gloves have a Metal Improved Cover and a additionally Gravsystem.")
        .WithIcon(GetItemSprite())
        .WithSizeInInventory(new Vector2int(2, 2));

        public static string IconFileName => "MetalHandsMK2.png";

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetRecipe(GetBlueprintRecipe())
                .WithFabricatorType(CraftTree.Type.Workbench)
                .WithCraftingTime(5f);
            customPrefab.SetEquipment(EquipmentType.Gloves);
            customPrefab.SetUnlock(MetalHandsMK1Prefab.Info.TechType);
            customPrefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
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
                        new Ingredient(MetalHandsMK1Prefab.Info.TechType, 1),
                        new Ingredient(TechType.AramidFibers, 1),
                        new Ingredient(TechType.CopperWire, 1),
                        new Ingredient(TechType.Magnetite, 2),
                        new Ingredient(TechType.ComputerChip, 1),
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
                        new Ingredient(MetalHandsMK1Prefab.Info.TechType, 1),
                        new Ingredient(TechType.AramidFibers, 2),
                        new Ingredient(TechType.CopperWire, 2),
                        new Ingredient(TechType.Magnetite, 4),
                        new Ingredient(TechType.AdvancedWiringKit, 1),
                        new Ingredient(TechType.Silicone, 1),
                    }
                };
            }
        }
    }
}
