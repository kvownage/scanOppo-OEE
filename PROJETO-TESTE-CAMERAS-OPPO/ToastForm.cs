using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PROJETO_TESTE_CAMERAS_OPPO
{
    public enum ToastTipo { Running, Falha, ClpDesconectado, ImeiOk, AnatelOk, OEE }

    public class ToastForm : Form
    {
        private Label  _lblMensagem;
        private Panel  _pnlIndicador;
        private Button _btnReset;

        // OEE layout
        private Label _lblOEETitulo;
        private Label _lblOEEValor;
        private Panel _pnlOEEDetalhe;
        private Label _lblOEEQual;
        private Label _lblOEEDisp;
        private Label _lblOEEPerf;

        private const int ToastHeight     = 52;
        private const int ToastWidthBase  = 340;
        private const int ToastWidthBotao = 400;
        private const int OEEWidth        = 160;
        private const int OEEHeight       = 172;
        private const int Radius          = 8;

        public ToastForm(string mensagem, ToastTipo tipo = ToastTipo.Running, Action onReset = null, int bottomOffset = 20, int rightOffset = 20)
        {
            bool temBotao   = tipo == ToastTipo.Falha;
            bool isOEE      = tipo == ToastTipo.OEE;
            int toastWidth  = isOEE ? OEEWidth  : (temBotao ? ToastWidthBotao : ToastWidthBase);
            int toastHeight = isOEE ? OEEHeight : ToastHeight;

            FormBorderStyle = FormBorderStyle.None;
            BackColor       = tipo == ToastTipo.Running   ? Color.FromArgb(28, 28, 28) :
                              tipo == ToastTipo.ImeiOk    ? Color.FromArgb(15, 50, 35) :
                              tipo == ToastTipo.AnatelOk  ? Color.FromArgb(10, 40, 65) :
                              tipo == ToastTipo.OEE       ? Color.FromArgb(18, 28, 48) :
                                                            Color.FromArgb(60, 20, 20);
            Size          = new Size(toastWidth, toastHeight);
            ShowInTaskbar = false;
            TopMost       = true;
            Opacity       = 0.93;
            StartPosition = FormStartPosition.Manual;

            var workArea = Screen.PrimaryScreen.WorkingArea;
            Location = new Point(workArea.Right - toastWidth - rightOffset, workArea.Bottom - toastHeight - bottomOffset);
            Region   = RoundedRegion(toastWidth, toastHeight, Radius);

            Color corIndicador = tipo == ToastTipo.AnatelOk ? Color.DeepSkyBlue :
                                 tipo == ToastTipo.OEE      ? Color.DodgerBlue  :
                                 (tipo == ToastTipo.Running || tipo == ToastTipo.ImeiOk)
                                                             ? Color.LimeGreen
                                                             : Color.OrangeRed;

            if (isOEE)
            {
                _pnlIndicador = new Panel
                {
                    Size      = new Size(8, 8),
                    BackColor = corIndicador,
                    Location  = new Point(12, 16)
                };
                _pnlIndicador.Region = RoundedRegion(8, 8, 4);

                _lblOEETitulo = new Label
                {
                    Text      = "OEE",
                    ForeColor = Color.FromArgb(130, 170, 215),
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 30, 22),
                    Location  = new Point(28, 10),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                _lblOEEValor = new Label
                {
                    Text      = "--",
                    ForeColor = Color.White,
                    Font      = new Font("Segoe UI", 32f, FontStyle.Bold),
                    AutoSize  = false,
                    Size      = new Size(toastWidth, 90),
                    Location  = new Point(0, 36),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // Painel de detalhe — visível só no hover
                _pnlOEEDetalhe = new Panel
                {
                    Size      = new Size(toastWidth, 70),
                    Location  = new Point(0, 102),
                    BackColor = Color.FromArgb(12, 20, 38),
                    Visible   = false
                };

                _lblOEEQual = new Label
                {
                    Text      = "Qualidade:       --",
                    ForeColor = Color.FromArgb(180, 210, 240),
                    Font      = new Font("Segoe UI", 8f),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 16, 20),
                    Location  = new Point(8, 6),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _lblOEEDisp = new Label
                {
                    Text      = "Disponibilidade: --",
                    ForeColor = Color.FromArgb(180, 210, 240),
                    Font      = new Font("Segoe UI", 8f),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 16, 20),
                    Location  = new Point(8, 26),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _lblOEEPerf = new Label
                {
                    Text      = "Performance:     --",
                    ForeColor = Color.FromArgb(180, 210, 240),
                    Font      = new Font("Segoe UI", 8f),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 16, 20),
                    Location  = new Point(8, 46),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                _pnlOEEDetalhe.Controls.Add(_lblOEEQual);
                _pnlOEEDetalhe.Controls.Add(_lblOEEDisp);
                _pnlOEEDetalhe.Controls.Add(_lblOEEPerf);

                // Hover nos controles e no painel
                Action mostrarDetalhe = () => { if (!IsDisposed) _pnlOEEDetalhe.Visible = true; };
                Action ocultarDetalhe = () => { if (!IsDisposed) _pnlOEEDetalhe.Visible = false; };

                foreach (Control c in new Control[] { this, _lblOEETitulo, _lblOEEValor, _pnlOEEDetalhe, _lblOEEQual, _lblOEEDisp, _lblOEEPerf, _pnlIndicador })
                {
                    c.MouseEnter += (s, e) => mostrarDetalhe();
                    c.MouseLeave += (s, e) => ocultarDetalhe();
                }

                Controls.Add(_pnlIndicador);
                Controls.Add(_lblOEETitulo);
                Controls.Add(_lblOEEValor);
                Controls.Add(_pnlOEEDetalhe);
                return;
            }

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

        public void AtualizarOEE(string valorOEE, string qual, string disp, string perf, Color corValor)
        {
            if (IsDisposed) return;
            if (_lblOEEValor != null) { _lblOEEValor.Text = valorOEE; _lblOEEValor.ForeColor = corValor; }
            if (_lblOEEQual  != null) _lblOEEQual.Text  = $"Qualidade:       {qual}";
            if (_lblOEEDisp  != null) _lblOEEDisp.Text  = $"Disponibilidade: {disp}";
            if (_lblOEEPerf  != null) _lblOEEPerf.Text  = $"Performance:     {perf}";
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
