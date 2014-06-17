using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class ProtaInputHandler : ReceptorInput
    {
        BarcoProtagonista prota;

        public ProtaInputHandler(BarcoProtagonista prota) 
        {
            this.prota = prota;
        }

        override public void W_apretado()
        {
            prota.mover(prota.acelerar(Barco.ACELERACION));
        }

        override public void A_apretado()
        {
            prota.rotateY(-Barco.VELOCIDAD_ROTACION);
        }

        override public void S_apretado()
        {
            prota.mover(prota.acelerar(-Barco.ACELERACION));
        }

        override public void D_apretado()
        {
            prota.rotateY(Barco.VELOCIDAD_ROTACION);
        }

        override public void P_apretado()
        {
            if ((DateTime.Now - prota.cooldown).TotalSeconds > 1.2)
            {
                prota.disparar();
                prota.cooldown = DateTime.Now;
            }
        }

    }
}
