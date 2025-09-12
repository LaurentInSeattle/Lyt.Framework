namespace Lyt.Utilities.Parallel;

public static class Parallelize
{
    public static void ActionOnIndices(int length, Action<int, int> action)
    {
        if (length < 0)
        {
            throw new Exception("Parallelize.ActionOnIndices: Length cannot be negative");
        }

        if (length == 0)
        {
            Debug.WriteLine("Parallelize.ActionOnIndices: Length is zero, doing nothing");
            return;
        }

        if (length < 4)
        {
            Debug.WriteLine("Parallelize.ActionOnIndices: Length less than four, no threads.");
            action(0, length);
            return;
        }

        // TODO: Use ProcessorCount
        // int processorCount = Environment.ProcessorCount;

        // 1 : Setup
        int taskCount = 4;
        int all = length;
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

    public static void ForEach<T>(IList<T> list, Action<T, int> action)
    {
        int length = list.Count;
        void Process(int from, int to)
        {
            for (int index = from; index < to; ++index)
            {
                T element = list[index];
                action(element, index);
            }
        }

        Parallelize.ActionOnIndices(length, Process);
    }

    public static void Actions(params Action[] actions)
    {
        int length = actions.Length;
        if (length < 0)
        {
            throw new Exception("Parallelize.Actions: Length cannot be negative");
        }

        if (length == 0)
        {
            Debug.WriteLine("Parallelize.Actions: Length is zero, doing nothing");
            return;
        }

        if (length == 1)
        {
            Debug.WriteLine("Parallelize.Actions: Length is one, no threading");
            actions[0](); 
            return;
        }

        // 1 : Create all tasks
        int taskCount = length;
        var tasks = new Task[taskCount];
        for (int taskIndex = 0; taskIndex < taskCount; ++taskIndex)
        {
            var task = new Task(() => actions[taskIndex]());
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

    public static void Loops (int length, Action<int> action)
    {
        if (length < 0)
        {
            throw new Exception("Parallelize.Loops: Length cannot be negative");
        }

        if (length == 0)
        {
            Debug.WriteLine("Parallelize.Loops: Length is zero, doing nothing");
            return;
        }

        if (length == 1)
        {
            Debug.WriteLine("Parallelize.Actions: Length is one, no threading");
            action(0);
            return;
        }

        // 1 : Setup tasks 
        int taskCount = length;
        var tasks = new Task[taskCount];
        for (int taskIndex = 0; taskIndex < taskCount; ++taskIndex)
        {
            // Copy index so that there is a capture in each loop 
            // Using taskIndex for the action parameter !!!
            int loopIndex = taskIndex;
            var task = new Task(() => action(loopIndex));
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

    public static void NestedLoops(int outerLength, int innerLength, Action<int, int> action)
    {
        int length = outerLength * innerLength;
        if (length < 0)
        {
            throw new Exception("Parallelize.Loops: Length cannot be negative");
        }

        if (length == 0)
        {
            Debug.WriteLine("Parallelize.Loops: Length is zero, doing nothing");
            return;
        }

        if (length == 1)
        {
            Debug.WriteLine("Parallelize.Actions: Length is one, no threading");
            action(0, 0);
            return;
        }

        // 1 : Setup tasks 
        int taskCount = length;
        var tasks = new Task[taskCount];
        int taskIndex = 0;
        for (int i = 0; i < outerLength; i++)
        {
            for (int j = 0; j < innerLength; j++)
            {
                // Copy indices so that there is a capture in each loop 
                // Using i and j for the action parameters will not work !!!
                int outer = i;
                int inner = j;
                var task = new Task(() => action(outer , inner ));
                tasks[taskIndex++] = task;
            }
        }

        // 2 : Start all tasks
        for (int index = 0; index < taskCount; ++index)
        {
            tasks[index].Start();
        }

        // 3 : Wait for completion 
        Task.WaitAll(tasks);
    }
}
