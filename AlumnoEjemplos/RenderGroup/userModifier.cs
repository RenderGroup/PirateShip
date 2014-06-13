using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Modifiers;

namespace AlumnoEjemplos.RenderGroup
{
    public class userModifier : TgcModifierPanel
    {
        UserControl1 control;
        public UserControl1 Control
        {
            get { return control; }
        }

        public userModifier(string varName, EjemploAlumno ejemplo)
            : base(varName)
        {
            control = new UserControl1(ejemplo);
            contentPanel.Controls.Add(control);
        }

        public override object getValue()
        {
            return null;
        }
    }
}
