using System.Collections.Generic;
using GenGra;
using UnityEngine;

public class EditorSimulationData : ScriptableObject
{
    private GenGraType Simulation;
    private IEnumerator<GraphType> ActiveGeneration;

    [field: SerializeField]
    public GenerationData GenerationData { get; set; }

    [field: SerializeField]
    public int Seed { get; set; }

    [field: SerializeField]
    public int CurrentStep { get; set; }

    [field: SerializeField]
    public List<GraphData> Steps { get; set; }

    private void OnEnable()
    {
        Steps = new List<GraphData>();
    }

    public void InitSimulation()
    {
        CurrentStep = -1;
        Seed = GenerationData.Seed >= 0 ?
                   GenerationData.Seed :
                   Random.Range(-1, int.MaxValue) + 1;
        Steps = new List<GraphData>();

        Simulation = GenerationData.ToGenGra();
        ActiveGeneration = Simulation.GenerationGraphStepByStep();
        GenerateNextStep();
    }

    public void GenerateNextStep()
    {
        if (ActiveGeneration.MoveNext())
        {
            Steps.Add(ActiveGeneration.Current.ToGraphData());
            CurrentStep = Steps.Count - 1;
        }
    }

    public void GoToStep(int step)
    {
        step = Mathf.Max(0, step);

        if (step >= Steps.Count)
        {
            for (var i = 0; i < step - Steps.Count + 1; i++)
                GenerateNextStep();
        }

        CurrentStep = Mathf.Min(step, Steps.Count - 1);
    }
}
