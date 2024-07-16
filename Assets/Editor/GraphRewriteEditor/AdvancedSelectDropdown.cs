using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class AdvancedSelectDropdown : AdvancedDropdown
{
    private readonly Func<IEnumerable<string>> getter;
    public Action<string> OnItemSelected { get; set; }

    public AdvancedSelectDropdown(Func<IEnumerable<string>> getter) : base(new AdvancedDropdownState())
    {
        this.getter = getter;
    }
    
    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);
        OnItemSelected?.Invoke(item.name);
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Select");

        IEnumerable<string> choices = getter();
        foreach (string choice in choices)
            root.AddChild(new AdvancedSelectDropdownItem(choice));

        return root;
    }
}

public class AdvancedSelectDropdownItem : AdvancedDropdownItem
{
    public AdvancedSelectDropdownItem(string name) : base(name)
    {
    }
}
