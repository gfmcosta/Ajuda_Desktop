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
using BusinessLogicLayer;
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
            Regex textUtilr = new Regex(@"[^0-9]");
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

        private void registar_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form Registar= new Registar();
            Registar.ShowDialog();
            this.Close();
        }

        private void entrar_Click(object sender, EventArgs e)
        {
            try
            {
                //parametrs finded
                if(textUtil.Text=="" || textSenha.Text == "")
                {
                    MessageBox.Show("Preencha todos os campos");
                }
                else
                {
                    //Login exist
                    if (BLL.Paciente.SearchByIDandPass(Convert.ToInt32(textUtil.Text), textSenha.Text).Rows.Count > 0)
                    {
                        //paciente finded
                        Globais.is2Authenticator = false;
                        Globais.loggedId = Convert.ToInt32(textUtil.Text);
                        Globais.Email = BLL.Paciente.SearchByIDandPass(Convert.ToInt32(textUtil.Text), textSenha.Text).Rows[0][4].ToString();
                        this.Hide();
                        Loading Loading = new Loading();
                        Loading.ShowDialog();
                        this.Close();
                    }
                    else if (BLL.Funcionario.SearchByIDandPass(Convert.ToInt32(textUtil.Text), textSenha.Text).Rows.Count > 0)
                    {
                        //find job
                        if (BLL.Medico.SearchByID(Convert.ToInt32(textUtil.Text)).Rows.Count > 0)
                        {
                            Globais.job = "Medico";
                        }
                        else if (BLL.Administrativo.SearchByID(Convert.ToInt32(textUtil.Text)).Rows.Count > 0)
                        {
                            Globais.job = "Administrativo";
                        }
                        else
                        {
                            Globais.job = "Enfermeiro";
                        }

                        //job finded
                        Globais.is2Authenticator = false;
                        Globais.loggedId = Convert.ToInt32(textUtil.Text);
                        Globais.Email = BLL.Funcionario.SearchByIDandPass(Convert.ToInt32(textUtil.Text), textSenha.Text).Rows[0][4].ToString();
                        this.Hide();
                        Loading Loading = new Loading();
                        Loading.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        //isNotValid
                        MessageBox.Show("Dados inválidos.");
                    }
                }
                
            }catch(Exception es)
            {
                //crashError
                MessageBox.Show("Erro. Contacte o administrador");
                Console.WriteLine(es.ToString());
            }
        }
    }
}