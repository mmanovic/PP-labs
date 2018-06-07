using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PP_lab2
{
    [Serializable]
    public class Board
    {
        public static int R = 6;
        public static int C = 7;
        public Column[] columns;
        public Board()
        {
            columns = new Column[C];
            for (int i = 0; i < C; i++)
            {
                columns[i] = new Column();
            }
        }
        public Board(Column[] columns)
        {
            this.columns = columns;
        }

        public bool Put(int xpos, int player)
        {
            int ypos = columns[xpos].LastPosition() + 1;
            if (!ValidPos(xpos, ypos))
            {
                throw new IndexOutOfRangeException("Stupac " + xpos + " je popunjen!");
            }
            columns[xpos].row[ypos] = player;
            return CheckWin(xpos, ypos);
        }

        public bool CheckWin(int xpos, int ypos)
        {
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] counter = new int[8];
            int init = columns[xpos].row[ypos];
            if (init == 0)
            {
                return false;
            }

            for (int i = 0; i < 8; i++)
            {
                int x = xpos, y = ypos;
                while (ValidPos(x, y) && columns[x].row[y] == init)
                {
                    x += dx[i];
                    y += dy[i];
                    counter[i]++;
                }
            }
            return (counter[0] + counter[7] >= 5) || (counter[1] + counter[6] >= 5)
                || (counter[2] + counter[5] >= 5) || (counter[3] + counter[4] >= 5);
        }
        public void Draw()
        {
            Console.WriteLine("----------");
            for (int j = R - 1; j >= 0; j--)
            {
                for (int i = 0; i < C; i++)
                {
                    char x;
                    int tmp = columns[i].row[j];
                    if (tmp == 0)
                    {
                        x = '.';
                    }
                    else if (tmp == 1)
                    {
                        x = '1';
                    }
                    else
                    {
                        x = '2';
                    }
                    Console.Write(x);
                }
                Console.WriteLine();
            }
            Console.WriteLine("\n----------");
        }
        public Board Duplicate()
        {
            Column[] newColumns = new Column[C];
            for (int i = 0; i < C; i++)
            {
                newColumns[i] = columns[i].Duplicate();
            }
            return new Board(newColumns);
        }
        public bool ValidPos(int xpos, int ypos)
        {
            return xpos >= 0 && xpos < C && ypos >= 0 && ypos < R;
        }

        [Serializable]
        public class Column
        {
            public int[] row;
            public Column()
            {
                row = new int[R];
            }
            public Column(int[] row)
            {
                this.row = row;
            }
            public int LastPosition()
            {
                for (int i = R - 1; i >= 0; i--)
                {
                    if (row[i] > 0)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public Column Duplicate()
            {
                int[] newRow = new int[R];
                Array.Copy(row, newRow, R);
                return new Column(newRow);
            }
        }

    }
}
