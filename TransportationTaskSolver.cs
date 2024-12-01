namespace TransportationTaskSolver;

interface ITransportationTaskSolver
{
  Task<int[,]> SolveAsync(TransportationTask task);
}

class NorthwestCornerMethod : ITransportationTaskSolver
{
  public Task<int[,]> SolveAsync(TransportationTask task)
  {
    return Task.Run(() =>
    {
      int[,] X = new int[task.N, task.M];
      int[] supply = (int[])task.Supplies.Clone();
      int[] demand = (int[])task.Demands.Clone();
      int i = 0, j = 0;

      while (i < task.N && j < task.M)
      {
        int quantity = Math.Min(supply[i], demand[j]);
        X[i, j] = quantity;
        supply[i] -= quantity;
        demand[j] -= quantity;

        if (supply[i] == 0) i++;
        else if (demand[j] == 0) j++;
      }

      return X;
    });
  }
}

class LeastCostMethod : ITransportationTaskSolver
{
  public Task<int[,]> SolveAsync(TransportationTask task)
  {
    return Task.Run(() =>
    {
      int N = task.N;
      int M = task.M;
      int[,] X = new int[N, M];
      int[] supply = (int[])task.Supplies.Clone();
      int[] demand = (int[])task.Demands.Clone();

      var cells = from i in Enumerable.Range(0, N)
                  from j in Enumerable.Range(0, M)
                  orderby task.Costs[i, j]
                  select new { i, j };

      foreach (var cell in cells)
      {
        int quantity = Math.Min(supply[cell.i], demand[cell.j]);
        if (quantity > 0)
        {
          X[cell.i, cell.j] = quantity;
          supply[cell.i] -= quantity;
          demand[cell.j] -= quantity;
        }

        if (supply.All(s => s == 0) && demand.All(d => d == 0))
          break;
      }

      return X;
    });
  }
}

