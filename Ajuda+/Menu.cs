using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

namespace Ajuda_ {
    public partial class Menu : Form {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public class PacienteRegistar {
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
            public Boolean Ativo { get; set; }
        }

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
            public Boolean Ativo { get; set; }
        }
        public class Utilizador {
            public int IdUtilizador { get; set; }
            public string Login { get; set; }
            public string Senha { get; set; }
        }
        public class FuncionarioRegistar {
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
            public string Senha { get; set; }
            public Boolean Ativo { get; set; }
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
            public Boolean Ativo { get; set; }

        }

        /* 
         {{
  "idMarcacao": 3,
  "idPaciente": 1,
  "idFuncionario": null,
  "idTecnico": 1,
  "data": "2021-06-13T00:00:00",
  "hora": "10:00:00",
  "tipo": "Consulta",
  "qrcode": "1",
  "relatorio": "",
  "ultimaAtualizacao": "2021-06-09T22:42:39.357",
  "funcionarioNavigation": null,
  "tecnicoNavigation": null,
  "pacienteNavigation": null,
  "links": [
    {
      "href": "https://localhost:44378/api/v1/marcacao/3",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "https://localhost:44378/api/v1/marcacao/3",
      "rel": "delete_marcacao",
      "method": "DELETE"
    },
    {
      "href": "https://localhost:44378/api/v1/marcacao",
      "rel": "create_marcacao",
      "method": "POST"
    },
    {
      "href": "https://localhost:44378/api/v1/marcacao/3",
      "rel": "update_marcacao",
      "method": "PUT"
    }
  ]
}}
         */
        public class Funcao {
            public int IdFuncao { get; set; }
            public string Descricao { get; set; }
        }
        public class Marcacao {
            public int IdMarcacao { get; set; }
            public int IdPaciente { get; set; }
            public int IdFuncionario { get; set; }
            public int IdTecnico { get; set; }
            public DateTime Data { get; set; }
            public TimeSpan Hora { get; set; }
            public string Tipo { get; set; }
            public string QRCODE { get; set; }
            public string Relatorio { get; set; }
            public DateTime UltimaAtualizacao { get; set; }

        }
        private void LimparForm(GroupBox crl) {
            foreach (var c in crl.Controls) {
                if (c is TextBox) {
                    ((TextBox)c).Text = String.Empty;
                } else if (c is DateTimePicker) {
                    ((DateTimePicker)c).Value = ((DateTimePicker)c).MinDate;
                } else if (c is ComboBox) {
                    ((ComboBox)c).SelectedIndex = -1;
                }
            }
        }

        private void CloseGroupBoxs(Form crl) {
            foreach (var c in crl.Controls) {
                if (c is GroupBox) {
                    ((GroupBox)c).Visible = false;
                }
            }
        }
        private void ClosePanel(Form crl) {
            foreach (var c in crl.Controls) {
                if (c is Panel && ((Panel)c).Tag == null) {
                    ((Panel)c).Visible = false;
                }
            }
        }

        public Menu() {
            InitializeComponent();
            dataMarcacaoAdd.MaxDate = DateTime.Now;
        }

        private void pictureBox4_Click(object sender, EventArgs e) {
            DialogResult result = MessageBox.Show("Terminar Sessão?", "Confirmação", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) {
                Application.Restart();
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private async void button1_Click(object sender, EventArgs e) {
            if (Globais.job == 3 || Globais.idLoggedFunc != 0) {
                //funcionario
                if (panelConsultas.Visible == true) {
                    panelConsultas.Visible = false;
                } else {
                    panelConsultas.Visible = true;
                    panelConsultas.BringToFront();
                }
            } else {
                //paciente
                LimparForm(HistoricoBox);
                CloseGroupBoxs(this);
                ClosePanel(this);
                HistoricoBox.Visible = true;
                tipoMarcacaoAdd.Text = "Consulta";
                if (Globais.job == 3) {
                    //administrativo
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "marcacao";
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }

                                } else {
                                    dataGridView2.DataSource = null;
                                }
                            }
                        }
                    }
                } else if (Globais.idLoggedFunc != 0) {
                    //tecnico
                    int idfunc = 0;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=nif%3D" + Globais.loggedId;
                    Funcionario func = new Funcionario();
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
                                if (results.Count > 0 && Globais.loggedId != "") {
                                    IList<Funcionario> searchResults = new List<Funcionario>();
                                    foreach (JToken result in results) {
                                        Funcionario searchResult = result.ToObject<Funcionario>();
                                        //adiciona à "estrutura"
                                        searchResults.Add(searchResult);
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    func = (Funcionario)bsDados.List[0];
                                    idfunc = func.IdFuncionario;
                                }
                            }
                        }
                    }
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idtecnico%3D" + idfunc.ToString();
                    Marcacao marcacao = new Marcacao();
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
                                if (results.Count > 0 && idfunc != 0) {
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados.DataSource;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }
                                }
                            }
                        }
                    }

                } else {
                    //e paciente
                    int idpac = 0;
                    nifHistorico.Visible = false;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=nif%3D" + Globais.loggedId;
                    Paciente pac = new Paciente();
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
                                    pac = (Paciente)bsDados.List[0];
                                    idpac = pac.IdPaciente;
                                }
                            }
                        }
                    }
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idpaciente%3D" + idpac.ToString();
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }

                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados.DataSource;
                                    dataGridView2.Columns["IdPaciente"].Visible = false;
                                    dataGridView2.Columns["IdFuncionario"].Visible = false;
                                    dataGridView2.Columns["IdTecnico"].Visible = false;
                                    dataGridView2.Columns["UltimaAtualizacao"].Visible = false;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }
                                }
                            }
                        }
                    }
                }
            }
            panelExames.Visible = false;
            panelPacientes.Visible = false;
            panelFuncionarios.Visible = false;

        }

        private async void button2_Click(object sender, EventArgs e) {
            if (Globais.job == 3 || Globais.idLoggedFunc != 0) {
                //funcionario
                if (panelExames.Visible == true) {
                    panelExames.Visible = false;
                } else {
                    panelExames.Visible = true;
                    panelExames.BringToFront();
                }
            } else {
                //paciente
                LimparForm(HistoricoBox);
                CloseGroupBoxs(this);
                ClosePanel(this);
                HistoricoBox.Visible = true;
                tipoMarcacaoAdd.Text = "Exame";
                if (Globais.job == 3) {
                    //administrativo
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "marcacao";
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }

                                } else {
                                    dataGridView2.DataSource = null;
                                }
                            }
                        }
                    }
                } else if (Globais.idLoggedFunc != 0) {
                    //tecnico
                    int idfunc = 0;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=nif%3D" + Globais.loggedId;
                    Funcionario func = new Funcionario();
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
                                if (results.Count > 0 && Globais.loggedId != "") {
                                    IList<Funcionario> searchResults = new List<Funcionario>();
                                    foreach (JToken result in results) {
                                        Funcionario searchResult = result.ToObject<Funcionario>();
                                        //adiciona à "estrutura"
                                        searchResults.Add(searchResult);
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    func = (Funcionario)bsDados.List[0];
                                    idfunc = func.IdFuncionario;
                                }
                            }
                        }
                    }
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idtecnico%3D" + idfunc.ToString();
                    Marcacao marcacao = new Marcacao();
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
                                if (results.Count > 0 && idfunc != 0) {
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados.DataSource;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }
                                }
                            }
                        }
                    }

                } else {
                    //e paciente
                    int idpac = 0;
                    nifHistorico.Visible = false;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=nif%3D" + Globais.loggedId;
                    Paciente pac = new Paciente();
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
                                    pac = (Paciente)bsDados.List[0];
                                    idpac = pac.IdPaciente;
                                }
                            }
                        }
                    }
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idpaciente%3D" + idpac.ToString();
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados.DataSource;
                                    dataGridView2.Columns["IdPaciente"].Visible = false;
                                    dataGridView2.Columns["IdFuncionario"].Visible = false;
                                    dataGridView2.Columns["IdTecnico"].Visible = false;
                                    dataGridView2.Columns["UltimaAtualizacao"].Visible = false;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }
                                }
                            }
                        }
                    }
                }
            }
            panelPacientes.Visible = false;
            panelFuncionarios.Visible = false;
            panelConsultas.Visible = false;
        }

        private void panel1_Paint(object sender, PaintEventArgs e) {

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

        private void button3_Click(object sender, EventArgs e) {
            this.Hide();
            QRReader QRReader = new QRReader();
            QRReader.ShowDialog();

        }

        private void Menu_Load(object sender, EventArgs e) {
            dataMarcacaoAdd.MaxDate = new DateTime(2029, 12, 24);
            dataMarcacaoAdd.MinDate = DateTime.Now;
            dataMarcacaoAdd.Value = DateTime.Now;
            dataMarcacaoEditar.MaxDate = new DateTime(2029, 12, 24);
            dataMarcacaoEditar.MinDate = new DateTime(2020, 12, 24);
            dataMarcacaoEditar.Value = DateTime.Now;
            if (Globais.Admin == false) {
                if (Globais.job == 3 || Globais.funcON == true) {
                    //func
                    button6.Visible = false;
                } else {
                    //paciente
                    button5.Visible = false;
                    button6.Visible = false;
                }
            }
            
        }

        private void panel3_Paint(object sender, PaintEventArgs e) {

        }

        private void button1_MouseHover(object sender, EventArgs e) {
        }

        private void button5_Click(object sender, EventArgs e) {
            if (Globais.Admin == false) {
                button17.Visible = false;
                button15.Location = button17.Location;
                panelPacientes.Location= new Point(423,420);
                panelPacientes.Size = new Size(134,140);
            }
            if (panelPacientes.Visible == true) {
                panelPacientes.Visible = false;
            } else {
                panelPacientes.Visible = true;
                panelPacientes.BringToFront();
            }
            panelExames.Visible = false;
            panelFuncionarios.Visible = false;
            panelConsultas.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e) {
            if (Globais.Admin == false) {
                button22.Visible = false;
                button19.Location = button22.Location;
                panelFuncionarios.Location = new Point(563, 420);
                panelFuncionarios.Size = new Size(134, 140);
            }
            if (panelFuncionarios.Visible == true) {
                panelFuncionarios.Visible = false;
            } else {
                panelFuncionarios.Visible = true;
                panelFuncionarios.BringToFront();
            }
            panelExames.Visible = false;
            panelPacientes.Visible = false;
            panelConsultas.Visible = false;
        }

        private void label10_Click(object sender, EventArgs e) {

        }

        private void label15_Click(object sender, EventArgs e) {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e) {

        }

        private async void button7_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            bool noExists = false;
            LimparForm(AddConsultasBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            AddConsultasBox.Visible = true;
            tipoMarcacaoAdd.Text = "Consulta";
            tipoMarcacaoRemover.Text = "Consulta";
            label2.Text = "Adicionar Consulta";
            ProfissionaisMarcacaoAdd.Items.Clear();

            List<Globais.Filter> filtros = new List<Globais.Filter>();

            var filtro = new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or",
            };

            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 2,
                Logic = "or"
            });
            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or"
            });
            var filter = new Globais.FilterDTO() {
                Offset = 0,
                Limit = 10
            };
            filtro.Filters = filtros;
            filter.Filter = filtro;

            String URI;
            URI = Globais.baseURL + "Funcionario/filter";
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                var serializedUtilizador = JsonConvert.SerializeObject(filter);
                var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(URI, content);
                if (response != null) {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject rsp = JObject.Parse(jsonString);
                    IList<JToken> results = rsp["value"].Children().ToList();
                    if (results.Count > 0) {
                        IList<Funcionario> searchResults = new List<Funcionario>();
                        foreach (JToken result in results) {
                            Funcionario searchResult = result.ToObject<Funcionario>();
                            //adiciona à "estrutura"
                            searchResults.Add(searchResult);
                        }
                        for (int i = 0; i < searchResults.Count; i++) {
                            if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Masculino ") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Dr. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Feminino") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Dra. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Masculino") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Enfermeiro " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Feminino") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Enfermeira " + searchResults[i].Nome);

                            } else if (searchResults[i].Funcao == 4) {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Técnico " + searchResults[i].Nome);
                            }
                        }
                    }

                } else {
                    MessageBox.Show("Erro. Contacte o administrador do sistema");
                    Console.WriteLine("Erro do servidor");
                }

            }

            BindingSource bsDados = new BindingSource();
            URI = Globais.baseURL + "marcacao";
            Marcacao marcacao = new Marcacao();
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                using (var response = await client.GetAsync(URI)) {
                    if (response.IsSuccessStatusCode) {
                        //it's client
                        //recebe a query em json
                        var MarcacaoJsonString = await response.Content.ReadAsStringAsync();
                        //transforma para string
                        JObject rsp = JObject.Parse(MarcacaoJsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            IList<Marcacao> searchResults = new List<Marcacao>();
                            foreach (JToken result in results) {
                                Marcacao searchResult = result.ToObject<Marcacao>();
                                //adiciona à "estrutura"
                                searchResults.Add(searchResult);
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            marcacao = (Marcacao)bsDados.List[0];
                            Globais.maxMarcacao = marcacao.IdMarcacao + 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        } else {
                            Globais.maxMarcacao = 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        }
                    }
                }
            }


        }

        private async void nifMarcacaoAdd_TextChanged(object sender, EventArgs e) {
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(nifMarcacaoAdd.Text);
            if (matches.Count > 0 || nifMarcacaoAdd.Text == " " || nifMarcacaoAdd.Text.Length > 9) {
                nifMarcacaoAdd.Text = "";
                System.Media.SystemSounds.Hand.Play();

            } else {
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + nifMarcacaoAdd.Text;
                Paciente paciente = new Paciente();
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
                            if (results.Count > 0 && nifMarcacaoAdd.Text != "") {
                                IList<Paciente> searchResults = new List<Paciente>();
                                foreach (JToken result in results) {
                                    Paciente searchResult = result.ToObject<Paciente>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                paciente = (Paciente)bsDados.List[0];
                                nomePaciente.Text = paciente.Nome;
                                sexoPaciente.Text = paciente.Sexo;
                                telemovelPaciente.Text = paciente.Telemovel;
                                nacioalidadePaciente.Text = paciente.Nacionalidade;
                                nascPaciente.Value = paciente.DataNasc.Date;
                                emailPaciente.Text = paciente.Email;
                                ccPaciente.Text = paciente.CC;
                                Globais.idPaciente = paciente.IdPaciente;

                            } else if (results.Count == 0 || nifMarcacaoAdd.Text == "") {
                                nomePaciente.Text = "";
                                sexoPaciente.Text = "";
                                telemovelPaciente.Text = "";
                                nacioalidadePaciente.Text = "";
                                nascPaciente.Value = DateTime.Now.Date;
                                emailPaciente.Text = "";
                                ccPaciente.Text = "";
                                paciente = null;
                                Globais.idPaciente = 0;
                            }
                        }
                    }
                }
            }
        }

        private async void button23_Click(object sender, EventArgs e) {
            string horas = "";
            string idfunc = "";
            bool nExists = false;
            if (nifMarcacaoAdd.Text != "" && DateTime.Now.Date <= dataMarcacaoAdd.Value.Date && comboHoraAdd.Text != "" && ProfissionaisMarcacaoAdd.Text != "") {

                foreach (char c in comboHoraAdd.Text) {
                    if (c == ':') {
                        break;
                    } else {
                        horas += c.ToString();
                    }
                }

                foreach (char c in ProfissionaisMarcacaoAdd.Text) {
                    if (c == ' ') {
                        break;
                    } else {
                        idfunc += c.ToString();
                    }
                }

                DateTime ts = new DateTime();
                ts = ts.AddHours(Convert.ToInt32(horas));
                List<Globais.Filter> filtros = new List<Globais.Filter>();

                var filtro = new Globais.Filter() {
                    Field = "Data",
                    Operator = "eq",
                    Value = dataMarcacaoAdd.Value.Date,
                    Logic = "and",
                };

                filtros.Add(new Globais.Filter() {
                    Field = "Hora",
                    Operator = "eq",
                    Value = ts.TimeOfDay,
                    Logic = "and"
                });
                filtros.Add(new Globais.Filter() {
                    Field = "IdTecnico",
                    Operator = "eq",
                    Value = idfunc,
                    Logic = "and"
                });
                filtros.Add(new Globais.Filter() {
                    Field = "Data",
                    Operator = "eq",
                    Value = dataMarcacaoAdd.Value.Date,
                    Logic = "and"
                });
                var filter = new Globais.FilterDTO() {
                    Offset = 0,
                    Limit = 10
                };
                filtro.Filters = filtros;
                filter.Filter = filtro;

                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "Marcacao/filter";
                Marcacao marcacao = new Marcacao();

                using (var client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                    var serializedUtilizador = JsonConvert.SerializeObject(filter);
                    var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(URI, content);
                    if (response != null) {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject rsp = JObject.Parse(jsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            MessageBox.Show("Já existe uma marcação neste horário.");
                            nExists = false;
                        } else {
                            nExists = true;
                        }

                    } else {
                        MessageBox.Show("Erro");
                    }
                }

                if (nExists == true && nomePaciente.Text!="") {
                    URI = Globais.baseURL + "marcacao";
                    using (var client = new HttpClient()) {
                        //Criar class Paciente. substituir filtyer na 131 por paciente.
                        //confirmar a response como na 109
                        marcacao = new Marcacao();
                        marcacao.IdPaciente = Globais.idPaciente;
                        marcacao.IdTecnico = Convert.ToInt32(idfunc);
                        if (Globais.job == 3) {
                            marcacao.IdFuncionario = Globais.idLoggedFunc;
                        } else {
                            marcacao.IdFuncionario = 0;
                        }
                        marcacao.Data = dataMarcacaoAdd.Value.Date;
                        marcacao.Hora = ts.TimeOfDay;
                        marcacao.Tipo = tipoMarcacaoAdd.Text;
                        marcacao.QRCODE = Globais.maxMarcacao.ToString();
                        marcacao.Relatorio = "";
                        marcacao.UltimaAtualizacao = DateTime.Now;
                        //TODO encriptar a senha
                        var serializedUtilizador = JsonConvert.SerializeObject(marcacao);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.Created) {
                            Console.WriteLine("Marcação realizada");
                            MessageBox.Show("Marcação Realizada");
                            LimparForm(AddConsultasBox);
                        } else {

                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }
                    }
                } else if(nomePaciente.Text == "") {
                    MessageBox.Show("Confirme os dados da marcaçao");
                }

            } else {
                MessageBox.Show("Preencha todos os campos.");
            }
        }

        private async void button8_Click(object sender, EventArgs e) {
            LimparForm(EditarConsultasBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            EditarConsultasBox.Visible = true;
            tipoMarcacaoAdd.Text = "Consulta";
            label31.Text = "Editar Consulta";
            profissionalMarcacaoEditar.Items.Clear();
            List<Globais.Filter> filtros = new List<Globais.Filter>();

            var filtro = new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or",
            };

            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 2,
                Logic = "or"
            });
            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or"
            });
            var filter = new Globais.FilterDTO() {
                Offset = 0,
                Limit = 10
            };
            filtro.Filters = filtros;
            filter.Filter = filtro;

            String URI;
            URI = Globais.baseURL + "Funcionario/filter";
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                var serializedUtilizador = JsonConvert.SerializeObject(filter);
                var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(URI, content);
                if (response != null) {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject rsp = JObject.Parse(jsonString);
                    IList<JToken> results = rsp["value"].Children().ToList();
                    if (results.Count > 0) {
                        IList<Funcionario> searchResults = new List<Funcionario>();
                        foreach (JToken result in results) {
                            Funcionario searchResult = result.ToObject<Funcionario>();
                            //adiciona à "estrutura"
                            searchResults.Add(searchResult);
                        }
                        for (int i = 0; i < searchResults.Count; i++) {
                            if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Masculino ") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Dr. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Feminino") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Dra. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Masculino") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Enfermeiro " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Feminino") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Enfermeira " + searchResults[i].Nome);

                            } else if (searchResults[i].Funcao == 4) {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Técnico " + searchResults[i].Nome);
                            }
                        }
                    }

                } else {
                    MessageBox.Show("Erro. Contacte o administrador do sistema");
                    Console.WriteLine("Erro do servidor");
                }

            }

            BindingSource bsDados = new BindingSource();
            URI = Globais.baseURL + "marcacao";
            Marcacao marcacao = new Marcacao();
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                using (var response = await client.GetAsync(URI)) {
                    if (response.IsSuccessStatusCode) {
                        //it's client
                        //recebe a query em json
                        var MarcacaoJsonString = await response.Content.ReadAsStringAsync();
                        //transforma para string
                        JObject rsp = JObject.Parse(MarcacaoJsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            IList<Marcacao> searchResults = new List<Marcacao>();
                            foreach (JToken result in results) {
                                Marcacao searchResult = result.ToObject<Marcacao>();
                                //adiciona à "estrutura"
                                searchResults.Add(searchResult);
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            marcacao = (Marcacao)bsDados.List[0];
                            Globais.maxMarcacao = marcacao.IdMarcacao + 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        } else {
                            Globais.maxMarcacao = 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        }
                    }
                }
            }
        }

        private async void codMarcacaoEditar_TextChanged(object sender, EventArgs e) {
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(codMarcacaoEditar.Text);
            if (matches.Count > 0 || codMarcacaoEditar.Text == " ") {
                codMarcacaoEditar.Text = "";
                System.Media.SystemSounds.Hand.Play();

            } else {
                int id = 0;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "marcacao?Query=idmarcacao%3D" + codMarcacaoEditar.Text;
                Marcacao marcacao = new Marcacao();
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
                            if (results.Count > 0 && codMarcacaoEditar.Text != "") {
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                if (searchResults.Count > 0) {
                                    bsDados.DataSource = searchResults.ToList();
                                    marcacao = (Marcacao)bsDados.List[0];
                                    id = marcacao.IdPaciente;
                                    nifMarcacaoEditar.Text = id.ToString();
                                    dataMarcacaoEditar.Value = marcacao.Data.Date;
                                    int horas = marcacao.Hora.Hours;
                                    relatorioUpdateConsulta.Text = marcacao.Relatorio;
                                    horaMarcacaoEditar.SelectedIndex = horaMarcacaoEditar.FindString(horas.ToString());
                                    profissionalMarcacaoEditar.SelectedIndex = profissionalMarcacaoEditar.FindString(marcacao.IdTecnico.ToString());
                                    dataMarcacaoEditar.Enabled = true;
                                    horaMarcacaoEditar.Enabled = true;
                                    profissionalMarcacaoEditar.Enabled = true;
                                    horaMarcacaoEditar.Enabled = true;
                                    button24.Enabled = true;
                                }
                            } else if (results.Count == 0 || codMarcacaoEditar.Text == "") {
                                relatorioUpdateConsulta.Text = "";
                                nifMarcacaoEditar.Text = "";
                                dataMarcacaoEditar.Value = DateTime.Now;
                                horaMarcacaoEditar.SelectedIndex = -1;
                                profissionalMarcacaoEditar.SelectedIndex = -1;
                                dataMarcacaoEditar.Enabled = false;
                                horaMarcacaoEditar.Enabled = false;
                                profissionalMarcacaoEditar.Enabled = false;
                                horaMarcacaoEditar.Enabled = false;
                                button24.Enabled = false;
                                marcacao = null;
                            }
                        }
                    }
                }
                if (marcacao != null) {
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "paciente?Query=idpaciente%3D" + id;
                    Paciente paciente = new Paciente();
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
                                    paciente = (Paciente)bsDados.List[0];
                                    nifMarcacaoEditar.Text = paciente.NIF;
                                    nomePaciente2.Text = paciente.Nome;
                                    sexoPaciente2.Text = paciente.Sexo;
                                    telemovelPaciente2.Text = paciente.Telemovel;
                                    nacionalidadePaciente2.Text = paciente.Nacionalidade;
                                    nascPaciente2.Value = paciente.DataNasc.Date;
                                    emailPaciente2.Text = paciente.Email;
                                    CCPaciente2.Text = paciente.CC;
                                    Globais.idPaciente = paciente.IdPaciente;

                                } else if (results.Count == 0) {
                                    nomePaciente2.Text = "";
                                    sexoPaciente2.Text = "";
                                    telemovelPaciente2.Text = "";
                                    nacionalidadePaciente2.Text = "";
                                    nascPaciente2.Value = DateTime.Now.Date;
                                    emailPaciente2.Text = "";
                                    CCPaciente2.Text = "";
                                    nifMarcacaoEditar.Text = "";
                                    Globais.idPaciente = 0;
                                    paciente = null;
                                }
                            }
                        }
                    }
                } else {
                    nomePaciente2.Text = "";
                    sexoPaciente2.Text = "";
                    telemovelPaciente2.Text = "";
                    nacionalidadePaciente2.Text = "";
                    nascPaciente2.Value = DateTime.Now.Date;
                    emailPaciente2.Text = "";
                    CCPaciente2.Text = "";
                    nifMarcacaoEditar.Text = "";
                    Globais.idPaciente = 0;
                }
            }
        }

        private void AddConsultasBox_Enter(object sender, EventArgs e) {

        }

        private async void nifMarcacaoEditar_TextChanged(object sender, EventArgs e) {
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(nifMarcacaoEditar.Text);
            if (matches.Count > 0 || nifMarcacaoEditar.Text == " " || nifMarcacaoEditar.Text.Length > 9) {
                nifMarcacaoEditar.Text = "";
                System.Media.SystemSounds.Hand.Play();

            } else {
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + nifMarcacaoEditar.Text;
                Paciente paciente = new Paciente();
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
                            if (results.Count > 0 && nifMarcacaoEditar.Text != "") {
                                IList<Paciente> searchResults = new List<Paciente>();
                                foreach (JToken result in results) {
                                    Paciente searchResult = result.ToObject<Paciente>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                paciente = (Paciente)bsDados.List[0];
                                nomePaciente2.Text = paciente.Nome;
                                sexoPaciente2.Text = paciente.Sexo;
                                telemovelPaciente2.Text = paciente.Telemovel;
                                nacionalidadePaciente2.Text = paciente.Nacionalidade;
                                nascPaciente2.Value = paciente.DataNasc.Date;
                                emailPaciente2.Text = paciente.Email;
                                CCPaciente2.Text = paciente.CC;
                                Globais.idPaciente = paciente.IdPaciente;

                            } else if (results.Count == 0 || nifMarcacaoEditar.Text == "") {
                                nomePaciente2.Text = "";
                                sexoPaciente2.Text = "";
                                telemovelPaciente2.Text = "";
                                nacionalidadePaciente2.Text = "";
                                nascPaciente2.Value = DateTime.Now.Date;
                                emailPaciente2.Text = "";
                                CCPaciente2.Text = "";
                                paciente = null;
                                Globais.idPaciente = 0;
                            }
                        }
                    }
                }
            }
        }

        private async void button24_Click(object sender, EventArgs e) {
            //Editar Consulta

            string horas = "";
            string idfunc = "";
            bool nExists = false;
            if (nifMarcacaoEditar.Text != "" && horaMarcacaoEditar.Text != "" && profissionalMarcacaoEditar.Text != "") {

                foreach (char c in horaMarcacaoEditar.Text) {
                    if (c == ':') {
                        break;
                    } else {
                        horas += c.ToString();
                    }
                }

                foreach (char c in profissionalMarcacaoEditar.Text) {
                    if (c == ' ') {
                        break;
                    } else {
                        idfunc += c.ToString();
                    }
                }

                DateTime ts = new DateTime();
                ts = ts.AddHours(Convert.ToInt32(horas));
                List<Globais.Filter> filtros = new List<Globais.Filter>();

                var filtro = new Globais.Filter() {
                    Field = "Data",
                    Operator = "eq",
                    Value = dataMarcacaoEditar.Value.Date,
                    Logic = "and",
                };

                filtros.Add(new Globais.Filter() {
                    Field = "Hora",
                    Operator = "eq",
                    Value = ts.TimeOfDay,
                    Logic = "and"
                });
                filtros.Add(new Globais.Filter() {
                    Field = "IdTecnico",
                    Operator = "eq",
                    Value = idfunc,
                    Logic = "and"
                });
                filtros.Add(new Globais.Filter() {
                    Field = "Data",
                    Operator = "eq",
                    Value = dataMarcacaoEditar.Value.Date,
                    Logic = "and"
                });
                filtros.Add(new Globais.Filter() {
                    Field = "IdMarcacao",
                    Operator = "neq",
                    Value = codMarcacaoEditar.Text,
                    Logic = "and"
                });
                var filter = new Globais.FilterDTO() {
                    Offset = 0,
                    Limit = 10
                };
                filtro.Filters = filtros;
                filter.Filter = filtro;

                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "Marcacao/filter";
                Marcacao marcacao = new Marcacao();

                using (var client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                    var serializedUtilizador = JsonConvert.SerializeObject(filter);
                    var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(URI, content);
                    if (response != null) {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        JObject rsp = JObject.Parse(jsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            MessageBox.Show("Já existe uma marcação neste horário.");
                            nExists = false;
                        } else {
                            nExists = true;
                        }

                    } else {
                        MessageBox.Show("Erro");
                    }
                }

                if (nExists == true && nomePaciente2.Text!="") {
                    URI = Globais.baseURL + "marcacao/" + codMarcacaoEditar.Text;
                    using (var client = new HttpClient()) {
                        //Criar class Paciente. substituir filtyer na 131 por paciente.
                        //confirmar a response como na 109
                        marcacao = new Marcacao();
                        marcacao.IdPaciente = Globais.idPaciente;
                        marcacao.IdTecnico = Convert.ToInt32(idfunc);
                        if (Globais.job == 3) {
                            marcacao.IdFuncionario = Globais.idLoggedFunc;
                        } else {
                            marcacao.IdFuncionario = 0;
                        }
                        marcacao.Data = dataMarcacaoEditar.Value.Date;
                        marcacao.Hora = ts.TimeOfDay;
                        marcacao.Tipo = tipoMarcacaoAdd.Text;
                        marcacao.QRCODE = codMarcacaoEditar.Text ;
                        marcacao.Relatorio = relatorioUpdateConsulta.Text;
                        marcacao.UltimaAtualizacao = DateTime.Now;
                        //TODO encriptar a senha
                        var serializedUtilizador = JsonConvert.SerializeObject(marcacao);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(URI, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                            Console.WriteLine("Marcação editada");
                            MessageBox.Show("Marcação Atualizada");
                            LimparForm(EditarConsultasBox);
                        } else {

                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }
                    }
                } else if (nomePaciente2.Text=="") {
                    MessageBox.Show("Confirme os dados da marcaçao");
                }

            } else {
                MessageBox.Show("Preencha todos os campos.");
            }
        }

        private async void codMarcacaoAdd_TextChanged(object sender, EventArgs e) {
            if (codMarcacaoAdd.Text == "") {
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "marcacao";
                Marcacao marcacao = new Marcacao();
                using (var client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                    using (var response = await client.GetAsync(URI)) {
                        if (response.IsSuccessStatusCode) {
                            //it's client
                            //recebe a query em json
                            var MarcacaoJsonString = await response.Content.ReadAsStringAsync();
                            //transforma para string
                            JObject rsp = JObject.Parse(MarcacaoJsonString);
                            IList<JToken> results = rsp["value"].Children().ToList();
                            if (results.Count > 0) {
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                marcacao = (Marcacao)bsDados.List[0];
                                Globais.maxMarcacao = marcacao.IdMarcacao + 1;
                                codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                            } else {
                                Globais.maxMarcacao = 1;
                                codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                            }
                        }
                    }
                }
            }
        }

        private void tipoMarcacaoAdd_TextChanged(object sender, EventArgs e) {
            if (tipoMarcacaoAdd.Text == "") {
                tipoMarcacaoAdd.Text = tipoMarcacaoRemover.Text;
            }
        }

        private void button9_Click(object sender, EventArgs e) {
            LimparForm(ApagarConsultaBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            ApagarConsultaBox.Visible = true;
            tipoMarcacaoAdd.Text = "Consulta";
            label46.Text = "Desmarcar Consulta";
        }

        private async void nifMarcacaoRemover_TextChanged(object sender, EventArgs e) {
            Boolean exists = false;
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(nifMarcacaoAdd.Text);
            if (matches.Count > 0 || nifMarcacaoRemover.Text == " " || nifMarcacaoRemover.Text.Length > 9) {
                nifMarcacaoRemover.Text = "";
                System.Media.SystemSounds.Hand.Play();

            } else {
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + nifMarcacaoRemover.Text;
                Paciente paciente = new Paciente();
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
                            if (results.Count > 0 && nifMarcacaoRemover.Text != "") {
                                IList<Paciente> searchResults = new List<Paciente>();
                                foreach (JToken result in results) {
                                    Paciente searchResult = result.ToObject<Paciente>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                paciente = (Paciente)bsDados.List[0];
                                Globais.idPaciente = paciente.IdPaciente;
                                exists = true;

                            } else if (results.Count == 0 || nifMarcacaoRemover.Text == "") {
                                paciente = null;
                                Globais.idPaciente = 0;
                                codMarcacaoRemover.Text = "";
                                tipoMarcacaoRemover.Text = "";
                                dataMarcacaoRemover.Value = DateTime.Now;
                                horaMarcacaoRemover.Text = "";
                                profissionalMarcacaoRemover.Text = "";
                                exists = false;
                                dataGridView1.DataSource = null;
                            }
                        }
                    }
                }
                if (exists == true) {
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idpaciente%3D" + Globais.idPaciente;
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Data.Date >= DateTime.Now.Date && searchResult.Tipo==tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView1.DataSource = bsDados;

                                } else {
                                    dataGridView1.DataSource = null;
                                }
                            }
                        }
                    }
                }

            }
        }

        private async void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex != -1) {

                DataGridViewRow dataGridViewRow = dataGridView1.Rows[e.RowIndex];
                string idfnc = "";
                idfnc = dataGridViewRow.Cells[1].Value.ToString();
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "funcionario?Query=idfuncionario%3D" + idfnc;
                Funcionario funcionario = new Funcionario();
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
                                    if (searchResult.Funcao == 1 && searchResult.Sexo == "Masculino ") {
                                        profissionalMarcacaoRemover.Text = (searchResult.IdFuncionario + " - Dr. " + searchResult.Nome);
                                    } else if (searchResult.Funcao == 1 && searchResult.Sexo == "Feminino") {
                                        profissionalMarcacaoRemover.Text = (searchResult.IdFuncionario + " - Dra. " + searchResult.Nome);
                                    } else if (searchResult.Funcao == 2 && searchResult.Sexo == "Masculino") {
                                        profissionalMarcacaoRemover.Text = (searchResult.IdFuncionario + " - Enfermeiro " + searchResult.Nome);
                                    } else if (searchResult.Funcao == 2 && searchResult.Sexo == "Feminino") {
                                        profissionalMarcacaoRemover.Text = (searchResult.IdFuncionario + " - Enfermeira " + searchResult.Nome);

                                    }
                                }
                            }
                        }
                    }
                }

                codMarcacaoRemover.Text = dataGridViewRow.Cells[0].Value.ToString();
                tipoMarcacaoRemover.Text = dataGridViewRow.Cells[6].Value.ToString();
                dataMarcacaoRemover.Value = DateTime.Parse(dataGridViewRow.Cells[4].Value.ToString());
                horaMarcacaoRemover.Text = dataGridViewRow.Cells[5].Value.ToString();
            }
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {

        }

        private async void button25_Click(object sender, EventArgs e) {
            if (codMarcacaoRemover.Text != "") {
            string URI = Globais.baseURL + "marcacao/" + codMarcacaoRemover.Text;
                using (var client = new HttpClient()) {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                    using (var response = await client.DeleteAsync(URI)) {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) {
                            MessageBox.Show("Marcação desmarcada com sucesso.");
                            nifMarcacaoRemover.Text = "";
                        } else {
                            MessageBox.Show("Erro na desmarcação da consulta");
                        }
                    }
                }

            } else {
                MessageBox.Show("Não é possível desmarcar a consulta.");
            }
        }

        private void dataGridView2_VisibleChanged(object sender, EventArgs e) {
        }

        private async void button10_Click(object sender, EventArgs e) {
            LimparForm(HistoricoBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            HistoricoBox.Visible = true;
            tipoMarcacaoAdd.Text = "Consulta";
            label28.Text = "Histórico Consultas";
            if (Globais.job == 3) {
                //administrativo
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "marcacao";
                Marcacao marcacao = new Marcacao();
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
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView2.DataSource = bsDados;
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                    if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                        row.DefaultCellStyle.BackColor = Color.Yellow;
                                    } else {
                                        row.DefaultCellStyle.BackColor = Color.Green;
                                    }

                            } else {
                                dataGridView2.DataSource = null;
                            }
                        }
                    }
                }
            } else if (Globais.idLoggedFunc!=0) {
                //tecnico
                int idfunc=0;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "funcionario?Query=nif%3D" + Globais.loggedId;
                Funcionario func = new Funcionario();
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
                            if (results.Count > 0 && Globais.loggedId != "") {
                                IList<Funcionario> searchResults = new List<Funcionario>();
                                foreach (JToken result in results) {
                                    Funcionario searchResult = result.ToObject<Funcionario>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                func = (Funcionario)bsDados.List[0];
                                idfunc = func.IdFuncionario;
                            }
                            }
                        }
                    }
                 bsDados = new BindingSource();
                 URI = Globais.baseURL + "marcacao?Query=idtecnico%3D" + idfunc.ToString();
                Marcacao marcacao = new Marcacao();
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
                            if (results.Count > 0 && idfunc != 0) {
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView2.DataSource = bsDados.DataSource;
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                    if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                        row.DefaultCellStyle.BackColor = Color.Yellow;
                                    } else {
                                        row.DefaultCellStyle.BackColor = Color.Green;
                                    }
                            }
                        }
                    }
                }

            } else {
                //e paciente
                int idpac=0;
                nifHistorico.Visible = false;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + Globais.loggedId;
                Paciente pac = new Paciente();
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
                                pac = (Paciente)bsDados.List[0];
                                idpac = pac.IdPaciente;
                            }
                        }
                    }
                }
                bsDados = new BindingSource();
                URI = Globais.baseURL + "marcacao?Query=idpaciente%3D" + idpac.ToString();
                Marcacao marcacao = new Marcacao();
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
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if(searchResult.Tipo== tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                    
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView2.DataSource = bsDados.DataSource;
                                dataGridView2.Columns["IdPaciente"].Visible = false;
                                dataGridView2.Columns["IdFuncionario"].Visible = false;
                                dataGridView2.Columns["IdTecnico"].Visible = false;
                                dataGridView2.Columns["UltimaAtualizacao"].Visible = false;
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                    if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                        row.DefaultCellStyle.BackColor = Color.Yellow;
                                    } else {
                                        row.DefaultCellStyle.BackColor = Color.Green;
                                    }
                            }
                        }
                    }
                }
            }
        }

        private async void nifHistorico_TextChanged(object sender, EventArgs e) {
            if (Globais.job == 3) {
                //administrativo
                int idPaciente = 0;
                Boolean nada = false;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + nifHistorico.Text;
                Paciente pac = new Paciente();
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
                            if (results.Count > 0 && nifHistorico.Text != "") {
                                IList<Paciente> searchResults = new List<Paciente>();
                                foreach (JToken result in results) {
                                    Paciente searchResult = result.ToObject<Paciente>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                pac = (Paciente)bsDados.List[0];
                                idPaciente = pac.IdPaciente;
                                nada = false;
                            } else if (nifHistorico.Text=="") {
                                nada = true;
                            } else {
                                bsDados.DataSource = null;
                            }
                        }
                    }
                }
                if (nada == true) {
                     bsDados = new BindingSource();
                     URI = Globais.baseURL + "marcacao";
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }

                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }

                                } else {
                                    dataGridView2.DataSource = null;
                                }
                            }
                        }
                    }
                } else {
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idpaciente%3D" + idPaciente;
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }

                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }

                                } else {
                                    dataGridView2.DataSource = null;
                                }
                            }
                        }
                    }

                }
            } else {
                //tecnico
                //filtrar por nif e idtecnico
                int idPaciente = 0;
                Boolean nada = false;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + nifHistorico.Text;
                Paciente pac = new Paciente();
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
                            if (results.Count > 0 && nifHistorico.Text != "") {
                                IList<Paciente> searchResults = new List<Paciente>();
                                foreach (JToken result in results) {
                                    Paciente searchResult = result.ToObject<Paciente>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                pac = (Paciente)bsDados.List[0];
                                idPaciente = pac.IdPaciente;
                                nada = false;
                            } else if (nifHistorico.Text == "") {
                                nada = true;
                            } else {
                                dataGridView2.DataSource = null;
                            }
                        }
                    }
                }
                if (nada == true) {
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "marcacao?Query=idtecnico%3D" + Globais.idLoggedFunc;
                    Marcacao marcacao = new Marcacao();
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
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }

                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }

                                } else {
                                    dataGridView2.DataSource = null;
                                }
                            }
                        }
                    }
                } else {
                    if (idPaciente != 0) {
                        List<Globais.Filter> filtros = new List<Globais.Filter>();

                        var filtro = new Globais.Filter() {
                            Field = "Idtecnico",
                            Operator = "eq",
                            Value = Globais.idLoggedFunc,
                            Logic = "and",
                        };

                        filtros.Add(new Globais.Filter() {
                            Field = "IdPaciente",
                            Operator = "eq",
                            Value = idPaciente,
                            Logic = "and"
                        });

                        filtros.Add(new Globais.Filter() {
                            Field = "Idtecnico",
                            Operator = "eq",
                            Value = Globais.idLoggedFunc,
                            Logic = "and"
                        });

                        var filter = new Globais.FilterDTO() {
                            Offset = 0,
                            Limit = 10
                        };
                        filtro.Filters = filtros;
                        filter.Filter = filtro;

                        URI = Globais.baseURL + "marcacao/filter";
                        using (var client = new HttpClient()) {
                            var serializedUtilizador = JsonConvert.SerializeObject(filter);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(URI, content);
                            if (response != null) {
                                var jsonString = await response.Content.ReadAsStringAsync();
                                JObject rsp = JObject.Parse(jsonString);
                                IList<JToken> results = rsp["value"].Children().ToList();
                                if (results.Count > 0) {
                                    IList<Marcacao> searchResults = new List<Marcacao>();
                                    foreach (JToken result in results) {
                                        Marcacao searchResult = result.ToObject<Marcacao>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                            searchResults.Add(searchResult);
                                        }

                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView2.DataSource = bsDados;
                                    foreach (DataGridViewRow row in dataGridView2.Rows)
                                        if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                            row.DefaultCellStyle.BackColor = Color.Yellow;
                                        } else {
                                            row.DefaultCellStyle.BackColor = Color.Green;
                                        }

                                } else {
                                    dataGridView2.DataSource = null;
                                }

                            } else {
                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }

                        }

                    }
                }
            }
        }

        private async void button4_Click(object sender, EventArgs e) {
            panelPacientes.Visible = false;
            panelFuncionarios.Visible = false;
            panelConsultas.Visible = false;
            LimparForm(PerfilBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            if (Globais.idLoggedFunc == 0 && Globais.job != 3) {
                label51.Text = "Perfil Paciente";
            } else {
                label51.Text = "Perfil Funcionário";
            }
            PerfilBox.Visible = true;

            Boolean nada = false;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "paciente?Query=nif%3D" + Globais.loggedId;
            Paciente pac = new Paciente();
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
                            pac = (Paciente)bsDados.List[0];
                            nada = false;
                            Globais.idPerfil = pac.IdPaciente;
                            Globais.idPerfilUTIL = pac.IdUtilizador;
                            nomePerfil.Text = pac.Nome;
                            sexoPerfil.Text = pac.Sexo;
                            telemovelPerfil.Text = pac.Telemovel;
                            nacionalidadePerfil.Text = pac.Nacionalidade;
                            nascPerfil.Value = pac.DataNasc.Date;
                            emailPerfil.Text = pac.Email;
                            ccPerfil.Text= pac.CC;
                            nifPerfil.Text = pac.NIF;
                        } else {
                            nada = true;
                        }
                    }
                }
            }
            if (nada == true) {
                bsDados = new BindingSource();
                URI = Globais.baseURL + "funcionario?Query=nif%3D" + Globais.loggedId;
                Funcionario fuc = new Funcionario();
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
                                fuc = (Funcionario)bsDados.List[0];
                                nada = false;
                                Globais.idPerfil = fuc.IdFuncionario;
                                Globais.idPerfilUTIL = fuc.IdUtilizador;
                                Globais.tipoPERFIL = fuc.Funcao;
                                nomePerfil.Text = fuc.Nome;
                                sexoPerfil.Text = fuc.Sexo;
                                telemovelPerfil.Text = fuc.Telemovel;
                                nacionalidadePerfil.Text = fuc.Nacionalidade;
                                nascPerfil.Value = fuc.DataNasc.Date;
                                emailPerfil.Text = fuc.Email;
                                ccPerfil.Text = fuc.CC;
                                nifPerfil.Text = fuc.NIF;
                            }
                        }
                    }
                }
            }
        }

        private void PerfilBox_Enter(object sender, EventArgs e) {

        }

        private async void button14_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            bool noExists = false;
            LimparForm(AddConsultasBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            AddConsultasBox.Visible = true;
            tipoMarcacaoAdd.Text = "Exame";
            tipoMarcacaoRemover.Text = "Exame";
            label2.Text = "Adicionar Exame";
            ProfissionaisMarcacaoAdd.Items.Clear();
            List<Globais.Filter> filtros = new List<Globais.Filter>();

            var filtro = new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or",
            };

            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 2,
                Logic = "or"
            });
            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or"
            });
            var filter = new Globais.FilterDTO() {
                Offset = 0,
                Limit = 10
            };
            filtro.Filters = filtros;
            filter.Filter = filtro;

            String URI;
            URI = Globais.baseURL + "Funcionario/filter";
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                var serializedUtilizador = JsonConvert.SerializeObject(filter);
                var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(URI, content);
                if (response != null) {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject rsp = JObject.Parse(jsonString);
                    IList<JToken> results = rsp["value"].Children().ToList();
                    if (results.Count > 0) {
                        IList<Funcionario> searchResults = new List<Funcionario>();
                        foreach (JToken result in results) {
                            Funcionario searchResult = result.ToObject<Funcionario>();
                            //adiciona à "estrutura"
                            searchResults.Add(searchResult);
                        }
                        for (int i = 0; i < searchResults.Count; i++) {
                            if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Masculino ") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Dr. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Feminino") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Dra. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Masculino") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Enfermeiro " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Feminino") {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Enfermeira " + searchResults[i].Nome);

                            } else if (searchResults[i].Funcao == 4) {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Técnico " + searchResults[i].Nome);
                            }
                        }
                    }

                } else {
                    MessageBox.Show("Erro. Contacte o administrador do sistema");
                    Console.WriteLine("Erro do servidor");
                }

            }

            BindingSource bsDados = new BindingSource();
            URI = Globais.baseURL + "marcacao";
            Marcacao marcacao = new Marcacao();
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                using (var response = await client.GetAsync(URI)) {
                    if (response.IsSuccessStatusCode) {
                        //it's client
                        //recebe a query em json
                        var MarcacaoJsonString = await response.Content.ReadAsStringAsync();
                        //transforma para string
                        JObject rsp = JObject.Parse(MarcacaoJsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            IList<Marcacao> searchResults = new List<Marcacao>();
                            foreach (JToken result in results) {
                                Marcacao searchResult = result.ToObject<Marcacao>();
                                //adiciona à "estrutura"
                                searchResults.Add(searchResult);
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            marcacao = (Marcacao)bsDados.List[0];
                            Globais.maxMarcacao = marcacao.IdMarcacao + 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        } else {
                            Globais.maxMarcacao = 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        }
                    }
                }
            }
        }

        private async void button13_Click(object sender, EventArgs e) {
            LimparForm(EditarConsultasBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            EditarConsultasBox.Visible = true;
            tipoMarcacaoAdd.Text = "Exame";
            label31.Text = "Editar Exame";
            List<Globais.Filter> filtros = new List<Globais.Filter>();
            profissionalMarcacaoEditar.Items.Clear();
            var filtro = new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or",
            };

            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 2,
                Logic = "or"
            });
            filtros.Add(new Globais.Filter() {
                Field = "Funcao",
                Operator = "eq",
                Value = 1,
                Logic = "or"
            });
            var filter = new Globais.FilterDTO() {
                Offset = 0,
                Limit = 10
            };
            filtro.Filters = filtros;
            filter.Filter = filtro;

            String URI;
            URI = Globais.baseURL + "Funcionario/filter";
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                var serializedUtilizador = JsonConvert.SerializeObject(filter);
                var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(URI, content);
                if (response != null) {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    JObject rsp = JObject.Parse(jsonString);
                    IList<JToken> results = rsp["value"].Children().ToList();
                    if (results.Count > 0) {
                        IList<Funcionario> searchResults = new List<Funcionario>();
                        foreach (JToken result in results) {
                            Funcionario searchResult = result.ToObject<Funcionario>();
                            //adiciona à "estrutura"
                            searchResults.Add(searchResult);
                        }
                        for (int i = 0; i < searchResults.Count; i++) {
                            if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Masculino ") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Dr. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 1 && searchResults[i].Sexo == "Feminino ") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Dra. " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Masculino ") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Enfermeiro " + searchResults[i].Nome);
                            } else if (searchResults[i].Funcao == 2 && searchResults[i].Sexo == "Feminino ") {
                                profissionalMarcacaoEditar.Items.Add(searchResults[i].IdFuncionario + " - Enfermeira " + searchResults[i].Nome);

                            } else if (searchResults[i].Funcao == 4) {
                                ProfissionaisMarcacaoAdd.Items.Add(searchResults[i].IdFuncionario + " - Técnico " + searchResults[i].Nome);
                            }
                        }
                    }

                } else {
                    MessageBox.Show("Erro. Contacte o administrador do sistema");
                    Console.WriteLine("Erro do servidor");
                }

            }

            BindingSource bsDados = new BindingSource();
            URI = Globais.baseURL + "marcacao";
            Marcacao marcacao = new Marcacao();
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                using (var response = await client.GetAsync(URI)) {
                    if (response.IsSuccessStatusCode) {
                        //it's client
                        //recebe a query em json
                        var MarcacaoJsonString = await response.Content.ReadAsStringAsync();
                        //transforma para string
                        JObject rsp = JObject.Parse(MarcacaoJsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            IList<Marcacao> searchResults = new List<Marcacao>();
                            foreach (JToken result in results) {
                                Marcacao searchResult = result.ToObject<Marcacao>();
                                //adiciona à "estrutura"
                                searchResults.Add(searchResult);
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            marcacao = (Marcacao)bsDados.List[0];
                            Globais.maxMarcacao = marcacao.IdMarcacao + 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        } else {
                            Globais.maxMarcacao = 1;
                            codMarcacaoAdd.Text = Globais.maxMarcacao.ToString();
                        }
                    }
                }
            }
        }

        private void button12_Click(object sender, EventArgs e) {
            LimparForm(ApagarConsultaBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            ApagarConsultaBox.Visible = true;
            tipoMarcacaoAdd.Text = "Exame";
            label46.Text = "Desmarcar Exame";
        }

        private async void button11_Click(object sender, EventArgs e) {
            LimparForm(HistoricoBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            HistoricoBox.Visible = true;
            tipoMarcacaoAdd.Text = "Exame";
            label28.Text = "Histórico Exames";
            if (Globais.job == 3) {
                //administrativo
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "marcacao";
                Marcacao marcacao = new Marcacao();
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
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView2.DataSource = bsDados;
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                    if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                        row.DefaultCellStyle.BackColor = Color.Yellow;
                                    } else {
                                        row.DefaultCellStyle.BackColor = Color.Green;
                                    }

                            } else {
                                dataGridView2.DataSource = null;
                            }
                        }
                    }
                }
            } else if (Globais.idLoggedFunc != 0) {
                //tecnico
                int idfunc = 0;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "funcionario?Query=nif%3D" + Globais.loggedId;
                Funcionario func = new Funcionario();
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
                            if (results.Count > 0 && Globais.loggedId != "") {
                                IList<Funcionario> searchResults = new List<Funcionario>();
                                foreach (JToken result in results) {
                                    Funcionario searchResult = result.ToObject<Funcionario>();
                                    //adiciona à "estrutura"
                                    searchResults.Add(searchResult);
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                func = (Funcionario)bsDados.List[0];
                                idfunc = func.IdFuncionario;
                            }
                        }
                    }
                }
                bsDados = new BindingSource();
                URI = Globais.baseURL + "marcacao?Query=idtecnico%3D" + idfunc.ToString();
                Marcacao marcacao = new Marcacao();
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
                            if (results.Count > 0 && idfunc != 0) {
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if (searchResult.Tipo == tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView2.DataSource = bsDados.DataSource;
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                    if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                        row.DefaultCellStyle.BackColor = Color.Yellow;
                                    } else {
                                        row.DefaultCellStyle.BackColor = Color.Green;
                                    }
                            }
                        }
                    }
                }

            } else {
                //e paciente
                int idpac = 0;
                nifHistorico.Visible = false;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente?Query=nif%3D" + Globais.loggedId;
                Paciente pac = new Paciente();
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
                                pac = (Paciente)bsDados.List[0];
                                idpac = pac.IdPaciente;
                            }
                        }
                    }
                }
                bsDados = new BindingSource();
                URI = Globais.baseURL + "marcacao?Query=idpaciente%3D" + idpac.ToString();
                Marcacao marcacao = new Marcacao();
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
                                IList<Marcacao> searchResults = new List<Marcacao>();
                                foreach (JToken result in results) {
                                    Marcacao searchResult = result.ToObject<Marcacao>();
                                    //adiciona à "estrutura"
                                    if(searchResult.Tipo== tipoMarcacaoAdd.Text) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView2.DataSource = bsDados.DataSource;
                                dataGridView2.Columns["IdPaciente"].Visible = false;
                                dataGridView2.Columns["IdFuncionario"].Visible = false;
                                dataGridView2.Columns["IdTecnico"].Visible = false;
                                dataGridView2.Columns["UltimaAtualizacao"].Visible = false;
                                foreach (DataGridViewRow row in dataGridView2.Rows)
                                    if (Convert.ToDateTime(row.Cells[4].Value.ToString()).Date < DateTime.Now.Date) {
                                        row.DefaultCellStyle.BackColor = Color.Yellow;
                                    } else {
                                        row.DefaultCellStyle.BackColor = Color.Green;
                                    }
                            }
                        }
                    }
                }
            }
        }

        private async void button18_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            bool noExists = false;
            LimparForm(PacienteADDBOX);
            CloseGroupBoxs(this);
            ClosePanel(this);
            PacienteADDBOX.Visible = true;
            tipoMarcacaoAdd.Text = "Paciente";
            label59.Text = "Adicionar Paciente";
            addData.Value = DateTime.Now.Date;
            addTipo.Items.Clear();
            label60.Visible = false;
            addTipo.Visible = false;
        }

        private void nifPerfil_TextChanged(object sender, EventArgs e) {

        }

        private void nascPerfil_ValueChanged(object sender, EventArgs e) {

        }

        private void ccPerfil_TextChanged(object sender, EventArgs e) {

        }

        private void emailPerfil_TextChanged(object sender, EventArgs e) {

        }

        private async void button26_Click(object sender, EventArgs e) {
            Boolean update=false;
            if (nomePerfil.Text=="" || emailPerfil.Text=="" || nascPerfil.Value.Date >= DateTime.Now.Date || telemovelPerfil.Text=="") {
                //invalid
                MessageBox.Show("Preencha todos os campos.");
            } else {
                if (Globais.idLoggedFunc == 0 && Globais.job != 3) {
                    //paciente
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=email%3D" + emailPerfil.Text;
                    Paciente pac = new Paciente();
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
                                        if (searchResult.NIF != nifPerfil.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    if (searchResults.Count > 0) {
                                        MessageBox.Show("O email indicado já foi registado por outro utilizador.");
                                    } else {
                                        update = true;
                                    }
                                }
                            }
                        }
                    }
                    if (update == true) {
                        //pronto para atualizar
                        URI = Globais.baseURL + "paciente/" + Globais.idPerfil;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            pac = new Paciente();
                            pac.IdPaciente = Globais.idPerfil;
                            pac.IdUtilizador = Globais.idPerfilUTIL;
                            pac.Nome = nomePerfil.Text;
                            pac.Sexo = sexoPerfil.Text;
                            pac.Telemovel = telemovelPerfil.Text;
                            pac.DataNasc = nascPerfil.Value.Date;
                            pac.Email = emailPerfil.Text;
                            pac.Nacionalidade = nacionalidadePerfil.Text;
                            pac.CC = ccPerfil.Text;
                            pac.NIF = nifPerfil.Text;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(pac);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Perfil editada");
                                MessageBox.Show("Perfil Atualizado");
                            } else {
                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    }
                } else {
                    update = false;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=email%3D" + emailPerfil.Text;
                    Funcionario fuc = new Funcionario();
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
                                        if (searchResult.NIF != nifPerfil.Text) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    if (searchResults.Count > 0) {
                                        MessageBox.Show("O email indicado já foi registado por outro utilizador.");
                                    } else {
                                        update = true;
                                    }
                                }
                            }
                        }
                    }
                    if (update == true) {
                        //pronto para atualizar
                        URI = Globais.baseURL + "funcionario/" + Globais.idPerfil;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            fuc = new Funcionario();
                            fuc.IdFuncionario = Globais.idPerfil;
                            fuc.IdUtilizador = Globais.idPerfilUTIL;
                            fuc.Funcao = Globais.tipoPERFIL;
                            fuc.Nome = nomePerfil.Text;
                            fuc.Sexo = sexoPerfil.Text;
                            fuc.Telemovel = telemovelPerfil.Text;
                            fuc.DataNasc = nascPerfil.Value.Date;
                            fuc.Nacionalidade = nacionalidadePerfil.Text;
                            fuc.Email = emailPerfil.Text;
                            fuc.CC = ccPerfil.Text;
                            fuc.NIF = nifPerfil.Text;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(fuc);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Perfil editado");
                                MessageBox.Show("Perfil Atualizado");
                            } else {
                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    }
                }
            }
        }

        private async void button20_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            bool noExists = false;
            LimparForm(PacienteADDBOX);
            CloseGroupBoxs(this);
            ClosePanel(this);
            PacienteADDBOX.Visible = true;
            tipoMarcacaoAdd.Text = "Funcionario";
            label59.Text = "Adicionar Funcionario";
            addTipo.Items.Clear();
            label60.Visible = true;
            addTipo.Visible = true;
            addData.Value = DateTime.Now.Date;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "funcao";
            Funcao funcao = new Funcao();
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Globais.Token);
                using (var response = await client.GetAsync(URI)) {
                    if (response.IsSuccessStatusCode) {
                        //it's client
                        //recebe a query em json
                        var MarcacaoJsonString = await response.Content.ReadAsStringAsync();
                        //transforma para string
                        JObject rsp = JObject.Parse(MarcacaoJsonString);
                        IList<JToken> results = rsp["value"].Children().ToList();
                        if (results.Count > 0) {
                            IList<Funcao> searchResults = new List<Funcao>();
                            foreach (JToken result in results) {
                                Funcao searchResult = result.ToObject<Funcao>();
                                //adiciona à "estrutura"
                                searchResults.Add(searchResult);
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            for (int i = 0; i < bsDados.Count; i++) {
                                funcao = (Funcao)bsDados.List[i];
                                addTipo.Items.Add(funcao.IdFuncao + " - " + funcao.Descricao);
                            }
                            addTipo.Sorted = true;
                            addTipo.Sorted = false;
                            addTipo.Items.Insert(0, "Escolha uma opção");
                        }
                    }
                }

            }
        }

        private void addNIF_TextChanged(object sender, EventArgs e) {
            Regex regexNumeros = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros.Matches(addNIF.Text);
            if (matches.Count > 0 || addNIF.Text == " " || addNIF.Text.Length > 9) {
                addNIF.Text = "";
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void addCC_TextChanged(object sender, EventArgs e) {
            Regex regexNumeros3 = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros3.Matches(addCC.Text);
            if (matches.Count > 0 || addCC.Text == " " || addCC.Text.Length > 9) {
                addCC.Text = "";
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private void addTelemovel_TextChanged(object sender, EventArgs e) {
            Regex regexNumeros2 = new Regex(@"[^0-9^]");
            MatchCollection matches = regexNumeros2.Matches(addTelemovel.Text);
            if (matches.Count > 0 || addTelemovel.Text == " " || addTelemovel.Text.Length > 9) {
                addTelemovel.Text = "";
                System.Media.SystemSounds.Hand.Play();

            }
        }

        private async void button27_Click(object sender, EventArgs e) {
            if (tipoMarcacaoAdd.Text == "Paciente") {
                //add paciente
                bool noExists = false;

                if (addEmail.Text == "" || addNome.Text == "" || addSexo.Text == "" || addTelemovel.Text == "" || addData.Value.Date > DateTime.Now.Date || addNIF.Text == "" || addCC.Text == "") {
                    //Form is not valid
                } else {
                    //Form is valid
                    List<Globais.Filter> filtros = new List<Globais.Filter>();

                    var filtro = new Globais.Filter() {
                        Field = "Email",
                        Operator = "eq",
                        Value = addEmail.Text,
                        Logic = "or",
                    };

                    filtros.Add(new Globais.Filter() {
                        Field = "Nif",
                        Operator = "eq",
                        Value = addNIF.Text,
                        Logic = "or"
                    });

                    filtros.Add(new Globais.Filter() {
                        Field = "CC",
                        Operator = "eq",
                        Value = addCC.Text,
                        Logic = "or"
                    });

                    var filter = new Globais.FilterDTO() {
                        Offset = 0,
                        Limit = 10
                    };
                    filtro.Filters = filtros;
                    filter.Filter = filtro;

                    String URI;
                    URI = Globais.baseURL + "Paciente/filter";
                    using (var client = new HttpClient()) {
                        var serializedUtilizador = JsonConvert.SerializeObject(filter);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        if (response != null) {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            if (jsonString == "{\"value\":[]}") {
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
                            PacienteRegistar paciente = new PacienteRegistar();
                            paciente.Nome = addNome.Text;
                            paciente.Sexo = addSexo.SelectedItem.ToString();
                            paciente.Telemovel = addTelemovel.Text;
                            paciente.Nacionalidade = addNacionalidade.Text;
                            paciente.DataNasc = addData.Value.Date;
                            paciente.Email = addEmail.Text;
                            paciente.NIF = addNIF.Text;
                            paciente.CC = addCC.Text;
                            paciente.Ativo = true;
                            //TODO encriptar a senha
                            string senha= "AJUDA" + addNIF.Text + "pswd";
                            paciente.Senha = Globais.ComputeSha256Hash(senha);
                            var serializedUtilizador = JsonConvert.SerializeObject(paciente);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.Created) {
                                Console.WriteLine("Inseriu Paciente");
                                MessageBox.Show("Paciente Criado \n Nif: " + addNIF.Text + "\n Senha: " + senha);
                                LimparForm(PacienteADDBOX);
                                addData.Value = DateTime.Now.Date;
                            } else {

                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    } else {
                        MessageBox.Show("Já existe um registo com os seus dados.");
                    }
                }
            } else {
                //add func
                int tipofunc=0;
                foreach (char c in addTipo.Text) {
                    if (Char.IsDigit(c)) {
                        string x = c.ToString();
                        tipofunc = Convert.ToInt32(x);
                    }

                }
                bool noExists = false;
                if (addEmail.Text == "" || addNome.Text == "" || addSexo.Text == "" || addTelemovel.Text == "" || addData.Value.Date >= DateTime.Now.Date || addNIF.Text == "" || addCC.Text == "") {
                    //Form is not valid
                    MessageBox.Show("Verifique os campos");
                } else {
                    //Form is valid
                    List<Globais.Filter> filtros = new List<Globais.Filter>();

                    var filtro = new Globais.Filter() {
                        Field = "Email",
                        Operator = "eq",
                        Value = addEmail.Text,
                        Logic = "or",
                    };

                    filtros.Add(new Globais.Filter() {
                        Field = "Nif",
                        Operator = "eq",
                        Value = addNIF.Text,
                        Logic = "or"
                    });

                    filtros.Add(new Globais.Filter() {
                        Field = "CC",
                        Operator = "eq",
                        Value = addCC.Text,
                        Logic = "or"
                    });

                    var filter = new Globais.FilterDTO() {
                        Offset = 0,
                        Limit = 10
                    };
                    filtro.Filters = filtros;
                    filter.Filter = filtro;

                    String URI;
                    URI = Globais.baseURL + "Funcionario/filter";
                    using (var client = new HttpClient()) {
                        var serializedUtilizador = JsonConvert.SerializeObject(filter);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        if (response != null) {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            if (jsonString == "{\"value\":[]}") {
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
                        URI = Globais.baseURL + "funcionario";
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            FuncionarioRegistar funcionario = new FuncionarioRegistar();
                            funcionario.Nome = addNome.Text;
                            funcionario.Sexo = addSexo.SelectedItem.ToString();
                            funcionario.Telemovel = addTelemovel.Text;
                            funcionario.Nacionalidade = addNacionalidade.Text;
                            funcionario.DataNasc = addData.Value.Date;
                            funcionario.Email = addEmail.Text;
                            funcionario.NIF = addNIF.Text;
                            funcionario.CC = addCC.Text;
                            funcionario.Funcao = tipofunc;
                            string senha = "FUNC" + addNIF.Text + "pswd";
                            funcionario.Senha = Globais.ComputeSha256Hash(senha);
                            funcionario.Ativo = true;
                            var serializedUtilizador = JsonConvert.SerializeObject(funcionario);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PostAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.Created) {
                                Console.WriteLine("Inseriu Funcionário");
                                MessageBox.Show("Funcionário Criado \n Nif: " + addNIF.Text + "\n Senha: " + senha);
                                LimparForm(PacienteADDBOX);
                                addData.Value = DateTime.Now.Date;
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
        }

        private async void button16_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            LimparForm(PacienteEditarBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            PacienteEditarBox.Visible = true;
            tipoMarcacaoAdd.Text = "Paciente";
            label70.Text = "Editar Paciente";
            editarData.Value = DateTime.Now.Date;
            dataGridView3.DataSource = null;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "paciente";
            Paciente paciente = new Paciente();
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
                            dataGridView3.DataSource = bsDados;
                            dataGridView3.Columns["IdPaciente"].Visible = false;
                            dataGridView3.Columns["IdUtilizador"].Visible = false;
                        } else {
                            dataGridView3.DataSource = null;
                        }
                    }
                }
            }
        }

        private async void button21_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            bool noExists = false;
            LimparForm(PacienteEditarBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            PacienteEditarBox.Visible = true;
            tipoMarcacaoAdd.Text = "Funcionario";
            label70.Text = "Editar Funcionario";
            editarData.Value = DateTime.Now.Date;
            dataGridView3.DataSource = null;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "funcionario";
            Funcionario funcionario = new Funcionario();
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
                            dataGridView3.DataSource = bsDados;
                            dataGridView3.Columns["IdFuncionario"].Visible = false;
                            dataGridView3.Columns["IdUtilizador"].Visible = false;
                            dataGridView3.Columns["Funcao"].Visible = false;
                        } else {
                            dataGridView3.DataSource = null;
                        }
                    }
                }
            }
        }

        private void PacienteEditarBox_Enter(object sender, EventArgs e) {

        }

        private async void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (tipoMarcacaoAdd.Text=="Paciente") {
                //paciente

                if (e.RowIndex != -1) {
                    DataGridViewRow dataGridViewRow = dataGridView3.Rows[e.RowIndex];
                    string nif = "";
                    nif = dataGridViewRow.Cells[9].Value.ToString();
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=nif%3D" + nif;
                    Paciente paciente = new Paciente();
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
                                        Globais.idPaciente = searchResult.IdPaciente;
                                        editarNome.Text = dataGridViewRow.Cells[2].Value.ToString();
                                        editarSexo.SelectedIndex = editarSexo.FindStringExact(dataGridViewRow.Cells[3].Value.ToString());
                                        editarData.Value = DateTime.Parse(dataGridViewRow.Cells[6].Value.ToString());
                                        editarTelemovel.Text = dataGridViewRow.Cells[4].Value.ToString();
                                        editarNacionalidade.Text = dataGridViewRow.Cells[5].Value.ToString();
                                        editarEmail.Text = dataGridViewRow.Cells[7].Value.ToString();
                                        editarCC.Text = dataGridViewRow.Cells[8].Value.ToString();
                                        editarNIF.Text = dataGridViewRow.Cells[9].Value.ToString();
                                        checkBox1.Checked = Convert.ToBoolean(dataGridViewRow.Cells[10].Value);
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                //funcionario
                if (e.RowIndex != -1) {
                    DataGridViewRow dataGridViewRow = dataGridView3.Rows[e.RowIndex];
                    string nif = "";
                    nif = dataGridViewRow.Cells[9].Value.ToString();
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=nif%3D" + nif;
                    Funcionario funcionario= new Funcionario();
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
                                        Globais.idPaciente = searchResult.IdFuncionario;
                                        editarNome.Text = dataGridViewRow.Cells[2].Value.ToString();
                                        editarSexo.SelectedIndex = editarSexo.FindStringExact(dataGridViewRow.Cells[3].Value.ToString());
                                        editarData.Value = DateTime.Parse(dataGridViewRow.Cells[6].Value.ToString());
                                        editarTelemovel.Text = dataGridViewRow.Cells[4].Value.ToString();
                                        editarNacionalidade.Text = dataGridViewRow.Cells[5].Value.ToString();
                                        editarEmail.Text = dataGridViewRow.Cells[7].Value.ToString();
                                        editarCC.Text = dataGridViewRow.Cells[8].Value.ToString();
                                        editarNIF.Text = dataGridViewRow.Cells[9].Value.ToString();
                                        checkBox1.Checked = Convert.ToBoolean(dataGridViewRow.Cells[11].Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void button28_Click(object sender, EventArgs e) {
            Boolean nExists = false;
            int idutil=0;
            int idfunc = 0;
            if (editarNome.Text == "" || editarSexo.Text == "" || editarData.Value.Date >= DateTime.Now.Date || editarTelemovel.Text == "" || editarNacionalidade.Text == "" || editarEmail.Text == "") {
                //form is not valid
            } else {
                //form is valid
                if (tipoMarcacaoAdd.Text == "Paciente") {
                    //atualizar paciente
                    List<Globais.Filter> filtros = new List<Globais.Filter>();

                    var filtro = new Globais.Filter() {
                        Field = "Email",
                        Operator = "eq",
                        Value = editarEmail.Text,
                        Logic = "or",
                    };

                    filtros.Add(new Globais.Filter() {
                        Field = "Nif",
                        Operator = "eq",
                        Value = editarNIF.Text,
                        Logic = "or"
                    });

                    filtros.Add(new Globais.Filter() {
                        Field = "CC",
                        Operator = "eq",
                        Value = editarCC.Text,
                        Logic = "or"
                    });

                    var filter = new Globais.FilterDTO() {
                        Offset = 0,
                        Limit = 10
                    };
                    filtro.Filters = filtros;
                    filter.Filter = filtro;

                    String URI;
                    URI = Globais.baseURL + "Paciente/filter";
                    using (var client = new HttpClient()) {
                        var serializedUtilizador = JsonConvert.SerializeObject(filter);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        if (response != null) {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            if (jsonString == "{\"value\":[]}") {
                                //We can register
                                
                            } else {
                                JObject rsp = JObject.Parse(jsonString);
                                IList<JToken> results = rsp["value"].Children().ToList();
                                if (results.Count > 0) {
                                    IList<Paciente> searchResults = new List<Paciente>();
                                    foreach (JToken result in results) {
                                        Paciente searchResult = result.ToObject<Paciente>();
                                        //adiciona à "estrutura"
                                        if (searchResult.IdPaciente != Globais.idPaciente) {
                                            //existe registo
                                            searchResults.Add(searchResult);
                                        } else {
                                            idutil = searchResult.IdUtilizador;
                                        }
                                    }
                                    if (searchResults.Count > 0) {
                                        nExists = false;
                                        MessageBox.Show("Já existe um registo com os seus dados.");
                                    } else {
                                        nExists = true;
                                    }
                                }
                            }

                        } else {
                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }

                    }
                    if (nExists == true) {
                        //pode dar update
                        URI = Globais.baseURL + "paciente/" + Globais.idPaciente;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            Paciente paciente = new Paciente();
                            paciente.IdPaciente = Globais.idPaciente;
                            paciente.IdUtilizador= idutil;
                            paciente.Nome = editarNome.Text;
                            paciente.Sexo = editarSexo.Text;
                            paciente.Telemovel = editarTelemovel.Text;
                            paciente.Nacionalidade = editarNacionalidade.Text;
                            paciente.DataNasc = editarData.Value.Date;
                            paciente.Email = editarEmail.Text;
                            paciente.CC = editarCC.Text;
                            paciente.NIF = editarNIF.Text;
                            paciente.Ativo = checkBox1.Checked;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(paciente);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Paciente editado");
                                MessageBox.Show("Paciente Atualizado");
                                LimparForm(PacienteEditarBox);
                            } else {

                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                        URI = Globais.baseURL + "paciente";
                        BindingSource bsDados = new BindingSource();
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
                                        dataGridView3.DataSource = bsDados;
                                        dataGridView3.Columns["IdPaciente"].Visible = false;
                                        dataGridView3.Columns["IdUtilizador"].Visible = false;
                                    } else {
                                        dataGridView3.DataSource = null;
                                    }
                                }
                            }
                        }
                    }
                } else {
                    //atualizar funcionario
                    List<Globais.Filter> filtros = new List<Globais.Filter>();

                    var filtro = new Globais.Filter() {
                        Field = "Email",
                        Operator = "eq",
                        Value = editarEmail.Text,
                        Logic = "or",
                    };

                    filtros.Add(new Globais.Filter() {
                        Field = "Nif",
                        Operator = "eq",
                        Value = editarNIF.Text,
                        Logic = "or"
                    });

                    filtros.Add(new Globais.Filter() {
                        Field = "CC",
                        Operator = "eq",
                        Value = editarCC.Text,
                        Logic = "or"
                    });

                    var filter = new Globais.FilterDTO() {
                        Offset = 0,
                        Limit = 10
                    };
                    filtro.Filters = filtros;
                    filter.Filter = filtro;

                    String URI;
                    URI = Globais.baseURL + "Funcionario/filter";
                    using (var client = new HttpClient()) {
                        var serializedUtilizador = JsonConvert.SerializeObject(filter);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(URI, content);
                        if (response != null) {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            if (jsonString == "{\"value\":[]}") {
                                //We can register

                            } else {
                                JObject rsp = JObject.Parse(jsonString);
                                IList<JToken> results = rsp["value"].Children().ToList();
                                if (results.Count > 0) {
                                    IList<Funcionario> searchResults = new List<Funcionario>();
                                    foreach (JToken result in results) {
                                        Funcionario searchResult = result.ToObject<Funcionario>();
                                        //adiciona à "estrutura"
                                        if (searchResult.IdFuncionario != Globais.idPaciente) {
                                            //existe registo
                                            searchResults.Add(searchResult);
                                        } else {
                                            idutil = searchResult.IdUtilizador;
                                            idfunc = searchResult.Funcao;
                                        }
                                    }
                                    if (searchResults.Count > 0) {
                                        nExists = false;
                                        MessageBox.Show("Já existe um registo com os seus dados.");
                                    } else {
                                        nExists = true;
                                    }
                                }
                            }

                        } else {
                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }

                    }
                    if (nExists == true) {
                        //pode dar update
                        URI = Globais.baseURL + "funcionario/" + Globais.idPaciente;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            Funcionario funcionario = new Funcionario();
                            funcionario.IdFuncionario = Globais.idPaciente;
                            funcionario.IdUtilizador = idutil;
                            funcionario.Nome = editarNome.Text;
                            funcionario.Sexo = editarSexo.Text;
                            funcionario.Telemovel = editarTelemovel.Text;
                            funcionario.Nacionalidade = editarNacionalidade.Text;
                            funcionario.DataNasc = editarData.Value.Date;
                            funcionario.Email = editarEmail.Text;
                            funcionario.CC = editarCC.Text;
                            funcionario.NIF = editarNIF.Text;
                            funcionario.Funcao = idfunc;
                            funcionario.Ativo = checkBox1.Checked;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(funcionario);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Funcionario editado");
                                MessageBox.Show("Funcionário Atualizado");
                                LimparForm(PacienteEditarBox);
                            } else {

                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                        URI = Globais.baseURL + "funcionario";
                        BindingSource bsDados = new BindingSource();
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
                                        dataGridView3.DataSource = bsDados;
                                        dataGridView3.Columns["IdFuncionario"].Visible = false;
                                        dataGridView3.Columns["IdUtilizador"].Visible = false;
                                        dataGridView3.Columns["Funcao"].Visible = false;
                                    } else {
                                        dataGridView3.DataSource = null;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

        private async void button17_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            LimparForm(RemoverPacienteBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            RemoverPacienteBox.Visible = true;
            tipoMarcacaoAdd.Text = "Paciente";
            label78.Text = "Remover Paciente";
            removerData.Value = DateTime.Now.Date;
            dataGridView3.DataSource = null;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "paciente";
            Paciente paciente = new Paciente();
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
                                if (searchResult.Ativo == true) {
                                    searchResults.Add(searchResult);
                                }
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            dataGridView4.DataSource = bsDados;
                            dataGridView4.Columns["IdPaciente"].Visible = false;
                            dataGridView4.Columns["IdUtilizador"].Visible = false;
                            dataGridView4.Columns["Ativo"].Visible = false;
                        } else {
                            dataGridView4.DataSource = null;
                        }
                    }
                }
            }
        }

        private async void button22_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            LimparForm(RemoverPacienteBox);
            CloseGroupBoxs(this);
            ClosePanel(this);
            RemoverPacienteBox.Visible = true;
            tipoMarcacaoAdd.Text = "Funcionario";
            label78.Text = "Remover Funcionario";
            removerData.Value = DateTime.Now.Date;
            dataGridView3.DataSource = null;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "funcionario";
            Funcionario funcionario = new Funcionario();
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
                                if (searchResult.Ativo == true) {
                                    searchResults.Add(searchResult);
                                }
                            }
                            //adiciona a uma tabela. neste caso à BindingSource
                            bsDados.DataSource = searchResults.ToList();
                            dataGridView4.DataSource = bsDados;
                            dataGridView4.Columns["IdFuncionario"].Visible = false;
                            dataGridView4.Columns["IdUtilizador"].Visible = false;
                            dataGridView4.Columns["Funcao"].Visible = false;
                            dataGridView4.Columns["Ativo"].Visible = false;
                        } else {
                            dataGridView4.DataSource = null;
                        }
                    }
                }
            }
        }

        private async void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e) {
            if (tipoMarcacaoAdd.Text == "Paciente") {
                //paciente

                if (e.RowIndex != -1) {
                    DataGridViewRow dataGridViewRow = dataGridView4.Rows[e.RowIndex];
                    string nif = "";
                    nif = dataGridViewRow.Cells[9].Value.ToString();
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=nif%3D" + nif;
                    Paciente paciente = new Paciente();
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
                                        Globais.idPaciente = searchResult.IdPaciente;
                                        Globais.maxMarcacao = searchResult.IdUtilizador;
                                        removerNome.Text = dataGridViewRow.Cells[2].Value.ToString();
                                        removerSexo.SelectedIndex = editarSexo.FindStringExact(dataGridViewRow.Cells[3].Value.ToString());
                                        removerData.Value = DateTime.Parse(dataGridViewRow.Cells[6].Value.ToString());
                                        removerTelemovel.Text = dataGridViewRow.Cells[4].Value.ToString();
                                        removerNacionalidade.Text = dataGridViewRow.Cells[5].Value.ToString();
                                        removerEmail.Text = dataGridViewRow.Cells[7].Value.ToString();
                                        removerCC.Text = dataGridViewRow.Cells[8].Value.ToString();
                                        removerNIF.Text = dataGridViewRow.Cells[9].Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                //funcionario
                if (e.RowIndex != -1) {
                    DataGridViewRow dataGridViewRow = dataGridView4.Rows[e.RowIndex];
                    string nif = "";
                    nif = dataGridViewRow.Cells[9].Value.ToString();
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=nif%3D" + nif;
                    Funcionario funcionario = new Funcionario();
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
                                        Globais.idPaciente = searchResult.IdFuncionario;
                                        Globais.maxMarcacao = searchResult.IdUtilizador;
                                        Globais.removerTipo = searchResult.Funcao;
                                        removerNome.Text = dataGridViewRow.Cells[2].Value.ToString();
                                        removerSexo.SelectedIndex = editarSexo.FindStringExact(dataGridViewRow.Cells[3].Value.ToString());
                                        removerData.Value = DateTime.Parse(dataGridViewRow.Cells[6].Value.ToString());
                                        removerTelemovel.Text = dataGridViewRow.Cells[4].Value.ToString();
                                        removerNacionalidade.Text = dataGridViewRow.Cells[5].Value.ToString();
                                        removerEmail.Text = dataGridViewRow.Cells[7].Value.ToString();
                                        removerCC.Text = dataGridViewRow.Cells[8].Value.ToString();
                                        removerNIF.Text = dataGridViewRow.Cells[9].Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void button29_Click(object sender, EventArgs e) {
            if (tipoMarcacaoAdd.Text == "Paciente") {
                //paciente
                if (Globais.idPaciente != 0) {
                    string URI = Globais.baseURL + "paciente/" + Globais.idPaciente;
                    using (var client = new HttpClient()) {
                        //Criar class Paciente. substituir filtyer na 131 por paciente.
                        //confirmar a response como na 109
                        Paciente paciente = new Paciente();
                        paciente.IdPaciente = Globais.idPaciente;
                        paciente.IdUtilizador = Globais.maxMarcacao;
                        paciente.Nome = removerNome.Text;
                        paciente.Sexo = removerSexo.Text;
                        paciente.Telemovel = removerTelemovel.Text;
                        paciente.Nacionalidade = removerNacionalidade.Text;
                        paciente.DataNasc = removerData.Value.Date;
                        paciente.Email = removerEmail.Text;
                        paciente.CC = removerCC.Text;
                        paciente.NIF = removerNIF.Text;
                        paciente.Ativo = false;
                        //TODO encriptar a senha
                        var serializedUtilizador = JsonConvert.SerializeObject(paciente);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(URI, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                            Console.WriteLine("Paciente eliminado");
                            MessageBox.Show("Paciente eliminado");
                            LimparForm(RemoverPacienteBox);
                            Globais.idPaciente = 0;
                            Globais.maxMarcacao = 0;
                        } else {

                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }
                    }
                    BindingSource bsDados = new BindingSource();
                    URI = Globais.baseURL + "paciente";
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
                                        if (searchResult.Ativo == true) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView4.DataSource = bsDados;
                                    dataGridView4.Columns["IdPaciente"].Visible = false;
                                    dataGridView4.Columns["IdUtilizador"].Visible = false;
                                    dataGridView4.Columns["Ativo"].Visible = false;
                                } else {
                                    dataGridView4.DataSource = null;
                                }
                            }
                        }
                    }

                } else {
                    MessageBox.Show("Não é possível eliminar o paciente");
                }
            } else {
                if (Globais.idPaciente != 0) {
                    string URI = Globais.baseURL + "funcionario/" + Globais.idPaciente;
                    using (var client = new HttpClient()) {
                        //Criar class Paciente. substituir filtyer na 131 por paciente.
                        //confirmar a response como na 109
                        Funcionario func = new Funcionario();
                        func.IdFuncionario = Globais.idPaciente;
                        func.IdUtilizador = Globais.maxMarcacao;
                        func.Nome = removerNome.Text;
                        func.Sexo = removerSexo.Text;
                        func.Telemovel = removerTelemovel.Text;
                        func.Nacionalidade = removerNacionalidade.Text;
                        func.DataNasc = removerData.Value.Date;
                        func.Email = removerEmail.Text;
                        func.CC = removerCC.Text;
                        func.NIF = removerNIF.Text;
                        func.Funcao = Globais.removerTipo;
                        func.Ativo = false;
                        //TODO encriptar a senha
                        var serializedUtilizador = JsonConvert.SerializeObject(func);
                        var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                        var response = await client.PutAsync(URI, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                            Console.WriteLine("Paciente eliminado");
                            MessageBox.Show("Paciente eliminado");
                            LimparForm(RemoverPacienteBox);
                            Globais.idPaciente = 0;
                            Globais.maxMarcacao = 0;
                        } else {

                            MessageBox.Show("Erro. Contacte o administrador do sistema");
                            Console.WriteLine("Erro do servidor");
                        }
                    }
                    BindingSource bsDados = new BindingSource();
                    URI = Globais.baseURL + "funcionario";
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
                                        if (searchResult.Ativo == true) {
                                            searchResults.Add(searchResult);
                                        }
                                    }
                                    //adiciona a uma tabela. neste caso à BindingSource
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView4.DataSource = bsDados;
                                    dataGridView4.Columns["IdFuncionario"].Visible = false;
                                    dataGridView4.Columns["IdUtilizador"].Visible = false;
                                    dataGridView4.Columns["Funcao"].Visible = false;
                                    dataGridView4.Columns["Ativo"].Visible = false;
                                } else {
                                    dataGridView4.DataSource = null;
                                }
                            }
                        }
                    }

                } else {
                    MessageBox.Show("Não é possível eliminar o paciente");
                }

            }
            //atualizar datagridview4

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {

        }

        private async void button15_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            LimparForm(pacienteHistorico);
            CloseGroupBoxs(this);
            ClosePanel(this);
            pacienteHistorico.Visible = true;
            tipoMarcacaoAdd.Text = "Paciente";
            label87.Text = "Histórico Paciente";
            dataGridView5.DataSource = null;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "paciente";
            Paciente paciente = new Paciente();
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
                            dataGridView5.DataSource = bsDados;
                        } else {
                            dataGridView5.DataSource = null;
                        }
                    }
                }
            }
        }

        private async void button19_Click(object sender, EventArgs e) {
            Globais.maxMarcacao = 0;
            Globais.idPaciente = 0;
            LimparForm(pacienteHistorico);
            CloseGroupBoxs(this);
            ClosePanel(this);
            pacienteHistorico.Visible = true;
            tipoMarcacaoAdd.Text = "Funcionario";
            label87.Text = "Histórico Funcionário";
            dataGridView5.DataSource = null;
            BindingSource bsDados = new BindingSource();
            string URI = Globais.baseURL + "funcionario";
            Funcionario  funcionario = new Funcionario();
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
                            dataGridView5.DataSource = bsDados;
                        } else {
                            dataGridView5.DataSource = null;
                        }
                    }
                }
            }
        }

        private async void button31_Click(object sender, EventArgs e) {
            nifHistoricoP.Text = "";
            idHistorico.Text = "";
            if (tipoMarcacaoAdd.Text == "Paciente") {
                dataGridView5.DataSource = null;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente";
                Paciente paciente = new Paciente();
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
                                dataGridView5.DataSource = bsDados;
                            } else {
                                dataGridView5.DataSource = null;
                            }
                        }
                    }
                }
            } else {
                dataGridView5.DataSource = null;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "funcionario";
                Funcionario funcionario = new Funcionario();
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
                                dataGridView5.DataSource = bsDados;
                            } else {
                                dataGridView5.DataSource = null;
                            }
                        }
                    }
                }
            }
        }

        private async void button30_Click(object sender, EventArgs e) {
            nifHistoricoP.Text = "";
            idHistorico.Text = "";
            if (tipoMarcacaoAdd.Text == "Paciente") {
                dataGridView5.DataSource = null;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "paciente";
                Paciente paciente = new Paciente();
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
                                    if (searchResult.Ativo == true) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView5.DataSource = bsDados;
                            } else {
                                dataGridView5.DataSource = null;
                            }
                        }
                    }
                }
            } else {
                dataGridView5.DataSource = null;
                BindingSource bsDados = new BindingSource();
                string URI = Globais.baseURL + "funcionario";
                Funcionario funcionario = new Funcionario();
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
                                    if (searchResult.Ativo == true) {
                                        searchResults.Add(searchResult);
                                    }
                                }
                                //adiciona a uma tabela. neste caso à BindingSource
                                bsDados.DataSource = searchResults.ToList();
                                dataGridView5.DataSource = bsDados;
                            } else {
                                dataGridView5.DataSource = null;
                            }
                        }
                    }
                }
            }
        }
        private async void nifHistoricoP_TextChanged(object sender, EventArgs e) {
            idHistorico.Text = "";
            Regex textNIFr = new Regex(@"[^0-9]");
            MatchCollection matches = textNIFr.Matches(nifHistoricoP.Text);
            if (matches.Count > 0 || nifHistoricoP.Text == " ")
            {
                nifHistoricoP.Text = nifHistoricoP.Text.Remove(nifHistoricoP.Text.Length-1);
                nifHistoricoP.SelectionStart = nifHistoricoP.Text.Length;
                nifHistoricoP.SelectionLength = 0;
            }
            if (tipoMarcacaoAdd.Text == "Paciente") {
                //paciente
                if (nifHistoricoP.Text == "" && idHistorico.Text == "") {
                    //carregar todos
                    dataGridView5.DataSource = null;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente";
                    Paciente paciente = new Paciente();
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
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                } else {
                    idHistorico.Text = "";
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=nifx%3D" + nifHistoricoP.Text;
                    Paciente paciente = new Paciente();
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
                                if (results.Count > 0 && nifHistoricoP.Text!="") {
                                    IList<Paciente> searchResults = new List<Paciente>();
                                    foreach (JToken result in results) {
                                        Paciente searchResult = result.ToObject<Paciente>();
                                        //adiciona à "estrutura"
                                        searchResults.Add(searchResult);
                                    }
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                }
            } else {
                //funcionario
                if (nifHistoricoP.Text == "" && idHistorico.Text == "") {
                    //carregar todos
                    dataGridView5.DataSource = null;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario";
                    Funcionario funcionario = new Funcionario();
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
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                } else {
                    idHistorico.Text = "";
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=nifx%3D" + nifHistoricoP.Text;
                    Funcionario paciente = new Funcionario();
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
                                if (results.Count > 0 && nifHistoricoP.Text != "") {
                                    IList<Funcionario> searchResults = new List<Funcionario>();
                                    foreach (JToken result in results) {
                                        Funcionario searchResult = result.ToObject<Funcionario>();
                                        //adiciona à "estrutura"
                                        searchResults.Add(searchResult);
                                    }
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void idHistorico_TextChanged(object sender, EventArgs e) {
            nifHistoricoP.Text = "";
            Regex idHistoricor = new Regex(@"[^0-9]");
            MatchCollection matches = idHistoricor.Matches(idHistorico.Text);
            if (matches.Count > 0 || idHistorico.Text == " ") {
                idHistorico.Text = idHistorico.Text.Remove(idHistorico.Text.Length - 1);
                idHistorico.SelectionStart = idHistorico.Text.Length;
                idHistorico.SelectionLength = 0;

            }
            if (tipoMarcacaoAdd.Text == "Paciente") {
                //paciente
                if (nifHistoricoP.Text == "" && idHistorico.Text == "") {
                    //carregar todos
                    dataGridView5.DataSource = null;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente";
                    Paciente paciente = new Paciente();
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
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                } else {
                    nifHistoricoP.Text = "";
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=idpaciente%3D" + idHistorico.Text;
                    Paciente paciente = new Paciente();
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
                                if (results.Count > 0 && idHistorico.Text != "") {
                                    IList<Paciente> searchResults = new List<Paciente>();
                                    foreach (JToken result in results) {
                                        Paciente searchResult = result.ToObject<Paciente>();
                                        //adiciona à "estrutura"
                                        searchResults.Add(searchResult);
                                    }
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                }
            } else {
                //funcionario
                if (nifHistoricoP.Text == "" && idHistorico.Text == "") {
                    //carregar todos
                    dataGridView5.DataSource = null;
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario";
                    Funcionario funcionario = new Funcionario();
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
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                } else {
                    nifHistoricoP.Text = "";
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=idfuncionario%3D" + idHistorico.Text;
                    Funcionario funcionario = new Funcionario();
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
                                if (results.Count > 0 && idHistorico.Text != "") {
                                    IList<Funcionario> searchResults = new List<Funcionario>();
                                    foreach (JToken result in results) {
                                        Funcionario searchResult = result.ToObject<Funcionario>();
                                        //adiciona à "estrutura"
                                        searchResults.Add(searchResult);
                                    }
                                    bsDados.DataSource = searchResults.ToList();
                                    dataGridView5.DataSource = bsDados;
                                } else {
                                    dataGridView5.DataSource = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button32_Click(object sender, EventArgs e) {
            if (senhaBOX.Visible == true) {
                senhaAntiga.Text = "";
                senhaNova.Text = "";
                senhaBOX.Visible = false;
            } else {
                senhaBOX.Visible = true;
            }
        }

        private async void button33_Click(object sender, EventArgs e) {
            int iduser = 0;
            string oldSenha="";
            bool senhaIgual = false;
            string newSenha = "";
            oldSenha = Globais.ComputeSha256Hash(senhaAntiga.Text);
            if (senhaNova.Text == senhaNovaC.Text) {
                if (tipoMarcacaoAdd.Text == "Paciente") {
                    //paciente
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "paciente?Query=nif%3D" + Globais.loggedId;
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
                                        iduser = searchResult.IdUtilizador;
                                    }
                                }
                            }
                        }
                    }
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "utilizador?Query=idutilizador%3D" + iduser;
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
                                    IList<Utilizador> searchResults = new List<Utilizador>();
                                    foreach (JToken result in results) {
                                        Utilizador searchResult = result.ToObject<Utilizador>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Senha == oldSenha) {
                                            senhaIgual = true;
                                        } else {
                                            senhaIgual = false;
                                            MessageBox.Show("Senha atual incorreta");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (senhaIgual == true) {
                        URI = Globais.baseURL + "utilizador/" + iduser;
                        using (var client = new HttpClient()) {

                            Utilizador util = new Utilizador();
                            util.IdUtilizador = iduser;
                            util.Login = Globais.loggedId;
                            util.Senha = senhaNovaC.Text;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(util);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Senha alterada");
                                MessageBox.Show("Senha alterada");
                            } else {
                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    }
            } else {
                    //funcionario
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "funcionario?Query=nif%3D" + Globais.loggedId;
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
                                        iduser = searchResult.IdUtilizador;
                                    }
                                }
                            }
                        }
                    }
                    bsDados = new BindingSource();
                    URI = Globais.baseURL + "utilizador?Query=idutilizador%3D" + iduser;
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
                                    IList<Utilizador> searchResults = new List<Utilizador>();
                                    foreach (JToken result in results) {
                                        Utilizador searchResult = result.ToObject<Utilizador>();
                                        //adiciona à "estrutura"
                                        if (searchResult.Senha == oldSenha) {
                                            senhaIgual = true;
                                        } else {
                                            senhaIgual = false;
                                            MessageBox.Show("Senha atual incorreta");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (senhaIgual == true) {
                        URI = Globais.baseURL + "utilizador/" + iduser;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            newSenha = Globais.ComputeSha256Hash(senhaNovaC.Text);
                            Utilizador util = new Utilizador();
                            util.IdUtilizador = iduser;
                            util.Login = Globais.loggedId;
                            util.Senha = newSenha;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(util);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Senha alterada");
                                MessageBox.Show("Senha alterada");
                                senhaAntiga.Text = "";
                                senhaNova.Text = "";
                                senhaNovaC.Text = "";
                                senhaBOX.Visible = false;
                            } else {
                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    }
                }
            } else {
                MessageBox.Show("Confirme a nova senha.");
            }
        }
    }
}
