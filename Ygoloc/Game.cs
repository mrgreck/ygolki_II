using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace Ygoloc
{
    class Game
    {
        public item[,] Table;

        Form1 form;

        Graphics g;
        item null_item ;

        bool player=true;

        item select_item;


        List<List<byte[,]>> delta_tables;
        int delta_table_index = -1;

        public Game(Bitmap bmap, Form1 form)
        {
            g = Graphics.FromImage(bmap);
            null_item = new item(-10, -10, 0, g);
            this.form = form;
            start();
        }

        public bool next_table()
        {
            if (delta_tables.Count-1 > delta_table_index)
            {
                delta_table_index += 1;
                byte[,] delta_table = delta_tables[0][delta_table_index];
                drow_game(convert_to_item_table(delta_table));
                return true;
            }
            return false;
        }

        public bool previous_table()
        {
            if (delta_table_index>=1)
            {
                delta_table_index -= 1;
                byte[,] delta_table = delta_tables[0][delta_table_index];
                drow_game(convert_to_item_table(delta_table));
                return true;
            }
            return false;
        }


        public void start()
        {
            Table = new item[8, 8];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Table[7 - i, 7 - j] = new item(7- i, 7 - j, 1, g);
                    Table[i,j] = new item(i, j, 2, g);
                }
            }
            
            drow_game();
            player = true;
        }


        public item get_item(int x, int y)
        {
            if (Table[x, y] != null)
            {
                return Table[x, y];
            }
            else
            {
                return null_item;
            }
        }

        public byte click_to_item(int x,int y)
        {
            drow_way_select_item(false);
            byte result = 0;

            item p = get_item(x, y);
            if (p.player!=0)
            {
                if (p.player == 1 && player) 
                {
                    p.way = get_way(p, Table);
                    select_item = p;
                    drow_way_select_item();
                    return result;
                }
                else if (p.player == 2 && !player)
                {
                    p.way = get_way(p, Table);
                    select_item = p;
                    drow_way_select_item();
                    return result;
                }
            }
            else
            {
                if (select_item != null)
                {
                    if(select_item.way.Contains(new Point(x, y)))
                    {
                        Table[select_item.position.X, select_item.position.Y] = null;
                        select_item.set_position(new Point(x, y));
                        Table[select_item.position.X, select_item.position.Y] = select_item;
                        result = check_win();
                        if (result == 0)
                        {
                            player = !player;
                            if (!player)
                            {
                                Point[] way =  calculat_best_way();
                                select_item = Table[way[0].X, way[0].Y];
                                select_item.way = get_way(select_item, Table);

                                Table[select_item.position.X, select_item.position.Y] = null;
                                select_item.set_position(way[1]);
                                Table[select_item.position.X, select_item.position.Y] = select_item;


                                

                                player = !player;
                                result = check_win();
                            }
                        }
                    }
                }
                select_item = null;
            }
            drow_game();
            return result;
        }

        

        private List<List<byte[,]>> generate_tables(byte[,] table)
        {
            List<List<byte[,]>> result = new List<List<byte[,]>>();
            List<byte[,]> delta_result = generate_tables(table,2);

            foreach(byte[,] dela_table in delta_result)
            {
                result.Add(generate_tables(dela_table, 1));
            }

            return result;
        }


        private List<byte[,]> generate_tables(byte[,] table, byte player)
        {
            List<byte[,]> result = new List<byte[,]>();

            item[,] own_table = convert_to_item_table(table);

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    if (own_table[i, j] != null)
                    {
                        if (own_table[i, j].player == player)
                        {
                            List<Point> delta_ways = get_way(own_table[i, j], own_table);
                            foreach (Point way in delta_ways)
                            {
                                byte[,] delta_table = convert_to_byte_table(own_table);
                                delta_table[i, j] = 0;
                                delta_table[way.X, way.Y] = player;
                                result.Add(delta_table);
                            } 
                        }
                    }
                }
            }


            return result;
        }

        private List<List<byte[,]>> generate_tables(item[,] table)
        {
            byte[,] delta_table = convert_to_byte_table(table);
            List<List<byte[,]>> delta_result = generate_tables(delta_table);
            delta_tables = new List<List<byte[,]>>();

            int counter = 0;
            for (int i = 0; i < delta_result.Count; i++)
            {
                
                form.update_progress(delta_result.Count-1,i, counter);
                delta_tables.Add(new List<byte[,]> ());

                foreach (byte[,] x in delta_result[i])
                {

                    foreach (List<byte[,]> xx in generate_tables(x))
                    {
                        foreach (byte[,] xxx in xx)
                        {
                            delta_tables[i].Add(xxx);
                        }
                    }
                }
                counter += delta_tables[i].Count;
            }
            
            return delta_tables;
            
        }

        private Point[] calculat_best_way()
        {
            Point[] result = new Point[2];
            List<Point[]> first = first_ways();
            List<List<byte[,]>> futures = generate_tables(Table);

            int best_way=0;
            double best_score = int.MaxValue;

            for (int i =0; (i<first.Count) && (first.Count==futures.Count);i++)
            {
                int caunter = 0;
                double score = 0;
                foreach(byte[,] x in futures[i])
                {
                    caunter += 1;
                    score += calculat_way(x);
                }

                score = score / caunter;
                


                if((first[i][0].X>4)&& (first[i][0].Y > 4))
                {
                    score = score* 0.5;
                }


                if (best_score > score)
                {
                    best_way = i;
                    best_score = score;
                }
            }


            result = first[best_way];
            return result;
        }


        private double calculat_way(byte[,] delta_table)
        {
            double score = 0;
            double player_score = 0;
            double PC_score = 0;

            for (int i = 0; i < 8; i++) 
            {
                for(int j = 0; j < 8; j++)
                {
                    if (delta_table[i, j] == 1)
                    {
                        player_score += Math.Sqrt(Math.Pow(0 - i, 2) + Math.Pow(0 - j, 2)) * (i + j) * (0.3 * Math.Abs(i - j));
                    }
                    else if (delta_table[i, j] == 2)
                    {
                        PC_score += Math.Sqrt(Math.Pow(7 - i, 2) + Math.Pow(7 - j, 2)) * ((8 - i) + (8 - j))*(0.3*Math.Abs(i-j));

                        
                    }
                }
            }
            score = PC_score - player_score;

            return score;
        }


        private List<Point[]> first_ways()
        {
            List<Point[]> result = new List<Point[]>();
            int index = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Table[i, j] != null)
                    {
                        if (Table[i, j].player == 2)
                        {
                            
                            List<Point> delta_ways = get_way(Table[i, j], Table);
                            foreach (Point way in delta_ways)
                            {
                                result.Add(new Point[2]);
                                result[index][0] = new Point(i, j);
                                result[index][1] = way;
                                index += 1;
                            }

                            
                        }
                    }
                }
            }

            return result;
        }

        private byte[,] convert_to_byte_table(item[,] delta_table)
        {
            byte[,] result = new byte[8, 8];

            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (delta_table[i, j] != null)
                    {
                        result[i, j] = delta_table[i, j].player;
                    }
                }
            }

            return result;
        }

        private item[,] convert_to_item_table(byte[,] delta_table)
        {
            item[,] result = new item[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (delta_table[i, j] != 0)
                    {
                        result[i, j] = new item(i, j, delta_table[i, j], g);
                    }
                }
            }

            return result;
        }

        private List<Point> get_way(item p,item[,] T)
        {
            List<Point> ways = new List<Point>();

            foreach (Point pos in p.get_way())
            {
                if (T[pos.X, pos.Y] == null)
                {
                    ways.Add(pos);
                }
                else
                {
                    ways=get_chail_way(ways, p.position, pos, T);
                }
            }

            return ways;
        }

        private List<Point> get_chail_way(List<Point> way, Point old_pos, Point new_pos, item[,] T)
        {
            Point delta_pos = new Point(new_pos.X - old_pos.X, new_pos.Y - old_pos.Y);
            Point way_pos = new Point(new_pos.X + delta_pos.X, new_pos.Y + delta_pos.Y);
            if (way_pos.X >= 0 && way_pos.Y >= 0 && way_pos.X < 8 && way_pos.Y < 8 && T[new_pos.X, new_pos.Y] != null && T[way_pos.X, way_pos.Y] == null &&!way.Contains(way_pos)) 
            {
                way.Add(way_pos);
                if (way_pos.X-1 >= 0 )
                {
                    way = get_chail_way(way, way_pos, new Point(way_pos.X-1, way_pos.Y), T);
                }
                if(way_pos.Y-1 >= 0)
                {
                    way = get_chail_way(way, way_pos, new Point(way_pos.X, way_pos.Y-1), T);
                }
                if (way_pos.X+1 < 8)
                {
                    way = get_chail_way(way, way_pos, new Point(way_pos.X+1, way_pos.Y), T);
                }
                if (way_pos.Y+1 < 8)
                {
                    way = get_chail_way(way, way_pos, new Point(way_pos.X, way_pos.Y+1), T);
                }
            }
            
            return way;
        }

        private string drow_way_select_item(bool drow=true)
        {
            string result = "";
            if (select_item!=null)
            {
                foreach (Point ww in select_item.way)
                {
                    result += ww.X + " " + ww.Y + "; ";
                    select(ww, drow);
                } 
            }
            return result;
        }

        public void clean_last_point(Point last_point, Point new_point)
        {
            drow_game();
            drow_way_select_item();
            select(last_point, false);
            select(new_point, true);
            drow_items();
        }

        public void drow_game()
        {
            bool flag = true;
            for (int i = 0; i < 8; i++)
            {
                flag = !flag;
                for (int j = 0; j < 8; j++)
                {
                    flag = !flag;
                    select(new Point(i,j));
                }
            }
            drow_items();
        }

        public void drow_game(item[,] delta_table)
        {
            bool flag = true;
            for (int i = 0; i < 8; i++)
            {
                flag = !flag;
                for (int j = 0; j < 8; j++)
                {
                    flag = !flag;
                    select(new Point(i, j));
                }
            }
            drow_items(delta_table);
        }

        private void select(Point p, bool new_point=false)
        {
            if ((p.Y + p.X) % 2 == 0)
            {
                if (new_point)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(120, 120, 0)), p.X * 50, p.Y * 50, 50, 50);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 0)), p.X * 50, p.Y * 50, 50, 50);
                }
            }
            else
            {
                if (new_point)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(200, 200, 00)), p.X * 50, p.Y * 50, 50, 50);
                }
                else
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, 255, 255)), p.X * 50, p.Y * 50, 50, 50);
                }
            }
        }

        private void drow_items()
        {
            foreach (item x in Table)
            {
                if (x!=null)
                {
                    x.drow_item();
                }
            }
        }

        private void drow_items(item[,] delta_table)
        {
            foreach (item x in delta_table)
            {
                if (x != null)
                {
                    x.drow_item();
                }
            }
        }


        private byte check_win()
        {
            bool[] player_flag = new bool[2];
            bool[] cyp_flag = new bool[2];


            for (int i = 0; i < 2; i++)
            {
                player_flag[i] = false;
                cyp_flag[i] = true;
            }


            byte player_id;
            
            for (int i=0;i<3;i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    
                    if (Table[i, j] == null)
                    {
                        cyp_flag[0] = false;
                    }
                    else if (Table[i, j].player == 1)
                    {
                        player_flag[0] = true;
                    }
                    
                    if (Table[7-i, 7-j] == null)
                    {
                        cyp_flag[1] = false;
                    }
                    else if (Table[7-i, 7-j].player == 2)
                    {
                        player_flag[1] = true;
                    }
                    
                    
                    
                }
            }


            if (player_flag[0] && cyp_flag[0])
            {
                start();
                return 1;
            }
            else if(player_flag[1] && cyp_flag[1])
            {
                start();
                return 2;
            }

            return 0;
        }

    }
}
