using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.RenderGroup
{
    class Lluvia : TgcAnimatedSprite, IUpdateRender, LluviaObserver
    {

        public Lluvia() : base(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\LLUVIA2.png", new Size(128, 128),16,20)
        {
            Position = new Vector2(-10, 0);
            Scaling = new Vector2(8, 4);
        }

        new public void render()
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();

            updateAndRender();

            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void cambioLluvia() 
        {
            if (!Escenario.elementos.Contains(this))
                Escenario.Agregar(this);
            else
                Escenario.Remove(this);
        }
    }
}
