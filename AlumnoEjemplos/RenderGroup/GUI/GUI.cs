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
    class GUI : ReceptorInput, IUpdateRender
    {
        static bool borrarFlag = false;

        TgcSprite botonMundo;
        TgcSprite botonGaviota;
        TgcSprite timon;
        TgcSprite barra;
        TgcAnimatedSprite lluvia;
        TgcAnimatedSprite gaviota;
        int traslacion = -150;
        Size screenSize = GuiController.Instance.Panel3d.Size;
        Boolean camara;

        public GUI()
        {
            crearSprites();
        }

        public void update()
        {
            coordenadasMouse();
        }

        public void render() 
        {
            renderizar();
        }

        public void dispose()
        {
            botonMundo.dispose();
            botonGaviota.dispose();
            timon.dispose();
            barra.dispose();
            lluvia.dispose();
            gaviota.dispose();
        }

        public void coordenadasMouse() //se fija si hace clic sobre un boton
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            //Obtener variacion XY del mouse
            float mouseX = 0f;
            float mouseY = 0f;
            float botonX = botonMundo.Position.X + botonMundo.Texture.Width;
            float botonY = botonMundo.Position.Y + botonMundo.Texture.Height;

            float boton2X = botonGaviota.Position.X + botonGaviota.Texture.Width;
            float boton2Y = botonGaviota.Position.Y + botonGaviota.Texture.Height;

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                mouseX = d3dInput.Xpos;// XposRelative;
                mouseY = d3dInput.Ypos;// YposRelative;

                if ((mouseX > botonMundo.Position.X) && (mouseX < botonX) && (mouseY > botonMundo.Position.Y) && (mouseY < botonY))
                {

                    //MessageBox.Show("CLIC EN SPRITE CUADRADO DERECHO");
                }
                if ((mouseX > botonGaviota.Position.X) && (mouseX < boton2X) && (mouseY > botonGaviota.Position.Y) && (mouseY < boton2Y))
                {
                    traslacion = -150;
                    //Crear Sprite animado para la gaviota
                    gaviota = new TgcAnimatedSprite(
                        GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\gaviotas2.png", //Textura de 1024 X 1024
                        new Size(256, 256), //Tamaño de un frame (128x128px en este caso)
                        16, //Cantidad de frames, (son 16 de 128x128px)
                        1 //Velocidad de animacion, en cuadros x segundo
                        );
                    //MessageBox.Show("CLIC EN SPRITE CUADRADO IZQUIERDO");
                }
            }
        }
        public void crearSprites()
        {
            botonMundo = new TgcSprite();
            botonMundo.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton2z.png");
            Size textureSize = botonMundo.Texture.Size;
            botonMundo.Position = new Vector2(screenSize.Width - textureSize.Width, screenSize.Height - textureSize.Height);

            botonGaviota = new TgcSprite();
            botonGaviota.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\boton2.png");
            textureSize = botonGaviota.Texture.Size;
            botonGaviota.Position = new Vector2((screenSize.Width - botonMundo.Texture.Size.Width) - textureSize.Width, screenSize.Height - textureSize.Height);

            timon = new TgcSprite();
            timon.Texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\timon.png");
            textureSize = timon.Texture.Size;
            timon.Position = new Vector2(0, screenSize.Height - textureSize.Height);

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

            //Crear Sprite animado para la gaviota
            gaviota = new TgcAnimatedSprite(
                GuiController.Instance.AlumnoEjemplosMediaDir + "RenderGroup\\texturas\\gaviotas2.png", //Textura de 1024 X 1024
                new Size(256, 256), //Tamaño de un frame (128x128px en este caso)
                16, //Cantidad de frames, (son 16 de 128x128px)
                1 //Velocidad de animacion, en cuadros x segundo
                );
        }

        public void renderizar()
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();

            camara = (Boolean)GuiController.Instance.Modifiers["camaraEnBarco"];

            if (camara)
            {
                gaviota.Scaling = new Vector2(1.4f, 1.4f);
                gaviota.Position = new Vector2(traslacion, 0);
                gaviota.setFrameRate(3);
                traslacion = traslacion + 12;
            }
            else
            {
                gaviota.Scaling = new Vector2(0.4f, 0.4f);
                gaviota.Position = new Vector2(traslacion, screenSize.Height / 3);
                gaviota.setFrameRate(1);
                traslacion = traslacion + 4;
            }
            if (traslacion > screenSize.Width)
            {
                gaviota.dispose();
            }
            else
            {
                gaviota.updateAndRender();
            }

            if (borrarFlag)
            {
                lluvia.updateAndRender();
            }
            botonMundo.render();
            botonGaviota.render();
            barra.render();
            timon.render();

            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public static void llueve() 
        {
            borrarFlag = !borrarFlag;
        }

    }
}
