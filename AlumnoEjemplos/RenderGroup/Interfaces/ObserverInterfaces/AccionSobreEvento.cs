using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class AccionSobreEvento
    {
        protected INocheDiaTemperaturaObserver observador;

        public AccionSobreEvento(INocheDiaTemperaturaObserver obs) { observador = obs; }

        virtual public void huboDescongelamiento(string Technique) { setTechnique(Technique); }

        virtual public void huboCongelamiento(string Technique) { setTechnique(Technique); }

        virtual public void seHizoDeDia(string Technique) { setTechnique(Technique); }

        virtual public void seHizoDeNoche(string Technique) { setTechnique(Technique);}

        virtual public void setTechnique(string Technique) { observador.setTechnique(Technique); }
    }
}
