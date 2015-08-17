using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ImageFragmentCreator
{
    public partial class Form1 : Form
    {
        private Bitmap img;
        private string imgPath = "../../res/a/";
        private string settingPath = "../../res/settings/";
        private Element[] elements;

        public Form1()
        {
            InitializeComponent();

            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            LoadFileName(imgPath);
            LoadComboBoxText(comboBox2, "../../res/種類.txt");
            LoadComboBoxText(comboBox3, "../../res/状態1.txt");
            LoadComboBoxText(comboBox4, "../../res/状態2.txt");
            LoadComboBoxText(comboBox5, "../../res/属性.txt");

            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox1.SelectedIndex = 0;
        }

        private void LoadFileName(String path)
        {
            string[] files = Directory.GetFiles(path, "*.png");
            string[] filename;
            char[] delimiterChars = {'/', '.'};

            for (int i = 0; i < files.Length; i++)
			{
                filename = files[i].Split(delimiterChars);

                comboBox1.Items.Add(filename[filename.Length - 2]);
			}
        }

        private void LoadComboBoxText(ComboBox comboBox, String path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
            {
                while (!sr.EndOfStream)
                {
                    comboBox.Items.Add(sr.ReadLine());
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (img != null) img.Dispose();
            img = new Bitmap(imgPath + comboBox1.Text + ".png");
            pictureBox1.Image = img;

            label10.Text = (comboBox1.SelectedIndex + 1) + "/" + comboBox1.Items.Count;

            elements = new Element[comboBox5.Items.Count - 1];

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = new Element();
            }

            LoadSetting();

            SetValue();

            UpdateLabel11();
        }

        private void LoadSetting()
        {
            string path = settingPath + comboBox1.Text + ".txt";

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
                {
                    textBox1.Text = sr.ReadLine();
                    comboBox2.SelectedIndex = int.Parse(sr.ReadLine());
                    comboBox3.SelectedIndex = int.Parse(sr.ReadLine());
                    comboBox4.SelectedIndex = int.Parse(sr.ReadLine());

                    for (int i = 1; i < comboBox5.Items.Count; i++)
                    {
                        elements[i - 1].pos.X = int.Parse(sr.ReadLine());
                        elements[i - 1].pos.Y = int.Parse(sr.ReadLine());
                        elements[i - 1].xr = int.Parse(sr.ReadLine());
                        elements[i - 1].yr = int.Parse(sr.ReadLine());
                        elements[i - 1].angle = int.Parse(sr.ReadLine());
                    }
                }
            } else {
                // file is not exist.

                textBox1.Text = "";
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                comboBox4.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex <= 0) return;
            comboBox1.SelectedIndex--;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == comboBox1.Items.Count - 1) return;
            comboBox1.SelectedIndex++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex <= 0) return;

            elements[comboBox5.SelectedIndex - 1].xr = trackBar1.Value;

            pictureBox1.Refresh();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex <= 0) return;

            elements[comboBox5.SelectedIndex - 1].yr = trackBar2.Value;

            pictureBox1.Refresh();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex <= 0) return;

            elements[comboBox5.SelectedIndex - 1].angle = trackBar3.Value;

            pictureBox1.Refresh();
        }

        private void Save()
        {
            using (StreamWriter sw = new StreamWriter(settingPath + comboBox1.Text + ".txt", false, Encoding.GetEncoding("Shift_JIS")))
            {
                sw.WriteLine(textBox1.Text);
                sw.WriteLine(comboBox2.SelectedIndex);
                sw.WriteLine(comboBox3.SelectedIndex);
                sw.WriteLine(comboBox4.SelectedIndex);

                Element element;

                for (int i = 1; i < comboBox5.Items.Count; i++)
                {
                    element = elements[i - 1];

                    sw.WriteLine(element.pos.X);
                    sw.WriteLine(element.pos.Y);
                    sw.WriteLine(element.xr);
                    sw.WriteLine(element.yr);
                    sw.WriteLine(element.angle);
                }
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (comboBox5.SelectedIndex <= 0) return;

            elements[comboBox5.SelectedIndex - 1].pos = e.Location;

            pictureBox1.Refresh();

            UpdateLabel11();
        }

        private void UpdateLabel11()
        {
            label11.Text = "";
            for (int i = 1; i < comboBox5.Items.Count; i++)
            {
                label11.Text = label11.Text + (elements[i - 1].pos.X + elements[i - 1].pos.Y == 0 ? "×" : "◯");
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            SolidBrush brush = new SolidBrush(Color.FromArgb(100, 200, 50, 100));
            Element element;

            if (comboBox5.SelectedIndex <= 0)
            {
                for (int i = 1; i < comboBox5.Items.Count; i++)
			    {
                    element = elements[i - 1];

                    g.TranslateTransform(element.pos.X, element.pos.Y);
                    g.RotateTransform(element.angle);
                    g.FillEllipse(brush, -element.xr / 2, -element.yr / 2, element.xr, element.yr);
                    g.DrawLine(Pens.Red, -element.xr / 2, 0, element.xr / 2, 0);    // x
                    g.DrawLine(Pens.Blue, 0, -element.yr / 2, 0, element.yr / 2);    // y
                    g.ResetTransform();
			    }
            } else {
                element = elements[comboBox5.SelectedIndex - 1];

                g.TranslateTransform(element.pos.X, element.pos.Y);
                g.RotateTransform(element.angle);
                g.FillEllipse(brush, -element.xr / 2, -element.yr / 2, element.xr, element.yr);
                g.DrawLine(Pens.Red, -element.xr / 2, 0, element.xr / 2, 0);    // x
                g.DrawLine(Pens.Blue, 0, -element.yr / 2, 0, element.yr / 2);    // y
                g.ResetTransform();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex <= 0) return;

            ResetValue();
        }

        private void ResetValue()
        {
            elements[comboBox5.SelectedIndex - 1].xr = trackBar1.Value = 1;
            elements[comboBox5.SelectedIndex - 1].yr = trackBar2.Value = 1;
            elements[comboBox5.SelectedIndex - 1].angle = trackBar3.Value = 0;
        }

        private void SetValue()
        {
            if (comboBox5.SelectedIndex <= 0) return;

            trackBar1.Value = elements[comboBox5.SelectedIndex - 1].xr;
            trackBar2.Value = elements[comboBox5.SelectedIndex - 1].yr;
            trackBar3.Value = elements[comboBox5.SelectedIndex - 1].angle;
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetValue();

            pictureBox1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex <= 0) return;

            comboBox5.SelectedIndex--;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex == comboBox5.Items.Count - 1) return;

            comboBox5.SelectedIndex++;
        }
    }
}
