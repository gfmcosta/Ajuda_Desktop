using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
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
        public class Paciente {
            public int IdPaciente;
            public int IdUtilizador;
            public string Nome;
            public string Sexo;
            public string Telemovel;
            public string Nacionalidade;
            public DateTime DataNasc;
            public string Email;
            public string CC;
            public string NIF;
            public string Senha;

        }
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

        private async void button2_Click(object sender, EventArgs e)
        {
            //Determines if a paciente have the same nif or email or CC.
            bool noExists = false; 

            if(textEmail.Text=="" || textNome.Text =="" || textApelido.Text == "" || comboSexo.Text == "" || textTelemovel.Text == "" || dateTimePicker1.Value.Date> DateTime.Now.Date || textNIF.Text == "" || textSenha.Text == ""|| textDocumento.Text=="")
            {
                //Form is not valid
            }
            else
            {
                //Form is valid
                List<Globais.Filter> filtros = new List<Globais.Filter>();

                var filtro = new Globais.Filter()
                {
                    Field = "Email",
                    Operator = "eq",
                    Value = textEmail.Text,
                    Logic = "or",
                };

                filtros.Add(new Globais.Filter()
                {
                    Field = "Nif",
                    Operator = "eq",
                    Value = textNIF.Text,
                    Logic = "or"
                });

                filtros.Add(new Globais.Filter()
                {
                    Field = "CC",
                    Operator = "eq",
                    Value = textDocumento.Text,
                    Logic = "or"
                });

                var filter = new Globais.FilterDTO()
                {
                    Offset = 0,
                    Limit = 10
                };
                filtro.Filters = filtros;
                filter.Filter = filtro;

                String URI;
                URI = Globais.baseURL + "Paciente/filter";
                using (var client = new HttpClient())
                {
                    var serializedUtilizador = JsonConvert.SerializeObject(filter);
                    var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(URI, content); 
                        if (response != null)
                        {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        if( jsonString == "{\"value\":[]}") {
                            //We can register
                            noExists = true;
                        } else {
                            noExists = false;
                        }

                        } else {
                        MessageBox.Show("Erro. Contacte o administrador do sistema");
                        Console.WriteLine("Erro do servidor");
                    }
                    
                }
                if (noExists == true) {
                    //Let's register
                    URI = Globais.baseURL + "paciente";
                    using (var client = new HttpClient()) {
                        //Criar class Paciente. substituir filtyer na 131 por paciente.
                        //confirmar a response como na 109
                        Paciente paciente = new Paciente();
                        paciente.Nome = textNome.Text + " " + textApelido.Text;
                        paciente.Sexo = comboSexo.SelectedItem.ToString();
                        paciente.Telemovel = textTelemovel.Text;
                        paciente.Nacionalidade = textNacionalidade.Text;
                        paciente.DataNasc = dateTimePicker1.Value.Date;
                        paciente.Email = textEmail.Text;
                        paciente.NIF = textNIF.Text;
                        paciente.CC = textDocumento.Text;
                        //TODO encriptar a senha
                        paciente.Senha = textSenha.Text;
                        var serializedUtilizador = JsonConvert.SerializeObject(paciente);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                            Console.WriteLine("Inseriu Paciente");

                            this.Hide();
                            Login lg = new Login();
                            lg.ShowDialog();
                            this.Close();
                        } else {
                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }
                    }
                    } else {
                    MessageBox.Show("Já existe um registo com os seus dados.");
                }
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

       

        private void textSenha_TextChanged(object sender, EventArgs e)
        {
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexApelido = new Regex(@"(?i)[^a-záéíóúàèìòùâêîôûãõç\\s ]");
            MatchCollection matches = regexApelido.Matches(textApelido.Text);
            if (matches.Count > 0 || textApelido.Text == " ")
            {
                textApelido.Text = "";
                label12.Text = "Introduza apenas Letras!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void comboNacionalidade_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textNacionalidade_TextChanged(object sender, EventArgs e)
        {
            label12.Visible = false;
            timer1.Stop();
            Regex regexNac = new Regex(@"(?i)[^a-záéíóúàèìòùâêîôûãõç\\s]");
            MatchCollection matches = regexNac.Matches(textNacionalidade.Text);
            if (matches.Count > 0 || textNacionalidade.Text == " ")
            {
                textNacionalidade.Text = "";
                label12.Text = "Introduza apenas Letras!";
                label12.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();

            }
        }
    }
}
