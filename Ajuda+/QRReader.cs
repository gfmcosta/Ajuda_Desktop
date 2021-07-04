using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using BasselTech_CamCapture;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Ajuda_
{
    public partial class QRReader : Form
    {
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

        Camera cam;
        Timer t;
        BackgroundWorker worker;
        Bitmap CapImage;
        public QRReader()
        {
            InitializeComponent();
            t = new Timer();
            pictureBox1.Size = new Size(400,400);
            // - um cadinho
            pictureBox1.Location= new Point(Screen.PrimaryScreen.Bounds.Width/2 -116, Screen.PrimaryScreen.Bounds.Height / 2-200);
            cam = new Camera(pictureBox1);
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork; 
            t.Tick += T_Tick; 
            t.Interval = 1;
        }
        void stopCamera() {
            cam.Stop();
        }
        private void T_Tick(object sender, EventArgs e)
        {
            CapImage = cam.GetBitmap();
            if (CapImage != null && !worker.IsBusy)
                worker.RunWorkerAsync();
        }

        private async void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            QRCodeDecoder Decoder = new QRCodeDecoder();
            try
            {
                string idMarcacao = "";
                //aqui ele guarda o qr code em forma de texto. Provavelmente deves guardar o numero da consulta quando fores criar QRCOde, para depois guardares numa string e procurares na BD se está na hora e data da consulta.
                string xpto = Decoder.decode(new QRCodeBitmapImage(CapImage));
                if (xpto != null) {  
                    foreach (char c in xpto) {
                        if (Char.IsDigit(c)) {
                            idMarcacao += c;
                        }
                    }
                    MessageBox.Show("Código da Marcação: " + idMarcacao);
                    if (Globais.idLoggedFunc == 0 && Globais.job != 3 && Globais.job!=5) {
                        //paciente
                        Boolean valid = false;
                        Marcacao marcacao = new Marcacao();
                        if (idMarcacao!= "" && idMarcacao!= null) {
                            BindingSource bsDados = new BindingSource();
                            string URI = Globais.baseURL + "marcacao?Query=idmarcacao%3D" + idMarcacao;
                            using (var client = new HttpClient()) {
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
                                                if (searchResult.IdPaciente == Globais.idPerfil) {
                                                    if (searchResult.Data.Date < DateTime.Now.Date) {
                                                        MessageBox.Show("Esta marcação já nao se encontra disponível");
                                                        valid = false;
                                                        xpto = null;
                                                        idMarcacao = null;
                                                    } else if (searchResult.Data.Date > DateTime.Now.Date) {
                                                        MessageBox.Show("Esta marcação realizar-se-à brevemente");
                                                        valid = false;
                                                        xpto = null;
                                                        idMarcacao = null;
                                                    } else if (searchResult.Data.Date == DateTime.Now.Date) {
                                                        if (searchResult.Relatorio.Contains("Entrada")) {
                                                            MessageBox.Show("Já sinalizou esta marcação anteriormente");
                                                            valid = false;
                                                            xpto = null;
                                                            idMarcacao = null;
                                                        } else {
                                                            MessageBox.Show("Entrada sinalizada com sucesso");
                                                            valid = true;
                                                            marcacao = searchResult;
                                                        }

                                                    }
                                                } else {
                                                    MessageBox.Show("Código de marcação incorreto");
                                                    valid = false;
                                                    xpto = null;
                                                    idMarcacao = null;
                                                }
                                            }
                                        } else {
                                            MessageBox.Show("Esta marcação não existe");
                                            valid = false;
                                            xpto = null;
                                            idMarcacao = null;
                                        }
                                    } else {
                                        MessageBox.Show("Erro. Contacte o administrador");
                                    }
                                }
                            }
                            if (valid) {
                                URI = Globais.baseURL + "marcacao/" + idMarcacao;
                                using (var client = new HttpClient()) {
                                    //Criar class Paciente. substituir filtyer na 131 por paciente.
                                    //confirmar a response como na 109
                                    marcacao.Relatorio = "Entrada sinalizada no dia " + DateTime.Now.Date + " às " + DateTime.Now.TimeOfDay;
                                    marcacao.UltimaAtualizacao = DateTime.Now;
                                    //TODO encriptar a senha
                                    var serializedUtilizador = JsonConvert.SerializeObject(marcacao);
                                    var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                                    var response = await client.PutAsync(URI, content);
                                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                        Console.WriteLine("Marcação editada");
                                        MessageBox.Show("Marcação Atualizada");
                                        xpto = null;
                                        idMarcacao = null;
                                        this.BackColor = Color.FromArgb(97, 199, 160);
                                        panel2.Visible = false;
                                        textBox1.Text = "";
                                        foreach (Control child in this.Controls) {
                                            child.Enabled = true;
                                        }
                                        button1.Enabled = false;
                                    } else {

                                        MessageBox.Show("Erro. Contacte o administrador do sistema");
                                        Console.WriteLine("Erro do servidor");
                                        xpto = null;
                                        idMarcacao = null;
                                    }
                                }
                            }
                        }
                    } else {
                        //funcionario
                        Boolean valid = false;
                        Marcacao marcacao = new Marcacao();
                        if (idMarcacao!= "" && idMarcacao != null) {
                            BindingSource bsDados = new BindingSource();
                            string URI = Globais.baseURL + "marcacao?Query=idmarcacao%3D" + idMarcacao;
                            using (var client = new HttpClient()) {
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
                                                if (searchResult.Data.Date < DateTime.Now.Date) {
                                                    MessageBox.Show("Esta marcação já nao se encontra disponível");
                                                    valid = false;
                                                    xpto = null;
                                                    idMarcacao = null;
                                                } else if (searchResult.Data.Date > DateTime.Now.Date) {
                                                    MessageBox.Show("Esta marcação realizar-se-à brevemente");
                                                    valid = false;
                                                    xpto = null;
                                                    idMarcacao = null;
                                                } else if (searchResult.Data.Date == DateTime.Now.Date) {
                                                    if (searchResult.Relatorio.Contains("Entrada")) {
                                                        MessageBox.Show("Já sinalizou esta marcação anteriormente");
                                                        valid = false;
                                                        xpto = null;
                                                        idMarcacao = null;
                                                    } else {
                                                        MessageBox.Show("Entrada sinalizada com sucesso");
                                                        valid = true;
                                                        marcacao = searchResult;
                                                    }

                                                }
                                            }
                                        } else {
                                            MessageBox.Show("Esta marcação não existe");
                                            valid = false;
                                            xpto = null;
                                            idMarcacao = null;
                                        }
                                    } else {
                                        MessageBox.Show("Erro. Contacte o administrador");
                                        xpto = null;
                                        idMarcacao = null;
                                    }
                                }
                            }
                            if (valid) {
                                URI = Globais.baseURL + "marcacao/" + idMarcacao;
                                using (var client = new HttpClient()) {
                                    //Criar class Paciente. substituir filtyer na 131 por paciente.
                                    //confirmar a response como na 109
                                    marcacao.Relatorio = "Entrada sinalizada no dia " + DateTime.Now.Date + " às " + DateTime.Now.TimeOfDay;
                                    marcacao.UltimaAtualizacao = DateTime.Now;
                                    //TODO encriptar a senha
                                    var serializedUtilizador = JsonConvert.SerializeObject(marcacao);
                                    var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                                    var response = await client.PutAsync(URI, content);
                                    if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                        Console.WriteLine("Marcação editada");
                                        MessageBox.Show("Marcação Atualizada");
                                        xpto = null;
                                        idMarcacao = null;
                                        this.BackColor = Color.FromArgb(97, 199, 160);
                                        panel2.Visible = false;
                                        textBox1.Text = "";
                                        foreach (Control child in this.Controls) {
                                            child.Enabled = true;
                                        }
                                        button1.Enabled = false;
                                    } else {

                                        MessageBox.Show("Erro. Contacte o administrador do sistema");
                                        Console.WriteLine("Erro do servidor");
                                        xpto = null;
                                        idMarcacao = null;
                                    }
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception es)
            {
                Console.WriteLine(es.ToString());
                //Signifa que nao encontrou QRCODE
            }
        }

        private void QRReader_Load(object sender, EventArgs e)
        {
            if (Globais.Email == null || Globais.Email=="")
            {
                //is the touch machine
                button3.Visible = false;
            }
            this.WindowState = FormWindowState.Maximized;
            label1.Size = new Size(label1.Width + 110, label1.Height);
            label1.Location = new Point(label1.Location.X - 80, label1.Location.Y);
            label1.Font = new Font("Arial", 24, FontStyle.Bold);
            label1.TextAlign = ContentAlignment.TopCenter;
            

        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(32, 66, 53);
            try
            {
                cam.Start();
                t.Start();
                button1.Enabled = true;
                button2.Enabled = false;
            }
            catch (Exception ex)
            {
                cam.Stop();
                Console.WriteLine(ex);
                MessageBox.Show("Erro");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(97, 199, 160);
            cam.Stop();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(97, 199, 160);
            panel2.Visible = false;
            textBox1.Text = "";
            foreach (Control child in this.Controls)
            {
                    child.Enabled = true;
            }
            button1.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(32, 66, 53);
            cam.Stop();
            button1.Enabled = false;
            button2.Enabled = true;
            panel2.Visible = true;
            textBox1.Text = "";
            foreach (Control child in this.Controls)
            {
                if (child != panel2)
                    child.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var formToShow = Application.OpenForms.Cast<Form>()
            .FirstOrDefault(c => c is Menu);
            if (formToShow != null)
            {
                formToShow.Show();
            }
            this.Close();
        }

        private async void button5_Click(object sender, EventArgs e) {
            if (Globais.idLoggedFunc == 0 && Globais.job != 3) {
                //paciente
                Boolean valid = false;
                Marcacao marcacao = new Marcacao();
                if (textBox1.Text != "" && textBox1.Text != " ") {
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "marcacao?Query=idmarcacao%3D" + textBox1.Text;
                    using (var client = new HttpClient()) {
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
                                        if (searchResult.IdPaciente == Globais.idPerfil) {
                                            if (searchResult.Data.Date < DateTime.Now.Date) {
                                                MessageBox.Show("Esta marcação já nao se encontra disponível");
                                                valid = false;
                                            } else if (searchResult.Data.Date > DateTime.Now.Date) {
                                                MessageBox.Show("Esta marcação realizar-se-à brevemente");
                                                valid = false;
                                            } else if (searchResult.Data.Date == DateTime.Now.Date) {
                                                if (searchResult.Relatorio.Contains("Entrada")) {
                                                    MessageBox.Show("Já sinalizou esta marcação anteriormente");
                                                    valid = false;
                                                } else {
                                                    MessageBox.Show("Entrada sinalizada com sucesso");
                                                    valid = true;
                                                    marcacao = searchResult;
                                                }

                                            }
                                        } else {
                                            MessageBox.Show("Código de marcação incorreto");
                                            valid = false;
                                        }
                                    }
                                } else {
                                    MessageBox.Show("Esta marcação não existe");
                                    valid = false;
                                }
                            } else {
                                MessageBox.Show("Erro. Contacte o administrador");
                            }
                        }
                    }
                    if (valid) {
                        URI = Globais.baseURL + "marcacao/" + textBox1.Text;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            marcacao.Relatorio = "Entrada sinalizada no dia " + DateTime.Now.Date + " às " + DateTime.Now.TimeOfDay;
                            marcacao.UltimaAtualizacao = DateTime.Now;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(marcacao);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Marcação editada");
                                MessageBox.Show("Marcação Atualizada");
                                this.BackColor = Color.FromArgb(97, 199, 160);
                                panel2.Visible = false;
                                textBox1.Text = "";
                                foreach (Control child in this.Controls) {
                                    child.Enabled = true;
                                }
                                button1.Enabled = false;
                            } else {

                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    }
                }
            } else {
                //funcionario
                Boolean valid = false;
                Marcacao marcacao = new Marcacao();
                if (textBox1.Text != "" && textBox1.Text != " ") {
                    BindingSource bsDados = new BindingSource();
                    string URI = Globais.baseURL + "marcacao?Query=idmarcacao%3D" + textBox1.Text;
                    using (var client = new HttpClient()) {
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
                                        if (searchResult.Data.Date < DateTime.Now.Date) {
                                            MessageBox.Show("Esta marcação já nao se encontra disponível");
                                            valid = false;
                                        } else if (searchResult.Data.Date > DateTime.Now.Date) {
                                            MessageBox.Show("Esta marcação realizar-se-à brevemente");
                                            valid = false;
                                        } else if (searchResult.Data.Date == DateTime.Now.Date) {
                                            if (searchResult.Relatorio.Contains("Entrada")) {
                                                MessageBox.Show("Já sinalizou esta marcação anteriormente");
                                                valid = false;
                                            } else {
                                                MessageBox.Show("Entrada sinalizada com sucesso");
                                                valid = true;
                                                marcacao = searchResult;
                                            }

                                        }
                                    }
                                } else {
                                    MessageBox.Show("Esta marcação não existe");
                                    valid = false;
                                }
                            } else {
                                MessageBox.Show("Erro. Contacte o administrador");
                            }
                        }
                    }
                    if (valid) {
                        URI = Globais.baseURL + "marcacao/" + textBox1.Text;
                        using (var client = new HttpClient()) {
                            //Criar class Paciente. substituir filtyer na 131 por paciente.
                            //confirmar a response como na 109
                            marcacao.Relatorio = "Entrada sinalizada no dia " + DateTime.Now.Date + " às " + DateTime.Now.TimeOfDay;
                            marcacao.UltimaAtualizacao = DateTime.Now;
                            //TODO encriptar a senha
                            var serializedUtilizador = JsonConvert.SerializeObject(marcacao);
                            var content = new StringContent(serializedUtilizador, Encoding.UTF8, "application/json");
                            var response = await client.PutAsync(URI, content);
                            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                                Console.WriteLine("Marcação editada");
                                MessageBox.Show("Marcação Atualizada");
                                this.BackColor = Color.FromArgb(97, 199, 160);
                                panel2.Visible = false;
                                textBox1.Text = "";
                                foreach (Control child in this.Controls) {
                                    child.Enabled = true;
                                }
                                button1.Enabled = false;
                            } else {

                                MessageBox.Show("Erro. Contacte o administrador do sistema");
                                Console.WriteLine("Erro do servidor");
                            }
                        }
                    }
                }

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e) {
            Regex textUtilr = new Regex(@"[^0-9]");
            MatchCollection matches = textUtilr.Matches(textBox1.Text);
            if (matches.Count > 0 || textBox1.Text == " ") {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
                textBox1.SelectionStart = textBox1.TextLength;
                System.Media.SystemSounds.Hand.Play();
            }
        }
    }
}
