using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ToyBox;

namespace ImageFragmentCreator
{
    public partial class Quiz : Form
    {
        bool isAnswer;
        Bitmap img;
        Identity identity;
        Element element;
        string file;

        private Size ansSize = new Size(800, 800);
        private Size questionSize = new Size(400, 400);

        ImageViewForm ivf;

        private string imgPath = "../../res/a/";
        private string settingPath = "../../res/settings/";
        public CheckBox[] kindChecks;
        public CheckBox[] elementChecks;
        public CheckBox[] updateChecks;
        public CheckBox[] damageChecks;

        private int[] totals;
        private List<int> appearingLogs;

        private string[] filename;

        public Quiz()
        {
            InitializeComponent();

            ivf = new ImageViewForm();
            ivf.Show();

            kindChecks = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6, checkBox7, checkBox8, checkBox9, checkBox10, checkBox11, checkBox12 };
            elementChecks = new CheckBox[] { checkBox24, checkBox23, checkBox22, checkBox21, checkBox20, checkBox19, checkBox18, checkBox17, checkBox26, checkBox25, checkBox16, checkBox15, checkBox14, checkBox13, checkBox27, checkBox28 };
            updateChecks = new CheckBox[] { checkBox29, checkBox30, checkBox31, checkBox32 };
            damageChecks = new CheckBox[] { checkBox33, checkBox34 };

            string[] files = Directory.GetFiles(settingPath, "*.txt");
            char[] delimiterChars = { '/', '.' };
            string[] splited;
            filename = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                splited = files[i].Split(delimiterChars);
                filename[i] = splited[splited.Length - 2];
            }

            totals = new int[filename.Length];
            appearingLogs = new List<int>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isAnswer)
            {
                Bitmap bmp = new Bitmap(imgPath + file + ".png");
                Graphics g = Graphics.FromImage(bmp);

                g.TranslateTransform(Trimming.centerLast.X, Trimming.centerLast.Y);
                g.DrawRectangle(Pens.Red, -img.Width / 2, -img.Height / 2, img.Width, img.Height);

                g.Dispose();

                ivf.BackgroundImage = bmp;

                button1.Text = "start";
                ivf.Size = ansSize;
                label1.Text = "";
            }
            else
            {
                // 画像選択
                List<string> correctFiles = new List<string>();
                int selectedImage;

                for (int i = 0; i < filename.Length; i++)
                {
                    file = filename[i];

                    identity = LoadSettingIdentity(file);

                    if (kindChecks[identity.kind].Checked && updateChecks[identity.update].Checked && damageChecks[identity.damage].Checked)
                    {
                        correctFiles.Add(file);
                    }
                }

                while (true)
                {
                    int i;

                    selectedImage = (int)MyRandom.Next((uint)correctFiles.Count);

                    for (i = 0; i < appearingLogs.Count; i++)
                    {
                        if (selectedImage == appearingLogs[i])
                        {
                            break;
                        }
                    }

                    if (i == appearingLogs.Count) break;
                }

                if (appearingLogs.Count == numericUpDown1.Value)
                {
                    appearingLogs.RemoveAt(0);
                    appearingLogs.Add(selectedImage);
                }
                else
                {
                    appearingLogs.Add(selectedImage);
                }

                //for (int i = 0; i < appearingLogs.Count; i++)
                //{
                //    Console.Write(appearingLogs[i] + " ");
                //}
                //Console.WriteLine();
                
                file = correctFiles[selectedImage];
                totals[selectedImage]++;

                // 属性選択
                int selectedElement;

                while (true)
                {
                    selectedElement = (int)MyRandom.Next((uint)elementChecks.Length);
                    LoadSetting(file, selectedElement);

                    if (elementChecks[selectedElement].Checked && element.pos != Point.Empty && element.xr > 1 && element.yr > 1)
                    {
                        break;
                    }
                }

                Bitmap bmp = new Bitmap(imgPath + correctFiles[selectedImage] + ".png");

                Size size = new Size(trackBar1.Value + (int)MyRandom.Next((uint)trackBar2.Value), trackBar1.Value + (int)MyRandom.Next((uint)trackBar2.Value));

                // 生成
                img = Trimming.TrimmingBitmap(element, bmp, size);

                if (radioButton1.Checked)
                {
                    ivf.BackgroundImage = img;
                    ivf.Size = questionSize;
                }
                else
                {
                    Bitmap hintImg = new Bitmap(bmp.Width, bmp.Height);
                    Graphics g = Graphics.FromImage(hintImg);

                    g.Clear(Color.Black);
                    g.DrawImage(img, new Rectangle(Trimming.centerLast.X - img.Width / 2, Trimming.centerLast.Y - img.Height / 2, img.Width, img.Height), new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    g.Dispose();

                    ivf.BackgroundImage = hintImg;
                    ivf.Size = ansSize;
                }

                button1.Text = "answer";
            }

            isAnswer = !isAnswer;
        }

        private Identity LoadSettingIdentity(string name)
        {
            string path = settingPath + name + ".txt";
            Identity identity = new Identity();
            
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
            {
                identity.name = sr.ReadLine();
                identity.kind = int.Parse(sr.ReadLine());
                identity.update = int.Parse(sr.ReadLine());
                identity.damage = int.Parse(sr.ReadLine());
            }

            return identity;
        }

        private void LoadSetting(string file, int elementNum)
        {
            string path = settingPath + file + ".txt";

            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
            {
                identity = new Identity();

                identity.name = sr.ReadLine();
                identity.kind = int.Parse(sr.ReadLine());
                identity.update = int.Parse(sr.ReadLine());
                identity.damage = int.Parse(sr.ReadLine());

                for (int i = 0; i < elementNum; i++)
                {
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                    sr.ReadLine();
                }

                element = new Element();
                element.pos.X = int.Parse(sr.ReadLine());
                element.pos.Y = int.Parse(sr.ReadLine());
                element.xr = int.Parse(sr.ReadLine());
                element.yr = int.Parse(sr.ReadLine());
                element.angle = int.Parse(sr.ReadLine());
            }
        }

        private bool elementCheckedToggle = false;
        private bool kindCheckedToggle = false;

        private void button2_Click(object sender, EventArgs e)
        {
            if (elementCheckedToggle)
            {
                for (int i = 0; i < elementChecks.Length; i++)
                {
                    elementChecks[i].Checked = true;
                }

                elementCheckedToggle = false;
            }
            else
            {
                for (int i = 0; i < elementChecks.Length; i++)
                {
                    elementChecks[i].Checked = false;
                }

                elementCheckedToggle = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (kindCheckedToggle)
            {
                for (int i = 0; i < kindChecks.Length; i++)
                {
                    kindChecks[i].Checked = true;
                }

                kindCheckedToggle = false;
            }
            else
            {
                for (int i = 0; i < kindChecks.Length; i++)
                {
                    kindChecks[i].Checked = false;
                }

                kindCheckedToggle = true;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Value = 15;
            trackBar2.Value = 20;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Value = 30;
            trackBar2.Value = 30;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            trackBar1.Value = 100;
            trackBar2.Value = 50;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = kindLabel[identity.kind];
        }

        private string[] kindLabel = {"駆逐艦", "軽巡洋艦", "重巡洋艦", "重雷装巡洋艦", "航空巡洋艦", "戦艦", "航空戦艦", "水上機母艦", "軽空母", "正規空母", "潜水艦", "その他"};
    }
}
