using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ajuda_
{
    public partial class Loading : Form
    {

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public Loading()
        {
            InitializeComponent();
        }
        int i;
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Loading_Load(object sender, EventArgs e)
        {
            //tinha aqui comentado
            i = 0;
            Globais.code = "";
            Random rnd = new Random();
            for (int y = 0; y < 6; y++)
            {
                int number = rnd.Next(10);
                Globais.code += number.ToString();
            }
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            i++;

            if (i == 20)
            {
                timer1.Stop();
                if (Globais.is2Authenticator == false)
                {
                    //open authenticator form

                    i = 0;
                    Globais.code = "";
                    Random rnd = new Random();
                    for (int y = 0; y < 6; y++)
                    {
                        int number = rnd.Next(10);
                        Globais.code += number.ToString();
                    }
                    try
                    {

                        SmtpClient client = new SmtpClient("smtp.gmail.com", 25);

                        NetworkCredential cred = new NetworkCredential("noreply.ajudamais@gmail.com", "AJUDA+admin");

                        MailMessage Msg = new MailMessage();
                        Msg.From = new MailAddress("noreply.ajudamais@gmail.com");
                        Msg.To.Add(Globais.Email);

                        // Assign the subject of our message.
                        Msg.Subject = "Código de Confirmação: " + Globais.loggedId;

                        // Create the content(body) of our message.
                        Msg.Body = "Olá Utilizador: "+ Globais.loggedId+ ". Reparámos que tentaste iniciar sessão na nossa aplicação. \n Para tua segurança por favor insere o código de verificação de 6 dígitos: "+ Globais.code+". \n \n Atenciosamente, \n A Equipa Ajuda+.";

                        // Send our account login details to the client.
                        client.Credentials = cred;

                        //Enabling SSL(Secure Sockets Layer, encyription) is reqiured by most email providers to send mail
                        client.EnableSsl = true;

                        // Send our email.
                        client.Send(Msg);
                        this.Hide();
                        Autenticador Autenticador = new Autenticador();
                        Autenticador.ShowDialog();
                        this.Close();
                    }
                    catch
                    {
                        // If Mail Doesnt Send Error Mesage Will Be Displayed
                        MessageBox.Show("Erro. Contacte o administrador.");
                    }

                }
                else
                {
                    //go to Menu
                    this.Hide();
                    Menu Menu = new Menu();
                    Menu.ShowDialog();
                    this.Close();
                    Globais.is2Authenticator = false;


                }
            }
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
