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
		Right,
		Up,
		Down
	}

	public enum GameType
	{
		Original,
		Fibonacci
	}

	class GameModel
	{
		public int[,] Board { get; private set; }
		public readonly int Size;
		public readonly GameType type;
		public int Score { get; private set; }

		Dictionary<Direction, Func<bool>> funcByDir;
		private int[] realValues;

		public GameModel(int size, GameType type)
		{
			this.type = type;
			Size = size;
			Board = new int[size, size];
			funcByDir = new Dictionary<Direction, Func<bool>>();
			funcByDir[Direction.Left] = MoveLeft;
			funcByDir[Direction.Right] = MoveRight;
			funcByDir[Direction.Up] = MoveUp;
			funcByDir[Direction.Down] = MoveDown;

			realValues = new int[21];
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
						Score += GetValue(merged);
						canMerge = false;
					}
					else
					{
						changed |= (pos != x);
						Board[pos, y] = Board[x, y];
						pos++;
						canMerge = true;
					}
				}
				for (; pos < Size; pos++)
					Board[pos, y] = 0;
			}
			return changed;
		}
		bool MoveRight()
		{
			bool changed = false;
			for (int y = 0; y < Size; y++)
			{
				int pos = Size - 1;
				bool canMerge = false;
				for (int x = Size - 1; x >= 0; x--)
				{
					if (Board[x, y] == 0) continue;
					int merged = -1;
					if (canMerge) merged = Merge(Board[pos + 1, y], Board[x, y]);
					if (merged != -1)
					{
						changed |= true;
						Board[pos + 1, y] = merged;
						Score += GetValue(merged);
						canMerge = false;
					}
					else
					{
						changed |= (pos != x);
						Board[pos, y] = Board[x, y];
						pos--;
						canMerge = true;
					}
				}
				for (; pos >= 0; pos--)
					Board[pos, y] = 0;
			}
			return changed;
		}
		bool MoveUp()
		{
			bool changed = false;
			for (int x = 0; x < Size; x++)
			{
				int pos = 0;
				bool canMerge = false;
				for (int y = 0; y < Size; y++)
				{
					if (Board[x, y] == 0) continue;
					int merged = -1;
					if (canMerge) merged = Merge(Board[x, pos - 1], Board[x, y]);
					if (merged != -1)
					{
						changed |= true;
						Board[x, pos - 1] = merged;
						Score += GetValue(merged);
						canMerge = false;
					}
					else
					{
						changed |= (pos != y);
						Board[x, pos] = Board[x, y];
						pos++;
						canMerge = true;
					}
				}
				for (; pos < Size; pos++)
					Board[x, pos] = 0;
			}
			return changed;
		}
		bool MoveDown()
		{
			bool changed = false;
			for (int x = 0; x < Size; x++)
			{
				int pos = Size - 1;
				bool canMerge = false;
				for (int y = Size - 1; y >= 0; y--)
				{
					if (Board[x, y] == 0) continue;
					int merged = -1;
					if (canMerge) merged = Merge(Board[x, pos + 1], Board[x, y]);
					if (merged != -1)
					{
						changed |= true;
						Board[x, pos + 1] = merged;
						Score += GetValue(merged);
						canMerge = false;
					}
					else
					{
						changed |= (pos != y);
						Board[x, pos] = Board[x, y];
						pos--;
						canMerge = true;
					}
				}
				for (; pos >= 0; pos--)
					Board[x, pos] = 0;
			}
			return changed;
		}

		public void EvaluateMove(Direction dir)
		{
			if (!funcByDir[dir]()) return;
			AddRandomElement();
		}
	}
}
