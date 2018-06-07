using System;
using MPI;

namespace PP_lab1
{
    class Filozofi
    {
        static int FORK_REQUEST = 0;
        static int FORK_RESPONSE = 1;
        static String indentation = null;
        static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                Intracommunicator comm = Communicator.world;
                bool leftRequest = false;
                bool rightRequest = false;
                int Id = comm.Rank;
                indentation = new String(' ', Id * 20);
                int N_OF_SLEEPING = 5;
                int MAX_SLEEPING = 500;
                int EATING_TIME = 4000;
                Random random = new Random();
                int nOfPhilo = comm.Size;
                int LeftId = (Id - 1);
                if (LeftId < 0)
                {
                    LeftId = nOfPhilo - 1;
                }
                int rightId = (Id + 1) % nOfPhilo;
                Fork leftFork = null;
                Fork rightFork = null;
                if (Id == 0)
                {
                    leftFork = new Fork(false, true);
                    rightFork = new Fork(false, true);
                }
                else if (Id == nOfPhilo - 1)
                {
                    leftFork = new Fork(false, false);
                    rightFork = new Fork(false, false);
                }
                else
                {
                    leftFork = new Fork(false, false);
                    rightFork = new Fork(false, true);
                }

                while (true)
                {

                    Console.WriteLine(indentation + "Mislim " + Id);
                    for (int i = 0; i < N_OF_SLEEPING; i++)
                    {
                        if (comm.ImmediateProbe(LeftId, 0) != null)
                        {
                            comm.Receive<Message>(LeftId, 0);
                            Console.WriteLine(indentation + "Saljem vilicu " + LeftId);
                            comm.Send<Message>(new Message(Id, FORK_RESPONSE), LeftId, 0);
                            leftFork.HasFork = false;
                            leftFork.IsClean = true;
                        }
                        if (comm.ImmediateProbe(rightId, 0) != null)
                        {
                            comm.Receive<Message>(rightId, 0);
                            Console.WriteLine(indentation + "Saljem vilicu " + rightId);
                            comm.Send<Message>(new Message(Id, FORK_RESPONSE), rightId, 0);
                            rightFork.HasFork = false;
                            rightFork.IsClean = true;
                        }
                        System.Threading.Thread.Sleep(random.Next(MAX_SLEEPING));
                    }
                    Console.WriteLine(indentation + "Gladan " + Id);
                    while (!(leftFork.HasFork && rightFork.HasFork))
                    {
                        Fork wanted = null;
                        int neighbourId;
                        for (int i = 0; i < 2; i++)
                        {
                            wanted = i == 0 ? leftFork : rightFork;
                            neighbourId = i == 0 ? LeftId : rightId;
                            if (!wanted.HasFork)
                            {
                                Console.WriteLine(indentation + "Trazim vilicu " + neighbourId);
                                Message message = new Message(Id, 0);
                                comm.Send<Message>(message, neighbourId, 0);
                            }
                            else
                            {
                                continue;
                            }
                            while (!wanted.HasFork)
                            {
                                Message message = comm.Receive<Message>(MPI.Unsafe.MPI_ANY_SOURCE, 0);
                                if (message.MessageType == FORK_RESPONSE)
                                {
                                    if (message.SourceId == LeftId)
                                    {
                                        leftFork.HasFork = true;
                                        leftFork.IsClean = true;
                                        leftRequest = false;
                                        Console.WriteLine(indentation + "Primam vilicu " + LeftId);
                                    }
                                    else
                                    {
                                        rightFork.HasFork = true;
                                        rightFork.IsClean = true;
                                        rightRequest = false;
                                        Console.WriteLine(indentation + "Primam vilicu " + rightId);
                                    }
                                }
                                else
                                {
                                    if (message.SourceId == LeftId)
                                    {
                                        leftRequest = parseRequest(comm, leftFork, true, LeftId, Id);
                                    }
                                    else
                                    {
                                        rightRequest = parseRequest(comm, rightFork, false, rightId, Id);
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine(indentation + "Jedem " + Id);
                    leftFork.IsClean = false;
                    rightFork.IsClean = false;
                    System.Threading.Thread.Sleep(random.Next(EATING_TIME));

                    if (leftRequest)
                    {
                        Console.WriteLine(indentation + "Saljem vilicu " + LeftId);
                        comm.Send<Message>(new Message(Id, FORK_RESPONSE), LeftId, 0);
                        leftRequest = false;
                        leftFork.HasFork = false;
                        leftFork.IsClean = true;
                    }
                    if (rightRequest)
                    {
                        Console.WriteLine(indentation + "Saljem vilicu " + rightId);
                        comm.Send<Message>(new Message(Id, FORK_RESPONSE), rightId, 0);
                        rightRequest = false;
                        rightFork.HasFork = false;
                        rightFork.IsClean = true;
                    }

                }


                //                misli(slucajan broj sekundi) ˇ ∧ odgovaraj na zahtjeve // asinkrono, s povremenom
                //provjerom
                //                while ¬imam obje vilice do
                //                    pošalji zahtjev za vilicom
                //                while ¬imam trazenu vilicu do
                //                    cekaj bilo koju poruku ˇ
                //                if poruka odgovor na zahtjev then
                //                ažuriraj vilice // dobio vilicu
                //                end if
                //                if poruka zahtjev then
                //                obradi zahtjev(odobri ili zabilježi) // drugi traže moju vilicu
                //                end if
                //                end while
                //                end while
                //                jedi
                //                odgovori na postojece zahtjeve // ako ih je bilo

            }
        }

        static bool parseRequest(Intracommunicator comm, Fork fork, bool isLeft, int id, int Id)
        {
            if (fork.HasFork && !fork.IsClean)
            {
                fork.HasFork = false;
                fork.IsClean = true;
                comm.Send<Message>(new Message(Id, FORK_RESPONSE), id, 0);
                Console.WriteLine(indentation + "Saljem vilicu " + id);
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}
