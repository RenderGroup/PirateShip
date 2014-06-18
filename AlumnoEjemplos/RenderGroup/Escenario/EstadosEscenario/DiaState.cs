using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    class DiaState : IEscenarioEventosState
    {
        public void cambioNocheDia()
        {
            Escenario.SeHizoDeNoche("RenderSceneNoche");

            Escenario.Estado = new NocheState();
        }

        public void cambioTemperatura()
        {
            Escenario.HuboCongelamiento("RenderSceneCongelada");

            Escenario.Estado = new DiaCongeladoState();
        }
    }
}
