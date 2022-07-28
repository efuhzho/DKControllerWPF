using DKCommunication.BasicFramework;
using DKCommunication.Dandick.DK81Series;
using DKCommunication.Core;
using System;
using System.IO.Ports;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace DKControllerWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged,ICommand
    {
        private readonly DK81Device dandick;


        #region 属性

        #region OperrateResult返回对象属性
        private bool _IsSuccess;
        /// <summary>
        /// 是否成功标志
        /// </summary>
        public bool IsSuccess
        {
            get { return _IsSuccess; }
            private set
            {
                _IsSuccess = value;
                NotifyPropertyChanged();
            }
        }

        private int _ErrorCode;
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode
        {
            get { return _ErrorCode; }
            private set
            {
                _ErrorCode = value;
                NotifyPropertyChanged();
            }
        }

        private string _Message;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message
        {
            get { return _Message; }
            private set
            {
                _Message = value;
                NotifyPropertyChanged();
            }
        }

        private string _ResponseContent = String.Empty;
        /// <summary>
        /// 下位机回复的原始报文
        /// </summary>
        public string ResponseContent
        {
            get { return _ResponseContent; }
            private set
            {
                _ResponseContent = value;
                NotifyPropertyChanged();
            }
        }
        #endregion OperrateResult返回对象属性

        #region 波特率ComboBox
        readonly List<string> _BaudRates = new List<string>()
        {
           "9600","115200"
        };
        /// <summary>
        /// 波特率
        /// </summary>
        public List<string> BaudRates
        {
            get { return _BaudRates; }
        }
        #endregion 波特率ComboBox

        #region 电能校验类型ComboBox
        private readonly string[] _ElectricityType = Enum.GetNames(typeof(ElectricityType));
        /// <summary>
        /// 电能校验类型下拉框列表
        /// </summary>
        public string[] Electricity_Type
        {
            get { return _ElectricityType; }
        }
        #endregion 电能校验类型ComboBox

        #region 设置电能校验参数
        #region 设置电能校验类型
        private ElectricityType _electricityType;
        /// <summary>
        /// 设置电能校验类型
        /// </summary>
        public ElectricityType EType
        {
            get { return _electricityType; }
            set
            {
                _electricityType = (ElectricityType)Enum.Parse(typeof(ElectricityType), value.ToString());
                NotifyPropertyChanged();
            }
        }
        #endregion 设置电能校验类型
        /// <summary>
        /// 设置表有功常数
        /// </summary>
        public float MPC { get; set; } = 3600;

        /// <summary>
        /// 设置表无功常数
        /// </summary>
        public float MQC { get; set; } = 3600;

        /// <summary>
        /// 设置表分频系数
        /// </summary>
        public uint MDIV { get; set; } = 1000;

        /// <summary>
        /// 设置表校验圈数
        /// </summary>
        public uint MROU { get; set; } = 10;
        #endregion 设置电能校验参数

        #region 读电能误差返回值
        private char _ElectricityFlag;
        /// <summary>
        /// 当前校验类型
        /// </summary>
        public char EFlag
        {
            get { return _ElectricityFlag; }
            private set { _ElectricityFlag = value; NotifyPropertyChanged(); }
        }

        private float _EV;
        /// <summary>
        /// 误差值，单位（%）
        /// </summary>
        public float EV
        {
            get { return _EV; }
            private set { _EV = value; NotifyPropertyChanged(); }
        }

        private uint _ROUND;
        /// <summary>
        /// 当前已校验圈数
        /// </summary>
        public uint ROUND
        {
            get { return _ROUND; }
            private set { _ROUND = value; NotifyPropertyChanged(); }
        }
        #endregion 读电能误差返回值


        #endregion 属性

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            dandick = new DK81Device();
            InitializeComponent();
            DataContext = this;
            innit();
            cbxDataFormat.ItemsSource = Enum.GetNames(typeof(DKCommunication.Core.DataFormat));
            cbxDataFormat.SelectedIndex = (int)dandick.ByteTransform.DataFormat;
        }

        /// <summary>
        /// 初始化ComboBox
        /// </summary>
        void innit()
        {
            cbxSerialPortsNames.Items.Clear();
            //初始化串口列表
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (string port in ports)
            {
                cbxSerialPortsNames.Items.Add(port);
            }

            cbxSerialPortsNames.SelectedIndex = 0;

            //初始化显示页面设置列表框
            cbxSetDisplayPage.ItemsSource = Enum.GetNames(typeof(DisplayPage));
            cbxSetDisplayPage.SelectedIndex = (int)dandick.DisplayPage;

            cbxSetSystemMode.ItemsSource = Enum.GetNames(typeof(SystemMode));
            cbxSetSystemMode.SelectedIndex = (int)dandick.SystemMode;

            //初始化接线模式设置列表框
            cbxSetWireMode.ItemsSource = Enum.GetNames(typeof(WireMode));
            cbxSetWireMode.SelectedIndex = (int)dandick.WireMode;

            //初始化闭环模式设置列表
            cbxSetClosedLoopMode.ItemsSource = Enum.GetNames(typeof(CloseLoopMode));
            cbxSetClosedLoopMode.SelectedIndex = (int)dandick.CloseLoopMode;

            //初始化谐波模式设置列表
            cbxSetClosedLoopHarmonicMode.ItemsSource = Enum.GetNames(typeof(HarmonicMode));
            cbxSetClosedLoopHarmonicMode.SelectedIndex = (int)dandick.HarmonicMode;

            //初始化谐波设置通道列表
            cbxHarmonicChannels.ItemsSource = Enum.GetNames(typeof(ChannelsHarmonic));

            //初始化有功功率设置通道列表
            cbxChannelWattPower.ItemsSource = Enum.GetNames(typeof(ChannelWattPower));

            //初始化无功功率设置通道列表
            cbxChannelWattLessPower.ItemsSource = Enum.GetNames(typeof(ChannelWattLessPower));

            //cbxEType.ItemsSource = Enum.GetNames(typeof(ElectricityType));
        }

        /// <summary>
        /// HandShake
        /// </summary>

        private void HandShake()
        {
            var result = dandick.Handshake();
            Update(result);
            if (result.IsSuccess)
            {
                txbIsPQ.Text = dandick.IsElectricity_Activated.ToString();
                txbIsDCS.Text = dandick.IsDCU_Activated.ToString();
                txbIsDCM.Text = dandick.IsDCM_Activated.ToString();
                txbIsACSource.Text = dandick.IsACU_Activated.ToString();
                txbVersion.Text = dandick.Version.ToString();
                txbSerialNO.Text = dandick.SN;
                txbModel.Text = dandick.Model;
            }
        }

        /// <summary>
        /// ReadACSourceRanges
        /// </summary>

        private void ReadACSourceRanges()
        {

            var result = dandick.ReadACSourceRanges();
            Update(result);
            if (result.IsSuccess)
            {
                txbURangeCount.Text = dandick.ACU_RangesCount.ToString();
                txbIRangeCount.Text = dandick.ACI_RangesCount.ToString();
                txbIProtectRangeCount.Text = dandick.IProtectRangesCount.ToString();
                txbURangsASingle.Text = dandick.URanges_Asingle.ToString();
                txbIRangsASingle.Text = dandick.IRanges_Asingle.ToString();
                txbIProtectRangsASingle.Text = dandick.IProtectRanges_Asingle.ToString();
                cbxACURangs.ItemsSource = dandick.ACU_RangesList;
                cbxACIRangs.ItemsSource = dandick.ACI_RangesList;
                cbxACIRangs.SelectedIndex = 0;
                cbxACURangs.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dandick.SerialPortInni(cbxSerialPortsNames.SelectedValue.ToString(), Convert.ToInt32(cbxBaudRate.SelectedValue));
                dandick.Open();
                if (dandick.IsOpen())
                {
                    btnOpen.IsEnabled = false;
                    btnClose.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cts?.Cancel();
                dandick.Close();
                if (!dandick.IsOpen())
                {
                    btnOpen.IsEnabled = true;
                    btnClose.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 刷新串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnrefresh_Click(object sender, RoutedEventArgs e)
        {
            cbxSerialPortsNames.IsEnabled = false;
            innit();
            cbxSerialPortsNames.IsEnabled = true;
        }

        /// <summary>
        /// ReadDCSourceRanges
        /// </summary>      
        private void ReadDCSourceRanges()
        {
            var result = dandick.ReadDCSourceRanges();
            Update(result);
            if (result.IsSuccess)
            {
                txbDCURangeCount.Text = dandick.DCU_RangesCount.ToString();
                txbDCIRangeCount.Text = dandick.DCI_RangesCount.ToString();
                cbxDCURangs.ItemsSource = dandick.DCU_Ranges;
                cbxDCIRangs.ItemsSource = dandick.DCI_Ranges;
            }
        }

        /// <summary>
        /// 读取直流表档位
        /// </summary>      
        private void ReadDCMeterRanges()
        {
            var d = dandick.ReadDCMeterRanges();
            Update(d);
            if (d.IsSuccess)
            {
                txbDCMURangeCount.Text = dandick.DCM_URangesCount.ToString();
                txbDCMIRangeCount.Text = dandick.DCM_IRangesCount.ToString();
                cbxDCMURangs.ItemsSource = dandick.DCM_URanges;
                cbxDCMIRangs.ItemsSource = dandick.DCM_IRanges;
            }
        }

        /// <summary>
        /// 数据类型格式定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDataFormat_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dandick.ByteTransform.DataFormat = (DKCommunication.Core.DataFormat)cbxDataFormat.SelectedIndex;
        }

        /// <summary>
        /// 设置当前显示页面
        /// </summary>     
        private void SetDisplayPage()
        {
            var d = dandick.SetDisplayPage((DisplayPage)Enum.Parse(typeof(DisplayPage), cbxSetDisplayPage.SelectedItem.ToString()));
            Update(d);
            if (d.IsSuccess)
            {
                tbxSetDisplayPage.Text = dandick.DisplayPage.ToString();
            }
        }

        /// <summary>
        /// 设置系统模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSystemMode()
        {
            var d = dandick.SetSystemMode((SystemMode)Enum.Parse(typeof(SystemMode), cbxSetSystemMode.SelectedItem.ToString()));
            Update(d);
            if (d.IsSuccess)
            {
                tbxSetSystemMode.Text = dandick.SystemMode.ToString();
            }
        }

        /// <summary>
        /// 交流源关闭命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopACSource()
        {
            var d = dandick.StopACSource();
            Update(d);
        }

        /// <summary>
        /// 交流源打开命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartACSource()
        {
            var d = dandick.StartACSource();
            Update(d);
        }

        /// <summary>
        /// 设置接线模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetWireMode()
        {
            var d = dandick.SetWireMode((WireMode)Enum.Parse(typeof(WireMode), cbxSetWireMode.SelectedItem.ToString()));
            Update((d));
            if (d.IsSuccess)
            {
                tbxSetWireModePage.Text = dandick.WireMode.ToString();
            }
        }

        /// <summary>
        /// 设置交流源档位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetACSourceRange()
        {
            var d = dandick.SetACSourceRange(cbxACSourceURanges.SelectedIndex, cbxACSourceIRanges.SelectedIndex);
            Update(d);
            if (d.IsSuccess)
            {
                tbxACSourceURange.Text = dandick.ACU_RangeIndex.ToString();
                tbxACSourceIRange.Text = dandick.ACI_RangeIndex.ToString();
            }
        }

        private void tbtnTabControl_Checked(object sender, RoutedEventArgs e)
        {
            tabControl.TabStripPlacement = System.Windows.Controls.Dock.Bottom;
        }

        private void tbtnTabControl_Unchecked(object sender, RoutedEventArgs e)
        {
            tabControl.TabStripPlacement = System.Windows.Controls.Dock.Left;
        }

        /// <summary>
        /// 设置交流源幅度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteACSourceAmplitude()
        {
            try
            {
                float[] amplitude = new float[9];
                amplitude[0] = float.Parse(txbAmplitudeUa.Text);
                amplitude[1] = float.Parse(txbAmplitudeUb.Text);
                amplitude[2] = float.Parse(txbAmplitudeUc.Text);
                amplitude[3] = float.Parse(txbAmplitudeIa.Text);
                amplitude[4] = float.Parse(txbAmplitudeIb.Text);
                amplitude[5] = float.Parse(txbAmplitudeIc.Text);
                amplitude[6] = float.Parse(txbAmplitudeIPa.Text);
                amplitude[7] = float.Parse(txbAmplitudeIPb.Text);
                amplitude[8] = float.Parse(txbAmplitudeIPc.Text);
                var result = dandick.WriteACSourceAmplitude(amplitude);
                Update(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 设置相位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WritePhase()
        {
            try
            {
                float[] amplitude = new float[6];
                amplitude[0] = float.Parse(txbAmplitudeFaiUa.Text);
                amplitude[1] = float.Parse(txbAmplitudeFaiUb.Text);
                amplitude[2] = float.Parse(txbAmplitudeFaiUc.Text);
                amplitude[3] = float.Parse(txbAmplitudeFaiIa.Text);
                amplitude[4] = float.Parse(txbAmplitudeFaiIb.Text);
                amplitude[5] = float.Parse(txbAmplitudeFaiIc.Text);

                var result = dandick.WritePhase(amplitude);
                Update(result);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 设置频率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WriteFrequency()
        {
            try
            {
                float f = float.Parse(txbWriteFrequency.Text);
                float fc = float.Parse(txbWriteFrequencyC.Text);

                var result = dandick.WriteFrequency(f, fc);
                Update(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 设置闭环模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetClosedLoop()
        {

            CloseLoopMode closeLoopMode = (CloseLoopMode)Enum.Parse(typeof(CloseLoopMode), cbxSetClosedLoopMode.Text);
            HarmonicMode harmonicMode = (HarmonicMode)Enum.Parse(typeof(HarmonicMode), cbxSetClosedLoopHarmonicMode.Text);
            var result = dandick.SetClosedLoop(closeLoopMode, harmonicMode);
            Update(result);
            tbxSetClosedLoop.Text = dandick.CloseLoopMode.ToString();
            tbxHarmonicMode.Text = dandick.HarmonicMode.ToString();
        }

        /// <summary>
        /// 设置谐波幅度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_WriteHarmonics(object sender, RoutedEventArgs e)
        {
            ChannelsHarmonic channelsHarmonic;
            try
            {
                Harmonics harmonic = new Harmonics()
                {
                    Amplitude = float.Parse(tbxWriteHarmonicsAmplitude.Text),
                    HarmonicTimes = byte.Parse(tbxWriteHarmonicsTimes.Text),
                    Angle = float.Parse(tbxWriteHarmonicsAngle.Text),
                };
                if ((bool)isMultiChannels.IsChecked)
                {
                    channelsHarmonic = (ChannelsHarmonic)int.Parse(HarmonicsMultiChannelsValue.Text);
                }
                else
                {
                    channelsHarmonic = (ChannelsHarmonic)Enum.Parse(typeof(ChannelsHarmonic), cbxHarmonicChannels.Text);

                }

                var result = dandick.WriteHarmonics(channelsHarmonic, harmonic);
                Update(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 清空指定通道谐波
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ClearHarmonics(object sender, RoutedEventArgs e)
        {
            try
            {
                ChannelsHarmonic channelsHarmonic;
                if ((bool)isMultiChannels.IsChecked)
                {
                    channelsHarmonic = (ChannelsHarmonic)int.Parse(HarmonicsMultiChannelsValue.Text);
                }
                else
                {
                    channelsHarmonic = (ChannelsHarmonic)Enum.Parse(typeof(ChannelsHarmonic), cbxHarmonicChannels.Text);

                }
                var result = dandick.ClearHarmonics(channelsHarmonic);
                Update(result);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private void isMultiChannels_Checked(object sender, RoutedEventArgs e)
        {
            tbIsMultiChl.Visibility = Visibility.Visible;
            cbxHarmonicChannels.IsEnabled = false;
            cbxHarmonicChannels.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void isMultiChannels_Unchecked(object sender, RoutedEventArgs e)
        {
            tbIsMultiChl.Visibility = Visibility.Collapsed;
            cbxHarmonicChannels.IsEnabled = true;
            cbxHarmonicChannels.Foreground = System.Windows.Media.Brushes.Black;
        }

        /// <summary>
        /// 设置有功功率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_WriteWattPower(object sender, RoutedEventArgs e)
        {
            try
            {
                ChannelWattPower channelWattPower = (ChannelWattPower)(byte)Enum.Parse(typeof(ChannelWattPower), cbxChannelWattPower.Text);
                //ChannelWattPower channelWattPower = (ChannelWattPower)cbxChannelWattPower.SelectedIndex;
                float data = float.Parse(tbxWriteWattPowerData.Text);
                var result = dandick.WriteWattPower(channelWattPower, data);
                Update(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 设置无功功率
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_WriteWattLessPower(object sender, RoutedEventArgs e)
        {
            try
            {
                ChannelWattLessPower channelWattLessPower = (ChannelWattLessPower)(byte)Enum.Parse(typeof(ChannelWattLessPower), cbxChannelWattLessPower.Text);
                //ChannelWattLessPower channelWattLessPower = (ChannelWattLessPower)cbxChannelWattLessPower.SelectedIndex;
                float data = float.Parse(tbxWriteWattLessPowerData.Text);
                var result = dandick.WriteWattLessPower(channelWattLessPower, data);
                Update(result);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private CancellationTokenSource _cts;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CanExecuteChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 开始读取数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ReadACSourceData(object sender, RoutedEventArgs e)
        {
            gridData.Visibility = Visibility.Visible;
            _cts = new CancellationTokenSource();
            //开启线程
            Task.Run(new Action(ReadingACSourceData), _cts.Token);
        }

        /// <summary>
        /// 更新界面
        /// </summary>
        private void ReadingACSourceData()
        {
            while (!_cts.IsCancellationRequested)
            {
                var result = dandick.ReadACSourceData();
                Dispatcher.Invoke(() =>
                {
                    Update(result);
                    lbUA.Content = dandick.UA;
                    lbUB.Content = dandick.UB;
                    lbUC.Content = dandick.UC;
                    lbURange.Content = dandick.ACU_Range;
                    lbIA.Content = dandick.IA;
                    lbIB.Content = dandick.IB;
                    lbIC.Content = dandick.IC;
                    lbIRange.Content = dandick.ACI_Range;
                    lbFaiUA.Content = dandick.UaPhase;
                    lbFaiUB.Content = dandick.UbPhase;
                    lbFaiUC.Content = dandick.UcPhase;
                    lbFaiIA.Content = dandick.IaPhase;
                    lbFaiIB.Content = dandick.IbPhase;
                    lbFaiIC.Content = dandick.IcPhase;
                    lbPA.Content = dandick.Pa;
                    lbPB.Content = dandick.Pb;
                    lbPC.Content = dandick.Pc;
                    lbPall.Content = dandick.P;
                    lbQA.Content = dandick.Qa;
                    lbQB.Content = dandick.Qb;
                    lbQC.Content = dandick.Qc;
                    lbQall.Content = dandick.Q;
                    lbSA.Content = dandick.Sa;
                    lbSB.Content = dandick.Sb;
                    lbSC.Content = dandick.Sc;
                    lbSall.Content = dandick.S;
                    lbPFA.Content = dandick.CosFaiA;
                    lbPFB.Content = dandick.CosFaiB;
                    lbPFC.Content = dandick.CosFaiC;
                    lbPFall.Content = dandick.CosFai;
                    lbFreq.Content = dandick.Frequency;                   
                    lbFreqC.Content = dandick.FrequencyC;
                });
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// 停止读取数据，取消线程，关闭源输出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_StopReadACSourceData(object sender, RoutedEventArgs e)
        {           
            _cts?.Cancel();  
            gridData.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// 【设置电能校验参数并启动电能校验】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var result = dandick.WriteElectricity(EType, MPC, MQC, MDIV, MROU);
            Update(result);
        }

        /// <summary>
        /// 更新公共返回对象
        /// </summary>
        /// <param name="operateResult">返回对象</param>
        private void Update(dynamic operateResult)
        {
            IsSuccess = operateResult.IsSuccess;
            ErrorCode = operateResult.ErrorCode;
            Message = operateResult.Message;
            ResponseContent = string.Empty;
            if (operateResult.IsSuccess)
            {
                ResponseContent = SoftBasic.ByteToHexString(operateResult.Content, ' ');
            }
        }

        /// <summary>
        /// 【读取电能误差】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReadElectricityDeviation(object sender, RoutedEventArgs e)
        {
            var result = dandick.ReadElectricityDeviation();
            if (result.IsSuccess)
            {
                byte[] data = new byte[1] { dandick.ElectricityDeviationDataFlag };
                EFlag = Encoding.ASCII.GetChars(data)[0];
                EV = dandick.ElectricityDeviationData;
                ROUND = dandick.ElectricityMeasuredRounds;
            }
            Update(result);
        }

        /// <summary>
        /// 按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickCommand(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Content)
            {
                case "ReadDCMeterRanges":
                    ReadDCMeterRanges(); break;

                case "ReadDCSourceRanges":
                    ReadDCSourceRanges(); break;

                case "ReadACSourceRanges":
                    ReadACSourceRanges(); break;

                case "HandShake":
                    HandShake(); break;

                case "SetDisplayPage":
                    SetDisplayPage(); break;

                case "SetSystemMode":
                    SetSystemMode(); break;

                case "StopACSource":
                    StopACSource(); break;

                case "StartACSource":
                    StartACSource(); break;

                case "SetWireMode":
                    SetWireMode(); break;

                case "SetACSourceRange":
                    SetACSourceRange(); break;

                case "WriteACSourceAmplitude":
                    WriteACSourceAmplitude(); break;

                case "WritePhase":
                    WritePhase(); break;

                case "WriteFrequency":
                    WriteFrequency(); break;

                case "SetClosedLoop":
                    SetClosedLoop(); break;


                default:
                    MessageBox.Show("没找到合适的命令标志"); break;
            }

        }

        public bool CanExecute(object parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object parameter)
        {
            
            throw new NotImplementedException();
        }
    }
}
