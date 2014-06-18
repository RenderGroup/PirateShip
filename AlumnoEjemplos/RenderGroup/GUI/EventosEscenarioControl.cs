using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TgcViewer;

namespace AlumnoEjemplos.RenderGroup
{
    public partial class EventosEscenarioControl : UserControl
    {
        public EventosEscenarioControl()
        {            
            InitializeComponent();
            agregarImagenesImageList();
        }


        private void agregarImagenesImageList()
        {
            string path = GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\";

            BackgroundImage = Image.FromFile(path + "barra2.png");
            btnAnimacion.BackgroundImage = Image.FromFile(path + "botonGaviota.png");
            btnDiaNoche.BackgroundImage = Image.FromFile(path + "botonNoche.png");
            btnLluvia.BackgroundImage = Image.FromFile(path + "botonLluvia.png");
            btnHielo.BackgroundImage = Image.FromFile(path + "botonHielo.png");

        }

        private void btnDiaNoche_Click(object sender, EventArgs e)
        {
            //ejemplo.btnDiaNoche();
        }

        private void btnAnimacion_Click(object sender, EventArgs e)
        {
            Gaviota.AnimadorDeGaviota.animar();
        }

        private void btnLluvia_Click(object sender, EventArgs e)
        {
            Escenario.CambioLluvia();
        }

        private void btnHielo_Click(object sender, EventArgs e)
        {
            //ejemplo.btnHielo();
        }
    }
}
