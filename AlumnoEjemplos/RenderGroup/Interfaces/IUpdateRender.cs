using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.RenderGroup
{
    interface IUpdateRender
    {
        void update();
        void render();
        void dispose();
        void tecnicas(string tecnica);//agregado*
    }
}
