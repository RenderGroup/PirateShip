using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class AccionesOceano : AccionSobreEvento
    {
        Oceano oceano;

        public AccionesOceano(Oceano oceano) : base(oceano)
        {
            this.oceano = oceano;
        }

        public override void huboCongelamiento(string Technique)
        {
            oceano.huboCongelamiento();

            base.huboCongelamiento(Technique);
        }

        public override void huboDescongelamiento(string Technique)
        {
            oceano.huboDescongelamiento();

            base.huboDescongelamiento(Technique);
        }

        public override void seHizoDeDia(string Technique)
        {
            oceano.cubemapDia();

            base.seHizoDeDia(Technique);
        }

        public override void seHizoDeNoche(string Technique)
        {
            oceano.cubemapNoche();

            base.seHizoDeNoche(Technique);
        }
    }
}
