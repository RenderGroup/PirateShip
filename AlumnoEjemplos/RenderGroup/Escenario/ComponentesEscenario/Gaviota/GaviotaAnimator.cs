using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;

namespace AlumnoEjemplos.RenderGroup
{
    class GaviotaAnimator : CamaraObserver
    {
        Gaviota gaviotaCamaraRotacional, gaviotaCamaraEnBarco, gaviotaAhora;

        public GaviotaAnimator() 
        {
            //gaviotaAhora es la gaviota que se va a animar, el TP empieza con la camara sobre el barco...
            gaviotaAhora = gaviotaCamaraEnBarco = new Gaviota(0, new Vector2(1.4f, 1.4f), 3, 12);
            gaviotaCamaraRotacional = new Gaviota(GuiController.Instance.Panel3d.Size.Height/3, new Vector2(1.4f, 1.4f), 1, 4);
        }

        public void animar() 
        {
            //para animar la gaviota la agregamos al escenario
            Escenario.Agregar(gaviotaAhora);
        }

        public void cambioLaCamara() 
        {
            //ante un cambio en la camara, si tenia una gaviota, quiero la otra
            gaviotaAhora = gaviotaAhora == gaviotaCamaraEnBarco ? gaviotaCamaraRotacional : gaviotaCamaraEnBarco;
        }
    }
}
