using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
	class ScoreboardForm : Form
	{
		Scoreboard board;

		public ScoreboardForm(Scoreboard board)
		{
			DoubleBuffered = true;
			this.board = board;

			var button = new Button
			{
				Location = new Point(0, (int)((float)ClientSize.Height / 12 * 11)),
				Size = new Size(ClientSize.Width, ClientSize.Height / 12),
				Text = "Clear Highscores"
			};
			Controls.Add(button);

			button.Click += (sender, args) =>
				{
					board.ClearScoreboard();
					Invalidate();
				};
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var graphics = e.Graphics;

			float cellWidht = (float)ClientSize.Width / 2;
			float cellHeight = (float)ClientSize.Height / 12;

			for (int i = 1; i <= 10; i++)
			{
				var score = board.GetKth(i);
				graphics.DrawString(
					i.ToString() + ". " + score.Item1,
					new Font("Arial", 16),
					Brushes.Black,
					new RectangleF(0, (i - 1) * cellHeight, cellWidht, cellHeight),
					new StringFormat
					{
						Alignment = StringAlignment.Near,
						LineAlignment = StringAlignment.Center,
						FormatFlags = StringFormatFlags.FitBlackBox
					}
					);
				graphics.DrawString(
					score.Item2.ToString(),
					new Font("Arial", 16),
					Brushes.Black,
					new RectangleF(cellWidht, (i - 1) * cellHeight, cellWidht, cellHeight),
					new StringFormat
					{
						Alignment = StringAlignment.Far,
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

	}
}
