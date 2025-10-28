using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ItemIcons.AtkIcons;
using ItemIcons.Utils;
using System.Collections.Generic;
using System.Linq;

namespace ItemIcons.ItemProviders;

internal sealed unsafe class NeedGreed : BaseItemProvider
{
    public override ItemProviderCategory Category => ItemProviderCategory.Loot;

    public override string AddonName => "NeedGreed";

    public NeedGreed()
    {
        Service.GameInteropProvider.InitializeFromAttributes(this);
        Service.AddonLifecycle.RegisterListener(AddonEvent.PostUpdate, AddonName, OnPostUpdate);
    }

    public override void Dispose()
    {
        Service.AddonLifecycle.UnregisterListener(AddonEvent.PostUpdate, AddonName, OnPostUpdate);
    }
    public void OnPostUpdate(AddonEvent type, AddonArgs args)
    {
        Service.Plugin.Renderer?.InvalidateAddonCache(AddonName);
    }

    public override IEnumerable<AtkItemIcon> GetIcons(nint drawnAddon)
    {
        var list = GetComponentList(drawnAddon);
        var length = GetListLength(list);

        return Enumerable.Range(0, length).Select(i => GetListIcon(list, i));
    }

    private static nint GetComponentList(nint drawnAddon) =>
        (nint)NodeUtils.GetAsAtkComponent<AtkComponentList>(((AtkUnitBase*)drawnAddon)->GetNodeById(6));

    private static int GetListLength(nint listComponent) =>
        ((AtkComponentList*)listComponent)->ListLength;

    private static LootItem GetLootItem(int idx) =>
        Loot.Instance()->Items[idx];

    public override IEnumerable<Item?> GetItems(nint addon) =>
        Enumerable.Range(0, 16).Select(i => (Item?)new Item(GetLootItem(i).ItemId));
}
