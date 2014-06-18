using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    interface ITemperaturaObserver
    {
        void huboCongelamiento(string Technique);

        void huboDescongelamiento(string Technique);

        void setTechnique(string Technique);
    }
}
