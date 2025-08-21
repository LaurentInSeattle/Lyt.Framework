namespace Lyt.Utilities.Parallel;

public static class Parallelizer
{
    public static void ParallelizeActionOnIndices(int arrayLength, Action<int, int> action)
    {
        // TODO: Use ProcessorCount
        // int processorCount = Environment.ProcessorCount;

        // 1 : Setup
        int taskCount = 4; 
        int all = arrayLength;
        int half = all / 2;
        int quart = half / 2;
        int[] indices = [0, quart, half, half + quart, all];
        var tasks = new Task[taskCount];
        for (int taskIndex = 0; taskIndex < taskCount; ++taskIndex)
        {
            int from = indices[taskIndex];
            int to = indices[1 + taskIndex];
            var task = new Task(() => action(from, to));
            tasks[taskIndex] = task;
        }

        // 2 : Start all tasks
        for (int taskIndex = 0; taskIndex < taskCount; ++taskIndex)
        {
            tasks[taskIndex].Start();
        }

        // 3 : Wait for completion 
        Task.WaitAll(tasks);
    }
}
