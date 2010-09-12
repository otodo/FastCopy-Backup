using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FastCopy_Backup
{
    class Program
    {
        static void Main(string[] args)
        {
            Fastcopy coping = new Fastcopy();
            coping.datapath();
            coping.StartCopy();
            Console.WriteLine("終了");
            Console.Read();
        }
    }

    class Fastcopy
    {
        private string fcpath;
        private string infopath = " ";
        private string op = " ";
        private string logfilename = "";
        private string savelog = " ";
        public struct pathinfo
        {
            public string title;
            public string inputfile;
            public string outputfile;
        }
        private pathinfo[] pathlist = new pathinfo[1];
        private int i=1;

        public Fastcopy()
        {
            fcpath = "C:\\Program Files\\FastCopy\\FastCopy.exe";
        }
        public Fastcopy(string inputPath)
        {
            fcpath = inputPath;
        }

        public void datapath()
        {
            infopath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            infopath = infopath + "\\datapath.txt";
        }
        public void datapath(string inputDatapath)
        {
            infopath = inputDatapath;
        }

        public void StartCopy()
        {
            if (infopath == " ") // StartCopyの前にdatapathが設定されていない場合
            {
                datapath();
            }
            if ((chkpathinfo()) == 0)//設定パス情報が正しい場合
            {
                getpathinfo();
                if (op == " ")
                {
                    op = "/force_close";//オプションとして強制終了だけは必須
                }
                foreach(pathinfo eachpath in pathlist)
                {
                    string go_fastcopy;
                    //go_fastcopy = "\"C:\\Program Files\\FastCopy\\FastCopy.exe\"" + " /force_close" + " \"D:\\temp\"" + " /to=\"D:\\temp\\FastCopy\"";
                    go_fastcopy = "\"" + fcpath +"\"" + " " + op + " " + eachpath.inputfile + " /to=" + eachpath.outputfile;
                    Process _process = new Process();
                    _process.StartInfo.FileName = go_fastcopy;
                    _process.StartInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
                    _process.StartInfo.UseShellExecute = false; // シェル機能を使用しない
                    _process.Start();
                    Console.WriteLine(go_fastcopy);
                    Console.WriteLine(eachpath.title + ": " + eachpath.inputfile + " → " + eachpath.outputfile + " へコピー中");
                    _process.WaitForExit();

                }
                if (!(savelog == " "))
                {

                    string oldlogpath = fcpath.Replace("FastCopy.exe", "") + "Log\\" + logfilename;
                    System.IO.File.Copy(oldlogpath, savelog);
                }

            }
            else
            {
                Console.WriteLine("処理を中断しました");
            }
        }

        private int chkpathinfo()
        {
            if (System.IO.File.Exists(infopath))
            {
                return 0;
            }
            else
            {
                Console.WriteLine("ファイルが存在しません");
                return -1;
            }
	
        }

        private void getpathinfo()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(infopath);
            string line;
            string filelog = "";
            while ((line = reader.ReadLine()) != null)
            {
                //コピー元・コピー先ディレクトリパスの読み込み
                if (!((line.StartsWith("#")) || (line.StartsWith("/"))))
                {
                    line = line.Replace("\r", "").Replace("\n", ""); //改行削除
                    //line = line.Replace("\\", "\\\\"); //\記号を2重 → 不要
                    string[] eachcol = line.Split(',');
                    if (i != 1)
                    {
                        pathlist.CopyTo(pathlist = new pathinfo[i], 0);
                    }
                    pathlist[i - 1].title = eachcol[0];
                    pathlist[i - 1].inputfile = "\"" + eachcol[1] + "\"";
                    pathlist[i - 1].outputfile = "\"" + eachcol[2] + "\"";
                    i++;
                }
                else if (line.StartsWith("/")) //オプション指定
                {
                    line = line.Replace("\r", "").Replace("\n", ""); //改行削除
                    string[] eachop = line.Split(' ');
                    foreach (string ops in eachop)
                    {
                        if (ops.IndexOf("/filelog") >= 0)
                        {
                            DateTime dt = DateTime.Now;
                            logfilename =  dt.ToString("yyMMddHHmm") + ".log";
                            string logfullpath = "/filelog=\"" + logfilename + "\"";
                            filelog = ops.Replace("/filelog", logfullpath);

                        }
                        else if (ops.IndexOf("/logdir") >= 0) //ログ保存先ディレクトリが指定されている場合
                        {
                            savelog = ops.Replace("/logdir=", "");
                            savelog = savelog + "\\" + logfilename;
                        }

                        if ((ops.IndexOf("/logdir") < 0) || (!ops.StartsWith("/filelog")))
                        {
                            op = op + " " + ops;
                        }
                        
                    }
                    if (!(filelog == ""))
                    {
                        op = op + " " + filelog;
                    }
                }
            }
            reader.Close();
        }

    }
}
