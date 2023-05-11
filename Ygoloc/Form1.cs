using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace Ygoloc
{
    
    public partial class Form1 : Form
    {
        Bitmap main;
        Point last_selection = new Point(0,0);

        Game game;

        public Form1()
        {
            InitializeComponent();
            main = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            pictureBox1.Image = main;
            game = new Game(main, this);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //game.clean_last_point(last_selection,new Point(e.X / 50, e.Y / 50));


            last_selection.X = e.X / 50;
            last_selection.Y = e.Y / 50;



            label2.Text ="Y: " + Convert.ToString(e.Y / 50);
            label3.Text ="X: " + Convert.ToString(e.X / 50);
            pictureBox1.Image = main;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Graphics g = pictureBox1.CreateGraphics();
            

            
            byte is_win = game.click_to_item(e.X / 50, e.Y / 50);
            if (is_win == 1)
            {
                g.DrawString("Win!!!", label1.Font, new SolidBrush(Color.Green), new Point(40, 150));
                Thread.Sleep(5000);
            }
            else if (is_win == 2)
            {
                g.DrawString("lose :(", label1.Font, new SolidBrush(Color.Red), new Point(40, 150));
                Thread.Sleep(5000);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game.next_table();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            game.drow_game();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            game.previous_table();
        }

        public void update_progress(int max_value, int value, int ways)
        {
            progressBar1.Maximum = max_value;
            progressBar1.Value = value;
            label5.Text = "" + ways;
        }
    }
}
