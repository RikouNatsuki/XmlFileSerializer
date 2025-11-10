namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnStandard = new System.Windows.Forms.Button();
            this.BtnCustom = new System.Windows.Forms.Button();
            this.LblXmlTarget = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BtnStandard
            // 
            this.BtnStandard.Font = new System.Drawing.Font("メイリオ", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BtnStandard.Location = new System.Drawing.Point(55, 105);
            this.BtnStandard.Name = "BtnStandard";
            this.BtnStandard.Size = new System.Drawing.Size(659, 51);
            this.BtnStandard.TabIndex = 0;
            this.BtnStandard.Text = "標準XmlSerializer で .xml 化";
            this.BtnStandard.UseVisualStyleBackColor = true;
            this.BtnStandard.Click += new System.EventHandler(this.BtnStandard_Click);
            // 
            // BtnCustom
            // 
            this.BtnCustom.Font = new System.Drawing.Font("メイリオ", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BtnCustom.Location = new System.Drawing.Point(55, 195);
            this.BtnCustom.Name = "BtnCustom";
            this.BtnCustom.Size = new System.Drawing.Size(659, 51);
            this.BtnCustom.TabIndex = 0;
            this.BtnCustom.Text = "カスタムXmlSerializer で .xml 化";
            this.BtnCustom.UseVisualStyleBackColor = true;
            this.BtnCustom.Click += new System.EventHandler(this.BtnCustom_Click);
            // 
            // LblXmlTarget
            // 
            this.LblXmlTarget.AutoSize = true;
            this.LblXmlTarget.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LblXmlTarget.Location = new System.Drawing.Point(48, 27);
            this.LblXmlTarget.Name = "LblXmlTarget";
            this.LblXmlTarget.Size = new System.Drawing.Size(190, 41);
            this.LblXmlTarget.TabIndex = 1;
            this.LblXmlTarget.Text = ".xml 化対象: ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.LblXmlTarget);
            this.Controls.Add(this.BtnCustom);
            this.Controls.Add(this.BtnStandard);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStandard;
        private System.Windows.Forms.Button BtnCustom;
        private System.Windows.Forms.Label LblXmlTarget;
    }
}

