using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ProcessMemoryLab.Simulation;

namespace ProcessMemoryLab.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private int _quantum = 2;
    [ObservableProperty] private string _result = "Execute a simulação para ver a linha do tempo e a fragmentação.";

    [RelayCommand]
    private void Simulate()
    {
        var jobs = new[] { new ProcessJob("P1", 0, 5), new ProcessJob("P2", 1, 3), new ProcessJob("P3", 2, 4) };
        var slices = ProcessMemoryEngine.ScheduleRoundRobin(jobs, Math.Clamp(Quantum, 1, 8));
        var holes = ProcessMemoryEngine.AllocateBestFit(100, [15, 25, 20], releasedBlock: 1);
        var pct = ProcessMemoryEngine.ExternalFragmentation(holes) * 100;
        Result = $"CPU (Round Robin, q={Math.Clamp(Quantum, 1, 8)}):\n{string.Join("  →  ", slices.Select(s => $"{s.ProcessId} [{s.Start}–{s.End}]"))}\n\nMemória (100 unidades, Best Fit):\n{string.Join("  ·  ", holes.Select(h => $"livre {h.Length} em {h.Start}"))}\nFragmentação externa: {pct:F0}% do espaço livre.";
    }
}
