using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;

namespace AlumnoEjemplos.RenderGroup
{
    //Una bola de cañon es un mesh con un bounding sphere circular que debe moverse con tiro parabolico
    //Mientas ve si hay algun barco en su camino para dañarlo...
    class BolaDeCanion : Elemento
    {
        public override void update()
        {
            float elapsedTime = GuiController.Instance.ElapsedTime;

            this.mover(100f * elapsedTime);

            base.update();
        }
    }
}
