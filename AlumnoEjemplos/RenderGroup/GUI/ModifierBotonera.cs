using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Modifiers;

namespace AlumnoEjemplos.RenderGroup
{
    public class ModifierBotonera : TgcModifierPanel
    {
        public ModifierBotonera(string varName) : base(varName)
        {
            contentPanel.Controls.Add(new EventosEscenarioControl());
        }

        public override object getValue()
        {
            throw new Exception("no puede pedirsele valores a al modifier de la botonera");
        }
    }
}
