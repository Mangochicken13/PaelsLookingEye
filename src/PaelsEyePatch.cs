using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace PaelsLookingEye;

[HarmonyPatch(typeof(NRelic))]
class PaelsLookingEyePatch()
{
    [HarmonyPatch(nameof(NRelic.Create))]
    [HarmonyPostfix]
    static NRelic NrelicCreate(NRelic relic)
    {
        PaelsLookingEyeMod.Logger.Info("loaded relic" + relic.Model.Id.ToString());
        return relic;
    }
}

[HarmonyPatch(typeof(NRelicInventoryHolder))]
class BasicHolderPatch()
{
    [HarmonyPatch(nameof(NRelicInventoryHolder._Ready))]
    [HarmonyPostfix]
    static void NrelicCreate(NRelicInventoryHolder __instance)
    {
        PaelsLookingEyeMod.Logger.Info("loaded relic" + __instance.Relic.Model.Id.ToString());
        var relic = __instance.Relic;
        var model = relic.Model;

        if (model is PaelsEye)
        {
            var icon_node = relic.Icon;

            var loaded = ProjectSettings.LoadResourcePack(OS.GetExecutablePath().GetBaseDir().PathJoin("mods/PaelsLookingEye/PaelsLookingEye.pck"));
            if (loaded) { icon_node.Texture = (Texture2D)ResourceLoader.Load<Texture>("res://PaelsLookingEye/images/relics/paels_eye.png"); }
            else { PaelsLookingEyeMod.Logger.Info("Oh my god its broken again"); }

            var looking_eye_scene = (PackedScene)Godot.ResourceLoader.Load("res://PaelsLookingEye/scenes/looking_eye.tscn");
            var scene_instance = looking_eye_scene.Instantiate();
            icon_node.AddChild(scene_instance);
            // scene_instance.Owner = icon_node;
        }
    }

    // [HarmonyPatch(nameof(NRelicBasicHolder._Ready))]
    // [HarmonyPostfix]
    // static void NRelicReload(NRelic __instance)
    // {
    //     PaelsLookingEyeMod.Logger.Info("loaded relic" + __instance.Model.Id.ToString());
    // }

}
