
namespace ModPlayer
{
	partial class SongForm
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
			this.PlayButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// PlayButton
			// 
			this.PlayButton.Location = new System.Drawing.Point(713, 415);
			this.PlayButton.Name = "PlayButton";
			this.PlayButton.Size = new System.Drawing.Size(75, 23);
			this.PlayButton.TabIndex = 0;
			this.PlayButton.Text = "Play";
			this.PlayButton.UseVisualStyleBackColor = true;
			// 
			// stopButton
			// 
			this.stopButton.Location = new System.Drawing.Point(632, 415);
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size(75, 23);
			this.stopButton.TabIndex = 1;
			this.stopButton.Text = "Stop";
			this.stopButton.UseVisualStyleBackColor = true;
			// 
			// SongForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.PlayButton);
			this.Name = "SongForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SongForm";
			this.ResumeLayout(false);

		}

		#endregion

		public System.Windows.Forms.Button PlayButton;
		public System.Windows.Forms.Button stopButton;
	}
}