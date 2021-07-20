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
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ajuda_ {

    public partial class Login : Form {
        public int segundo = 0;
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public class Paciente {
            public int IdPaciente { get; set; }
            public int IdUtilizador { get; set; }
            public string Nome { get; set; }
            public string Sexo { get; set; }
            public string Telemovel { get; set; }
            public string Nacionalidade { get; set; }
            public DateTime DataNasc { get; set; }
            public string Email { get; set; }
            public string CC { get; set; }
            public string NIF { get; set; }
        }

        public class Funcionario {
            public int IdFuncionario { get; set; }
            public int IdUtilizador { get; set; }
            public string Nome { get; set; }
            public string Sexo { get; set; }
            public string Telemovel { get; set; }
            public string Nacionalidade { get; set; }
            public DateTime DataNasc { get; set; }
            public string Email { get; set; }
            public string CC { get; set; }
            public string NIF { get; set; }
            public int Funcao { get; set; }
        }

        public class Utilizador {
            public string Login { get; set; }
            public string Senha { get; set; }
        }

        public class Auth {
            public string Token { get; set; }
            public string Message { get; set; }
        }

        public Login() {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e) {
        }

        private void pictureBox4_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void pictureBox5_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void textUtil_TextChanged(object sender, EventArgs e) {
            label1.Visible = false;
            Regex textUtilr = new Regex(@"[^0-9]");
            MatchCollection matches = textUtilr.Matches(textUtil.Text);
            if (matches.Count > 0 || textUtil.Text == " " || textUtil.Text.Length > 9) {
                textUtil.Text = textUtil.Text.Remove(textUtil.Text.Length - 1);
                textUtil.SelectionStart = textUtil.TextLength;
                label1.Visible = true;
                timer1.Start();
                System.Media.SystemSounds.Hand.Play();
            }
        }

        private void textUtil_MouseHover(object sender, EventArgs e) {
            ToolTip toolTip2 = new ToolTip();
            toolTip2.AutoPopDelay = 3000;
            toolTip2.SetToolTip(textUtil, "Nome de Utilizador");
        }

        private void textSenha_MouseHover(object sender, EventArgs e) {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 3000;
            toolTip1.SetToolTip(textSenha, "Senha");
        }

        private void timer1_Tick(object sender, EventArgs e) {
            segundo++;

            if (segundo == 10) {
                label1.Visible = false;
                timer1.Stop();
                segundo = 0;
            }
        }

        private void textSenha_TextChanged(object sender, EventArgs e) {
        }

        private void panelQR_MouseClick(object sender, MouseEventArgs e) {
            panelQR.Visible = false;
            textUtil.Enabled = true;
            textSenha.Enabled = true;
        }

        private void pictureBox7_Click(object sender, EventArgs e) {
            panelQR.Visible = false;
            textUtil.Enabled = true;
            textSenha.Enabled = true;
        }

        private void pictureBox6_Click(object sender, EventArgs e) {
            panelQR.Visible = true;
            panelQR.BringToFront();
            textUtil.Enabled = false;
            textSenha.Enabled = false;
            textUtil.Text = "";
            textSenha.Text = "";
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e) {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e) {
            if (dragging) {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e) {
            dragging = false;
        }

        private void registar_Click(object sender, EventArgs e) {
            this.Hide();
            Form Registar = new Registar();
            Registar.ShowDialog();
            this.Close();
        }

        private async void entrar_Click(object sender, EventArgs e) {
            try {
                //parameters finded
                if (textUtil.Text == "" || textSenha.Text == "") {
                    MessageBox.Show("Preencha todos os campos");
                } else {
                    string senha = textSenha.Text;
                    Globais.Token = "";
                    //Generate a token if login exists
                    String URI;
                    URI = Globais.baseURL + "auth/login?login=" + textUtil.Text + "&senha=" + senha;
                    Utilizador util = new Utilizador();
                    util.Login = textUtil.Text;
                    util.Senha = senha;

                    using (var client = new HttpClient()) {
                        var serializedUtilizador = JsonConvert.SerializeObject(util);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        //se as credenciais nao corresponderem
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                            //unauthorized
                            MessageBox.Show("Acesso negado. Verifique as suas credenciais.");
                        } else {
                            if (response != null) {
                                var jsonString = await response.Content.ReadAsStringAsync();
                                Auth deserializedAuth = JsonConvert.DeserializeObject<Auth>(jsonString);
                                Globais.Token = deserializedAuth.Token;
                            } else {
                                MessageBox.Show("Erro servidor.");
                            }
                        }
                    }

                    if (Globais.Token != "") {
                        BindingSource bsDados = new BindingSource();
                        URI = Globais.baseURL + "paciente?Query=nif%3D" + textUtil.Text;
                        using (var client = new HttpClient()) {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                            using (var response = await client.GetAsync(URI)) {
                                if (response.IsSuccessStatusCode) {
                                    //it's client
                                    //recebe a query em json
                                    var PacienteJsonString = await response.Content.ReadAsStringAsync();
                                    //transforma para string
                                    JObject rsp = JObject.Parse(PacienteJsonString);
                                    IList<JToken> results = rsp["value"].Children().ToList();
                                    if (results.Count > 0) {
                                        IList<Paciente> searchResults = new List<Paciente>();
                                        foreach (JToken result in results) {
                                            Paciente searchResult = result.ToObject<Paciente>();
                                            //adiciona à "estrutura"
                                            searchResults.Add(searchResult);
                                        }
                                        //adiciona a uma tabela. neste caso à BindingSource
                                        bsDados.DataSource = searchResults.ToList();
                                        Paciente paciente = (Paciente)bsDados.List[0];

                                        Globais.is2Authenticator = false;
                                        Globais.loggedId = textUtil.Text;
                                        Globais.Email = paciente.Email;
                                        Globais.Nome = paciente.Nome;
                                        this.Hide();
                                        Loading Loading = new Loading();
                                        Loading.ShowDialog();
                                        this.Close();
                                    }
                                } else {
                                    MessageBox.Show("Erro. Contacte o administrador");
                                    Application.Restart();
                                }
                            }
                        }

                        if (Globais.loggedId == "") {
                            //it is not a client
                            //verify if he's staff

                            bsDados = new BindingSource();
                            URI = Globais.baseURL + "funcionario?Query=nif%3D" + textUtil.Text;
                            using (var client = new HttpClient()) {
                                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                                using (var response = await client.GetAsync(URI)) {
                                    if (response.IsSuccessStatusCode) {
                                        //it's client
                                        //recebe a query em json
                                        var PacienteJsonString = await response.Content.ReadAsStringAsync();
                                        //transforma para string
                                        JObject rsp = JObject.Parse(PacienteJsonString);
                                        IList<JToken> results = rsp["value"].Children().ToList();
                                        if (results.Count > 0) {
                                            IList<Funcionario> searchResults = new List<Funcionario>();
                                            foreach (JToken result in results) {
                                                Funcionario searchResult = result.ToObject<Funcionario>();
                                                //adiciona à "estrutura"
                                                searchResults.Add(searchResult);
                                            }
                                            //adiciona a uma tabela. neste caso à BindingSource
                                            bsDados.DataSource = searchResults.ToList();
                                            Funcionario funcionario = (Funcionario)bsDados.List[0];

                                            Globais.is2Authenticator = false;
                                            Globais.loggedId = textUtil.Text;
                                            Globais.Email = funcionario.Email;
                                            Globais.Nome = funcionario.Nome;
                                            Globais.idLoggedFunc = funcionario.IdFuncionario;
                                            if (funcionario.Funcao == 3) {
                                                Globais.job = 3;
                                            } else if (funcionario.Funcao == 5) {
                                                Globais.job = 5;
                                            } else {
                                                Globais.funcON = true;
                                            }
                                            if (funcionario.Funcao == 5) {
                                                Globais.Admin = true;
                                            }
                                            this.Hide();
                                            Loading Loading = new Loading();
                                            Loading.ShowDialog();
                                            this.Close();
                                            //    this.Hide();
                                            //    Menu Menu = new Menu();
                                            //    Menu.ShowDialog();
                                            //    this.Close();
                                        }
                                    } else {
                                        MessageBox.Show("Erro. Contacte o administrador");
                                        Application.Restart();
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception es) {
                //crashError
                MessageBox.Show("Erro. Contacte o administrador");
                Application.Restart();
                Console.WriteLine(es.ToString());
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {
        }

        private void button1_Click(object sender, EventArgs e) {
        }

        private void labelAPP_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://mega.nz/file/r4dmRTLQ#RhFuM5PAFQPc_bpsg5U5o_vXgAUt1CQk6OhBm_jklDk");
        }

        private void panelQR_Paint(object sender, PaintEventArgs e) {
        }
    }
}