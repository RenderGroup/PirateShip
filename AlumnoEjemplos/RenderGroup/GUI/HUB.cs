
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

            barra.render();
            timon.render();

            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        public void dispose()
        {
            timon.dispose();
            barra.dispose();
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
    }
}
