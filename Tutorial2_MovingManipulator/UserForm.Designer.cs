namespace Tutorial2_MovingManipulator
{
	partial class UserForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.numericUpDown_RotateAmount = new System.Windows.Forms.NumericUpDown();
            this.lbl_RotateAmount = new System.Windows.Forms.Label();
            this.btn_Start = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RotateAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown_RotateAmount
            // 
            this.numericUpDown_RotateAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown_RotateAmount.Location = new System.Drawing.Point(160, 36);
            this.numericUpDown_RotateAmount.Name = "numericUpDown_RotateAmount";
            this.numericUpDown_RotateAmount.Size = new System.Drawing.Size(92, 22);
            this.numericUpDown_RotateAmount.TabIndex = 0;
            this.numericUpDown_RotateAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDown_RotateAmount.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_RotateAmount.ValueChanged += new System.EventHandler(this.numericUpDown_RotateAmount_ValueChanged);
            // 
            // lbl_RotateAmount
            // 
            this.lbl_RotateAmount.AutoSize = true;
            this.lbl_RotateAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_RotateAmount.Location = new System.Drawing.Point(31, 38);
            this.lbl_RotateAmount.Name = "lbl_RotateAmount";
            this.lbl_RotateAmount.Size = new System.Drawing.Size(99, 16);
            this.lbl_RotateAmount.TabIndex = 1;
            this.lbl_RotateAmount.Text = "Rotate Amount:";
            // 
            // btn_Start
            // 
            this.btn_Start.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Start.Location = new System.Drawing.Point(13, 110);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(259, 61);
            this.btn_Start.TabIndex = 2;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            // 
            // UserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.lbl_RotateAmount);
            this.Controls.Add(this.numericUpDown_RotateAmount);
            this.Name = "UserForm";
            this.Text = "IPC Tutorial 2: Moving the Manipulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UserForm_FormClosing);
            this.Load += new System.EventHandler(this.UserForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_RotateAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.NumericUpDown numericUpDown_RotateAmount;
        private System.Windows.Forms.Label lbl_RotateAmount;
        private System.Windows.Forms.Button btn_Start;
	}
}

