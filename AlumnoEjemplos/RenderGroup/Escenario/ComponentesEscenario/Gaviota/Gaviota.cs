using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.RenderGroup
{
    class Gaviota : TgcAnimatedSprite, IUpdateRender
    {
        static public GaviotaAnimator AnimadorDeGaviota = new GaviotaAnimator();

        float velocidadEnX;

        public Gaviota(float altura, Vector2 escalado, float frameRate, float velocidad)
            : base(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\gaviotas2.png", new Size(256, 256), 16, frameRate) 
        {
            Position = new Vector2(-150, altura);
            Scaling = escalado;
            velocidadEnX = velocidad;
        }
        
        new public void update() 
        {
            //se mueve en X segun su velocidad
            Position += new Vector2(velocidadEnX, 0);

            //si se fue de la pantalla...quitarla del escenario
            if (Position.X > GuiController.Instance.Panel3d.Size.Width)
            {
                Position = new Vector2(-150, Position.Y);
                Escenario.Remover(this);
            }
        }
        
        new public void render() 
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();

            updateAndRender();

            GuiController.Instance.Drawer2D.endDrawSprite();
        }
    }
}
