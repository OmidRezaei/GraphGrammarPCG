using System;
using EditorExtensions;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SimulationOptions : GraphElement
{
    public SliderInt StepSlider { get; }
    public IntegerField SeedField { get; }
    public ToolbarButton StartButton { get; }
    public ToolbarButton ResetButton { get; }
    public ToolbarButton StepBackButton { get; }
    public ToolbarButton StepForwardButton { get; }

    public SimulationOptions()
    {
        this.AddUXMLByName();

        style.position = Position.Absolute;
        style.backgroundColor =
            new Color(
                0.275f,
                0.275f,
                0.275f,
                0.75f);

        layer = 2;
        capabilities |= Capabilities.Movable | Capabilities.Ascendable;
        usageHints = UsageHints.DynamicTransform;
        this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));

        StartButton = this.Q<ToolbarButton>("start-button");
        ResetButton = this.Q<ToolbarButton>("reset-button");
        StepBackButton = this.Q<ToolbarButton>("step-back-button");
        StepForwardButton = this.Q<ToolbarButton>("step-forward-button");

        SeedField = this.Q<IntegerField>("seed-intfield");
        SeedField.SetEnabled(false);

        StepSlider = this.Q<SliderInt>("step-slider");
    }

    private void BuildContextualMenu(ContextualMenuPopulateEvent obj)
    {
        throw new NotImplementedException();
    }

    public override bool IsSelectable()
    {
        return true;
    }

    public override bool IsMovable()
    {
        return true;
    }
}
