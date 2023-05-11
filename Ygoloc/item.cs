using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Ygoloc
{
    struct best_way
    {
        public double score { get; set; }
        public Point way { get; set; }
        public item i { get; set; }
    }

    class item
    {
        
        public Point position;
        public byte player=0;


        Pen bordr_pen = new Pen(Color.Gold, 5);

        SolidBrush one_pen = new SolidBrush(Color.Peru);
        SolidBrush two_pen = new SolidBrush(Color.Green);

        public Graphics g;

        public List<Point> way;
        public best_way b_way;


        public item(int x,int y, byte p, Graphics g)
        {
            position.X = x;
            position.Y = y;
            player = p;
            this.g = g;
            

            b_way.way = new Point(x, y);
            b_way.score = 0;
            b_way.i = this;
        }



        public void drow_item()
        {
            if (player==1)
            {
                g.FillEllipse(one_pen, position.X*50+5, position.Y*50+5, 40, 40);
            }
            else
            {
                g.FillEllipse(two_pen, position.X * 50 + 5, position.Y * 50 + 5, 40, 40);
            }
            
            g.DrawEllipse(bordr_pen, position.X * 50 + 5, position.Y * 50 + 5, 40, 40);
        }

        public List<Point> get_way()
        {
            List<Point> ways = new List<Point>();

            if (position.X + 1 < 8)
            {
                ways.Add(new Point(position.X+1, position.Y));
            }
            if (position.X-1>=0)
            {
                ways.Add(new Point(position.X - 1, position.Y));
            }
            if (position.Y+1<8)
            {
                ways.Add(new Point(position.X, position.Y + 1));
            }
            if (position.Y-1>=0)
            {
                ways.Add(new Point(position.X, position.Y - 1));
            }

            return ways;
        }

        public void set_position(Point new_position)
        {
            if (way.Contains(new_position))
            {
                position = new_position;
            }
        }
    }
}
