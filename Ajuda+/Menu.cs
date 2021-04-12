﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ajuda_
{
    public partial class Menu : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Menu()
        {
            InitializeComponent();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Terminar Sessão?", "Confirmação",MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Restart();
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (panelConsultas.Visible == true)
            {
                panelConsultas.Visible = false;
            }
            else
            {
                panelConsultas.Visible = true;
            }
            panelExames.Visible = false;
            panelPacientes.Visible = false;
            panelFuncionarios.Visible = false;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (panelExames.Visible == true)
            {
                panelExames.Visible = false;
            }
            else
            {
                panelExames.Visible = true;
            }
            
            panelPacientes.Visible = false;
            panelFuncionarios.Visible = false;
            panelConsultas.Visible = false;
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

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            QRReader QRReader = new QRReader();
            QRReader.ShowDialog();
            
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (panelPacientes.Visible == true)
            {
                panelPacientes.Visible = false;
            }
            else
            {
                panelPacientes.Visible = true;
            }
            panelExames.Visible = false;
            panelFuncionarios.Visible = false;
            panelConsultas.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (panelFuncionarios.Visible == true)
            {
                panelFuncionarios.Visible = false;
            }
            else
            {
                panelFuncionarios.Visible = true;
            }
            panelExames.Visible = false;
            panelPacientes.Visible = false;
            panelConsultas.Visible = false;
        }
    }
}
