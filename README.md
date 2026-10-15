# process-memory-lab

Compare Round Robin CPU scheduling with Best Fit memory allocation. The app shows the schedule and free memory blocks, including external fragmentation.

## Run

Requires the .NET 10 SDK. Browser builds also need the WebAssembly workload: `dotnet workload install wasm-tools`.

### Desktop

~~~sh
dotnet run --project ProcessMemoryLab.Desktop/ProcessMemoryLab.Desktop.csproj
~~~

### Browser

~~~sh
dotnet run --project ProcessMemoryLab.Browser/ProcessMemoryLab.Browser.csproj
~~~

## Example

Change the time slice, run the simulation, and compare the CPU timeline and free memory blocks.
