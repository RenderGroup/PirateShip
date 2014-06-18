using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class NocheCongeladaState : IEscenarioEventosState
    {
        public void cambioNocheDia()
        {
            Escenario.SeHizoDeDia("RenderSceneCongelada");

            Escenario.Estado = new DiaCongeladoState();
        }

        public void cambioTemperatura()
        {
            Escenario.HuboDescongelamiento("RenderSceneNoche");

            Escenario.Estado = new NocheState();
        }
    }
}
