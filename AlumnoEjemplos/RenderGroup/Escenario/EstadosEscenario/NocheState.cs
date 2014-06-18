using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class NocheState : IEscenarioEventosState
    {
        public void cambioNocheDia()
        {
            Escenario.SeHizoDeDia("RenderScene");

            Escenario.Estado = new DiaState();
        }

        public void cambioTemperatura()
        {
            Escenario.HuboCongelamiento("RenderSceneNocheCongelada");

            Escenario.Estado = new NocheCongeladaState();
        }
    }
}
