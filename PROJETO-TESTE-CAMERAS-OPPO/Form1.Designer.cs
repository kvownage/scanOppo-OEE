namespace PROJETO_TESTE_CAMERAS_OPPO
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblErroTcp = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ledCommCLP = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.ledCommKeyence1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ledCommKeyence2 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ledCommSensorHikro1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnTesteImei = new System.Windows.Forms.Button();
            this.lblOrderIdLabel = new System.Windows.Forms.Label();
            this.lblOrderIdValor = new System.Windows.Forms.Label();
            this.dataGridDados = new System.Windows.Forms.DataGridView();
            this.Order_Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Prod_Model = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mat_Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Material_DESC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status_Leitura = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDados)).BeginInit();
            this.SuspendLayout();
            // 
            // lblErroTcp
            // 
            this.lblErroTcp.BackColor = System.Drawing.Color.Red;
            this.lblErroTcp.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblErroTcp.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblErroTcp.ForeColor = System.Drawing.Color.White;
            this.lblErroTcp.Location = new System.Drawing.Point(0, 749);
            this.lblErroTcp.Name = "lblErroTcp";
            this.lblErroTcp.Size = new System.Drawing.Size(1423, 45);
            this.lblErroTcp.TabIndex = 50;
            this.lblErroTcp.Text = "Falha na leitura dos codigos";
            this.lblErroTcp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblErroTcp.Visible = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.MidnightBlue;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(15, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(1393, 86);
            this.label3.TabIndex = 10;
            this.label3.Text = "LEITURA DINÂMICA OPPO";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(76, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 41);
            this.label1.TabIndex = 19;
            this.label1.Text = "CLP COMM";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ledCommCLP
            // 
            this.ledCommCLP.BackColor = System.Drawing.Color.Lime;
            this.ledCommCLP.Location = new System.Drawing.Point(56, 111);
            this.ledCommCLP.Name = "ledCommCLP";
            this.ledCommCLP.Size = new System.Drawing.Size(21, 41);
            this.ledCommCLP.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(256, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(139, 41);
            this.label2.TabIndex = 21;
            this.label2.Text = "KEYENCE 1 COMM";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ledCommKeyence1
            // 
            this.ledCommKeyence1.BackColor = System.Drawing.Color.Lime;
            this.ledCommKeyence1.Location = new System.Drawing.Point(236, 111);
            this.ledCommKeyence1.Name = "ledCommKeyence1";
            this.ledCommKeyence1.Size = new System.Drawing.Size(21, 41);
            this.ledCommKeyence1.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(440, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(139, 41);
            this.label5.TabIndex = 23;
            this.label5.Text = "KEYENCE 2 COMM";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ledCommKeyence2
            // 
            this.ledCommKeyence2.BackColor = System.Drawing.Color.Lime;
            this.ledCommKeyence2.Location = new System.Drawing.Point(420, 111);
            this.ledCommKeyence2.Name = "ledCommKeyence2";
            this.ledCommKeyence2.Size = new System.Drawing.Size(21, 41);
            this.ledCommKeyence2.TabIndex = 22;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(617, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(139, 41);
            this.label7.TabIndex = 25;
            this.label7.Text = "SENSOR HIKRO 1";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ledCommSensorHikro1
            // 
            this.ledCommSensorHikro1.BackColor = System.Drawing.Color.Lime;
            this.ledCommSensorHikro1.Location = new System.Drawing.Point(597, 111);
            this.ledCommSensorHikro1.Name = "ledCommSensorHikro1";
            this.ledCommSensorHikro1.Size = new System.Drawing.Size(21, 41);
            this.ledCommSensorHikro1.TabIndex = 24;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(23, 198);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(119, 45);
            this.btnReset.TabIndex = 34;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnTesteImei
            // 
            this.btnTesteImei.BackColor = System.Drawing.Color.DarkOrange;
            this.btnTesteImei.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnTesteImei.ForeColor = System.Drawing.Color.White;
            this.btnTesteImei.Location = new System.Drawing.Point(152, 198);
            this.btnTesteImei.Margin = new System.Windows.Forms.Padding(4);
            this.btnTesteImei.Name = "btnTesteImei";
            this.btnTesteImei.Size = new System.Drawing.Size(119, 45);
            this.btnTesteImei.TabIndex = 51;
            this.btnTesteImei.Text = "Teste IMEI";
            this.btnTesteImei.UseVisualStyleBackColor = false;
            this.btnTesteImei.Visible = false;
            this.btnTesteImei.Click += new System.EventHandler(this.btnTesteImei_Click);
            // 
            // lblOrderIdLabel
            // 
            this.lblOrderIdLabel.BackColor = System.Drawing.Color.MidnightBlue;
            this.lblOrderIdLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOrderIdLabel.ForeColor = System.Drawing.Color.White;
            this.lblOrderIdLabel.Location = new System.Drawing.Point(19, 263);
            this.lblOrderIdLabel.Name = "lblOrderIdLabel";
            this.lblOrderIdLabel.Size = new System.Drawing.Size(85, 28);
            this.lblOrderIdLabel.TabIndex = 40;
            this.lblOrderIdLabel.Text = "ORDER ID:";
            this.lblOrderIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOrderIdValor
            // 
            this.lblOrderIdValor.BackColor = System.Drawing.Color.White;
            this.lblOrderIdValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOrderIdValor.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblOrderIdValor.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lblOrderIdValor.Location = new System.Drawing.Point(107, 263);
            this.lblOrderIdValor.Name = "lblOrderIdValor";
            this.lblOrderIdValor.Size = new System.Drawing.Size(719, 28);
            this.lblOrderIdValor.TabIndex = 41;
            this.lblOrderIdValor.Text = "-";
            this.lblOrderIdValor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dataGridDados
            // 
            this.dataGridDados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridDados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Order_Id,
            this.Prod_Model,
            this.Mat_Id,
            this.Material_DESC,
            this.Status_Leitura});
            this.dataGridDados.Location = new System.Drawing.Point(21, 307);
            this.dataGridDados.Name = "dataGridDados";
            this.dataGridDados.RowHeadersWidth = 51;
            this.dataGridDados.RowTemplate.Height = 24;
            this.dataGridDados.Size = new System.Drawing.Size(1381, 360);
            this.dataGridDados.TabIndex = 36;
            // 
            // Order_Id
            // 
            this.Order_Id.HeaderText = "Order ID";
            this.Order_Id.MinimumWidth = 6;
            this.Order_Id.Name = "Order_Id";
            this.Order_Id.Width = 125;
            // 
            // Prod_Model
            // 
            this.Prod_Model.HeaderText = "Prod Model";
            this.Prod_Model.MinimumWidth = 6;
            this.Prod_Model.Name = "Prod_Model";
            this.Prod_Model.Width = 125;
            // 
            // Mat_Id
            // 
            this.Mat_Id.HeaderText = "Mat ID";
            this.Mat_Id.MinimumWidth = 6;
            this.Mat_Id.Name = "Mat_Id";
            this.Mat_Id.Width = 125;
            // 
            // Material_DESC
            // 
            this.Material_DESC.HeaderText = "Material DESC";
            this.Material_DESC.MinimumWidth = 6;
            this.Material_DESC.Name = "Material_DESC";
            this.Material_DESC.Width = 125;
            // 
            // Status_Leitura
            // 
            this.Status_Leitura.HeaderText = "Status Leitura";
            this.Status_Leitura.MinimumWidth = 6;
            this.Status_Leitura.Name = "Status_Leitura";
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.MidnightBlue;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.Control;
            this.label9.Location = new System.Drawing.Point(15, 164);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(1393, 14);
            this.label9.TabIndex = 26;
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1423, 794);
            this.Controls.Add(this.lblErroTcp);
            this.Controls.Add(this.lblOrderIdValor);
            this.Controls.Add(this.lblOrderIdLabel);
            this.Controls.Add(this.dataGridDados);
            this.Controls.Add(this.btnTesteImei);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ledCommSensorHikro1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ledCommKeyence2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ledCommKeyence1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ledCommCLP);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridDados)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label ledCommCLP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label ledCommKeyence1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label ledCommKeyence2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label ledCommSensorHikro1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnTesteImei;
        private System.Windows.Forms.Label lblOrderIdLabel;
        private System.Windows.Forms.Label lblOrderIdValor;
        private System.Windows.Forms.DataGridView dataGridDados;
        private System.Windows.Forms.DataGridViewTextBoxColumn Order_Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Prod_Model;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mat_Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn Material_DESC;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status_Leitura;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblErroTcp;
    }
}

