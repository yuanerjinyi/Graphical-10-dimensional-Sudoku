using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using OpenCvSharp;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading;
using static System.Console;
namespace ConsoleApp1
{
    //用坍缩算法对数独进行求解，数独用矩阵数组表示，矩阵的维度是3，一二维分别表示数独的行列，第三维度的数据长度为12，
    //其中索引0表示当前位置块解得的数字，索引1-10表示待求位置可以填充的数字，索引11表示待求位置可以填入数字的可能个数。
    //对每个待求解的位置块做如下处理，对所有待求解位置块的第三维的索引11的数据进行升序处理，对求得的升序列进行遍历。升序历中是
    //下一次代求块的位置，在遍历中获取当前位置块的剩余可能解（可能填入的数字），对可能解进行遍历，一旦填入的数字满足要求，又将获取待解块的
    //的升序历，循环往复进行求解。直至数独中的所有块都填入数字并满足要求，数独求解完成。
    class Sudoku
    {
        int[,,] Su_matrix = new int[10,10,12]; //定义3维数独矩阵
        int hutiao = 0;
        List<int> Decomposed_layer = new List<int>();
        static int width = 500;                //画布宽高
        static int height = 500;               //
        string wind = "数独";                  //图像显示窗口名称

        static int width1 = 1920;                //画布宽高
        static int height1 = 451;               //
        string wind1 = "数独求解过程";                  //图像显示窗口名称

        // 创建一个黑色的空白图像
        Mat img = new Mat(new Size(width, height), MatType.CV_8UC3, new Scalar(0, 0, 0));
        Mat img1 = new Mat(new Size(width1, height1), MatType.CV_8UC3, new Scalar(0, 0, 0));
        //新建一个显示窗口，并移动窗口到指点位置
        private void Img_show()
        {
           
            Cv2.NamedWindow(wind, WindowFlags.AutoSize);
            Cv2.MoveWindow(wind, 300, 0);

            //Cv2.NamedWindow(wind1, WindowFlags.AutoSize);
            //Cv2.ImShow(wind1,img1);
            //Cv2.MoveWindow(wind1, 0, 1);

        }
        //显示数独图像
        private void Print_img()
        {
            Scalar Background = new Scalar(24,215,24);
            Scalar Wireframe_color = new Scalar(114,25,248);
            Scalar Numeral_color = new Scalar(255,17,17);
            int bh;
            img.SetTo(Background);
            float kuandu = (float)(height - 1) / 10;
            //int zb = (int)(i * kuandu);
            for (int i = 0; i <= 10; i++)
            {
              
                //WriteLine($"{kuandu,-8}{i*kuandu}");
                Cv2.Line(img, new Point(0, i * kuandu), new Point(499, i * kuandu), Wireframe_color, thickness: 1);
                Cv2.Line(img,new Point(i * kuandu, 0),new Point(i * kuandu, 499),Wireframe_color, thickness: 1);
            }
            for (int i=0;i<Su_matrix.GetLength(0);i++)
            {
                for (int j=0;j<Su_matrix.GetLength(1);j++)
                {
                    if (Su_matrix[i, j, 0] != 0)
                    {
                        Size zwh = Cv2.GetTextSize($"{Su_matrix[i,j,0]}", HersheyFonts.HersheyComplex, 1, 1, out bh);
                        Cv2.PutText(img, $"{Su_matrix[i, j, 0]}", new Point((2*j+1)*kuandu/2-zwh.Width/2-4,(2*i+1)*kuandu/2+zwh.Height/2 ), HersheyFonts.HersheyComplex, 1, Numeral_color);

                    }
                }
            }
            
            //WriteLine($"绘制的文字宽高{zwh}\t字的基线{bh}");
            Cv2.ImShow(wind,img);
            Cv2.WaitKey(1);
        }
        //显示方法，用颜色代替数字，并显示出来
        private void Print_img1()
        {
            Scalar Background = new Scalar(0, 0, 0);
            Scalar Wireframe_color = new Scalar(255,255,255);
            int bh;
            List<Scalar> colors = new List<Scalar>
            {
            new Scalar(0, 0, 255),   // 红色
            new Scalar(255, 0, 0),   // 蓝色
            new Scalar(0, 255, 0),   // 绿色
            new Scalar(0, 255, 255), // 黄色
            new Scalar(0, 165, 255), // 橙色
            new Scalar(128, 0, 128), // 紫色
            new Scalar(255, 255, 0), // 青色
            new Scalar(255, 0, 255), // 粉红色
            new Scalar(19, 69, 139), // 棕色
            new Scalar(128, 128, 0)  // 青绿色
            };
            img.SetTo(Background);
            float kuandu =(float)(height - 1) / 10;
            //int zb = (int)(i * kuandu);
            for (int i = 0; i <= 10; i++)
            {

                //WriteLine($"{kuandu,-8}{i*kuandu}");
                Cv2.Line(img, new Point(0, i * kuandu), new Point(499, i * kuandu), Wireframe_color, thickness: 2);
                Cv2.Line(img, new Point(i * kuandu, 0), new Point(i * kuandu, 499), Wireframe_color, thickness: 2);
            }
            for (int i = 0; i < Su_matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Su_matrix.GetLength(1); j++)
                {
                    if (Su_matrix[i, j, 0] != 0)
                    {

                       
                            Rect region = new Rect((int)(j * kuandu), (int)(i *kuandu), (int)kuandu, (int)kuandu); // (x, y, 宽度, 高度)
                            img.Rectangle(region, colors[Su_matrix[i, j, 0] - 1], -1); // 在蒙版上绘制一个填充矩形
                    }
                }
            }

            //WriteLine($"绘制的文字宽高{zwh}\t字的基线{bh}");
            Cv2.ImShow(wind, img);
            Cv2.WaitKey(1);
        }
        public void Print_img2()
        {
            Scalar Background = new Scalar(0, 0, 0);
            Scalar Wireframe_color = new Scalar(255, 255, 255);
            //int bz = Decomposed_layer.Count > 15? Decomposed_layer.Count - 15:0;
            int bz = 73;
            List<Scalar> colors = new List<Scalar>
            {
            new Scalar(0, 0, 255),   // 红色
            new Scalar(255, 0, 0),   // 蓝色
            new Scalar(0, 255, 0),   // 绿色
            new Scalar(0, 255, 255), // 黄色
            new Scalar(0, 165, 255), // 橙色
            new Scalar(128, 0, 128), // 紫色
            new Scalar(255, 255, 0), // 青色
            new Scalar(255, 0, 255), // 粉红色
            new Scalar(19, 69, 139), // 棕色
            new Scalar(128, 128, 0)  // 青绿色
            };
            img1.SetTo(Background);
            int kuandu = (height1 - 1) / 10;

            //int zb = (int)(i * kuandu);
            if (Decomposed_layer.Count >= 75)
            {
                for (int i = bz; i < Decomposed_layer.Count; i++)
                {
                    int row = Decomposed_layer[i] / 10;
                    int column = Decomposed_layer[i] % 10;
                    Rect region = new Rect((int)((i - bz) * 2 * kuandu), (int)((Su_matrix[row, column, 0] - 1) * kuandu), (int)kuandu, (int)kuandu); // (x, y, 宽度, 高度)
                    img1.Rectangle(region, colors[Su_matrix[row, column, 0] - 1], -1); // 矩形颜色填充
                    if (i != Decomposed_layer.Count - 1)
                    {

                        int row1 = Decomposed_layer[i + 1] / 10;
                        int column1 = Decomposed_layer[i + 1] % 10;
                        Point point_start = new Point((2 * (i - bz) + 1) * kuandu, (Su_matrix[row, column, 0] - 1) * kuandu + kuandu / 2);
                        Point point_end = new Point((2 * (i - bz) + 2) * kuandu, (Su_matrix[row1, column1, 0] - 1) * kuandu + kuandu / 2);
                        Cv2.Line(img1, point_start, point_end, Wireframe_color, thickness: 1);
                    }
                }
            }
            
            //WriteLine($"绘制的文字宽高{zwh}\t字的基线{bh}");
            Cv2.ImShow(wind1, img1);
            Cv2.WaitKey(1);
        }
        public void Img()
        {
            Img_show();
            Print_img();

        }
        //数字填充判定，当在待解块内填入一个数字，用此方法对当前数字进行判定，是否满足要求
        private bool Pdetermined(int shu, int[] dw,int[,,] matrix)
        {
            bool bz = true;
            int weidu = matrix.Length;
            for (int i=0;i<matrix.GetLength(0);i++)
            {
                if (i != dw[1] && matrix[dw[0], i, 0]!=0)
                {
                    if (matrix[dw[0], i, 0] == shu)
                        return false;
                            
                }
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (i != dw[0] && matrix[i,dw[1], 0] != 0)
                {
                    if (matrix[i,dw[1], 0] ==shu)
                        return false;

                }
            }
            return true;
        }
        //数独求解主方法，调用此方法，程序开始求解数独
        public void Sudoku_main()
        {
            Img_show();
            List<student> zxk = new List<student>();
            Init();

            Min_value(zxk,Su_matrix);
            bool bz = true;
            WriteLine("数独初始化");

            Print();
            //int currentRow = Console.CursorTop;
            //Console.WriteLine($"当前输出行的位置是第 {currentRow + 1} 行。");
            foreach (var i in zxk)
            {
                if(Recursive_solving(new[] { i.AgeSerial_number / 10, i.AgeSerial_number % 10 }, Su_matrix))
                {
                    bz = false;
                    WriteLine($"数独解算成功");
                    Print();
                    Cv2.WaitKey(0);
                    Cv2.DestroyWindow(wind);
                    break;
                }

            }
            if (bz)
                WriteLine("数独求解失败");

        }
        //递归求解模块（核心），利用递归对数独的每个块进行求解
        private bool Recursive_solving(int[] dw,int[,,] matrix)
        {
            Decomposed_layer.Add(dw[0]*10+dw[1]);
            Console.SetCursorPosition(0, 552);
            Print();
            //Thread.Sleep(33);
            ////Clear();
            int dqz = matrix[dw[0], dw[1], 11];
            //int[] Dw = new int[2];
            //Array.Copy(dw, Dw, dw.Length);
            List<int> djz = new List<int>();
            List<int> csz = new List<int>();
            List<student> zxk = new List<student>();

            Residual_value(djz,dw,matrix);
            foreach (var i in djz)
            {
                if (Pdetermined(i,dw,matrix))
                {
                    Number_fill(i,dw,csz,matrix);
                    Print_img1();
                    //Print_img2();
                    if (Remaining_blocks(matrix))
                        return true;
                    Min_value(zxk,matrix);
                    foreach (var j in zxk)
                    {
                       
                        if (Recursive_solving(new[] { j.AgeSerial_number / 10, j.AgeSerial_number % 10 }, matrix))
                            return true;
                        else if (Decomposed_layer.Count!=hutiao)
                        {
                            zxk.Clear();
                            Reduction(i, dw, csz, matrix);
                            Number_restores(dqz, dw, matrix);
                            Decomposed_layer.RemoveAt(Decomposed_layer.Count - 1);
                            return false;
                        }

                    }
                    zxk.Clear();
                    Reduction(i, dw, csz, matrix);
                }
            }
            hutiao = Decomposed_layer.Count > 30? Decomposed_layer.Count - 30: 1;
            Number_restores(dqz,dw,matrix);
            Decomposed_layer.RemoveAt(Decomposed_layer.Count - 1);
            return false;
        }
        //判定当前数字填入块是否满足要求
        private bool Remaining_blocks(int[,,] matrix)
        {
            for (int i=0;i<matrix.GetLength(0);i++)
            {
                for (int j=0;j<matrix.GetLength(1);j++)
                {
                    if (matrix[i, j, 0] == 0)
                        return false;
                }
            }
            return true;
        }
        //当前块的数值填充，当前块填入数字经判定满足要求后，将该数字填入当前块
        private void Number_fill(int shu,int[] dw,List<int> jl,int[,,] matrix)
        {
            matrix[dw[0], dw[1], 0] = shu;
            matrix[dw[0], dw[1], shu] = 0;
            matrix[dw[0], dw[1], 11] = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i != dw[1] && matrix[dw[0], i,11]!=0 && matrix[dw[0], i,shu]!=0)
                {
                    matrix[dw[0],i, shu] = 0;
                    matrix[dw[0],i, 11]--;
                    jl.Add(dw[0]*10+i);
                }
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                if (i != dw[0] && matrix[i, dw[1],11] != 0 && matrix[i,dw[1],shu]!=0)
                {
                    matrix[i, dw[1], shu] = 0;
                    matrix[i,dw[1],11]--;
                    jl.Add(i*10+dw[1]);
                }
            }
        }
        //将当前块数据还原
        private void Reduction(int shu,int[] dw,List<int> jl,int[,,] matrix)
        {
            matrix[dw[0], dw[1], 0] = 0;
            matrix[dw[0], dw[1], shu] = shu;
            matrix[dw[0], dw[1], 11] = 0;
            foreach (var i in jl)
            {
                matrix[i / 10, i % 10, shu] = shu;
                matrix[i / 10, i % 10, 11]++;
            }
            jl.Clear();
        }
        //将当前块可能解的个数还原
        private void Number_restores(int shu,int[] dw,int[,,] matrix)
        {
            matrix[dw[0], dw[1], 11] = shu;

        }
        //返回当前块可能解的数值列表，这个数字列表是被随机打乱的
        private void Residual_value(List<int> li,int[] dw,int[,,] matrix)
        {
            int rand;
            int[] cs = new int[10];
            do
            {
                rand = rand10();
                for (int j = 0; j < cs.Length; j++)
                {
                    if (cs[j] == 0)
                    {
                        cs[j] = rand;
                        if (matrix[dw[0], dw[1], rand] != 0)
                            li.Add(rand);
                        break;
                    }
                    else if (cs[j] == rand)
                    {
                        j = -1;
                        rand = rand10();
                    }
                }
            } while (li.Count!=matrix[dw[0],dw[1],11]);
        }
        //返回下一次待解块的有序升序列
        private void Min_value(List<student> li,int[,,] matrix)
        {
            for (int i=0;i<matrix.GetLength(0);i++)
            {
                for (int j=0;j<matrix.GetLength(1);j++)
                {
                    if (matrix[i, j, 0] == 0)
                        li.Add(new student(matrix[i,j,11],i*10+j));
                }
            }
            li.Sort(new stupara());

        }
        

        public bool Collapse_algorithm(int[] dw,ref int[,,] matrix)
        {


            return true;
        }
        //0-99范围内的随机数生成
        public byte rangd100()
        {
            byte[] randomBytes = new byte[1];
            byte Max = (byte.MaxValue / 100) * 100;
            RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
            do
            {
                rngServiceProvider.GetBytes(randomBytes);

            } while (!(randomBytes[0]<Max));


            return (byte)(randomBytes[0]%100);
        }
        //1-10 随机数生成
         public byte rand10()
        {
            byte[] randomBytes = new byte[1];
            byte Max = (byte.MaxValue /10) * 10;
            RNGCryptoServiceProvider rngServiceProvider = new RNGCryptoServiceProvider();
            do
            {
                rngServiceProvider.GetBytes(randomBytes);

            } while (!(randomBytes[0] < Max));


            return (byte)((byte)(randomBytes[0] % 10)+1);
        }
        //数独初始化，在数独内随机选取10个块，填入数字，进行数独的初始化
        public void Init()
        {
            byte[] Sjcs = new byte[10];
            bool bz = true;
            for (int i=0;i<Su_matrix.GetLength(0);i++)
            {
                for (int j=0;j<Su_matrix.GetLength(1);j++)
                {
                    for (int k=0;k<Su_matrix.GetLength(2);k++)
                    {
                        if (k == Su_matrix.GetLength(2) - 1)
                            Su_matrix[i, j, k] = 10;
                        else
                            Su_matrix[i, j, k] = k;
                    }
                }
            }
            for (int i=0;i<Sjcs.Length;)
            {
                Sjcs[i] = rangd100();
                int[] dw = new int[2] {Sjcs[i]/10,Sjcs[i]%10 };
                int rand;
                int[] cs = new int[10];
                List<int> libi = new List<int>();
                for (int j = 0; j < i; j++)
                {
                    if (Sjcs[i] == Sjcs[j])
                        continue;
                }
                do
                {
                    rand = rand10();
                    for (int j = 0; j < cs.Length; j++)
                    {
                        if (cs[j] == 0)
                        {
                            cs[j] = rand;
                            break;
                        }
                        else if (cs[j] == rand)
                        {
                            j = -1;
                            rand = rand10();
                        }
                    }
                    //Su_matrix[dw[0], dw[1], 0] = rand;
                } while (!Pdetermined(rand,dw , Su_matrix));
                Number_fill(rand,dw,libi,Su_matrix);
                i++;
            }
            //foreach (var az in randomBytes)
            ////WriteLine($"{az}");
            //Int32 result = BitConverter.ToInt32(randomBytes, 0);
            //WriteLine($"{result}");


        }
        //数独的显示，此方法将数独输出到控制台并实时刷新显示
        public void Print()
        {
            for (int i=0;i<Su_matrix.GetLength(0);i++)
            {
                Write($"{" ",40}");
                for (int j=0;j<Su_matrix.GetLength(1);j++)
                {
                    if (Su_matrix[i, j, 0] == 0)
                        Write($"{" ",4}") ;
                    else
                        Write($"{Su_matrix[i,j,0],4}");
                }
                WriteLine("\n");
            }

        }


    }
    //定义的数据类，该类用于存储数独块的待解数字个数和块的序号
    class student
    {
        public student(int remainder, int AgeSerial_number) 
        { 
            this.remainder = remainder; 
            this.AgeSerial_number = AgeSerial_number; 
        }
        public int remainder;
        public int AgeSerial_number;
    }
    //比较函数 用于student类的自定义比较函数
    class stupara : IComparer<student>
    {
        public int Compare(student x, student y)
        {

            return x.remainder.CompareTo(y.remainder);

        }
    }
    class cheshi
    {
        public void cs(student sv,List<student> ja)
        {
            ja.Add(sv);
        }

    }
    class Img_cl
    {
        public static void Imgli1()
        {
            string[] img_format = { "jpg", "png", "jpeg" };
            string[] filel = Directory.GetFiles(@"D:\Image\验证");
            WriteLine($"第一个文件{filel[0]}");
            Console.WriteLine(String.Join(Environment.NewLine, filel));
            for (int i4 = 0; i4 < filel.Length; i4++)
            {
                string filename = filel[i4].Substring(filel[i4].LastIndexOf(@".") + 1).ToLower();
                bool biaozhi = false;
                for (int i5 = 0; i5 < img_format.Length; i5++)         //可以用foreach(var sri in img_format){}
                {
                    if (filename == img_format[i5])
                    {
                        biaozhi = true;
                        break;
                    }
                }

                if (biaozhi)
                {
                    using (Mat img = Cv2.ImRead(filel[i4]))
                    {
                        //Mat img1 = new Mat();
                        //Cv2.CvtColor(img, img, ColorConversionCodes.BGR2GRAY);
                        Cv2.NamedWindow("Demo", WindowFlags.KeepRatio);
                        Cv2.MoveWindow("Demo", 0, 0);
                        Cv2.ImShow("Demo", img);
                        Cv2.WaitKey(500);
                        //Cv2.DestroyWindow("Demo");
                    };
                }

                WriteLine(filename);
            }
        }
        public static void Imgli2()
        {
            Mat img, res, drs;
            int bh;
            string wind = "Windows1";
            img = Cv2.ImRead(@"D:\Image\验证\lsfjlejljlkk_0.jpg");
            Cv2.NamedWindow(wind, WindowFlags.AutoSize);
            Cv2.MoveWindow(wind, 0, 0);
            Cv2.ImShow(wind,img);
            Cv2.WaitKey(500);
            Cv2.Resize(img, res = new Mat(), new Size(), 0.4, 0.4);
            Size zwh= Cv2.GetTextSize("@#$%^*.", HersheyFonts.HersheyComplex, 0.3, 1, out bh);
            WriteLine($"绘制的文字宽高{zwh}\t字的基线{bh}");
            Cv2.PutText(res,"fljljlfj",new Point(res.Width/2,200), HersheyFonts.HersheyPlain, 0.1,new Scalar(0,0,255));
            Cv2.ImShow(wind,res);
            Cv2.WaitKey(500);
            Cv2.DestroyWindow(wind);
            WriteLine($"图片宽高属性：{res.Width}*{res.Height}\n图片第一行第一列的像素值{res.At<Vec3b>(0,0)[0]}");
            for (short bz=0;bz<res.Width;bz++)
            {
                //WriteLine($"{res.At<Vec3b>(0, bz)}");
            }

        }
    }
    enum Mju
    {
        Red,
        Bul,
        Rea
    }
    
    class Qrt
    {
        private int a1;
        public int A
        {
            get
            {
                return a1;
            }
        }
    }
    static class Rqrt
    {
        static public Qrt Red()
        {
            Qrt b = new Qrt();
            return b;
        }
    }
    struct Rw
    {
        public int a, b, c;


    }
    class Math
    {
        private int a, b, c;
        public Math(int a,int b,int c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }
        public int A
        {
            set { a = value; }
            get { return a; }
        }
        public int Sum()
        {
            return a + b + c;
        }
    }
    struct Math1
    {
        public static void Cd(Math a,int b)
        {
            a.A = b;
        }
    }
    static class Average
    {
       static public int average(this Math md) //扩展方法要求类久方法都是静态的
        {
            return md.Sum() / 3;
        }

    }
    class Class1
    {
        public string name = "时天";
        public void dy()
        {
            Console.WriteLine($"方法一调用\t名字属性{name}");

        }


    }
    class Class2 : Class1                 //类的继承 Class2类继承了Class1
    {
        public int age = 25;
        public void dy1()
        {
            Console.WriteLine($"方法二调用\t年龄属性{age}");
        }

    }
    class SuoYing
    {
        string name1, name2;
        public string this[int bj]
        {
            set
            {
                switch (bj)
                {
                    case 0: name1 = value;
                        break;
                    case 1:name2 = value;
                        break;
                    default:
                        break;

                }
             
            }
            get
            {
                switch (bj)
                {
                    case 0:return name1;
                    case 1:return name2;
                    default:return null;
                }
            }


        }



    }
    class RSX
    {
        private DateTime data;
        private string name;
        private string sex;
        public RSX()                       //g构造函数
        {
            data = DateTime.Now;
            Console.WriteLine($"时间\t{data}");

        }
        public DateTime Data
        {
            get { return data; }
        }
        public string Name
        {
            set { name = value; }
            get { return name; }


        }
        public string Sex
        {

            set
            {
                if (value == "男" || value == "女")
                    sex = value;
                else
                    Write("输入错误性别只能为男或女\n");

            }
            get { return sex; }

        }


    }
    class DG     //斐波拉数递归求解
    {
        public static ulong digui(int bg)    //方法递归
        {
            if (bg == 1 || bg == 2)
                return 1;
            else
                return digui(bg - 1) + digui(bg - 2);

        }
    }
    class wl    //类声明
    {
        public int a, b,c=100;
        static public int jtbl = 100;
        static public int add()
        {

            //return (a + b);
            return 100;
        }
        public void daydata()
        {

            DateTime dt = DateTime.Now;
            Console.Write(dt);
        }
    }
    static class jtl
    {
        public const int changliang = 100;
        static public void jfa()
        {
            WriteLine("静态类成员的调用");

        }
    }
    class Program
    {
        static void Slie(ref wl af1,ref int bc)
        {
            Write($"af1 原始值{af1.c}\n");
            af1.c = 200;
            Write($"af1的值被改变为{af1.c}\n");
            //Write($"bc输入的值为{bc}");

        }
       
        static void Main(string[] args)
        {
            bool bl = true;
            int ac = 100;
            int fa, fb;
            ulong a1, a2, a3, a4;
            double fc = 200.352,fd=30.2;
            string bc;
            string b = @"fsfe\\5\n55////\\\\\\\\\22"; //@ 字符串原格式输出
            Console.WriteLine($"{fc%fd}");
            Console.WriteLine("The first app in Beginning Visual C# 2015!1225fsfdee时间逻辑");
            Console.WriteLine($"{b}{ac}");
            Console.WriteLine("占位符{0}{0}{1}",20,30); //{}占位符
            int buj = 4;
            buj <<= 2;
            Write($"移位操作{buj}\n");
            Write($"移位运算符左移{1<<1}\n移位运算符右移{2>>1}\n");
            //静态字段
            Write($"类的静态变量外部调用 {wl.jtbl}\n");
            wl shili2 = new wl();
            //jtl jsl=new jtl(); 静态类无法实列化
            jtl.jfa();
            //jtl.changliang = 200; 无法访问
            WriteLine($"常量{jtl.changliang}");
            //jtl.changliang= 20; 常量无法被赋值
            //Write($"用实列调用静态变量{shili2.jtbl}\n");无法用实列类型对静态类型字段进行访问
            /*Console.WriteLine("输入你的名字");
            bc=Console.ReadLine();
            Console.WriteLine($"欢迎你{bc}");
            Console.WriteLine("输入第一个数字");
            fa = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"输入的数字为{fa}");
            Console.WriteLine("输入第二个数字");
            fb = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"输入的第二个数字为{fb}");
            Console.WriteLine($"两个数字之和为\t{fa}+{fb}={fa+fb}");
            Console.WriteLine("fslfjleiifk");
            WriteLine($"{5&15}\t{10^5}");//位运算符
            string ga = Console.ReadLine().ToLower();//字符转换为小写
            if (ga == "125")
            {
                WriteLine("输入正确");
            }
            else
            {
                WriteLine("输入不正确");
            }*/
            int b12 = 13;
            Console.WriteLine($"三元运算符?:{(b12 == 13 ? "正确":"不正确")}");//三元运算符
            a1 = 1;
            a2 = 1;
            a3 = 2;
            a4 = 0;
            for (int a=4;a<50;a++)
            {
                a4 = a3 + a1;
                a3 = a4;
                a2 = a3;
                a1 = a2;
                if ((a - 4) % 3 == 0)
                    Console.WriteLine("");
                Console.Write($"{a4,-20}");



            }
            int ba1 = 30;
            //{a,20:F5}  格式说明符
            for (ulong a=0;a<100;a++)
            {
                if (a % 2 == 0)
                    WriteLine($"|{a,20:F5}是偶数|");
                else
                    WriteLine($"|{a,-20:c}是奇数|");
            }
            for (int a = 1; a <= 9; a++)
            {
                for (int b1=1; b1<=a;b1++)
                {
                    Write($"{b1}*{a}={b1*a}\t");
                }
                Write("\n");
            }
            wl Wl1 = new wl();
            Wl1.a = 10;
            Wl1.b = 20;
            Console.WriteLine($"类输出的数据{wl.add()}");
            Random rd = new Random();
            for (int aaq=1;aaq<=1000;aaq++)
            {
                Console.Write($"{rd.Next(1,10)}\t");
                if (aaq % 10 == 0)
                {
                    Wl1.daydata();
                    Console.Write("\n");
                }
            }
            int aj = 20;
            wl shilie = new wl();
            Write($"w1实列c的值为{shilie.c}\n");
            Slie(ref shilie, ref aj);
            Write($"w1实列的值改变为{shilie.c}\n");
            /*while (true)
            {
                Write($"输入斐波那契数序号\n");
                int abc1 = 0;
                string str1 = ReadLine();
                bool flag = int.TryParse(str1, out abc1);
                if (flag)
                {
                    Write($"序号对应的斐波拉数字为{DG.digui(abc1)}\n");
                }
                //Write($"输入的斐波拉为{DG.digui(Convert.ToUInt64(ReadLine()))}\n");
                else if (str1 == "tc")
                {
                    WriteLine($"已退出斐波拉循环");
                    break;
                }
                   
                else
                    Write("输入错误\n");
            }*/
          /*  RSX r1 = new RSX();
            Console.WriteLine("请输入姓名");
            r1.Name = Console.ReadLine();
            Console.WriteLine("请输入性别");
            r1.Sex = Console.ReadLine();
            WriteLine($"r1的属性\t姓名：{r1.Name}\t性别：{r1.Sex}\t日期：{r1.Data}");*/
            //索引
            SuoYing Sy = new SuoYing();
            Sy[0] = "第一个名字";
            //Sy[1] = "第二个名字";
            Console.WriteLine($"{Sy[0]}\t{Sy[1]}");
            Class2 lei1 = new Class2();            //
            Console.WriteLine($"{lei1.name}\t{lei1.age}");
            string zf1 = "*";
            int py = 60;
            for (int bz1 = 10+py; bz1 >=1+py;bz1--)
            {
                if (bz1 == 10 + py)
                {
                    for (int bz3 = bz1; bz3 > 1; bz3--)
                        Write(" ");
                    WriteLine($"{zf1}");
                }
                else
                {
                    for (int bz2=bz1-1;bz2>0;bz2--)
                        Write(" ");
                    Write($"{zf1}");
                    for (int bz3 = 2*(10+py) - 2 * bz1 - 1; bz3 > 0; bz3--)
                        Write(" ");
                    WriteLine($"{zf1}");
                }
            }
            for (int bz1 =  py; bz1 <= 10 + py; bz1++)
            {
                if (bz1 == 10 + py)
                {
                    for (int bz3 = bz1; bz3 > 1; bz3--)
                        Write(" ");
                    WriteLine($"{zf1}");
                }
                else
                {
                    for (int bz2 = bz1 - 1; bz2 > 0; bz2--)
                        Write(" ");
                    Write($"{zf1}");
                    for (int bz3 = 2 * (10 + py) - 2 * bz1 - 1; bz3 > 0; bz3--)
                        Write(" ");
                    WriteLine($"{zf1}");
                }
            }
            Math md = new Math(10,20,30);
            WriteLine($"调用Math的求和方法{md.Sum()}");
            //用扩展方法求平均值
            WriteLine($"扩展方法的调用求平均值{md.average()}");
            for (int i=0;i<10;i++)
            {
                switch (i)
                {
                    case 5:
                        for (int j=i;j>0;j--)
                        {
                            WriteLine($"{j}");
                        }
                        break;
                }

            }
           
            //Mat srcImage = new Mat(@"D:\Image\wangye.jpg");
            

            //Cv2.ImShow("伪彩色",img);
            //Cv2.WaitKey(1000);
            string pij1 = @"D:\书籍\c#";
            string pij2 = @"\02.jpg";
            WriteLine(pij1+pij2);
            Qrt sqf = Rqrt.Red();
            WriteLine($"Qrt的实例返回值{sqf.A}");

            //Img_cl.Imgli2();
            //Img_cl.Imgli1();
            Class1 byz=new Class1();
            WriteLine($"{byz.name}");
            Rw Strj=new Rw();                     //结构体可以用new实例化
            WriteLine($"{Strj.a}");
            Rw strj1;                             //结构也可以不用new实例化，但要进行初始化
            strj1.a = 100;                         
            WriteLine($"{strj1.a}");            
            Math lz1 = new Math(3,4,4);
            WriteLine($"lz1类的a值为{lz1.A}");   //
            Math1.Cd(lz1,100);                   //以类传递参数是以引用的形式在传递
            WriteLine($"lz1类的a值为{lz1.A}");

            //枚举类型是值类型
            Mju t1 = Mju.Red;
            Mju t2 = Mju.Bul;
            Mju t3 = Mju.Rea;
            WriteLine($"t1:{t1}={(int)t1}\tt2:{t2}={(int)t2}\tt3:{t3}={(int)t3}");

            //数组
            int[,,] shuzhu = new int[3,10, 20];
            WriteLine($"数组的维度：{shuzhu.Rank}");
            WriteLine($"数组的第一维度的长度:{shuzhu.GetLength(0)}");
            WriteLine($"数组的第二维度的长度:{shuzhu.GetLength(1)}");
            WriteLine($"数组的第三维度的长度:{shuzhu.GetLength(2)}");

            /* RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
             byte[] byteCsp = new byte[20];
             csp.GetBytes(byteCsp);
             Console.WriteLine(BitConverter.ToString(byteCsp));*/
            Sudoku sdsl = new Sudoku();
            int[] jsl = new int[10];
            for (int i=0;i<1000;i++)
            {
                if(i%10==0)
                    WriteLine("");
                int j = sdsl.rand10();
                jsl[j - 1]++;
                Write($"{j}\t");
            }
            WriteLine("");
            for (int i=0;i<jsl.Length;i++)
            {
                WriteLine($"{i+1}出现的频率：{(float)jsl[i]/100}");
            }
            int[] cs = new int[100];
            int rand;
            do
            {
                rand = sdsl.rangd100()+1;
                for (int j = 0; j < cs.Length; j++)
                {
                    if (cs[j] == 0)
                    {
                        cs[j] = rand;
                        WriteLine($"第{j}个数：{rand}");
                        break;
                    }
                    else if (cs[j] == rand)
                    {
                        j = -1;
                        rand =sdsl.rangd100()+1;
                    }
                }
            } while (cs[99]==0);

            foreach (var i in cs)
                Write($"{i}\t");
            Array.Sort(cs);
            WriteLine("");
            foreach (var i in cs)
                Write($"{i}\t");

            //列表
            List<student> liebiao = new List<student>();
            liebiao.Add(new student(10,20));
            liebiao.Add(new student(5, 19));
            liebiao.Add(new student(11, 8));
            liebiao.Add(new student(3, 50));
            liebiao.Add(new student(20, 61));
            liebiao.Add(new student(5, 6));
            liebiao.Sort(new stupara());
            WriteLine($"\n列表数据长度{liebiao.Count}");
            foreach (var ju in liebiao)
                WriteLine($"{ju.remainder,-10}{ju.AgeSerial_number}");
            liebiao.RemoveAt(0);
            WriteLine($"列表数据长度{liebiao.Count}");
            foreach (var ju in liebiao)
                WriteLine($"{ju.remainder,-10}{ju.AgeSerial_number}");
            cheshi csj = new cheshi();
            csj.cs(new student(30,100),liebiao);
            WriteLine($"列表数据长度{liebiao.Count}");
            liebiao.Sort(new stupara());
            foreach (var ju in liebiao)
                WriteLine($"{ju.remainder,-10}{ju.AgeSerial_number}");

            //sdsl.Init();
            //sdsl.Print();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            sdsl.Sudoku_main();
            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            /*Mat image = new Mat(@"D:\Image\验证\lsfjlejljlkk_3.jpg");
            Mat outimg = image.Clone();

            // 定义颜色（例如，红色）
            Scalar color = new Scalar(0, 0, 255); // BGR 颜色表示，红色为 (0, 0, 255)

            // 定义要填充颜色的矩形区域
            Rect region = new Rect(100, 100, 300, 50); // (x, y, 宽度, 高度)

            // 创建一个与原始图像相同大小的蒙版
            /*Mat mask = new Mat(image.Size(), MatType.CV_8UC1, new Scalar(255));
            mask.Rectangle(region, color, -1); // 在蒙版上绘制一个填充矩形，颜色使用上面定义的红色

            // 将蒙版应用于图像
            Cv2.BitwiseAnd(image, outimg, mask);

            Cv2.NamedWindow("Image", WindowFlags.Normal);
            outimg.Rectangle(region, color, -1);
            for (int y = region.Top; y <region.Bottom; y++)
            {
                for (int x = region.Left; x <region.Right; x++)
                {
                    // 直接将新颜色赋值给像素
                    outimg.Set(y, x, color);
                }
            }

            // 显示图像
            Cv2.ImShow("Image", outimg);
           
            Cv2.WaitKey(0);*/

            Console.ReadKey();

        }
    }
}
