using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Printing;
using System.IO.Ports;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Threading;


namespace EasyPrinting
{
    public partial class Form1 : Form
    {
        List<string> arch = new List<string>();
        //Thread hilo;
        Thread ohilo;
        SerialPort ard = new SerialPort();
        DriveInfo[] allDrives;
        public Form1()
        {
            InitializeComponent();
        }

        private string unidad;
        private int pag;
        private int cre;

        public string u
        {
            get
            {
                return unidad;
            }
            set
            {
                this.unidad = value;
            }
        }

        public int p
        {
            get
            {
                return pag;
            }
            set
            {
                this.pag = value;
            }
        }

        public int c
        {
            get
            {
                return cre;
            }
            set
            {
                this.cre = value;
            }
        }

        //public void abrirpuerto()
        //{
        //    ard.BaudRate = 9600;
        //    ard.PortName = "COM5";
        //    ard.Open();
        //}


        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Focus();
            try
            {
                //abrirpuerto();
                CheckForIllegalCrossThreadCalls = false;
                p = 0;
                c = 0;
                funcion1();
                //ard.WriteLine("i");
                //hilo = new Thread(new ThreadStart(hilo1));
                ohilo = new Thread(new ThreadStart(hilo2));
                //hilo.Start();
                ohilo.Start();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        //public void hilo1()
        //{
        //    while (true)
        //    {
        //        c = coins();
        //        lcrd.Text = c.ToString();
        //        Thread.Sleep(1500);
        //    }
        //}

        public void hilo2()
        {
            while (true)
            {
                if(c >= p && p != 0)
                {
                    button3.BackColor = Color.Green;
                    button3.Focus();
                }
                Thread.Sleep(5000);
            }
        }

        public void funcion1()
        {
            allDrives = DriveInfo.GetDrives();
            foreach (var a in allDrives)
            {
                if (a.IsReady != false && a.DriveType == DriveType.Removable)
                {
                    comboBox1.Items.Add(a.Name);
                }
            }
        }

        public void funcion2()
        {
            arch.Clear();
            comboBox2.Items.Clear();
            u = comboBox1.SelectedItem.ToString();
            DriveInfo uni = new DriveInfo(@u);
            DirectoryInfo root = uni.RootDirectory;
            string[] files = Directory.GetFiles(u, "*.doc");
            foreach (string s in files)
            {
                FileInfo f = null;
                try
                {
                    f = new FileInfo(s);
                }
                catch (FileNotFoundException ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    continue;
                }
                string ft = string.Format("{0}", f.Name);
                comboBox2.Items.Add(ft);
                arch.Add(s.ToString());
            }
        }

        //public int coins()
        //{
        //    int coin = 0;
        //    try
        //    {
        //        coin = int.Parse(ard.ReadLine());
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return coin;
        //}

        public void funcion3()
        {

            var app = new Microsoft.Office.Interop.Word.Application();
            var document = app.Documents.Open(@arch[comboBox2.SelectedIndex]);
            var nop = document.ComputeStatistics(WdStatistic.wdStatisticPages, false);
            lcosto.Text = nop.ToString();
            document.Close();
            app.Quit();
            p = nop;
        }

        public void borrar()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox1.Text = "Seleccione";
            comboBox2.Text = "Seleccione";
            button3.BackColor = Color.Red;
            comboBox2.Enabled = false;
            comboBox1.Focus();
            p = 0;
            lcosto.Text = "0";
            //ard.WriteLine("i");
            //c = coins();
            lcrd.Text = c.ToString();
            u = "";
            funcion1();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    if(comboBox1.SelectedIndex != -1)
                    {
                        funcion2();
                        comboBox2.Enabled = true;
                        comboBox2.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Favor de seleccionar una unidad");
                    }
                    break;

                case Keys.Insert:
                    comboBox1.Items.Clear();
                    comboBox1.Text = "";
                    funcion1();
                    break;
            }
        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    comboBox2.Items.Clear();
                    comboBox2.Text = "";
                    funcion2();
                    button3.Focus();
                    break;

                case Keys.Right:
                    if (comboBox2.SelectedIndex != -1)
                    {
                        funcion3();
                        button3.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Favor de seleccionar un documento");
                    }
                    break;

                case Keys.End:
                    if (comboBox2.SelectedIndex != -1)
                    {
                        funcion3();
                        button3.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Favor de seleccionar un documento");
                    }
                    break;
            }
        }

        public void funcion4()
        {
            DialogResult q = MessageBox.Show("¿Deseas imprimir el documento?", "", MessageBoxButtons.YesNo);
            if (q == DialogResult.Yes)
            {
                var app = new Microsoft.Office.Interop.Word.Application();
                var document = app.Documents.Open(@arch[comboBox2.SelectedIndex]);
                document.PrintOut();
                document.Close();
                app.Quit();
                MessageBox.Show("Su documento ha sido impreso, Gracias por usar nuestro servicio :)");
                int res = c - p;
                c = res;
                borrar();
            }
        }

        private void button3_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.End:
                    if (c >= p)
                    {
                        funcion4();
                    }
                    else
                    {
                        MessageBox.Show("favor de introducir los creditos necesarios para imprimir");
                    }
                    break;

                case Keys.Home:
                    DialogResult hi = MessageBox.Show("¿Deseas cancelar la impresion y empezar de nuevo?, Los creditos se conservan", "", MessageBoxButtons.YesNo);
                    if(hi == DialogResult.Yes)
                    {
                        borrar();
                    }
                    break;
                case Keys.Space:
                    c += 1;
                    lcrd.Text = c.ToString();
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.E:
                    if (c >= p)
                    {
                        funcion4();
                    }
                    else
                    {
                        MessageBox.Show("falta datos a introducir");
                    }
                    break;

                case Keys.Space:
                    c += 1;
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
