using PROJETO_TESTE_CAMERAS_OPPO.Services;
using PROJETO_TESTE_CAMERAS_OPPO.Variaveis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.Json;
using System.IO;

namespace PROJETO_TESTE_CAMERAS_OPPO
{
    public partial class Form1 : Form
    {
        public CameraService _leitorKeyence1, _leitorKeyence2, _sensorHikro1, _sensorHikro2, _tcpServer5000;
        private bool _startScan = false;
        private bool isProd;
        private bool _aguardandoReset = false;

        private bool _sensorHikro1Enabled, _sensorHikro2Enabled, _leitorKeyence1Enabled, _tcpServer5000Enabled;
        private int  _sensorHikro1Port,    _sensorHikro2Port,    _leitorKeyence1Port,    _tcpServer5000Port;
        private int  _sensorHikro1Reg,     _sensorHikro2Reg,     _leitorKeyence1Reg,     _tcpServer5000Reg;
        private string _configLinha;
        private string _configEstacao;
        private string _configUsuario;
        private string _configSenha;
        public string prefixImei;
        public string Imei = "";
        public List<string> codigosLidos = new List<string>();
        public ClpService _clpService = new ClpService();
        int[] registradores;

        private System.Windows.Forms.Timer _timerBlink;
        private bool _blinkState = false;

        IWebDriver driver;
        public Form1()
        {
            InitializeComponent();
            _sensorHikro1 = new CameraService();
            _sensorHikro2 = new CameraService();
            _leitorKeyence1 = new CameraService();
            _tcpServer5000 = new CameraService();

            _sensorHikro1.OnError   += () => SinalizarErroTcp("Falha de leitura: Sensor Hikro 1");
            _sensorHikro2.OnError   += () => SinalizarErroTcp("Falha de leitura: Sensor Hikro 2");
            _leitorKeyence1.OnError += () => SinalizarErroTcp("Falha de leitura: Leitor Keyence 1");
            _tcpServer5000.OnError  += () => SinalizarErroTcp("Falha de leitura: Servidor TCP 5000");

            CarregarConfiguracoes();

            _timerBlink = new System.Windows.Forms.Timer();
            _timerBlink.Interval = 400;
            _timerBlink.Tick += (s, ev) =>
            {
                _blinkState = !_blinkState;
                btnReset.BackColor = _blinkState ? Color.Yellow : Color.Orange;
                btnReset.ForeColor = Color.Black;
                btnReset.UseVisualStyleBackColor = false;
            };
        }

        private void CarregarConfiguracoes()
        {
            string caminhoLinha = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "linha.json");

            if (!File.Exists(caminhoLinha))
                throw new FileNotFoundException("Arquivo de configuração não encontrado: linha.json");

            using (var doc = JsonDocument.Parse(File.ReadAllText(caminhoLinha)))
            {
                var root = doc.RootElement;

                _configLinha   = root.GetProperty("linha").GetString();
                _configEstacao = root.GetProperty("estacao").GetString();
                _configUsuario = root.GetProperty("usuario").GetString();
                _configSenha   = root.GetProperty("senha").GetString();
                isProd         = root.GetProperty("isProd").GetBoolean();
            }

            string caminhoPrefixo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "prefixo.json");

            if (!File.Exists(caminhoPrefixo))
                throw new FileNotFoundException("Arquivo de configuração não encontrado: prefixo.json");

            using (var doc = JsonDocument.Parse(File.ReadAllText(caminhoPrefixo)))
            {
                prefixImei = doc.RootElement.GetProperty("prefixImei").GetString();
            }

            string caminhoServers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "servers.json");

            if (!File.Exists(caminhoServers))
                throw new FileNotFoundException("Arquivo de configuração não encontrado: servers.json");

            using (var doc = JsonDocument.Parse(File.ReadAllText(caminhoServers)))
            {
                var servers = doc.RootElement.GetProperty("servers");

                var s1 = servers.GetProperty("sensorHikro1");
                _sensorHikro1Enabled = s1.GetProperty("enabled").GetBoolean();
                _sensorHikro1Port    = s1.GetProperty("porta").GetInt32();
                _sensorHikro1Reg     = s1.GetProperty("registrador").GetInt32();

                var s2 = servers.GetProperty("sensorHikro2");
                _sensorHikro2Enabled = s2.GetProperty("enabled").GetBoolean();
                _sensorHikro2Port    = s2.GetProperty("porta").GetInt32();
                _sensorHikro2Reg     = s2.GetProperty("registrador").GetInt32();

                var k1 = servers.GetProperty("leitorKeyence1");
                _leitorKeyence1Enabled = k1.GetProperty("enabled").GetBoolean();
                _leitorKeyence1Port    = k1.GetProperty("porta").GetInt32();
                _leitorKeyence1Reg     = k1.GetProperty("registrador").GetInt32();

                var t1 = servers.GetProperty("tcpServer5000");
                _tcpServer5000Enabled = t1.GetProperty("enabled").GetBoolean();
                _tcpServer5000Port    = t1.GetProperty("porta").GetInt32();
                _tcpServer5000Reg     = t1.GetProperty("registrador").GetInt32();
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            _startScan = true;

            this.ShowInTaskbar = true;
            this.WindowState   = FormWindowState.Minimized;

            MostrarToastRunning();

            if (!isProd)
            {
                IniciarComunicacoes();
                ScanCycle();
            }

            try
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--start-maximized");

                var service = ChromeDriverService.CreateDefaultService(AppDomain.CurrentDomain.BaseDirectory);
                driver = new ChromeDriver(service, options);
                driver.Navigate().GoToUrl("http://172.29.185.215/asymes/opc?_ver=V2026R01-20251217-OB&_mc=faa99ce4#");

                FazerLogin(_configUsuario, _configSenha);
                AbrirMenuAWIP();
                SelecionarOperador(_configUsuario);
                SelecionarProcesso(_configEstacao);
                SelecionarPrimeiraOrdem();
                MonitorarCampoOrderId();
                MonitorarCampoStatus();

                if (!isProd)
                {
                    await IniciarTcpServers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Não foi possível conectar ao sistema.\nVerifique se está conectado à rede correta.\n\nDetalhe: {ex.Message}",
                    "Erro de conexão",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
        }

        private void IniciarComunicacoes()
        {
            try
            {
                _clpService.Conectar("192.168.250.1", 502);
                ledCommCLP.BackColor = Color.Green;
            }
            catch
            {
                ledCommCLP.BackColor = Color.Red;
            }

            // Sinaliza ao CLP que a estação está online
            try { _clpService.EscreverRegistro(21, 1); } catch { }

            // Verifica se havia falha pendente antes de iniciar
            var regs = _clpService.LerRegistradores(20, 1);
            if (regs != null && regs[0] == 1)
            {
                lblErroTcp.BackColor = Color.Red;
                lblErroTcp.Text = "Falha na leitura dos codigos";
                lblErroTcp.Visible = true;
                IniciarPiscadaBtnReset();

                // Substitui o toast Running pelo toast de Falha com botão Reset
                if (_toastRunning != null && !_toastRunning.IsDisposed)
                    _toastRunning.FecharImediato();

                if (_toastFalha == null || _toastFalha.IsDisposed)
                {
                    _toastFalha = new ToastForm("Falha na leitura dos codigos", ToastTipo.Falha,
                        () => this.Invoke((Action)ExecutarReset));
                    _toastFalha.Mostrar();
                }
            }

            //_clpService.EscreverRegistro(4, 0);
        }

        private Task IniciarTcpServers()
        {
            var tasks = new List<Task>();

            if (_sensorHikro1Enabled)   tasks.Add(_sensorHikro1.IniciarTcpServer(_sensorHikro1Port));
            if (_sensorHikro2Enabled)   tasks.Add(_sensorHikro2.IniciarTcpServer(_sensorHikro2Port));
            if (_leitorKeyence1Enabled) tasks.Add(_leitorKeyence1.IniciarTcpServer(_leitorKeyence1Port));
            if (_tcpServer5000Enabled)  tasks.Add(_tcpServer5000.IniciarTcpServer(_tcpServer5000Port));

            return tasks.Count > 0 ? Task.WhenAll(tasks) : Task.CompletedTask;
        }

        private void btnTesteImei_Click(object sender, EventArgs e)
        {
            var rng = new Random();
            var prefixosInvalidos = new[] { "86", "87", "88", "89" };
            string prefixo;
            do { prefixo = rng.Next(10, 99).ToString(); }
            while (Array.Exists(prefixosInvalidos, p => prefixo.StartsWith(p)));

            string digitos = string.Concat(Enumerable.Range(0, 13).Select(_ => rng.Next(0, 10).ToString()));

            string imeiOriginal = Imei;
            Imei = prefixo + digitos;

            EnviarParaPaginaWeb();

            Imei = imeiOriginal;
        }

        private void btnReset_Click(object sender, EventArgs e) => ExecutarReset();

        private void ExecutarReset()
        {
            _aguardandoReset = false;
            _lastImeiNotificado = null;
            PararPiscadaBtnReset();
            Imei = "";
            codigosLidos.Clear();

            LimparStatusLeitura();
            if (VarGlobal.LeiturasTCP != null)
                lock (VarGlobal.LeiturasTCP) { VarGlobal.LeiturasTCP.Clear(); }

            LimparCamposWebReset();

            _clpService.EscreverRegistro(20, 0);
            lblErroTcp.BackColor = Color.Green;
            lblErroTcp.Text = "Reset realizado, faça a leitura novamente";
            lblErroTcp.Visible = true;

            MostrarToastRunning();
        }

        private void LimparCamposWebReset()
        {
            try
            {
                var campoImei = driver?.FindElement(By.CssSelector("input[name='imei']"));
                if (campoImei != null && !string.IsNullOrEmpty(campoImei.GetAttribute("value")))
                    campoImei.Clear();

                var campoAttachment = driver?.FindElement(By.CssSelector("input[name='attachmentCode']"));
                if (campoAttachment != null && !string.IsNullOrEmpty(campoAttachment.GetAttribute("value")))
                    campoAttachment.Clear();
            }
            catch { }
        }

        public async Task ScanCycle()
        {
            int tentativasCLP = 0;

            while (_startScan)
            {
                registradores = _clpService.LerRegistradores(0, 100);

                if (registradores == null)
                {
                    tentativasCLP++;
                    this.Invoke((Action)(() =>
                    {
                        ledCommCLP.BackColor = Color.Red;

                        string msg = $"Falha de comunicação com CLP, Reconectando {tentativasCLP}";

                        if (_toastFalha == null || _toastFalha.IsDisposed)
                        {
                            if (_toastRunning != null && !_toastRunning.IsDisposed)
                                _toastRunning.FecharImediato();

                            _toastFalha = new ToastForm(msg, ToastTipo.Falha);
                            _toastFalha.Mostrar();
                        }
                        else
                        {
                            _toastFalha.AtualizarMensagem(msg);
                        }
                    }));

                    await Task.Delay(3000);
                    bool reconectou = _clpService.TentarReconectar();

                    this.Invoke((Action)(() =>
                    {
                        ledCommCLP.BackColor = reconectou ? Color.Green : Color.Red;
                        if (reconectou)
                        {
                            tentativasCLP = 0;
                            MostrarToastRunning();
                        }
                    }));
                    continue;
                }

                ledCommCLP.BackColor = Color.Green;

                if (VarGlobal.LeiturasTCP != null && VarGlobal.LeiturasTCP.Count > 0)
                {
                    lock (VarGlobal.LeiturasTCP)
                    {
                        foreach (var item in VarGlobal.LeiturasTCP)
                        {
                            // Valida se é IMEI (começa com o prefixo)
                            if (!string.IsNullOrEmpty(prefixImei) && item.StartsWith(prefixImei))
                            {
                                Imei = item;
                                MostrarToastImei(Imei);
                            }

                            // Valida se contém algum Mat_Id da tabela carregada pelo Selenium
                            bool jaEmCodigosLidos = codigosLidos.Contains(item);
                            if (!jaEmCodigosLidos)
                            {
                                foreach (DataGridViewRow row in dataGridDados.Rows)
                                {
                                    if (row.IsNewRow) continue;
                                    string matId = row.Cells["Mat_Id"].Value?.ToString()?.Trim();
                                    if (string.IsNullOrEmpty(matId)) continue;

                                    bool bateu;
                                    if (matId == "LAST-6")
                                    {
                                        string ultimos6 = Imei?.Length >= 6 ? Imei.Substring(Imei.Length - 6) : null;
                                        bateu = !string.IsNullOrEmpty(ultimos6) && item.Contains(ultimos6);
                                    }
                                    else
                                    {
                                        bateu = item.Contains(matId);
                                    }

                                    if (bateu)
                                    {
                                        codigosLidos.Add(item);
                                        row.Cells["Status_Leitura"].Value = "OK";
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Verifica se todos os Mat_Id foram lidos
                    int totalMatIds = dataGridDados.Rows.Cast<DataGridViewRow>()
                        .Count(r => !r.IsNewRow && r.Cells["Mat_Id"].Value != null);

                    if (!_aguardandoReset && totalMatIds > 0 && codigosLidos.Count >= totalMatIds && !string.IsNullOrEmpty(Imei))
                    {
                        _aguardandoReset = true;

                        var linhasSemOk = dataGridDados.Rows.Cast<DataGridViewRow>()
                            .Where(r => !r.IsNewRow && r.Cells["Mat_Id"].Value != null &&
                                        r.Cells["Status_Leitura"].Value?.ToString() != "OK")
                            .ToList();

                        if (linhasSemOk.Count > 0)
                        {
                            foreach (var row in linhasSemOk)
                                row.Cells["Status_Leitura"].Value = "FALHA";

                            string matIdsFaltando = string.Join(", ", linhasSemOk.Select(r => r.Cells["Mat_Id"].Value?.ToString()));
                            SinalizarErroTcp($"Falha: Mat_Id(s) sem leitura confirmada: {matIdsFaltando}");
                        }
                        else
                        {
                            EnviarParaPaginaWeb();
                        }
                    }
                }

                
                atualizaUI();
                await Task.Delay(100);
            }
        }

        private void SelecionarOperador(string operador)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Clica no botão trigger do campo codeview
            IWebElement trigger = wait.Until(d =>
                d.FindElements(By.CssSelector(".ux-codeviewx-trigger"))
                 .FirstOrDefault(e => e.Displayed)
            );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", trigger);

            // Aguarda a grid aparecer e seleciona a linha que contém o operador (linha/usuario)
            IWebElement linha = wait.Until(d =>
                d.FindElements(By.CssSelector("tr.x-grid-row"))
                 .FirstOrDefault(row =>
                 {
                     var celulas = row.FindElements(By.CssSelector("div.x-grid-cell-inner"));
                     return celulas.Count > 0 && celulas[0].Text.Trim().Contains(operador);
                 })
            );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", linha);
        }

        private void MonitorarCampoStatus()
        {
            string mensagemAnterior = "";

            Task.Run(async () =>
            {
                while (_startScan)
                {
                    try
                    {
                        string mensagemAtual = (string)((IJavaScriptExecutor)driver)
                            .ExecuteScript(@"
                                var header = Array.from(document.querySelectorAll('.x-fieldset-header-text'))
                                    .find(el => el.textContent.trim() === 'Scan History');
                                return header
                                    ?.closest('fieldset')
                                    ?.querySelector('.x-box-item')
                                    ?.innerText?.trim() ?? '';
                            ");

                        if (!string.IsNullOrEmpty(mensagemAtual) && mensagemAtual != mensagemAnterior)
                        {
                            mensagemAnterior = mensagemAtual;
                            EscreverLog("SCAN HISTORY", mensagemAtual);
                        }
                    }
                    catch { }

                    await Task.Delay(500);
                }
            });
        }

        private void MonitorarCampoOrderId()
        {
            string valorAnterior = "";

            Task.Run(async () =>
            {
                while (_startScan)
                {
                    try
                    {
                        string valorAtual = (string)((IJavaScriptExecutor)driver)
                            .ExecuteScript("return document.querySelector('input[name=\"orderId\"]')?.value ?? '';");

                        if (valorAtual != valorAnterior)
                        {
                            valorAnterior = valorAtual;
                            // Selenium e lógica pesada ficam na thread de background
                            AoMudarOrderIdBackground(valorAtual);
                        }
                    }
                    catch
                    {
                        // Se o driver não responde, o navegador foi fechado
                        try { var _ = driver.Title; }
                        catch
                        {
                            this.Invoke((Action)(() => this.Close()));
                            return;
                        }
                    }

                    await Task.Delay(300);
                }
            });
        }

        private bool _comunicacoesIniciadas = false;
        private ToastForm _toastFalha;
        private ToastForm _toastRunning;
        private ToastForm _toastImei;
        private string _lastImeiNotificado = null;
        private string _currentOrderId = "";
        private string _caminhoLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

        private void MostrarToastRunning()
        {
            if (_toastFalha != null && !_toastFalha.IsDisposed)
                _toastFalha.FecharImediato();

            if (_toastRunning != null && !_toastRunning.IsDisposed)
                _toastRunning.FecharImediato();

            _toastRunning = new ToastForm("Sistema de Scan Ativo", ToastTipo.Running);
            _toastRunning.Mostrar();
        }

        private void MostrarToastImei(string imei)
        {
            if (imei == _lastImeiNotificado) return;
            _lastImeiNotificado = imei;

            if (_toastImei != null && !_toastImei.IsDisposed)
                _toastImei.FecharImediato();

            // Posiciona acima do toast Running (52px altura + 20 offset + 8 margem = 80)
            _toastImei = new ToastForm($"IMEI: {imei} APONTADO!", ToastTipo.ImeiOk, bottomOffset: 80);
            _toastImei.MostrarEFechar(2500);
        }

        private void EscreverLog(string tipo, string mensagem)
        {
            string linha = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | [{tipo}] | Order: {(string.IsNullOrEmpty(_currentOrderId) ? "-" : _currentOrderId)} | {mensagem}";
            File.AppendAllText(_caminhoLog, linha + Environment.NewLine, Encoding.UTF8);
        }

        private void AoMudarOrderIdBackground(string novoValor)
        {
            _currentOrderId = string.IsNullOrWhiteSpace(novoValor) ? "" : novoValor;

            // Atualiza label na thread de UI
            this.Invoke((Action)(() =>
                lblOrderIdValor.Text = string.IsNullOrWhiteSpace(novoValor) ? "-" : novoValor
            ));

            if (!string.IsNullOrWhiteSpace(novoValor))
            {
                // WebDriverWait roda aqui, na thread de background, sem bloquear a UI
                CarregarTabelaAttachmentMaterial();

                if (isProd && !_comunicacoesIniciadas)
                {
                    _comunicacoesIniciadas = true;
                    this.Invoke((Action)(() =>
                    {
                        IniciarComunicacoes();
                        AtualizarRegistradoresServers(true);
                        ScanCycle();
                    }));
                    _ = IniciarTcpServers();
                }
                else if (!isProd)
                {
                    AtualizarRegistradoresServers(true);
                }
            }
            else
            {
                AtualizarRegistradoresServers(false);
                this.Invoke((Action)(() => dataGridDados.Rows.Clear()));
            }
        }

        private void SinalizarErroTcp(string mensagem)
        {
            EscreverLog("FALHA", $"IMEI: {(string.IsNullOrEmpty(Imei) ? "-" : Imei)} | {mensagem}");
            this.Invoke((Action)(() =>
            {
                lblErroTcp.BackColor = Color.Red;
                lblErroTcp.Text = mensagem;
                lblErroTcp.Visible = true;
                IniciarPiscadaBtnReset();

                if (_toastImei != null && !_toastImei.IsDisposed)
                    _toastImei.FecharImediato();

                if (_toastFalha != null && !_toastFalha.IsDisposed)
                    _toastFalha.FecharImediato();

                if (_toastRunning != null && !_toastRunning.IsDisposed)
                    _toastRunning.FecharImediato();

                _toastFalha = new ToastForm(mensagem, ToastTipo.Falha, () => this.Invoke((Action)ExecutarReset));
                _toastFalha.Mostrar();
            }));
            _clpService.EscreverRegistro(20, 1);
        }

        private void IniciarPiscadaBtnReset()
        {
            if (!_timerBlink.Enabled)
            {
                _blinkState = false;
                _timerBlink.Start();
            }
        }

        private void PararPiscadaBtnReset()
        {
            _timerBlink.Stop();
            btnReset.BackColor = SystemColors.Control;
            btnReset.ForeColor = SystemColors.ControlText;
            btnReset.UseVisualStyleBackColor = true;
        }

        private void LimparStatusLeitura()
        {
            foreach (DataGridViewRow row in dataGridDados.Rows)
            {
                if (!row.IsNewRow)
                    row.Cells["Status_Leitura"].Value = null;
            }
        }

        private void AtualizarRegistradoresServers(bool ordemSelecionada)
        {
            int valor = ordemSelecionada ? 1 : 0;

            if (_sensorHikro1Enabled)   _clpService.EscreverRegistro(_sensorHikro1Reg,   valor);
            if (_sensorHikro2Enabled)   _clpService.EscreverRegistro(_sensorHikro2Reg,   valor);
            if (_leitorKeyence1Enabled) _clpService.EscreverRegistro(_leitorKeyence1Reg, valor);
            if (_tcpServer5000Enabled)  _clpService.EscreverRegistro(_tcpServer5000Reg,  valor);
        }

        private void CarregarTabelaAttachmentMaterial()
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                // Aguarda pelo menos uma linha aparecer dentro do grid correto
                wait.Until(d =>
                    d.FindElements(By.XPath("//div[contains(@class,'x-fieldset-header-text') and contains(text(),'Attachment Material Scan List')]/ancestor::fieldset//tr[contains(@class,'x-grid-row')]")).Count > 0
                );

                var linhas = driver.FindElements(By.XPath("//div[contains(@class,'x-fieldset-header-text') and contains(text(),'Attachment Material Scan List')]/ancestor::fieldset//tr[contains(@class,'x-grid-row')]"));

                this.Invoke((Action)(() => dataGridDados.Rows.Clear()));

                var linhasData = new List<string[]>();
                foreach (var linha in linhas)
                {
                    var celulas = linha.FindElements(By.CssSelector("div.x-grid-cell-inner"));
                    if (celulas.Count < 4) continue;

                    linhasData.Add(new[]
                    {
                        celulas[0].Text.Trim(),
                        celulas[1].Text.Trim(),
                        celulas[2].Text.Trim(),
                        celulas[3].Text.Trim()
                    });
                }

                // Atualiza o grid na thread de UI de uma vez só
                this.Invoke((Action)(() =>
                {
                    foreach (var row in linhasData)
                        dataGridDados.Rows.Add(row[0], row[1], row[2], row[3]);
                }));
            }
            catch { /* grid ainda não carregou */ }
        }

        private void SelecionarProcesso(string processo)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Pega todos os triggers visíveis e clica no segundo (campo de processo)
            IWebElement trigger = wait.Until(d =>
            {
                var triggers = d.FindElements(By.CssSelector(".ux-codeviewx-trigger"))
                                .Where(e => e.Displayed)
                                .ToList();
                return triggers.Count >= 2 ? triggers[1] : null;
            });
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", trigger);

            // Aguarda a grid aparecer e seleciona a linha que contém o processo
            IWebElement linha = wait.Until(d =>
                d.FindElements(By.CssSelector("tr.x-grid-row"))
                 .FirstOrDefault(row =>
                 {
                     var celulas = row.FindElements(By.CssSelector("div.x-grid-cell-inner"));
                     return celulas.Count > 0 && celulas[0].Text.Trim().Contains(processo);
                 })
            );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", linha);
        }

        private void AbrirMenuAWIP()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Aguarda a tela principal carregar (botão hamburguer visível)
            IWebElement btnMenu = wait.Until(d =>
                d.FindElements(By.CssSelector("button[data-qtip='Menu']"))
                 .FirstOrDefault(e => e.Displayed)
            );

            // Clica via JavaScript (mais confiável com ExtJS)
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", btnMenu);

            // Aguarda o item de menu com texto contendo "AWIP1097" aparecer
            IWebElement menuItem = wait.Until(d =>
                d.FindElements(By.CssSelector("div.menuItem"))
                 .FirstOrDefault(e => e.Text.Contains("AWIP1097"))
            );

            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", menuItem);
        }

        private void FazerLogin(string usuario, string senha)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            wait.Until(d => d.FindElement(By.Id("j_username")).Displayed);

            var js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("document.getElementById('j_factory').value = 'CAASY01';");
            js.ExecuteScript("document.getElementById('j_language').value = 'en';");

            driver.FindElement(By.Id("j_username")).Clear();
            driver.FindElement(By.Id("j_username")).SendKeys(usuario);

            driver.FindElement(By.Id("j_password")).Clear();
            driver.FindElement(By.Id("j_password")).SendKeys(senha);

            driver.FindElement(By.CssSelector("input.btnWelcomeLogin")).Click();
        }

        private void EnviarNoSite(IWebDriver driver, string imei, List<string> attachmentCodes)
        {
            if (!string.IsNullOrEmpty(imei))
            {
                IWebElement txtIMEI = driver.FindElement(By.Name("imei"));
                txtIMEI.Clear();
                txtIMEI.SendKeys(imei);
                txtIMEI.SendKeys(OpenQA.Selenium.Keys.Enter);
            }

            IWebElement txtAttachmentCode = driver.FindElement(By.Name("attachmentCode"));

            foreach (var code in attachmentCodes)
            {
                txtAttachmentCode.Clear();
                txtAttachmentCode.SendKeys(code);
                txtAttachmentCode.SendKeys(OpenQA.Selenium.Keys.Enter);
            }

            VarGlobal.LeiturasTCP.Clear();
        }

        private void btnGetInformacoes_Click(object sender, EventArgs e)
        {
           /* VarGlobal.MatIds.Clear();
            EspelharGridNoDataGrid(driver, dataGridDados);

            foreach (DataGridViewRow row in dataGridDados.Rows)
            {
                if (row.IsNewRow) continue;

                var mat = row.Cells["Mat_Id"].Value?.ToString()?.Trim();

                if (!string.IsNullOrEmpty(mat))
                    VarGlobal.MatIds.Add(mat);
            }

            VarGlobal.MatIds = VarGlobal.MatIds.Distinct().ToList();*/
            _clpService.EscreverRegistro(4, 1);
        }

        public List<string> FiltrarLeituras(List<string> leiturasTCP, List<string> matIds)
        {
            return leiturasTCP
                .Where(item =>
                    (!string.IsNullOrWhiteSpace(item)) &&
                    (
                        item.StartsWith("8") ||
                        matIds.Any(mat => item.StartsWith(mat))
                    )
                )
                .Distinct()
                .ToList();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _startScan = false;
            _timerBlink?.Stop();

            AtualizarRegistradoresServers(false);

            if (_clpService != null)
            {
                try { _clpService.EscreverRegistro(21, 0); } catch { }
                _clpService.Desconectar();
            }

            try
            {
                driver?.Quit();
                driver?.Dispose();
            }
            catch { }
        }

        public void EspelharGridNoDataGrid(IWebDriver driver, DataGridView dgv)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Espera existir pelo menos uma linha
            wait.Until(d => d.FindElements(By.CssSelector("tr.x-grid-row")).Count > 0);

            var linhas = driver.FindElements(By.CssSelector("tr.x-grid-row"));

            dgv.Rows.Clear();

            foreach (var linha in linhas)
            {
                var colunas = linha.FindElements(By.TagName("td"));

                // Garantia que tem pelo menos 4 colunas
                if (colunas.Count < 4)
                    continue;

                string col1 = colunas[0].Text.Trim(); // Coluna 1
                string col2 = colunas[1].Text.Trim(); // Coluna 2
                string matId = colunas[2].Text.Trim(); // Coluna 3 (Mat Id)
                string col4 = colunas[3].Text.Trim(); // Coluna 4

                // Adiciona no DataGridView
                dgv.Rows.Add(col1, col2, matId, col4);
            }
        }

        private bool AguardarCodigoNaBarcodeId(string codigo, int timeoutMs = 1000)
        {
            var inicio = DateTime.Now;
            while ((DateTime.Now - inicio).TotalMilliseconds < timeoutMs)
            {
                // Descobre o índice (1-based) da coluna Barcode ID pelo cabeçalho
                var headers = driver.FindElements(By.XPath(
                    "//div[contains(@class,'x-fieldset-header-text') and contains(text(),'Attachment Material Scan List')]" +
                    "/ancestor::fieldset//span[contains(@class,'x-column-header-text')]"
                ));
                int colIndex = -1;
                for (int i = 0; i < headers.Count; i++)
                {
                    if (headers[i].Text.Trim() == "Barcode ID")
                    {
                        colIndex = i + 1; // XPath td[] é 1-based
                        break;
                    }
                }

                if (colIndex > 0)
                {
                    var cells = driver.FindElements(By.XPath(
                        $"//div[contains(@class,'x-fieldset-header-text') and contains(text(),'Attachment Material Scan List')]" +
                        $"/ancestor::fieldset//tr[contains(@class,'x-grid-row')]/td[{colIndex}]/div[contains(@class,'x-grid-cell-inner')]"
                    ));
                    if (cells.Any(c => c.Text.Trim() == codigo))
                        return true;
                }

                System.Threading.Thread.Sleep(200);
            }
            return false;
        }

        private string AguardarRespostaStatus(string statusAnterior, int timeoutMs = 1000)
        {
            var inicio = DateTime.Now;
            while ((DateTime.Now - inicio).TotalMilliseconds < timeoutMs)
            {
                string atual = driver.FindElement(By.Id("opcStatus-body")).Text.Trim();
                if (atual != statusAnterior)
                    return atual;
                System.Threading.Thread.Sleep(200);
            }
            return driver.FindElement(By.Id("opcStatus-body")).Text.Trim();
        }

        private bool AguardarStatusContem(string texto, int timeoutMs = 2000)
        {
            var inicio = DateTime.Now;
            while ((DateTime.Now - inicio).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    string atual = driver.FindElement(By.Id("opcStatus-body")).Text.Trim();
                    if (atual.Contains(texto))
                        return true;
                }
                catch { }
                System.Threading.Thread.Sleep(200);
            }
            return false;
        }

        private bool AguardarCampoAttachmentAtivo(int timeoutMs = 1000)
        {
            var inicio = DateTime.Now;
            while ((DateTime.Now - inicio).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    var campo = driver.FindElement(By.CssSelector("input[name='attachmentCode']"));
                    if (campo.Enabled && campo.Displayed)
                        return true;
                }
                catch { }
                System.Threading.Thread.Sleep(100);
            }
            return false;
        }

        private void EnviarParaPaginaWeb()
        {
            // --- Envia IMEI ---
            IWebElement campoImei = driver.FindElement(By.CssSelector("input[name='imei']"));
            campoImei.Clear();
            campoImei.SendKeys(Imei);
            campoImei.SendKeys(OpenQA.Selenium.Keys.Enter);

            // Aguarda plataforma responder "Ready" antes de enviar attachments
            if (!AguardarStatusContem("Ready"))
            {
                string msgStatus = driver.FindElement(By.Id("opcStatus-body")).Text.Trim();
                string motivo = string.IsNullOrEmpty(msgStatus) ? "sem resposta da plataforma" : msgStatus;
                SinalizarErroTcp($"IMEI rejeitado: {motivo}");
                return;
            }

            // --- Envia códigos de attachment ---
            IWebElement campoAttachment = driver.FindElement(By.CssSelector("input[name='attachmentCode']"));

            foreach (var codigo in codigosLidos.Where(c => c != Imei))
            {
                campoAttachment.Clear();
                campoAttachment.SendKeys(codigo);
                campoAttachment.SendKeys(OpenQA.Selenium.Keys.Enter);

                bool apareceu = AguardarCodigoNaBarcodeId(codigo);
                if (!apareceu)
                {
                    string msgErro = driver.FindElement(By.Id("opcStatus-body")).Text.Trim();
                    SinalizarErroTcp(string.IsNullOrEmpty(msgErro) ? $"Falha ao confirmar código: {codigo}" : msgErro);
                    return;
                }
            }

            // --- Verifica sucesso final ---
            string msgFinal = AguardarRespostaStatus(driver.FindElement(By.Id("opcStatus-body")).Text.Trim());
            if (msgFinal.Contains("Success"))
            {
                string imeiApontado = Imei;
                codigosLidos.Clear();
                Imei = "";
                _aguardandoReset = false;
                if (VarGlobal.LeiturasTCP != null)
                    lock (VarGlobal.LeiturasTCP) { VarGlobal.LeiturasTCP.Clear(); }
                EscreverLog("SUCESSO", $"IMEI: {imeiApontado}");
                this.Invoke((Action)(() =>
                {
                    LimparStatusLeitura();
                    lblErroTcp.BackColor = Color.Green;
                    lblErroTcp.Text = $"IMEI {imeiApontado} FOI APONTADO COM SUCESSO";
                    lblErroTcp.Visible = true;
                }));
            }
        }

        private void SelecionarPrimeiraOrdem()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            // Navega do input name="orderId" até o td.x-trigger-cell irmão e clica no trigger
            IWebElement trigger = wait.Until(d =>
                d.FindElement(By.XPath(
                    "//input[@name='orderId']/ancestor::td/following-sibling::td[contains(@class,'x-trigger-cell')]//div[contains(@class,'ux-codeviewx-trigger')]"
                ))
            );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", trigger);

            // Aguarda a lista aparecer e clica na primeira linha
            IWebElement primeiraLinha = wait.Until(d =>
                d.FindElements(By.CssSelector("tr.x-grid-row"))
                 .FirstOrDefault(r => r.Displayed)
            );
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", primeiraLinha);
        }

        private void atualizaUI()
        {
            if (registradores == null) return;
        }
        private void EstilizarGrid(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.LightGray;

            dgv.EnableHeadersVisualStyles = false;

            // Cabeçalho
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 40;

            // Linhas
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // Linhas alternadas
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            // Ajustes gerais
            dgv.RowTemplate.Height = 35;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            dgv.RowHeadersVisible = false;
        }
    }
}
