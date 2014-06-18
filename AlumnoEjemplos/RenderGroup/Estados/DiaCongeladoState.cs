using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class DiaCongeladoState : IEscenarioEventosState
    {
        public void cambioNocheDia()
        {
            Escenario.SeHizoDeNoche("RenderSceneNocheCongelada");

            Escenario.Estado = new NocheCongeladaState();
        }

        public void cambioTemperatura()
        {
            Escenario.HuboDescongelamiento("RenderScene");

            Escenario.Estado = new DiaState();
        }
    }
}
