using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.RenderGroup
{
    class BarcoEnemigo : Barco
    {
        public override void update()
        {
            this.Effect.SetValue("blendStart", (float)GuiController.Instance.Modifiers["blend start"]);

            base.update();
        }
    }
}
