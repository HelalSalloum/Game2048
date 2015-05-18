using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game2048
{
	public class AskingNameForm : Form
	{
		public AskingNameForm(GameModel game)
		{
			var label = new Label
			{
				Location = new Point(0, 0),
				Size = new Size(ClientSize.Width, 30),
				Text = "Enter your name"
			};
			var box = new TextBox
			{
				Location = new Point(0, label.Bottom),
				Size = label.Size
			};
			var button = new Button
			{
				Location = new Point(0, box.Bottom),
				Size = label.Size,
				Text = "OK"
			};
			Controls.Add(label);
			Controls.Add(box);
			Controls.Add(button);
			button.Click += (sender, args) => { 
				game.Player = box.Text;
				Close();
			};
		}
	}
}
