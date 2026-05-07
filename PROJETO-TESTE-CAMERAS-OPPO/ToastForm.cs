using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PROJETO_TESTE_CAMERAS_OPPO
{
    public enum ToastTipo { Running, Falha, ClpDesconectado, ImeiOk, AnatelOk, OEE, Esteira }

    public class ToastForm : Form
    {
        private Label  _lblMensagem;
        private Panel  _pnlIndicador;
        private Button _btnReset;

        // Success Rate layout
        private Label _lblOEETitulo;
        private Label _lblOEEValor;
        private Panel _pnlGradiente;
        private Panel _pnlOEEDetalhe;
        private Label _lblSucessos;
        private Label _lblFalhasOp;
        private Label _lblFalhasMaq;
        private float _successRate = -1f;

        // Esteira layout
        private Button _btnEsteira;

        private const int ToastHeight      = 52;
        private const int ToastWidthBase   = 340;
        private const int ToastWidthBotao  = 400;
        private const int OEEWidth         = 160;
        private const int OEEHeight        = 190;
        private const int EsteiraWidth     = 110;
        private const int EsteiraHeight    = 52;
        private const int Radius           = 8;

        public ToastForm(string mensagem, ToastTipo tipo = ToastTipo.Running, Action onReset = null, int bottomOffset = 20, int rightOffset = 20, Action onToggleEsteira = null, bool esteiraParada = false)
        {
            bool temBotao   = tipo == ToastTipo.Falha;
            bool isOEE      = tipo == ToastTipo.OEE;
            bool isEsteira  = tipo == ToastTipo.Esteira;

            int toastWidth  = isOEE     ? OEEWidth     :
                              isEsteira ? EsteiraWidth  :
                              temBotao  ? ToastWidthBotao : ToastWidthBase;
            int toastHeight = isOEE     ? OEEHeight    :
                              isEsteira ? EsteiraHeight :
                              ToastHeight;

            FormBorderStyle = FormBorderStyle.None;
            BackColor       = tipo == ToastTipo.Running   ? Color.FromArgb(28, 28, 28)  :
                              tipo == ToastTipo.ImeiOk    ? Color.FromArgb(15, 50, 35)  :
                              tipo == ToastTipo.AnatelOk  ? Color.FromArgb(10, 40, 65)  :
                              tipo == ToastTipo.OEE       ? Color.FromArgb(18, 28, 48)  :
                              tipo == ToastTipo.Esteira   ? Color.FromArgb(18, 28, 48)  :
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

            if (isEsteira)
            {
                _btnEsteira = new Button
                {
                    FlatStyle = FlatStyle.Flat,
                    Size      = new Size(toastWidth - 16, 34),
                    Location  = new Point(8, 9),
                    Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                    Cursor    = Cursors.Hand
                };
                _btnEsteira.FlatAppearance.BorderSize = 0;
                _btnEsteira.Region = RoundedRegion(toastWidth - 16, 34, 4);
                _btnEsteira.Click += (s, e) => onToggleEsteira?.Invoke();
                AtualizarBotaoEsteira(esteiraParada);
                Controls.Add(_btnEsteira);
                return;
            }

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
                    Text      = "SUCCESS RATE",
                    ForeColor = Color.FromArgb(130, 170, 215),
                    Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 30, 22),
                    Location  = new Point(28, 10),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Valor principal — termina em y=102, sem sobreposição com a barra abaixo
                _lblOEEValor = new Label
                {
                    Text      = "--",
                    ForeColor = Color.White,
                    Font      = new Font("Segoe UI", 32f, FontStyle.Bold),
                    AutoSize  = false,
                    Size      = new Size(toastWidth, 66),
                    Location  = new Point(0, 36),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                // Barra de gradiente vermelho→amarelo→verde (sempre visível)
                _pnlGradiente = new Panel
                {
                    Size      = new Size(toastWidth - 16, 10),
                    Location  = new Point(8, 108),
                    BackColor = Color.FromArgb(18, 28, 48)
                };
                _pnlGradiente.Paint += PintarGradienteSR;

                // Painel de detalhe — sempre visível
                _pnlOEEDetalhe = new Panel
                {
                    Size      = new Size(toastWidth, 68),
                    Location  = new Point(0, 122),
                    BackColor = Color.FromArgb(12, 20, 38),
                    Visible   = true
                };

                _lblSucessos = new Label
                {
                    Text      = "Sucessos: --",
                    ForeColor = Color.FromArgb(180, 210, 240),
                    Font      = new Font("Segoe UI", 8f),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 16, 18),
                    Location  = new Point(8, 5),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _lblFalhasOp = new Label
                {
                    Text      = "Falha Operacional: --",
                    ForeColor = Color.FromArgb(180, 210, 240),
                    Font      = new Font("Segoe UI", 8f),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 16, 18),
                    Location  = new Point(8, 26),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _lblFalhasMaq = new Label
                {
                    Text      = "Falha Máquina: --",
                    ForeColor = Color.FromArgb(180, 210, 240),
                    Font      = new Font("Segoe UI", 8f),
                    AutoSize  = false,
                    Size      = new Size(toastWidth - 16, 18),
                    Location  = new Point(8, 47),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                _pnlOEEDetalhe.Controls.Add(_lblSucessos);
                _pnlOEEDetalhe.Controls.Add(_lblFalhasOp);
                _pnlOEEDetalhe.Controls.Add(_lblFalhasMaq);

                Controls.Add(_pnlIndicador);
                Controls.Add(_lblOEETitulo);
                Controls.Add(_lblOEEValor);
                Controls.Add(_pnlGradiente);
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

        // Gradiente: vermelho(0%) → amarelo(70%) → verde(85%+)
        // Thresholds escolhidos para linha de produção: ≥85% excelente, 70-85% regular, <70% crítico
        private void PintarGradienteSR(object sender, PaintEventArgs e)
        {
            var ctrl = (Panel)sender;
            var g    = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = ctrl.Width, h = ctrl.Height;

            // Barra com cantos arredondados
            using (var path = new GraphicsPath())
            {
                int r = 4;
                path.AddArc(0,         0,         r * 2, r * 2, 180, 90);
                path.AddArc(w - r * 2, 0,         r * 2, r * 2, 270, 90);
                path.AddArc(w - r * 2, h - r * 2, r * 2, r * 2,   0, 90);
                path.AddArc(0,         h - r * 2, r * 2, r * 2,  90, 90);
                path.CloseFigure();

                using (var gb = new LinearGradientBrush(new Point(0, 0), new Point(w, 0), Color.Empty, Color.Empty))
                {
                    var blend = new ColorBlend(4);
                    blend.Colors    = new[] { Color.FromArgb(220, 60, 60), Color.FromArgb(240, 190, 50), Color.FromArgb(60, 200, 80), Color.FromArgb(60, 200, 80) };
                    blend.Positions = new[] { 0f, 0.70f, 0.85f, 1.0f };
                    gb.InterpolationColors = blend;
                    g.FillPath(gb, path);
                }
            }

            // Marcador branco na posição atual da taxa de sucesso
            if (_successRate >= 0f)
            {
                int x = (int)(_successRate * w);
                x = Math.Max(1, Math.Min(w - 1, x));
                using (var pen = new Pen(Color.White, 2f))
                    g.DrawLine(pen, x, 0, x, h);
            }
        }

        public void Mostrar()
        {
            Show();

            // Slide up
            var workArea = Screen.PrimaryScreen.WorkingArea;
            int targetY  = Top;
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

        public void AtualizarSuccessRate(string valor, int sucessos, int falhasOp, int falhasMaq, Color corValor)
        {
            if (IsDisposed) return;
            int total = sucessos + falhasOp + falhasMaq;
            _successRate = total > 0 ? (float)sucessos / total : -1f;
            if (_lblOEEValor  != null) { _lblOEEValor.Text = valor; _lblOEEValor.ForeColor = corValor; }
            if (_lblSucessos  != null) _lblSucessos.Text  = $"Sucessos: {sucessos}";
            if (_lblFalhasOp  != null) _lblFalhasOp.Text  = $"Falha Operacional: {falhasOp}";
            if (_lblFalhasMaq != null) _lblFalhasMaq.Text = $"Falha Máquina: {falhasMaq}";
            if (_pnlGradiente != null && !_pnlGradiente.IsDisposed) _pnlGradiente.Invalidate();
        }

        public void AtualizarBotaoEsteira(bool parada)
        {
            if (_btnEsteira == null) return;
            if (parada)
            {
                _btnEsteira.Text      = "START";
                _btnEsteira.BackColor = Color.FromArgb(30, 130, 60);
                _btnEsteira.ForeColor = Color.White;
            }
            else
            {
                _btnEsteira.Text      = "STOP";
                _btnEsteira.BackColor = Color.FromArgb(180, 50, 30);
                _btnEsteira.ForeColor = Color.White;
            }
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
