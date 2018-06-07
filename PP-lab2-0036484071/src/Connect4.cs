using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPI;

namespace PP_lab2
{
    public class Connect4
    {
        public static int TASK_DEPTH_INIT = 2;
        public static int TASK_DEPTH = 7;
        public static int COMPUTER = 2;
        public static int PLAYER = 1;
        public static int R = 6;
        public static int C = 7;
        static int START = 1;
        static int STOP = 3;
        static int TASK = 5;
        static int RESULT = 6;
        Intracommunicator comm;
        int N;
        int Id;
        public Connect4(Intracommunicator comm, int N, int Id)
        {
            this.comm = comm;
            this.N = N;
            this.Id = Id;
        }

        public double CalculateStateValue(Board b, int currPlayer, int currMove, int depth)
        {
            b = b.Duplicate();
            if (currMove != -1)
                if (b.Put(currMove, currPlayer))
                    return (currPlayer == COMPUTER ? 1 : -1);

            if (depth >= TASK_DEPTH)
                return 0;

            int counter = 0;
            double sum = 0, tmp;
            for (int move = 0; move < C; move++)
                if (b.columns[move].LastPosition() < R - 1)
                {
                    tmp = CalculateStateValue(b, Reverse(currPlayer), move, depth + 1);
                    if (tmp == -1 && currPlayer == PLAYER)
                        return -1;
                    if (tmp == 1 && currPlayer == COMPUTER)
                        return 1;
                    sum += tmp;
                    counter++;
                }
            return sum / counter;

        }

        public double CalculateMoveValue(Board b, string key, int currPlayer, int currMove,
                           int depth, Dictionary<string, double> map)
        {
            b = b.Duplicate();
            if (b.Put(currMove, currPlayer))
                return (currPlayer == COMPUTER ? 1 : -1);
            key += currMove;
            if (depth >= TASK_DEPTH_INIT)
            {
                double _;
                map.TryGetValue(key, out _);
                return _;
            }

            int counter = 0;
            double sum = 0, tmp;
            for (int move = 0; move < C; move++)
                if (b.columns[move].LastPosition() < R - 1)
                {
                    tmp = CalculateMoveValue(b, key, Reverse(currPlayer), move, depth + 1, map);
                    if (tmp == 1 && currPlayer == COMPUTER)
                        return 1;
                    if (tmp == -1 && currPlayer == PLAYER)
                        return -1;
                    sum += tmp;
                    counter++;
                }
            return sum / counter;

        }

        public int CalculateNextMove(Board b)
        {
            List<Task> tasks = GenerateTasks(new Board(), "", PLAYER, -1, 0, new List<Task>());
            var map = new Dictionary<string, double>();
            
            int stopped = 0;
            if (N == 1)
            {
                foreach (Task task in tasks)
                {
                    map[task.key] = CalculateStateValue(task.b, Reverse(task.nextPlayer), -1, 0);
                }
            }
            else
            {
                Message startMsg = new Message(0, START);
                comm.Broadcast<Message>(ref startMsg, 0);
                Message msg;
                
                while (tasks.Any() || stopped < N - 1)
                {
                    msg = comm.Receive<Message>(MPI.Unsafe.MPI_ANY_SOURCE, 0);

                    if (msg.MessageType == RESULT)
                    {
                        map[msg.task.key] = msg.task.value;
                    }
                  
                    if (tasks.Any())
                    {
                        Message newTask = new Message(0, TASK);
                        newTask.task = tasks[0];
                        tasks.RemoveAt(0);
                        comm.Send<Message>(newTask, msg.SourceId, 0);
                    }
                    else
                    {
                        stopped++;
                        comm.Send<Message>(new Message(0, STOP), msg.SourceId, 0);
                    }
                }
            }
            
            double bestSol = Double.MinValue, currSol;
            int bestMove = -1;
            for (int move = 0; move < C; move++)
            {
                if (b.columns[move].LastPosition() < R - 1)
                {
                    currSol = CalculateMoveValue(b, "", COMPUTER, move, 1, map);
                    Console.WriteLine(move + " " + currSol);
                    if (currSol > bestSol)
                    {
                        bestSol = currSol;
                        bestMove = move;
                    }
                }
            }
            return bestMove;
        }
        public List<Task> GenerateTasks(Board b, string key, int currPlayer, int currMove, int depth, List<Task> tasks)
        {
            b = b.Duplicate();
            if (currMove != -1)
            {
                if (b.Put(currMove, currPlayer))
                    return tasks;
                key += currMove;
            }

            if (depth >= TASK_DEPTH_INIT)
                tasks.Add(new Task(b, Reverse(currPlayer), key));
            else
            {
                for (int move = 0; move < C; move++)
                    if (b.columns[move].LastPosition() < R - 1)
                        GenerateTasks(b, key, Reverse(currPlayer), move, depth + 1, tasks);
            }
            return tasks;
        }


        public int Reverse(int player)
        {
            return player == 1 ? 2 : 1;
        }

    }
}
