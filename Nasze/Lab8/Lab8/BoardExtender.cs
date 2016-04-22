using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGreedyFish
{
    static class BoardExtender
    {
        /// <summary>
        /// Wyznacza ruchy i liczbę ryb zebranych dla algorytmu zachłannego (ruch wykonywany jest do najbliższego najlepszego pola [takiego z największą liczbą ryb])
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moves">Tablica ruchów wykonanych przez pingwiny</param>
        /// <returns>Liczba ryb zebrana przez pingwiny</returns>
        public static int GreedyAlgorithm1(this Board board, out Move[] moves)
        {
			moves = null;
            int fishes = 0;

			if (board.penguins == null || board.penguins.Count == 0 || board.AllFishes() == 0)
				return 0;

			List<Move> myMoves = new List<Move>();
			while(true)
			{
				int maxPenguin = -1;
				int maxScore = 0;
				int maxDir = -1;
				int maxLen = int.MaxValue;
				Point bestPoint = new Point(-1,-1);

				for(int i = 0; i < board.penguins.Count; i++)
				{
					for (int dir = 0; dir < 6; dir++)
					{
						int len = 0;
						Point dirPoint = board.GetNeighbour(board.penguins[i].x, board.penguins[i].y, dir);
						while(dirPoint.IsValid() && !board.IsPenguinAtField(dirPoint.x, dirPoint.y) && board.grid[dirPoint.x, dirPoint.y] != 0)
						{
							len++;
							if(board.grid[dirPoint.x, dirPoint.y] > maxScore)
							{
								maxPenguin = i;
								maxScore = board.grid[dirPoint.x, dirPoint.y];
								maxDir = dir;
								maxLen = len;
								bestPoint = dirPoint;
                            }
							else if (board.grid[dirPoint.x, dirPoint.y] == maxScore)
							{
								if (i > maxPenguin || dir > maxDir || len > maxLen)
								{
									dirPoint = board.GetNeighbour(dirPoint.x, dirPoint.y, dir);
									continue;
								}

								maxPenguin = i;
								maxScore = board.grid[dirPoint.x, dirPoint.y];
								maxDir = dir;
								maxLen = len;
								bestPoint = dirPoint;
							}
							dirPoint = board.GetNeighbour(dirPoint.x, dirPoint.y, dir);
						}
					}
				}
				if (maxPenguin == -1 || maxScore == 0)
					break;
				int x = board.penguins[maxPenguin].x;
				int y = board.penguins[maxPenguin].y;
				Move makeMove = new Move(new Point(x, y), bestPoint, board.grid[x, y], maxPenguin);
				myMoves.Add(makeMove);
				fishes += board.MovePenguin(maxPenguin, bestPoint.x, bestPoint.y);
				board.grid[x, y] = 0;
			
			}
			moves = myMoves.ToArray();
            return fishes; //zwrócona liczba ryb
        }

        /// <summary>
        /// Wyznacza ruchy i liczbę ryb zebranych dla algorytmu zachłannego (ruch wykonywany jest do końca kierunku, aż do napotkania "dziury" lub końca siatki)
        /// </summary>
        /// <param name="board"></param>
        /// <param name="moves">Tablica ruchów wykonanych przez pingwiny</param>
        /// <returns>Liczba ryb zebrana przez pingwiny</returns>
        public static int GreedyAlgorithm2(this Board board, out Move[] moves)
        {
            moves = null;
            int fishes = -1;
            
            return fishes; //zwrócona liczba ryb
        }
    }
}
