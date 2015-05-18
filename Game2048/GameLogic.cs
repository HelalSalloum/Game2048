using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048
{
	public enum Direction
	{
		Left,
		Down,
		Right,
		Up
	}

	public enum GameType
	{
		Original,
		Fibonacci
	}

	public class GameModel
	{
		public int[,] Board { get; private set; }
		public int[,] OldBoard { get; private set; }
		public Tuple<int, int>[,] MoveTo { get; private set; }
		public readonly int Size;
		public readonly GameType type;
		public int Score { get; private set; }
		public int OldScore { get; private set; }
		public int Timer { get; set; }
		public string Player;

		private int[] realValues;

		public GameModel(int size, GameType type)
		{
			this.type = type;
			Size = size;
			Board = new int[size, size];
			OldBoard = new int[size, size];
			MoveTo = new Tuple<int, int>[size, size];

			realValues = new int[31];
			if (type == GameType.Original)
			{
				for (int i = 1; i < realValues.Length; i++)
					realValues[i] = 1 << i;
			}
			else if (type == GameType.Fibonacci)
			{
				realValues[1] = 1;
				realValues[2] = 2;
				for (int i = 3; i < realValues.Length; i++)
					realValues[i] = realValues[i - 1] + realValues[i - 2];
			}
		}
		
		public void Start()
		{
			Timer = -1;
			Score = 0;
			for (int i = 0; i < Size; i++)
				for (int j = 0; j < Size; j++)
					Board[i, j] = 0;
			AddRandomElement();
		}

		public int GetValue(int x)
		{
			if (x >= realValues.Length)
				throw new ArgumentException();
			return realValues[x];
		}

		private int Merge(int a, int b)
		{
			if (a > b)
			{
				int tmp = a;
				a = b;
				b = tmp;
			}
			if (type == GameType.Original)
				return (a == b ? a + 1 : -1);
			else if (type == GameType.Fibonacci)
				return ((b == 1 || a + 1 == b) ? b + 1 : -1);
			else throw new ArgumentException();
		}

		public bool GameOver()
		{
			for (int i = 0; i < Size; i++)
				for (int j = 0; j < Size; j++)
					if (Board[i, j] == 0)
						return false;
			for (int i = 0; i < Size; i++)
				for (int j = 0; j < Size - 1; j++)
					if (Merge(Board[i, j], Board[i, j + 1]) != -1)
						return false;
			for (int i = 0; i < Size - 1; i++)
				for (int j = 0; j < Size; j++)
					if (Merge(Board[i, j], Board[i + 1, j]) != -1)
						return false;
			return true;
		}

		List<Tuple<int, int>> GetEmptyCells()
		{
			var emptyPositions = new List<Tuple<int, int>>();
			for (int i = 0; i < Size; i++)
				for (int j = 0; j < Size; j++)
					if (Board[i, j] == 0)
						emptyPositions.Add(Tuple.Create(i, j));
			return emptyPositions;
		}

		void AddRandomElement()
		{
			var rnd = new Random();
			int newValue = (rnd.Next() % 10 == 0 ? 2 : 1);
			var emptyPositions = GetEmptyCells();
			if (emptyPositions.Count == 0)
				throw new ArgumentException();
			int index = rnd.Next(emptyPositions.Count);
			int x = emptyPositions[index].Item1;
			int y = emptyPositions[index].Item2;
			Board[x, y] = newValue;
			return;
		}

		void Rotate()
		{
			int[,] rotatedBoard = new int[Size, Size];
			Tuple<int, int>[,] rotatedMoveTo = new Tuple<int, int>[Size, Size];
			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
				{
					int nx = Size - 1 - y;
					int ny = x;
					rotatedBoard[nx, ny] = Board[x, y];
					rotatedMoveTo[nx, ny] = MoveTo[x, y];
					if (rotatedMoveTo[nx, ny] == null)
						continue;
					int nrx = Size - 1 - rotatedMoveTo[nx, ny].Item2;
					int nry = rotatedMoveTo[nx, ny].Item1;
					rotatedMoveTo[nx, ny] = new Tuple<int, int>(nrx, nry);
				}
			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
				{
					Board[x, y] = rotatedBoard[x, y];
					MoveTo[x, y] = rotatedMoveTo[x, y];
				}
		}

		bool MoveLeft()
		{
			bool changed = false;
			for (int y = 0; y < Size; y++)
			{
				int pos = 0;
				bool canMerge = false;
				for (int x = 0; x < Size; x++)
				{
					if (Board[x, y] == 0) continue;
					int merged = -1;
					if (canMerge) merged = Merge(Board[pos - 1, y], Board[x, y]);
					if (merged != -1)
					{
						changed |= true;
						Board[pos - 1, y] = merged;
						MoveTo[x, y] = new Tuple<int, int>(pos - 1, y);
						Score += GetValue(merged);
						canMerge = false;
					}
					else
					{
						changed |= (pos != x);
						Board[pos, y] = Board[x, y];
						MoveTo[x, y] = new Tuple<int, int>(pos, y);
						pos++;
						canMerge = true;
					}
				}
				for (; pos < Size; pos++)
					Board[pos, y] = 0;
			}
			return changed;
		}

		public bool TryEvaluateMove(Direction dir)
		{
			for (int x = 0; x < Size; x++)
				for (int y = 0; y < Size; y++)
					OldBoard[x, y] = Board[x, y];
			OldScore = Score;
			bool changed = false;
			for (int i = 0; i < 4; i++)
			{
				if ((int)dir == i)
					changed |= MoveLeft();
				Rotate();
			}
			if (changed) AddRandomElement();
			return changed;
		}
	}
}
