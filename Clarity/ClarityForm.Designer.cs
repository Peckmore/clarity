namespace Clarity
{
    partial class ClarityForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.timerFade = new System.Windows.Forms.Timer();
            this.timerDeactivate = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // timerFade
            // 
            this.timerFade.Interval = 12;
            this.timerFade.Tick += new System.EventHandler(this.timerFade_Tick);
            // 
            // timerDeactivate
            // 
            this.timerDeactivate.Interval = 25;
            this.timerDeactivate.Tick += new System.EventHandler(this.timerDeactivate_Tick);
            // 
            // ClarityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(836, 475);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClarityForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Clarity";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.ClarityForm_Activated);
            this.Deactivate += new System.EventHandler(this.ClarityForm_Deactivate);
            this.Shown += new System.EventHandler(this.ClarityForm_Shown);
            this.SizeChanged += new System.EventHandler(this.ClarityForm_SizeChanged);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ClarityForm_MouseClick);
            this.MouseEnter += new System.EventHandler(this.ClarityForm_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ClarityForm_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ClarityForm_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerFade;
        private System.Windows.Forms.Timer timerDeactivate;
    }
}