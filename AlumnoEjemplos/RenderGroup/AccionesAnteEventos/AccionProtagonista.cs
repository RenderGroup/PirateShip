using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class AccionProtagonista : AccionSobreEvento
    {
        BarcoProtagonista prota;

        public AccionProtagonista(BarcoProtagonista prota) : base(prota) { this.prota = prota; }

        public override void huboCongelamiento(string Technique)
        {
            prota.movimientoDeslizante();

            base.huboCongelamiento(Technique);
        }

        public override void huboDescongelamiento(string Technique)
        {
            prota.movimientoNormal();

            base.huboDescongelamiento(Technique);
        }
    }
}
