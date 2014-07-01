
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
        TgcSprite timon;
        TgcSprite barra;
        Size screenSize = GuiController.Instance.Panel3d.Size;
        private TgcText2d texPuntos;
        

        public HUB()
        {
            crearSprites();
            crearTexto();
        }

        public void update()
        {
            if (Escenario.MUERTES != 0)
                texPuntos.Text = "Ships down: " + Escenario.MUERTES.ToString();
        }

        public void render() 
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();

            barra.render();
            timon.render();

            GuiController.Instance.Drawer2D.endDrawSprite();
            texPuntos.render();
        }

        public void dispose()
        {
            timon.dispose();
            barra.dispose();
            texPuntos.dispose();
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
        }

        public override void A_apretado()
        {
            timon.Rotation += Barco.VELOCIDAD_ROTACION * GuiController.Instance.ElapsedTime;
        }

        public override void D_apretado()
        {
            timon.Rotation -= Barco.VELOCIDAD_ROTACION * GuiController.Instance.ElapsedTime;
        }
        
        private void crearTexto()
        {
            texPuntos = new TgcText2d();
            texPuntos.Text = "KILL'EM!";
            texPuntos.Color = Color.DarkGoldenrod;
            texPuntos.Align = TgcText2d.TextAlign.CENTER;
            texPuntos.Position = new Point((int)((barra.Position.X + timon.Texture.Size.Width)), (int)(screenSize.Height - barra.Texture.Height*0.9));
            texPuntos.Size = new Size(barra.Texture.Size.Width - timon.Texture.Size.Width, 70);
            texPuntos.changeFont(new System.Drawing.Font("Candara", 28, FontStyle.Bold));
        }
    }
}
