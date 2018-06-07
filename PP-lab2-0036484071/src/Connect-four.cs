using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPI;
using System.Timers;
using System.Diagnostics;

namespace PP_lab2
{
    class Program
    {
        static int NEXT_TASK = 2;
        static int STOP = 3;
        static int EXIT = 4;
        static int TASK = 5;
        static int RESULT = 6;
        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {

                Intracommunicator comm = Communicator.world;
                int Id = comm.Rank;
                Connect4 game = new Connect4(comm, comm.Size, Id);
                if (Id == 0)
                {
                    Board board = new Board();
                    //game.CalculateNextMove(board);
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    Console.WriteLine("Igra krece!");
                    while (true)
                    {
                        board.Draw();
                        bool kraj = false;
                        while (true)
                        {
                            Console.WriteLine("Daj potez:");
                            try
                            {
                                int column = Int32.Parse(Console.ReadLine());
                                if (board.Put(column, 1))
                                {
                                    kraj = true;
                                }
                                break;
                            }
                            catch (Exception)
                            {
                            }

                        }
                        if (kraj)
                        {
                            Console.WriteLine("Pobijedio je igrac!");
                            break;
                        }
                        timer.Reset();
                        timer.Start();
                        int nextMove = game.CalculateNextMove(board.Duplicate());
                        Console.WriteLine("Potrebno vrijeme za izračun: " + timer.ElapsedMilliseconds);
                        Console.WriteLine("Računalo igra potez " + nextMove);

                        if (board.Put(nextMove, 2))
                        {
                            Console.WriteLine("Pobijedilo je računalo!");
                            break;
                        }

                    }
                    board.Draw();
                    Message exitMsg = new Message(0, EXIT);
                    comm.Broadcast<Message>(ref exitMsg, 0);

                }
                else
                {
                    while (true)
                    {
                        Message msg = null;
                        comm.Broadcast<Message>(ref msg, 0); 
                        if (msg.MessageType == EXIT)
                        {
                            break;
                        }
                        comm.Send<Message>(new Message(Id, NEXT_TASK), 0, 0);

                        while (true)
                        {  
                            msg = comm.Receive<Message>(0, 0);
                            if (msg.MessageType == STOP)
                            {
                                break;
                            }
                            else if (msg.MessageType == TASK)
                            {
                                Task task = msg.task;
                                task.value = game.CalculateStateValue(task.b, game.Reverse(task.nextPlayer), -1, 0);
                                Message answer = new Message(Id, RESULT);
                                answer.task = task;
                                comm.Send<Message>(answer, 0, 0);
                            }
                            
                        }
                    }

                }
            }
        }
    }
}
