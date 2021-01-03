using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ajuda_
{
    public partial class Registar : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public int segundo;
        public Registar()
        {
            InitializeComponent();
        }

        private void Registar_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form Login = new Login();
            Login.ShowDialog();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void textNome_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexNome = new Regex(@"(?i)[^a-záéíóúàèìòùâêîôûãõç\\s ]");
            MatchCollection matches = regexNome.Matches(textNome.Text);
            if (matches.Count > 0 || textNome.Text == " ")
            {
                textNome.Text = "";
                label12.Text = "Introduza apenas Letras!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            segundo++;
            if (segundo == 10)
            {
                label12.Visible = false;
                timer1.Stop();
                segundo = 0;
            }
        }

        private void textTelemovel_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(textTelemovel.Text);
            if (matches.Count > 0 || textTelemovel.Text == " ")
            {
                textTelemovel.Text = "";
                label12.Text = "Introduza apenas Números!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void textNIF_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(textNIF.Text);
            if (matches.Count > 0 || textNIF.Text == " ")
            {
                textNIF.Text = "";
                label12.Text = "Introduza apenas Números!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void textDocumento_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(textDocumento.Text);
            if (matches.Count > 0 || textDocumento.Text == " ")
            {
                textDocumento.Text = "";
                label12.Text = "Introduza apenas Números!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void textUtil_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexTN = new Regex(@"[^A-Za-z0-9_]");
            MatchCollection matches = regexTN.Matches(textUtil.Text);
            if (matches.Count > 0 || textUtil.Text == " ")
            {
                textUtil.Text = "";
                label12.Text = "Introduza apenas Letras e números!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void textSenha_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexTN = new Regex(@"[^A-Za-z0-9_]");
            MatchCollection matches = regexTN.Matches(textSenha.Text);
            if (matches.Count > 0 || textSenha.Text == " ")
            {
                textSenha.Text = "";
                label12.Text = "Introduza apenas Letras e números!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }
    }
}
