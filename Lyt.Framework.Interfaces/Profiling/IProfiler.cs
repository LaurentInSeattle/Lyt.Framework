namespace Lyt.Framework.Interfaces.Profiling;

public interface IProfiler
{
    Task FullGcCollect(int delay = 0);

    int[] CollectionCounts();

    void MemorySnapshot(string comment = "", bool withGCCollect = true); 
}
