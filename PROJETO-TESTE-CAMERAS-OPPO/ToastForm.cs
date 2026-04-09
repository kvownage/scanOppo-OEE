using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PROJETO_TESTE_CAMERAS_OPPO
{
    public enum ToastTipo { Running, Falha, ClpDesconectado, ImeiOk }

    public class ToastForm : Form
    {
        private Label  _lblMensagem;
        private Panel  _pnlIndicador;
        private Button _btnReset;

        private const int ToastHeight    = 52;
        private const int ToastWidthBase = 340;
        private const int ToastWidthBotao = 400;
        private const int Radius         = 8;

        public ToastForm(string mensagem, ToastTipo tipo = ToastTipo.Running, Action onReset = null, int bottomOffset = 20)
        {
            bool temBotao  = tipo == ToastTipo.Falha;
            int toastWidth = temBotao ? ToastWidthBotao : ToastWidthBase;

            FormBorderStyle = FormBorderStyle.None;
            BackColor       = tipo == ToastTipo.Running
                                ? Color.FromArgb(28, 28, 28)
                                : tipo == ToastTipo.ImeiOk
                                    ? Color.FromArgb(15, 50, 35)
                                    : Color.FromArgb(60, 20, 20);
            Size            = new Size(toastWidth, ToastHeight);
            ShowInTaskbar   = false;
            TopMost         = true;
            Opacity         = 0.93;
            StartPosition   = FormStartPosition.Manual;

            var workArea = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(workArea.Right - toastWidth - 20, workArea.Bottom - ToastHeight - bottomOffset);
            Region   = RoundedRegion(toastWidth, ToastHeight, Radius);

            Color corIndicador = (tipo == ToastTipo.Running || tipo == ToastTipo.ImeiOk)
                                    ? Color.LimeGreen
                                    : Color.OrangeRed;
            _pnlIndicador = new Panel
            {
                Size      = new Size(10, 10),
                BackColor = corIndicador,
                Location  = new Point(16, (ToastHeight - 10) / 2)
            };
            _pnlIndicador.Region = RoundedRegion(10, 10, 5);

            int labelWidth = temBotao ? toastWidth - 130 : toastWidth - 44;
            _lblMensagem = new Label
            {
                Text      = mensagem,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9f, FontStyle.Regular),
                AutoSize  = false,
                Size      = new Size(labelWidth, ToastHeight),
                Location  = new Point(34, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Controls.Add(_pnlIndicador);
            Controls.Add(_lblMensagem);

            if (temBotao)
            {
                _btnReset = new Button
                {
                    Text      = "Reset",
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(180, 40, 40),
                    FlatStyle = FlatStyle.Flat,
                    Size      = new Size(70, 30),
                    Location  = new Point(toastWidth - 86, (ToastHeight - 30) / 2),
                    Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Cursor    = Cursors.Hand
                };
                _btnReset.FlatAppearance.BorderColor = Color.FromArgb(220, 60, 60);
                _btnReset.Region = RoundedRegion(70, 30, 4);
                _btnReset.Click += (s, e) => onReset?.Invoke();
                Controls.Add(_btnReset);
            }
        }

        public void Mostrar()
        {
            Show();

            // Slide up
            var workArea = Screen.PrimaryScreen.WorkingArea;
            int targetY  = Top; // already set by constructor with bottomOffset
            Top          = workArea.Bottom + 10;
            Opacity      = 0.93;

            var timerSlide = new System.Windows.Forms.Timer { Interval = 10 };
            timerSlide.Tick += (s, e) =>
            {
                if (Top > targetY)
                    Top -= 5;
                else
                {
                    Top = targetY;
                    timerSlide.Stop();
                    timerSlide.Dispose();
                }
            };
            timerSlide.Start();
        }

        public void MostrarEFechar(int duracaoMs)
        {
            Mostrar();

            var timerAuto = new System.Windows.Forms.Timer { Interval = duracaoMs };
            timerAuto.Tick += (s, e) =>
            {
                timerAuto.Stop();
                timerAuto.Dispose();
                FecharImediato();
            };
            timerAuto.Start();
        }

        public void AtualizarMensagem(string mensagem)
        {
            if (!IsDisposed && _lblMensagem != null)
                _lblMensagem.Text = mensagem;
        }

        public void FecharImediato()
        {
            if (IsDisposed) return;

            var fadeTimer = new System.Windows.Forms.Timer { Interval = 20 };
            fadeTimer.Tick += (s, ev) =>
            {
                if (IsDisposed) { ((System.Windows.Forms.Timer)s).Stop(); return; }
                if (Opacity > 0.05)
                    Opacity -= 0.07;
                else
                {
                    ((System.Windows.Forms.Timer)s).Stop();
                    ((System.Windows.Forms.Timer)s).Dispose();
                    if (!IsDisposed) Close();
                }
            };
            fadeTimer.Start();
        }

        private static Region RoundedRegion(int width, int height, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            path.AddArc(width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            path.AddArc(width - radius * 2, height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(0, height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }
    }
}
