using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class DropdownButton : Button
{
    public Func<IEnumerable<string>> DropdownGetter { get; set; }

    public string ActiveItem { get; private set; }
    public Action<string> OnActiveItemChanged { get; set; }

    public DropdownButton()
    {
        clicked += OpenDropdownWindow;
    }

    private void OpenDropdownWindow()
    {
        if (DropdownGetter == null)
            return;

        var dd = new AdvancedSelectDropdown(DropdownGetter);
        dd.OnItemSelected += SetActiveItem;
        dd.Show(layout);
    }

    public void SetActiveItem(string item)
    {
        OnItemSelected(item);
    }

    private void OnItemSelected(string item)
    {
        ActiveItem = item;
        text = item;
        OnActiveItemChanged?.Invoke(ActiveItem);
    }
}
