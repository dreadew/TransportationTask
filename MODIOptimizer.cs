using TransportationProblem;

namespace Optimizers;

class MODIOptimizer
{
  public Task<int[,]> OptimizeAsync(TransportationTask task, int[,] X)
  {
    return Task.Run(() =>
    {
      int N = task.N;
      int M = task.M;
      int[,] C = task.Costs;
      bool isOptimal = false;

      while (!isOptimal)
      {
        int[] u = new int[N];
        int[] v = new int[M];
        bool[] uSet = new bool[N];
        bool[] vSet = new bool[M];
        uSet[0] = true;

        var basicCells = new List<(int i, int j)>();
        for (int i = 0; i < N; i++)
          for (int j = 0; j < M; j++)
            if (X[i, j] > 0)
              basicCells.Add((i, j));

        bool updated;
        do
        {
          updated = false;
          foreach (var (i, j) in basicCells)
          {
            if (uSet[i] && !vSet[j])
            {
              v[j] = C[i, j] - u[i];
              vSet[j] = true;
              updated = true;
            }
            else if (!uSet[i] && vSet[j])
            {
              u[i] = C[i, j] - v[j];
              uSet[i] = true;
              updated = true;
            }
          }
        } while (updated);

        var deltas = new List<(int i, int j, int delta)>();
        for (int i = 0; i < N; i++)
        {
          for (int j = 0; j < M; j++)
          {
            if (X[i, j] == 0)
            {
              int delta = C[i, j] - u[i] - v[j];
              deltas.Add((i, j, delta));
            }
          }
        }

        var minDelta = deltas.OrderBy(d => d.delta).First();

        if (minDelta.delta >= 0)
        {
          isOptimal = true;
        }
        else
        {
          isOptimal = true;
        }
      }

      return X;
    });
  }
}
