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
    public partial class Login : Form
    {
        public int segundo=0;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
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

        private void textUtil_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = false;
            Regex textUtilr = new Regex(@"[^A-Za-z0-9]");
            MatchCollection matches = textUtilr.Matches(textUtil.Text);
            if (matches.Count > 0 || textUtil.Text == " ")
            {
                textUtil.Text = textUtil.Text.Remove(textUtil.Text.Length-1);
                textUtil.SelectionStart = textUtil.TextLength;
                label1.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();
            }
        }

        private void textUtil_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip2 = new ToolTip();
            toolTip2.AutoPopDelay = 3000;
            toolTip2.SetToolTip(textUtil, "Nome de Utilizador");
        }

        private void textSenha_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 3000;
            toolTip1.SetToolTip(textSenha, "Senha");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            segundo++;

            if (segundo == 10)
            {
                label1.Visible = false;
                timer1.Stop();
                segundo = 0;


            }
        }

        private void textSenha_TextChanged(object sender, EventArgs e)
        {
            label1.Visible = false;
            Regex textSenhar = new Regex(@"[^A-Za-z0-9]");
            MatchCollection matches = textSenhar.Matches(textSenha.Text);
            if (matches.Count > 0 || textSenha.Text == " ")
            {
                textSenha.Text = textSenha.Text.Remove(textSenha.Text.Length - 1);
                textSenha.SelectionStart = textSenha.TextLength;
                label1.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();
            }
        }

        private void panelQR_MouseClick(object sender, MouseEventArgs e)
        {
            panelQR.Visible = false;
            textUtil.Enabled = true;
            textSenha.Enabled = true;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            panelQR.Visible = false;
            textUtil.Enabled = true;
            textSenha.Enabled = true;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            panelQR.Visible = true;
            panelQR.BringToFront();
            textUtil.Enabled = false;
            textSenha.Enabled = false;
            textUtil.Text = "";
            textSenha.Text = "";
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
    }
}
