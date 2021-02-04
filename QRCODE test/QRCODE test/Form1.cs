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
namespace QRCODE_test
{
    public partial class Form1 : Form
    {
        Camera cam;
        Timer t;
        BackgroundWorker worker;
        Bitmap CapImage;

        public Form1()
        {
            InitializeComponent();
            t = new Timer();
            cam = new Camera(pictureBox1);
            worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            t.Tick += T_Tick;
            t.Interval = 1;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            CapImage = cam.GetBitmap();
            if(CapImage != null && !worker.IsBusy)
                worker.RunWorkerAsync();
            
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            QRCodeDecoder Decoder = new QRCodeDecoder();
            try
            {
                //aqui ele guarda o qr code em forma de texto. Provavelmente deves guardar o numero da consulta quando fores criar QRCOde, para depois guardares numa string e procurares na BD se está na hora e data da consulta.
                MessageBox.Show(Decoder.decode(new QRCodeBitmapImage(CapImage)));
            }catch(Exception es)
            {
                //Signifa que nao encontrou QRCODE
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                cam.Start();
                t.Start();
                button2.Enabled = true;
                button1.Enabled = false;
            }catch(Exception ex)
            {
                cam.Stop();
                Console.WriteLine(ex);
                MessageBox.Show("Erro");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cam.Stop();
            button2.Enabled = false;
            button1.Enabled = true;
        }
    }
}
