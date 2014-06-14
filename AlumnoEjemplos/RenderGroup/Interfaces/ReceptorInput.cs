using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    //interfaz que implementan los interesados en actuar sobre el input
    public abstract class ReceptorInput
    {
        public virtual void W_apretado() { }
        public virtual void A_apretado() { }
        public virtual void S_apretado() { }
        public virtual void D_apretado() { }
        public virtual void P_apretado() { }
        public virtual void ClickDerecho() { }
        public virtual void ClickIzquierdo() { }
    }
}
