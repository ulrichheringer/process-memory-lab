using ProcessMemoryLab.Simulation;

var slices = ProcessMemoryEngine.ScheduleRoundRobin([new("A", 0, 4), new("B", 1, 2)], 2);
if (slices.Sum(s => s.End - s.Start) != 6 || slices[0].ProcessId != "A" || slices[1].ProcessId != "B")
    throw new Exception("Round Robin schedule is invalid.");
var holes = ProcessMemoryEngine.AllocateBestFit(100, [20, 30, 10], 1);
if (Math.Abs(ProcessMemoryEngine.ExternalFragmentation(holes) - (1 - 40d / 70d)) > 0.001)
    throw new Exception("External fragmentation calculation is invalid.");
Console.WriteLine("Process and memory checks passed.");
