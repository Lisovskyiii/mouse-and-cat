using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace mouse_and_cat
{
    class Program
    {
        static bool okey = true;
        static bool no_okey = false;
        static void Main(string[] args)
        {
            try
            {
                int all_cat = 0; int all_mouse = 0; int res_mouse = 0; int res_cat = 0; byte first_mouse = 0; byte first_cat = 0;
                string stroka = "";
                Start_File();
                ReadFile(stroka, ref all_cat, ref all_mouse, ref res_mouse, ref res_cat, ref first_mouse, ref first_cat);
                End_File(all_cat, all_mouse, res_mouse, res_cat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Error(e);
            }
        }
        public static void Error(Exception e)
        {
            FileStream f = new FileStream("PursuitLog.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(f, Encoding.Default);
            writer.WriteLine("\n{0}",e.Message);
            writer.Flush();
            f.Close();
        }
        public static void ReadFile(string stroka, ref int all_cat, ref int all_mouse, ref int res_mouse, ref int res_cat, ref byte first_mouse, ref byte first_cat)
        {
            FileStream f = new FileStream("ChaseData.txt", FileMode.Open);
            StreamReader reader = new StreamReader(f, Encoding.Default);
            int distance = Convert.ToInt32(reader.ReadLine().Trim());
            if (Check_distance(distance)== no_okey)
            {
                throw new Exception("Вы вышли за предел");
            }  
            while (!reader.EndOfStream)
            {
                stroka = reader.ReadLine();
                if (stroka != "")
                {
                    if (Check_Commands(stroka) == okey)
                    {
                        Wow( distance, stroka, ref all_cat, ref all_mouse, ref res_mouse, ref res_cat, ref first_cat, ref first_mouse);
                    }
                    if ((first_cat > 1 & first_mouse > 1) || (res_cat!=0 & res_mouse!=0))
                    {
                        if (Play(res_mouse, res_cat) == okey) { }
                        else { break; }
                    }
                }
            }
            f.Close();
        }
        public static bool Check_distance(int distance)
        {
            const int high = 10000;
            const int low = 1;
            if (distance >= low & distance <= high)
            {    return okey;}
            else
            {  return no_okey; }
        }
        public static bool Check_Commands(string stroka)
        {
            Regex regex = new Regex(@"[A-Z]{1}\s{0,}[-]{0,1}[0-9]{0,}");
            if (!regex.IsMatch(stroka))
              {
                throw new Exception("Invalid data entry"); }
                return okey; 
        }
        public static void Mouse(int m, int distance, ref int res_mouse)
        {
            res_mouse = res_mouse + m;
            while(res_mouse > distance)
            {
                res_mouse = res_mouse - distance;
            }
            while(res_mouse < 0)
            {
                res_mouse = distance + res_mouse;
            }
        }
        public static void Cat(int c, int distance, ref int res_cat)
        {
            res_cat = res_cat + c;
            while(res_cat > distance)
            {
                res_cat = res_cat - distance;
            }
            while(res_cat <0)
            {
                res_cat = distance + res_cat;
            }
        }
        public static bool Play(int res_mouse, int res_cat)
        {
            if (res_mouse == res_cat)
            { return no_okey; }
            else
            { return okey; }
        }
        public static void All_Mouse(int m, ref int all_mouse)
        {
            if (m < 0) { m = -m; }
            all_mouse = all_mouse + m;
        }
        public static void All_Cat(int c, ref int all_cat)
        {
            if (c < 0) { c = -c; }
            all_cat = all_cat + c;
        }
        public static void Start_File()
        {
            File.Delete("PursuitLog.txt");
            FileStream f = new FileStream("PursuitLog.txt", FileMode.OpenOrCreate);
            StreamWriter writer = new StreamWriter(f, Encoding.Default);
            writer.WriteLine("Cat and Mouse\n");
            writer.WriteLine("Cat Mouse Distance");
            writer.WriteLine("—-----------------");
            writer.Flush();
            f.Close();
        }
        public static void End_File(int all_cat,int all_mouse,int res_mouse, int res_cat)
        {
            FileStream f = new FileStream("PursuitLog.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(f, Encoding.Default);
            writer.WriteLine("------------------\n\n");
            writer.WriteLine("Distance traveled:   Mouse    Cat");
            writer.WriteLine("\t\t{0,10}{1,7}", all_mouse,all_cat);
            if (Play(res_mouse, res_cat) == okey)
            { writer.WriteLine("\nMouse evaded Cat"); }
            else
            {
                writer.WriteLine("\nMouse caught at:  {0,2}",res_cat);

            }
            writer.Flush();
            writer.Close();
        }
        public static void Wow( int distance, string stroka, ref int all_cat, ref int all_mouse, ref int res_mouse, ref int res_cat, ref byte first_mouse, ref byte first_cat)
        {
           byte count = 0;
           if (stroka.StartsWith("M")) {count++;}
           else if (stroka.StartsWith("C")) {count = 2;}
           else if (stroka.StartsWith("P")) { count = 3; }
           switch (count)
                    {
                        case 1:
                            first_mouse++;
                            int m = Convert.ToInt32(stroka.Remove(0, 1).Trim());
                            if (first_mouse> 1)
                            {
                                All_Mouse(m, ref all_mouse);
                            }
                            Mouse(m, distance, ref res_mouse);
                            break;
                        case 2:
                            first_cat++;
                            int c = Convert.ToInt32(stroka.Remove(0,1).Trim());
                            if (first_cat > 1)
                            {
                                All_Cat(c, ref all_cat);
                            }
                            Cat(c, distance , ref res_cat);
                            break;
                        case 3:
                            Input(res_mouse, res_cat, all_cat, all_mouse);
                            break;
                        default:
                            throw new Exception("You should use M,C,P");
                    }
        }
        public static void Input(int res_mouse, int res_cat, int all_cat, int all_mouse)
        {
            FileStream f = new FileStream("PursuitLog.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(f, Encoding.Default);
            string hm = "??";
            string clear = ""; 
            if (res_cat == 0 & res_mouse == 0)
            { writer.WriteLine("Cat and mouse don't play");  }
            else if (res_cat == 0)
            { writer.WriteLine("{0,3}{1,6}{2,9}", hm, res_mouse, clear); }
            else if (res_mouse == 0)
            { writer.WriteLine("{0,3}{1,6}{2,9}", res_cat, hm, clear); }
            else {int distance = Math.Abs(res_cat - res_mouse);
                    writer.WriteLine("{0,3}{1,6}{2,9}", res_cat, res_mouse,distance); }
            writer.Flush();
            f.Close();          
        }
    }
}
