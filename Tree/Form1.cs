using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tree
{
    public partial class Form1 : Form
    {
        int filesFound = 0, filesAll = 0;
        bool down = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            
            filesFound = 0;
            filesAll = 0;
            treeView1.Nodes.Clear();
           
            new Thread(() =>
            {
                System.Diagnostics.Stopwatch sw = new Stopwatch();
                sw.Restart();
                sw.Start();
                OutputTreeView(treeView1);
                Action action = () => textBox6.Text = (sw.ElapsedMilliseconds / 1000.0).ToString();
                if (InvokeRequired)
                    Invoke(action);
                else
                    action();
                
                sw.Stop();
            }).Start();
           

        }
        private List<string> GetFiles(string path, string pattern)
        {
            var prev = 0;
            var files = new List<string>();
            char PathSeparator = '\\';
            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                filesFound += files.Count;
                foreach (var directory in Directory.GetDirectories(path))
                {
                    _manualEvent.WaitOne();
                    files.AddRange(GetFiles(directory, pattern));                                    
                    Action b = () => textBox4.Text = filesFound.ToString();
                    if (InvokeRequired)
                    {
                        Invoke(b);
                    }
                    else
                    {
                        b();
                    }
                    Action c = () => textBox3.Text = directory.ToString();
                    if (InvokeRequired)
                    {
                        Invoke(c);
                    }
                    if (files.Count > 0)
                    {
                        TreeNode LastNode = null;
                        
                        foreach (string PathToFile in files)
                        {

                            string SubPathAgg = string.Empty;

                            foreach (string SubPath in PathToFile.Split(PathSeparator))
                            {
                                SubPathAgg += SubPath + PathSeparator;

                                TreeNode[] Nodes = treeView1.Nodes.Find(SubPathAgg, true);

                                if (Nodes.Length == 0)
                                {
                                    if (LastNode == null)
                                    {
                                        Action action = () => LastNode = treeView1.Nodes.Add(SubPathAgg, SubPath);
                                        if (InvokeRequired)
                                            Invoke(action);
                                        else
                                            action();
                                    }
                                    else
                                    {
                                        Action action = () => LastNode = LastNode.Nodes.Add(SubPathAgg, SubPath);
                                        if (InvokeRequired)
                                            Invoke(action);
                                        else
                                            action();

                                    }
                                }
                                else
                                {
                                    LastNode = Nodes[0];
                                }
                            }
                        }
                    }

                }
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }
        void OutputTreeView(TreeView TreeviewNode)
        {
            string PatternFile = textBox2.Text; ;
            string CurrentDirectory = textBox1.Text;

            DirectoryInfo DirectoryCurrent = new DirectoryInfo(CurrentDirectory);       
            var files = GetFiles(CurrentDirectory, $"{PatternFile}*");
        }

        private ManualResetEvent _manualEvent = new ManualResetEvent(true);

        private void Resume()
        {
            _manualEvent.Set();
        }

        private void Pause()
        {
            _manualEvent.Reset();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (down == false)
            {
                down = true;
                Pause();
            }
            else { Resume(); down = false; }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }

}
