
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.RenderGroup
{
    class HUB : ReceptorInput, IUpdateRender
    {
        static bool borrarFlag = false;

        TgcSprite timon;
        TgcSprite barra;
        TgcAnimatedSprite lluvia;
        Size screenSize = GuiController.Instance.Panel3d.Size;
        Boolean camara;

        public HUB()
        {
            crearSprites();
        }

        public void update()
        {
        }

        public void render() 
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();

            if (borrarFlag)
            {
                lluvia.updateAndRender();
            }

            barra.render();
            timon.render();

            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void dispose()
        {
            timon.dispose();
            barra.dispose();
            lluvia.dispose();
        }
        public void crearSprites()
        {
            Size textureSize;

            timon = new TgcSprite();
            timon.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\timon.png");
            textureSize = timon.Texture.Size;
            timon.Position = new Vector2(0, screenSize.Height - (textureSize.Height / 1.8f));
            timon.RotationCenter = new Vector2(129, 129);
            

            barra = new TgcSprite();
            barra.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\barra.png");
            textureSize = barra.Texture.Size;
            barra.Position = new Vector2(0, screenSize.Height - textureSize.Height);

            //Crear Sprite animado para la lluvia
            lluvia = new TgcAnimatedSprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\LLUVIA2.png", //Textura de 512 X 512
                new Size(128, 128), //Tamaño de un frame (128x128px en este caso)
                16, //Cantidad de frames, (son 16 de 128x128px)
                20 //Velocidad de animacion, en cuadros x segundo
                );

            lluvia.Position = new Vector2(-10, 0);
            lluvia.Scaling = new Vector2(8, 4);

        }

        public void renderizar()
        {
            }

        public static void cambioLluvia() 
        {
            borrarFlag = !borrarFlag;
        }

        public override void A_apretado()
        {
            timon.Rotation += Barco.VELOCIDAD_ROTACION * GuiController.Instance.ElapsedTime;
        }

        public override void D_apretado()
        {
            timon.Rotation -= Barco.VELOCIDAD_ROTACION * GuiController.Instance.ElapsedTime;
        } 
    }
}
