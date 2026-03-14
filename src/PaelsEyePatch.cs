using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Relics;
using MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens;
using MegaCrit.Sts2.Core.Nodes.Screens.RelicCollection;

namespace PaelsLookingEye;

[HarmonyPatch]
class BasicHolderPatch()
{
    static Texture2D? PaelsEyeBase {
        get {
            var loaded = ProjectSettings.LoadResourcePack(OS.GetExecutablePath().GetBaseDir().PathJoin("mods/PaelsLookingEye/PaelsLookingEye.pck"));
            if (loaded) {
                var eye = ResourceLoader.Load<Texture2D>("res://PaelsLookingEye/images/relics/paels_eye.png");
                return eye;
            }
            else {
                PaelsLookingEyeMod.Logger.Info("Oh my god its broken again");
            }

            return null;
        }
    }

    static PackedScene LookingEyeScene => ResourceLoader.Load<PackedScene>("res://PaelsLookingEye/scenes/looking_eye.tscn");

    // Inventory visuals
    [HarmonyPatch(typeof(NRelicInventoryHolder), nameof(NRelicInventoryHolder._Ready))]
    [HarmonyPostfix]
    static void OnInventoryRelicReady(NRelicInventoryHolder __instance)
    {
        var relic = __instance.Relic;
        var model = relic.Model;

        if (model is PaelsEye)
        {
            var icon_node = relic.Icon;

            icon_node.Texture = PaelsEyeBase;

            var scene_instance = LookingEyeScene.Instantiate();
            icon_node.AddChild(scene_instance);
            scene_instance.Owner = icon_node;
        }
    }

    // Holder for shops i think??
    [HarmonyPatch(typeof(NRelicBasicHolder), nameof(NRelicBasicHolder._Ready))]
    [HarmonyPostfix]
    static void OnBaseRelicReady(NRelicBasicHolder __instance)
    {
        var relic = __instance.Relic;
        var model = relic.Model;

        if (model is PaelsEye)
        {
            var icon_node = relic.Icon;
            icon_node.AddChild(LookingEyeScene.Instantiate());
        }
    }

    // Ancient Event visual
    [HarmonyPatch(typeof(NEventOptionButton), nameof(NEventOptionButton._Ready))]
    [HarmonyPostfix]
    static void OnEventOptionButtonReady(NEventOptionButton __instance)
    {
        if (__instance.Event is AncientEventModel && __instance.Option.Relic is PaelsEye)
        {
            var icon_node = __instance.GetNode<TextureRect>("%RelicIcon");
            icon_node.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;
            icon_node.Texture = PaelsEyeBase;
            icon_node.AddChild(LookingEyeScene.Instantiate());
        }
    }

    // Relic Collection
    [HarmonyPatch(typeof(NRelicCollectionEntry), nameof(NRelicCollectionEntry._Ready))]
    [HarmonyPostfix]
    static void OnRelicCollectionEntryReady(NRelicCollectionEntry __instance)
    {
        if (__instance.relic is PaelsEye && __instance.ModelVisibility == ModelVisibility.Visible)
        {
            var icon_node = __instance._relicNode.GetChild(0) as TextureRect;
            icon_node.Texture = PaelsEyeBase;
            icon_node.AddChild(LookingEyeScene.Instantiate());
        }
    }

    // Large inspect relic view
    [HarmonyPatch(typeof(NInspectRelicScreen), nameof(NInspectRelicScreen._Ready))]
    [HarmonyPostfix]
    static void OnInspectRelicReady(NInspectRelicScreen __instance)
    {
        var icon_node = __instance._relicImage;
        icon_node.AddChild(LookingEyeScene.Instantiate());
    }

    [HarmonyPatch(typeof(NInspectRelicScreen), nameof(NInspectRelicScreen.UpdateRelicDisplay))]
    [HarmonyPostfix]
    static void OnInspectRelicRedraw(NInspectRelicScreen __instance)
    {
        var relicModel = __instance._relics[__instance._index];
        var icon_node = __instance._relicImage;
        var is_paels_eye = false;
        if (relicModel is PaelsEye)
        {
            // Not overwriting the base file, ideally allows toggling individual sections.
            icon_node.Texture = PaelsEyeBase;
            is_paels_eye = true;
        }
        else
        {
            is_paels_eye = false;
        }

        var child = icon_node.GetNode<TextureRect>("LookingEye");
        if (child != null)
        {
            child.Set("visible", is_paels_eye);
        }
    }
}
