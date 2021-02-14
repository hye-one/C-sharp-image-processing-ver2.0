
namespace Day015_01_컬러영상처리_Beta1_
{
    partial class subForm02
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
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.moveH_num = new System.Windows.Forms.NumericUpDown();
            this.moveH_val = new System.Windows.Forms.Label();
            this.moveW_num = new System.Windows.Forms.NumericUpDown();
            this.moveW_val = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.moveH_num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveW_num)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(277, 284);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(102, 48);
            this.btn_OK.TabIndex = 0;
            this.btn_OK.Text = "확인";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(433, 284);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(108, 48);
            this.btn_cancel.TabIndex = 1;
            this.btn_cancel.Text = "취소";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // moveH_num
            // 
            this.moveH_num.Location = new System.Drawing.Point(433, 111);
            this.moveH_num.Name = "moveH_num";
            this.moveH_num.Size = new System.Drawing.Size(120, 25);
            this.moveH_num.TabIndex = 2;
            // 
            // moveH_val
            // 
            this.moveH_val.AutoSize = true;
            this.moveH_val.Location = new System.Drawing.Point(274, 113);
            this.moveH_val.Name = "moveH_val";
            this.moveH_val.Size = new System.Drawing.Size(67, 15);
            this.moveH_val.TabIndex = 3;
            this.moveH_val.Text = "가로방향";
            // 
            // moveW_num
            // 
            this.moveW_num.Location = new System.Drawing.Point(431, 172);
            this.moveW_num.Name = "moveW_num";
            this.moveW_num.Size = new System.Drawing.Size(120, 25);
            this.moveW_num.TabIndex = 4;
            // 
            // moveW_val
            // 
            this.moveW_val.AutoSize = true;
            this.moveW_val.Location = new System.Drawing.Point(277, 172);
            this.moveW_val.Name = "moveW_val";
            this.moveW_val.Size = new System.Drawing.Size(67, 15);
            this.moveW_val.TabIndex = 5;
            this.moveW_val.Text = "세로방향";
            // 
            // subForm02
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 364);
            this.Controls.Add(this.moveW_val);
            this.Controls.Add(this.moveW_num);
            this.Controls.Add(this.moveH_val);
            this.Controls.Add(this.moveH_num);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_OK);
            this.Name = "subForm02";
            this.Text = "subForm02";
            ((System.ComponentModel.ISupportInitialize)(this.moveH_num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.moveW_num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Label moveH_val;
        private System.Windows.Forms.Label moveW_val;
        public System.Windows.Forms.NumericUpDown moveH_num;
        public System.Windows.Forms.NumericUpDown moveW_num;
    }
}