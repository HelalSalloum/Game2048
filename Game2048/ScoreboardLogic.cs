using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048
{
	public class Scoreboard
	{
		List<Tuple<string, int>> scores;
		FileInfo file;

		public Scoreboard(int size, GameType gameType)
		{
			string fileName = "Scoreboard" + gameType.ToString() + size.ToString() + ".txt";
			file = new FileInfo(Path.Combine(Environment.CurrentDirectory, fileName));
			scores = new List<Tuple<string, int>>();
		}

		void RestoreScoresFromFile()
		{
			if (!File.Exists(file.FullName))
			{
				scores.Clear();
				return;
			}
			string[] lines = File.ReadAllLines(file.FullName);
			scores.Clear();
			foreach (var line in lines)
			{
				var tmp = line.Split(' ');
				if (tmp.Length < 2)
					throw new ArgumentException();
				string name = "";
				for (int i = 0; i < tmp.Length - 1; i++)
				{
					name += tmp[i];
					if (i != tmp.Length - 2)
						name += " ";
				}
				scores.Add(new Tuple<string, int>(name, int.Parse(tmp[tmp.Length - 1])));
			}
		}

		public void ClearScoreboard()
		{
			scores.Clear();
			WriteScoresToFile();
		}

		void WriteScoresToFile()
		{
			string[] lines = new string[scores.Count];
			for (int i = 0; i < scores.Count; i++)
				lines[i] = scores[i].Item1 + " " + scores[i].Item2.ToString();
			File.WriteAllLines(file.FullName, scores.Select(
				(score) => (score.Item1 + " " + score.Item2.ToString())));
		}

		public void AddNewScore(string name, int score)
		{
			RestoreScoresFromFile();
			scores.Add(new Tuple<string,int>(name, score));
			scores = scores
				.OrderByDescending((item) => (item.Item2))
				.Take(10)
				.ToList();
			WriteScoresToFile();
		}

		public Tuple<string, int> GetKth(int k)
		{
			if (k <= 0 || k > 10)
				throw new ArgumentException();
			RestoreScoresFromFile();
			scores = scores
				.OrderByDescending((item) => (item.Item2))
				.Take(10)
				.ToList();
			if (k > scores.Count)
				return new Tuple<string, int>("", 0);
			return scores[k - 1];
		}
	}
}
