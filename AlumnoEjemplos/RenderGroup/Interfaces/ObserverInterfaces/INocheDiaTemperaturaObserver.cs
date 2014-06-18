using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    interface INocheDiaTemperaturaObserver
    {
        AccionSobreEvento Accion { get; set; }

        void setTechnique(string Technique);
    }
}
