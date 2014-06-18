using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class ProtaInputHandler : ReceptorInput
    {
        protected BarcoProtagonista prota;

        public ProtaInputHandler(BarcoProtagonista prota) 
        {
            this.prota = prota;

            prota.inputManager = this;

            prota.acelerarSegunInput = () => {};
        }

        override public void W_apretado()
        {
            setearAceleracionProta(prota.ACELERACION_INSTANTANEA);
        }

        override public void S_apretado()
        {
            setearAceleracionProta(-prota.ACELERACION_INSTANTANEA);
        }

        override public void D_apretado()
        {
            prota.rotateY(Barco.VELOCIDAD_ROTACION);
        }

        override public void A_apretado()
        {
            prota.rotateY(-Barco.VELOCIDAD_ROTACION);
        }

        override public void P_apretado()
        {
            if ((DateTime.Now - prota.cooldown).TotalSeconds > .5)
            {
                prota.disparar();
                prota.cooldown = DateTime.Now;
            }
        }

        public void setearAceleracionProta(float valorAceleracion)
        {
            prota.acelerarSegunInput = () =>
            {
                prota.acelerar(valorAceleracion);

                prota.acelerarSegunInput = (() =>  prota.desacelerar(Barco.FACTOR_DESACELERATIVO));

            };
        }

        virtual public void cambiarCamara() 
        {
            prota.inputManager = new ProtaCamInputHandler(prota);
        }

        virtual public void manejarCamara() { }
    }
}
