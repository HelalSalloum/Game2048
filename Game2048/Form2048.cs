using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
	class Form2048 : Form
	{
		GameModel game;
		TableLayoutPanel table;
		int timeOnCycle;

		public Form2048(GameModel game)
		{
			DoubleBuffered = true;
			timeOnCycle = 40;
			this.game = game;

			table = new TableLayoutPanel();
			for (int i = 0; i < game.Size; i++)
			{
				table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / game.Size));
				table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / game.Size));
			}

			this.game.Start();
		}

		void DrawNumbers(System.Drawing.Graphics graphics)
		{
			float statPanelHeight = 40;
			float cellWidht = (float)ClientSize.Width / game.Size;
			float cellHeight = (float)(ClientSize.Height - statPanelHeight) / game.Size;

			if (game.Timer == -1)
			{
				for (int x = 0; x < game.Size; x++)
					for (int y = 0; y < game.Size; y++)
					{
						if (game.Board[x, y] == 0) continue;
						string number = (game.GetValue(game.Board[x, y])).ToString();
						graphics.DrawString(
							number,
							new Font("Arial", 16),
							Brushes.Blue,
							new RectangleF(x * cellWidht, statPanelHeight + y * cellHeight, cellWidht, cellHeight),
							new StringFormat
							{
								Alignment = StringAlignment.Center,
								LineAlignment = StringAlignment.Center,
								FormatFlags = StringFormatFlags.FitBlackBox
							}
							);
					}
				return;
			}

			for (int x = 0; x < game.Size; x++)
				for (int y = 0; y < game.Size; y++)
				{
					if (game.OldBoard[x, y] == 0) continue;
					float realX = ((float)x + (float)game.Timer / timeOnCycle * (game.MoveTo[x, y].Item1 - x));
					float realY = ((float)y + (float)game.Timer / timeOnCycle * (game.MoveTo[x, y].Item2 - y));
					string number = (game.GetValue(game.OldBoard[x, y])).ToString();
					graphics.DrawString(
						number,
						new Font("Arial", 16),
						Brushes.Blue,
						new RectangleF(realX * cellWidht, statPanelHeight + realY * cellHeight, cellWidht, cellHeight),
						new StringFormat
						{
							Alignment = StringAlignment.Center,
							LineAlignment = StringAlignment.Center,
							FormatFlags = StringFormatFlags.FitBlackBox
						}
						);
				}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;

			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			float statPanelHeight = 40;
			float cellWidht = (float)ClientSize.Width / game.Size;
			float cellHeight = (float)(ClientSize.Height - statPanelHeight) / game.Size;

			graphics.DrawString(
				"Score: " + (game.Timer == -1 ? game.Score.ToString() : game.OldScore.ToString()),
				new Font("Arial", 16),
				Brushes.Green,
				new Rectangle(0, 0, ClientSize.Width, (int)statPanelHeight),
				new StringFormat
				{
					Alignment = StringAlignment.Near,
					LineAlignment = StringAlignment.Center,
					FormatFlags = StringFormatFlags.FitBlackBox
				}
				);

			for (int i = 0; i <= game.Size; i++)
			{
				graphics.DrawLine(new Pen(Color.Black, 1),
					i * cellWidht, statPanelHeight, i * cellWidht, ClientSize.Height);
				graphics.DrawLine(new Pen(Color.Black, 1),
					0, statPanelHeight + i * cellHeight, ClientSize.Width, statPanelHeight + i * cellHeight);
			}
			DrawNumbers(graphics);

			if (game.Timer == -1 && game.GameOver())
			{
				graphics.DrawString(
					"GAME OVER",
					new Font("Arial", 30),
					Brushes.Red,
					new Rectangle(0, 0, ClientSize.Width, ClientSize.Height),
					new StringFormat
					{
						Alignment = StringAlignment.Center,
						LineAlignment = StringAlignment.Center,
						FormatFlags = StringFormatFlags.FitBlackBox
					}
					);
			}
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			Invalidate();
		}

		protected void EndMove()
		{
			game.Timer = -1;
			Invalidate();
		}
		
		protected void TryMoveInDirection(Direction dir)
		{
			if (!game.TryEvaluateMove(dir))
				return;
			game.Timer = 0;
			var timer = new System.Windows.Forms.Timer();
			timer.Interval = 1;
			timer.Tick += (sender, args) =>
				{
					game.Timer++;
					Invalidate();
					if (game.Timer == timeOnCycle)
					{
						timer.Stop();
						EndMove();
					}
				};
			timer.Start();
		}

		void TryInsertIntoScoreboard()
		{
			var askingForm = new AskingNameForm(game);
			askingForm.ShowDialog();
			var scoreboard = new Scoreboard(game.Size, game.type);
			scoreboard.AddNewScore(game.Player, game.Score);
			var scoreboardForm = new ScoreboardForm(scoreboard);
			scoreboardForm.ShowDialog();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (game.Timer != -1) return;
			if (e.KeyData == Keys.R)
			{
				game.Start();
				Invalidate();
				return;
			}
			if (game.GameOver()) return;
			if (e.KeyData == Keys.Up || e.KeyData == Keys.W)
				TryMoveInDirection(Direction.Up);
			else if (e.KeyData == Keys.Down || e.KeyData == Keys.S)
				TryMoveInDirection(Direction.Down);
			else if (e.KeyData == Keys.Left || e.KeyData == Keys.A)
				TryMoveInDirection(Direction.Left);
			else if (e.KeyData == Keys.Right || e.KeyData == Keys.D)
				TryMoveInDirection(Direction.Right);
			else
				return;
			if (game.GameOver())
				TryInsertIntoScoreboard();
		}
	}
}
