using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    //interfaz que implementan los interesados en actuar sobre el input
    public interface IReceptorInput
    {
        void W_apretado();
        void A_apretado();
        void S_apretado();
        void D_apretado();
        void P_apretado();
    }
}
