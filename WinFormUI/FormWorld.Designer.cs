﻿namespace WinFormUI
{
	partial class FormWorld
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			movementTimer = new System.Windows.Forms.Timer(components);
			SuspendLayout();
			// 
			// movementTimer
			// 
			movementTimer.Enabled = true;
			movementTimer.Tick += movementTimer_Tick;
			// 
			// FormWorld
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(903, 741);
			Name = "FormWorld";
			Text = "Los Inmortales";
			Load += FormWorld_Load;
			Click += FormWorld_Click;
			Paint += FormWorld_Paint;
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.Timer movementTimer;
	}
}
