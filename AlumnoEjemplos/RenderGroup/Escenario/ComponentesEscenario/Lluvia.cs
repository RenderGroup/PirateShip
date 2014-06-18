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
    class Lluvia : TgcAnimatedSprite, IUpdateRender, ILluviaObserver
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

            //si no es el ultimo elemento lo pone al final... por cuestiones de render
            if (Escenario.Elementos.Last() != this)
                Escenario.Agregar(Escenario.Remover(this));

            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void cambioLluvia() 
        {
            if (!Escenario.Elementos.Contains(this))
                Escenario.Agregar(this);
            else
                Escenario.Remover(this);
        }
    }
}
