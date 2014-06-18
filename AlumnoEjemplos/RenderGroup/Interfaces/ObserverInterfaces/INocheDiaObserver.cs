using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    interface INocheDiaObserver
    {
        void seHizoDeDia(string Technique);

        void seHizoDeNoche(string Technique);

        void setTechnique(string Technique);
    }
}
