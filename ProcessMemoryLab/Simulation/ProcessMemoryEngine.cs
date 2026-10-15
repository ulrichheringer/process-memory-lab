namespace ProcessMemoryLab.Simulation;

public sealed record ProcessJob(string Id, int Arrival, int Burst);
public sealed record CpuSlice(string ProcessId, int Start, int End);
public sealed record MemoryHole(int Start, int Length);

public static class ProcessMemoryEngine
{
    public static IReadOnlyList<CpuSlice> ScheduleRoundRobin(IEnumerable<ProcessJob> jobs, int quantum)
    {
        if (quantum < 1) throw new ArgumentOutOfRangeException(nameof(quantum));
        var pending = jobs.Where(p => p.Burst > 0).OrderBy(p => p.Arrival).ToArray();
        var remaining = pending.ToDictionary(p => p.Id, p => p.Burst);
        var ready = new Queue<ProcessJob>();
        var slices = new List<CpuSlice>();
        var time = pending.Length == 0 ? 0 : Math.Max(0, pending[0].Arrival);
        var next = 0;
        var completed = new HashSet<string>();
        while (completed.Count < pending.Length)
        {
            while (next < pending.Length && pending[next].Arrival <= time) ready.Enqueue(pending[next++]);
            if (ready.Count == 0) { time = pending[next].Arrival; continue; }
            var process = ready.Dequeue();
            var start = time;
            var duration = Math.Min(quantum, remaining[process.Id]);
            time += duration;
            remaining[process.Id] -= duration;
            slices.Add(new(process.Id, start, time));
            while (next < pending.Length && pending[next].Arrival <= time) ready.Enqueue(pending[next++]);
            if (remaining[process.Id] > 0) ready.Enqueue(process); else completed.Add(process.Id);
        }
        return slices;
    }

    public static IReadOnlyList<MemoryHole> AllocateBestFit(int totalSize, IEnumerable<int> requests, int? releasedBlock = null)
    {
        var holes = new List<MemoryHole> { new(0, totalSize) };
        var allocated = new List<MemoryHole>();
        foreach (var request in requests)
        {
            var index = Enumerable.Range(0, holes.Count).Where(i => holes[i].Length >= request)
                .OrderBy(i => holes[i].Length).FirstOrDefault(-1);
            if (index < 0) continue;
            var hole = holes[index];
            allocated.Add(new(hole.Start, request));
            holes[index] = new(hole.Start + request, hole.Length - request);
            if (holes[index].Length == 0) holes.RemoveAt(index);
        }
        if (releasedBlock is >= 0 && releasedBlock < allocated.Count)
        {
            var block = allocated[releasedBlock.Value];
            holes.Add(block);
            holes.Sort((a, b) => a.Start.CompareTo(b.Start));
        }
        return holes.Where(h => h.Length > 0).ToArray();
    }

    public static double ExternalFragmentation(IReadOnlyList<MemoryHole> holes)
    {
        var free = holes.Sum(h => h.Length);
        return free == 0 ? 0 : 1d - holes.Max(h => h.Length) / (double)free;
    }
}
