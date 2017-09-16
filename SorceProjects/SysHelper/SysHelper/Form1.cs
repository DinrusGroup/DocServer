using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shell32;
using System.IO;

namespace SysHelper
{
    public partial class Form1 : Form
    {

        bool finished = false;
        int dirCount, fileCount, msec = 0, sec = 0, min = 0;
       

        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var Shl = new Shell();
           
            

            dirCount = fileCount = 0;

            Folder Recycler = Shl.NameSpace(10);
            FolderItems items = Recycler.Items();


            

            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    timer.Enabled = true;                   
                    timer.Start();

                    FolderItem FI = items.Item(i);
                    string FileName = Recycler.GetDetailsOf(FI, 0);
                    string FilePath = Recycler.GetDetailsOf(FI, 1);
                    string RecyleDate = Recycler.GetDetailsOf(FI, 2);

                    if (FI.IsFolder)
                    {
                        Directory.Delete(FI.Path, true);
                        dirCount++;
                    }
                    else
                    {
                        File.Delete(FI.Path);
                        fileCount++;
                    }
                }
                catch (Exception)
                {

                }

                finished = true;
                timer.Stop();
                timer.Enabled = false;
            }

			
			lRecBin1.Visible = true;
			lRecBin1.Text = String.Format("Удалено папок {0}; файлов {1}. Время:{2} мин {3} сек {4} мсек", dirCount, fileCount, min, sec, msec);
				
		}

        void tickCount(object sender, EventArgs e)
        {
            while (!finished)
            {
                ++msec;
                sec = msec / 1000;
                min = sec / 60;                
            }

        }

        public void deleteDir(string dir)
		{			
			   Directory.Delete(dir, true);
		}
		
		private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            dgChooseDir = new FolderBrowserDialog();
            dgChooseDir.ShowDialog();
            tbDir.Text = dgChooseDir.SelectedPath;			
		}

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            deleteDir(tbDir.Text);
            linkLabel3.Text = "Папка успешно удалена";
        }

        private void lRecBin1_MouseMove(object sender, MouseEventArgs e)
        {
            lRecBin1.Visible = false;
        }
    }
}
    

