namespace NGINXConfigurator
{
    partial class NginxConfigForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.commonConfLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // commonConfLabel
            // 
            this.commonConfLabel.AutoSize = true;
            this.commonConfLabel.Location = new System.Drawing.Point(13, 13);
            this.commonConfLabel.Name = "commonConfLabel";
            this.commonConfLabel.Size = new System.Drawing.Size(118, 13);
            this.commonConfLabel.TabIndex = 0;
            this.commonConfLabel.Text = "Общая Конфигурация";
            // 
            // NginxConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 470);
            this.Controls.Add(this.commonConfLabel);
            this.Name = "NginxConfigForm";
            this.Text = "Конфигуратор HTTP-сервера NGINX";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label commonConfLabel;
    }
}

