# Task.WhenAll vs Parallel.ForEachAsync...?

 * Task.Run vs ThreadPool.Queue... ?

---

### Task.WhenAll

```c#
var tasks = new Task[]
for(;;) tasks[i] = async function
await Task.WhenAll(tasks)
```

---

### Parallel.ForEachAsync

```c#
optional ParallelOptions.MaxDegreeOfParallelism
await Parallel.ForEachAsync(,, async => await async function)
```