using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class AccionSkybox : AccionSobreEvento
    {
        PirateSkyBox skybox;

        public AccionSkybox(PirateSkyBox skybox) : base(skybox) { this.skybox = skybox; }

        public override void seHizoDeDia(string Technique)
        {
            skybox.cargarTexturas();

            base.seHizoDeDia(Technique);
        }

        public override void seHizoDeNoche(string Technique)
        {
            skybox.cargarTexturasNoche();

            base.seHizoDeNoche(Technique);
        }
    }
}
