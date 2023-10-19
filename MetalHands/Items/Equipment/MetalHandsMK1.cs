using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;
using static CraftData;
using System.Collections;

namespace MetalHands.Items.Equipment
{
    public static class MetalHandsMK1Prefab
    {
        public static PrefabInfo Info { get; } = PrefabInfo
        .WithTechType("MetalHandsMK1",
            "Metal Improved Gloves",
            "This Gloves have a Metal Improved Cover and allow Working with Hard Matrials without hurting the Person who wear it. Warning this Personal Safty Equiment is for Passive use avoiding severe injury . Do not use it as Tool.")
        .WithIcon(GetItemSprite())
        .WithSizeInInventory(new Vector2int(2, 2));

        public static string IconFileName => "MetalHandsMK1.png";

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            customPrefab.SetGameObject(GetGameObjectAsync);
            customPrefab.SetRecipe(GetBlueprintRecipe())
                .WithFabricatorType(CraftTree.Type.Fabricator)
                .WithStepsToFabricatorTab("Personal", "Equipment")
                .WithCraftingTime(3f);
            customPrefab.SetEquipment(EquipmentType.Gloves);
            //This Config was for Custom Databoxes
            //customPrefab.SetUnlock(TechType.AcidOld);
            //This is Temporary
            customPrefab.SetUnlock(TechType.ReinforcedDiveSuit);
            customPrefab.SetPdaGroupCategory(TechGroup.Personal,TechCategory.Equipment);
            customPrefab.Register();
        }

        public static IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.ReinforcedGloves);
            yield return task;
            GameObject prefab = task.GetResult();
            GameObject obj = GameObject.Instantiate(prefab);
            prefab.SetActive(false);

            gameObject.Set(obj);
        }

        public static Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), IconFileName));
        }

        public static RecipeData GetBlueprintRecipe()
        {
            if (Plugin.Config.Config_Hardcore == false)
            {
                return new RecipeData()
                {
                    craftAmount = 1,
                    Ingredients =
                    {
                        new Ingredient(TechType.PlasteelIngot, 1),
                        new Ingredient(TechType.Diamond, 2),
                        new Ingredient(TechType.FiberMesh, 2),
                        new Ingredient(TechType.Silicone, 1)
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
                        new Ingredient(TechType.PlasteelIngot, 2),
                        new Ingredient(TechType.Nickel,4),
                        new Ingredient(TechType.AramidFibers, 2),
                        new Ingredient(TechType.Silicone, 2),
                    }
                };
            }
        }
    }
}
