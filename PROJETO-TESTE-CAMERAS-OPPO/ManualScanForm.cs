using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PROJETO_TESTE_CAMERAS_OPPO
{
    public class ManualScanForm : Form
    {
        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);
        [DllImport("user32.dll")] static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")] static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();

        private Label   _lblInstrucao;
        private Label   _lblDescricao;
        private TextBox _txtCodigo;
        private Label   _lblErro;
        private Button  _btnConfirmar;
        private Button  _btnPular;

        private readonly Func<string, bool> _validar;

        public string CodigoScaneado => _txtCodigo.Text.Trim();

        public ManualScanForm(string descricao, Func<string, bool> validar = null)
        {
            _validar = validar;

            Text            = "Leitura Manual";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            StartPosition   = FormStartPosition.CenterScreen;
            Size            = new Size(420, 220);
            TopMost         = true;

            _lblInstrucao = new Label
            {
                Text      = "Bipe o item abaixo:",
                Location  = new Point(20, 16),
                Size      = new Size(380, 18),
                Font      = new Font("Segoe UI", 9f)
            };

            _lblDescricao = new Label
            {
                Text      = descricao,
                Location  = new Point(20, 38),
                Size      = new Size(380, 26),
                Font      = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.OrangeRed
            };

            _txtCodigo = new TextBox
            {
                Location  = new Point(20, 72),
                Size      = new Size(375, 25),
                Font      = new Font("Segoe UI", 11f)
            };

            _lblErro = new Label
            {
                Text      = "Código inválido para este item. Tente novamente.",
                Location  = new Point(20, 103),
                Size      = new Size(375, 18),
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.Red,
                Visible   = false
            };

            _btnConfirmar = new Button
            {
                Text      = "Confirmar",
                Location  = new Point(215, 138),
                Size      = new Size(85, 30),
                Font      = new Font("Segoe UI", 9f),
                BackColor = Color.FromArgb(0, 120, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            _btnConfirmar.FlatAppearance.BorderSize = 0;

            _btnPular = new Button
            {
                Text      = "Pular",
                Location  = new Point(310, 138),
                Size      = new Size(80, 30),
                Font      = new Font("Segoe UI", 9f),
                BackColor = Color.FromArgb(180, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            _btnPular.FlatAppearance.BorderSize = 0;

            _txtCodigo.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    Confirmar();
                }
            };

            _btnConfirmar.Click += (s, e) => Confirmar();

            _btnPular.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            Controls.AddRange(new Control[] { _lblInstrucao, _lblDescricao, _txtCodigo, _lblErro, _btnConfirmar, _btnPular });

            Shown += (s, e) => ForcarFoco();
        }

        private void ForcarFoco()
        {
            IntPtr janelaAtual  = GetForegroundWindow();
            uint   threadAtual  = GetCurrentThreadId();
            uint   threadAlheia = GetWindowThreadProcessId(janelaAtual, IntPtr.Zero);

            AttachThreadInput(threadAlheia, threadAtual, true);
            SetForegroundWindow(Handle);
            AttachThreadInput(threadAlheia, threadAtual, false);

            _txtCodigo.Focus();
            _txtCodigo.Select();
        }

        private void Confirmar()
        {
            string codigo = _txtCodigo.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo))
                return;

            if (_validar != null && !_validar(codigo))
            {
                _lblErro.Visible = true;
                _txtCodigo.Clear();
                _txtCodigo.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
