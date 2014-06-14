using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.RenderGroup
{
    class Escenario
    {
        public const float currentScaleXZ = 165f;
        public const float currentScaleY = 0.8f;
        
        static public TgcBox lightMesh = TgcBox.fromSize(new Vector3(10, 10, 10), Color.Red);
        static public Oceano oceano = new Oceano(currentScaleXZ, currentScaleY);
        static public Isla isla = new Isla();
        static public PirateSkyBox skyBox = new PirateSkyBox();

        static public List<IUpdateRender> elementos = new List<IUpdateRender> { skyBox, isla, oceano };

        public static float time = 0;
        static bool lluvia = false;

        static public void UpdateElementos() 
        {
            //usando "elementos" directamente hay un error por modificacion de lista en iteracion
            var updateables = new List<IUpdateRender>(elementos);

            Escenario.time += GuiController.Instance.ElapsedTime;

            foreach (IUpdateRender elemento in updateables) elemento.update();
        }

        static public void RenderElementos() { foreach (IUpdateRender elemento in elementos) elemento.render(); lightMesh.render(); }

        static public void DisposeElementos() { foreach (IUpdateRender elemento in elementos) elemento.dispose(); }

        static public void CrearCuantosEnemigos(int cuantos) { for (int i = 0; i < cuantos; i++) Add(Construir.Enemigo()); }

        static public void llueve() { oceano.mar.Effect.SetValue("llueve", lluvia = !lluvia); }

        static public void BotonDiaNoche_Click() 
        {
            skyBox.botonDiaNoche_Click(isla, oceano);
        }

        static public void Add(IUpdateRender elemento) { elementos.Add(elemento); }
        static public void Remove(IUpdateRender elemento) { elementos.Remove(elemento);}
    }
}
