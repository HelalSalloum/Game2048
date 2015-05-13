using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
	class MyForm : Form
	{
		GameModel game;
		TableLayoutPanel table;

		public MyForm(GameModel game)
		{
			this.game = game;

			table = new TableLayoutPanel();
			for (int i = 0; i < game.Size; i++)
			{
				table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / game.Size));
				table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / game.Size));
			}

			this.game.Start();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;

			DoubleBuffered = true;
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			float statPanelHeight = 40;
			float cellWidht = (float)ClientSize.Width / game.Size;
			float cellHeight = (float)(ClientSize.Height - statPanelHeight) / game.Size;

			graphics.DrawString(
				"Score: " + game.Score.ToString(),
				new Font("Arial", 16),
				Brushes.Green,
				new Rectangle(0, 0, ClientSize.Width, (int)statPanelHeight),
				new StringFormat
				{
					Alignment = StringAlignment.Center,
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

			if (game.GameOver())
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

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (e.KeyData == Keys.R)
			{
				game.Start();
				Invalidate();
				return;
			}
			if (game.GameOver()) return;
			if (e.KeyData == Keys.Up || e.KeyData == Keys.W)
				game.EvaluateMove(Direction.Up);
			else if (e.KeyData == Keys.Down || e.KeyData == Keys.S)
				game.EvaluateMove(Direction.Down);
			else if (e.KeyData == Keys.Left || e.KeyData == Keys.A)
				game.EvaluateMove(Direction.Left);
			else if (e.KeyData == Keys.Right || e.KeyData == Keys.D)
				game.EvaluateMove(Direction.Right);
			else
				return;
			Invalidate();
		}
	}
}
