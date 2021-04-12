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

namespace Ajuda_
{
    public partial class QRReader : Form
    {
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

        private void T_Tick(object sender, EventArgs e)
        {
            CapImage = cam.GetBitmap();
            if (CapImage != null && !worker.IsBusy)
                worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            QRCodeDecoder Decoder = new QRCodeDecoder();
            try
            {
                //aqui ele guarda o qr code em forma de texto. Provavelmente deves guardar o numero da consulta quando fores criar QRCOde, para depois guardares numa string e procurares na BD se está na hora e data da consulta.
                MessageBox.Show(Decoder.decode(new QRCodeBitmapImage(CapImage)));
            }
            catch (Exception es)
            {
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
    }
}
