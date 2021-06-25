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
    public partial class Autenticador : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Autenticador()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        String code;
        private void t6_TextChanged(object sender, EventArgs e)
        {
            Regex textt6 = new Regex(@"[^0-9]");
            MatchCollection matches = textt6.Matches(t6.Text);
            if (matches.Count > 0 || t6.Text == " ")
            {
                t6.Text = t6.Text.Remove(t6.Text.Length - 1);
                t6.SelectionStart = t6.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }else if (t6.Text == "")
            {
                t5.Focus();
            }
            else if(t6.Text!="")
            {
                code = t1.Text + t2.Text + t3.Text + t4.Text + t5.Text+t6.Text;
                if (code == Globais.code)
                {
                        MessageBox.Show("Bem Vindo/a " + Globais.Nome);

                    Globais.is2Authenticator = true;
                    //go to Menu
                    this.Hide();
                    Loading Loading = new Loading();
                    Loading.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Código inválido");
                }
            }
        }

        private void t1_TextChanged(object sender, EventArgs e)
        {
            Regex textt1= new Regex(@"[^0-9]");
            MatchCollection matches = textt1.Matches(t1.Text);
            if (matches.Count > 0 || t1.Text == " ")
            {
                t1.Text = t1.Text.Remove(t1.Text.Length - 1);
                t1.SelectionStart = t1.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }else if (t1.Text == "")
            {
                t1.Focus();
            }
            else
            {
                t2.Focus();
            }
        }

        private void t2_TextChanged(object sender, EventArgs e)
        {
            Regex textt2 = new Regex(@"[^0-9]");
            MatchCollection matches = textt2.Matches(t2.Text);
            if (matches.Count > 0 || t2.Text == " ")
            {
                t2.Text = t2.Text.Remove(t2.Text.Length - 1);
                t2.SelectionStart = t2.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }else if (t2.Text=="")
            {
                t1.Focus();
            }
            else
            {
                t3.Focus();
            }
        }

        private void t3_TextChanged(object sender, EventArgs e)
        {
            Regex textt3 = new Regex(@"[^0-9]");
            MatchCollection matches = textt3.Matches(t3.Text);
            if (matches.Count > 0 || t3.Text == " ")
            {
                t3.Text = t3.Text.Remove(t3.Text.Length - 1);
                t3.SelectionStart = t3.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }else if (t3.Text == "")
            {
                t2.Focus();
            }
            else
            {
                t4.Focus();
            }
        }

        private void t4_TextChanged(object sender, EventArgs e)
        {
            Regex textt4 = new Regex(@"[^0-9]");
            MatchCollection matches = textt4.Matches(t4.Text);
            if (matches.Count > 0 || t4.Text == " ")
            {
                t4.Text = t4.Text.Remove(t4.Text.Length - 1);
                t4.SelectionStart = t4.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }else if (t4.Text == "")
            {
                t3.Focus();
            }
            else
            {
                t5.Focus();
            }
        }

        private void t5_TextChanged(object sender, EventArgs e)
        {
            Regex textt5 = new Regex(@"[^0-9]");
            MatchCollection matches = textt5.Matches(t5.Text);
            if (matches.Count > 0 || t5.Text == " ")
            {
                t5.Text = t5.Text.Remove(t5.Text.Length - 1);
                t5.SelectionStart = t5.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }else if (t5.Text == "")
            {
                t4.Focus();
            }
            else
            {
                t6.Focus();
            }
        }

        private void entrar_Click(object sender, EventArgs e)
        {
            code = t1.Text + t2.Text + t3.Text + t4.Text + t5.Text + t6.Text;
            if (code == Globais.code)
            {
                MessageBox.Show("Bem Vindo/a " + Globais.Nome);

                Globais.is2Authenticator = true;
                //go to Menu
                this.Hide();
                Loading Loading = new Loading();
                Loading.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Código inválido");
            }
        }

        private void Autenticador_Load(object sender, EventArgs e)
        {
            int x = 0;
            Boolean isEmail=false;
            foreach (char ch in Globais.Email.ToCharArray())
            {
                //write * to censure email
                    if (x > 2)
                    {
                        if (ch == '@')
                        {
                        
                        x = 0;
                            label1.Text += ch;
                        }
                        else
                        {
                            label1.Text += "*";
                        }
                    }
                    else
                    {
                        x++;
                        label1.Text += ch;
                    }
             
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void t6_KeyPress(object sender, KeyPressEventArgs e)
        {
            


        }

        private void t6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                t6.Text = "";
                t5.Focus();
                
            }
        }

        private void t5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                t5.Text = "";
                t4.Focus();

            }
           
        }

        private void t4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                t4.Text = "";
                t3.Focus();
                
            }
        }

        private void t3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                t3.Text = "";
                t2.Focus();
                
            }
        }

        private void t2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Back)
            {
                t2.Text = "";
                t1.Focus();
                
            }
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
    }
}
