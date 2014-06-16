namespace AlumnoEjemplos.RenderGroup
{
    partial class EventosEscenarioControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnHielo = new System.Windows.Forms.Button();
            this.btnLluvia = new System.Windows.Forms.Button();
            this.btnDiaNoche = new System.Windows.Forms.Button();
            this.btnAnimacion = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnHielo
            // 
            this.btnHielo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(27)))), ((int)(((byte)(12)))));
            this.btnHielo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHielo.Location = new System.Drawing.Point(39, 196);
            this.btnHielo.Name = "btnHielo";
            this.btnHielo.Size = new System.Drawing.Size(34, 40);
            this.btnHielo.TabIndex = 7;
            this.btnHielo.UseVisualStyleBackColor = true;
            this.btnHielo.Click += new System.EventHandler(this.btnHielo_Click);
            // 
            // btnLluvia
            // 
            this.btnLluvia.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(27)))), ((int)(((byte)(12)))));
            this.btnLluvia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLluvia.Location = new System.Drawing.Point(39, 138);
            this.btnLluvia.Name = "btnLluvia";
            this.btnLluvia.Size = new System.Drawing.Size(34, 40);
            this.btnLluvia.TabIndex = 6;
            this.btnLluvia.UseVisualStyleBackColor = true;
            this.btnLluvia.Click += new System.EventHandler(this.btnLluvia_Click);
            // 
            // btnDiaNoche
            // 
            this.btnDiaNoche.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(27)))), ((int)(((byte)(12)))));
            this.btnDiaNoche.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDiaNoche.Location = new System.Drawing.Point(39, 80);
            this.btnDiaNoche.Name = "btnDiaNoche";
            this.btnDiaNoche.Size = new System.Drawing.Size(34, 40);
            this.btnDiaNoche.TabIndex = 5;
            this.btnDiaNoche.UseVisualStyleBackColor = true;
            this.btnDiaNoche.Click += new System.EventHandler(this.btnDiaNoche_Click);
            // 
            // btnAnimacion
            // 
            this.btnAnimacion.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(27)))), ((int)(((byte)(12)))));
            this.btnAnimacion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnimacion.Location = new System.Drawing.Point(39, 22);
            this.btnAnimacion.Name = "btnAnimacion";
            this.btnAnimacion.Size = new System.Drawing.Size(34, 40);
            this.btnAnimacion.TabIndex = 4;
            this.btnAnimacion.UseVisualStyleBackColor = true;
            this.btnAnimacion.Click += new System.EventHandler(this.btnAnimacion_Click);
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnHielo);
            this.Controls.Add(this.btnLluvia);
            this.Controls.Add(this.btnAnimacion);
            this.Controls.Add(this.btnDiaNoche);
            this.Name = "UserControl1";
            this.Size = new System.Drawing.Size(112, 256);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnHielo;
        private System.Windows.Forms.Button btnLluvia;
        private System.Windows.Forms.Button btnDiaNoche;
        private System.Windows.Forms.Button btnAnimacion;
    }
}
