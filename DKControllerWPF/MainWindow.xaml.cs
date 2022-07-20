using DKCommunication.BasicFramework;
using DKCommunication.Dandick.DK81Series;
using DKCommunication.Core;
using System;
using System.IO.Ports;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace DKControllerWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DK81Device dandick;

        public MainWindow()
        {
            dandick = new DK81Device();
            InitializeComponent();
            DataContext = this;
            innit();
            cbxDataFormat.ItemsSource = Enum.GetNames(typeof(DKCommunication.Core.DataFormat));
            cbxDataFormat.SelectedIndex = (int)dandick.ByteTransform.DataFormat;
        }

        void innit()
        {
            cbxSerialPortsNames.Items.Clear();
            //初始化串口列表
            string[] ports = SerialPort.GetPortNames();
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
        }

        /// <summary>
        /// Handshake
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            var result = dandick.Handshake();
            txbIsSuccess.Text = result.IsSuccess.ToString();
            txbErrorCode.Text = result.ErrorCode.ToString();
            txbMesseage.Text = result.Message;
            if (result.IsSuccess)
            {
                txbResponse.Text = SoftBasic.ByteToHexString(result.Content, ' ');
                txbIsPQ.Text = dandick.IsPQ_Activated.ToString();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ReadACSourceRanges(object sender, RoutedEventArgs e)
        {
           
            var d = dandick.ReadACSourceRanges();
            ReadACSourceRangesIsSuccess.Text = d.IsSuccess.ToString();
            ReadACSourceRangesErrorCode.Text = d.ErrorCode.ToString();
            ReadACSourceRangesMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                ReadACSourceRangesResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
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
                dandick.SerialPortInni(cbxSerialPortsNames.SelectedValue.ToString());
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ReadDCSourceRanges(object sender, RoutedEventArgs e)
        {
           
            var d = dandick.ReadDCSourceRanges();
            ReadDCSourceRangesIsSuccess.Text = d.IsSuccess.ToString();
            ReadDCSourceRangesErrorCode.Text = d.ErrorCode.ToString();
            ReadDCSourceRangesMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                ReadDCSourceRangesResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');

                txbDCURangeCount.Text = dandick.DCU_RangesCount.ToString();
                txbDCIRangeCount.Text = dandick.DCI_RangesCount.ToString();
                cbxDCURangs.ItemsSource = dandick.DCU_Ranges;
                cbxDCIRangs.ItemsSource = dandick.DCI_Ranges;
            }
        }

        /// <summary>
        /// 读取直流表档位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ReadDCMeterRanges(object sender, RoutedEventArgs e)
        {
            
            var d = dandick.ReadDCMeterRanges();
            ReadDCMeterRangesIsSuccess.Text = d.IsSuccess.ToString();
            ReadDCMeterRangesErrorCode.Text = d.ErrorCode.ToString();
            ReadDCMeterRangesMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                ReadDCMeterRangesResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');

                txbDCMURangeCount.Text = dandick.DCM_URangesCount.ToString();
                txbDCMIRangeCount.Text = dandick.DCM_IRangesCount.ToString();
                cbxDCMURangs.ItemsSource = dandick.DCM_URanges;
                cbxDCMIRangs.ItemsSource = dandick.DCM_IRanges;
            }
        }

        private void cbxDataFormat_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            dandick.ByteTransform.DataFormat = (DKCommunication.Core.DataFormat)cbxDataFormat.SelectedIndex;
        }

        /// <summary>
        /// 设置当前显示页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_SetDisplayPage(object sender, RoutedEventArgs e)
        {
            var d = dandick.SetDisplayPage((DisplayPage)Enum.Parse(typeof(DisplayPage), cbxSetDisplayPage.SelectedItem.ToString()));
            tbxSetDisplayPageErrorCode.Text = d.ErrorCode.ToString();
            tbxSetDisplayPageIsSuccess.Text = d.IsSuccess.ToString();
            tbxSetDisplayPageMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxSetDisplayPageResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
            }
            tbxSetDisplayPage.Text = dandick.DisplayPage.ToString();

        }

        /// <summary>
        /// 设置系统模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_SetSystemMode(object sender, RoutedEventArgs e)
        {
            var d = dandick.SetSystemMode((SystemMode)Enum.Parse(typeof(SystemMode), cbxSetSystemMode.SelectedItem.ToString()));
            tbxSetSystemModeErrorCode.Text = d.ErrorCode.ToString();
            tbxSetSystemModeIsSuccess.Text = d.IsSuccess.ToString();
            tbxSetSystemModeMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxSetSystemModeResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
            }
            tbxSetSystemMode.Text = dandick.SystemMode.ToString();
        }

        /// <summary>
        /// 交流源关闭命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_StopACSource(object sender, RoutedEventArgs e)
        {
            var d = dandick.StopACSource();
            tbxStopACSourceErrorCode.Text = d.ErrorCode.ToString();
            tbxStopACSourceIsSuccess.Text = d.IsSuccess.ToString();
            tbxStopACSourceMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxStopACSourceeResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
            }
        }

        /// <summary>
        /// 交流源打开命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_StarACSource(object sender, RoutedEventArgs e)
        {
            var d = dandick.StartACSource();
            tbxStartACSourceErrorCode.Text = d.ErrorCode.ToString();
            tbxStartACSourceIsSuccess.Text = d.IsSuccess.ToString();
            tbxStartACSourceMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxStartACSourceResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
            }
        }

        /// <summary>
        /// 设置接线模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_SetWireMode(object sender, RoutedEventArgs e)
        {
            var d = dandick.SetWireMode((WireMode)Enum.Parse(typeof(WireMode), cbxSetWireMode.SelectedItem.ToString()));
            tbxSetWireModeErrorCode.Text = d.ErrorCode.ToString();
            tbxSetWireModeIsSuccess.Text = d.IsSuccess.ToString();
            tbxSetWireModeMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxSetWireModeResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
            }
            tbxSetWireModePage.Text = dandick.WireMode.ToString();
        }

        /// <summary>
        /// 设置交流源档位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_SetACSourceRange(object sender, RoutedEventArgs e)
        {
            var d = dandick.SetACSourceRange(cbxACSourceURanges.SelectedIndex, cbxACSourceIRanges.SelectedIndex);
            tbxSetACSourceRangeErrorCode.Text = d.ErrorCode.ToString();
            tbxSetACSourceRangeIsSuccess.Text = d.IsSuccess.ToString();
            tbxSetACSourceRangeMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxSetACSourceRangeResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');
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
        private void Button_Click_WriteACSourceAmplitude(object sender, RoutedEventArgs e)
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
                tbxWriteACSourceAmplitudeIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteACSourceAmplitudeErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteACSourceAmplitudeMesseage.Text = result.Message.ToString();
                if (result.IsSuccess)
                {
                    tbxWriteACSourceAmplitudeResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                }
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
        private void Button_Click_WritePhase(object sender, RoutedEventArgs e)
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
                tbxWritePhaseIsSuccess.Text = result.IsSuccess.ToString();
                tbxWritePhaseErrorCode.Text = result.ErrorCode.ToString();
                tbxWritePhaseMesseage.Text = result.Message.ToString();
                if (result.IsSuccess)
                {
                    tbxWritePhaseResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                }
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
        private void Button_Click_WriteFrequency(object sender, RoutedEventArgs e)
        {
           
            try
            {
                float f = float.Parse(txbWriteFrequency.Text);
                float fc = float.Parse(txbWriteFrequencyC.Text);

                var result = dandick.WriteFrequency(f, fc);
                tbxWriteFrequencyIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteFrequencyErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteFrequencyMesseage.Text = result.Message.ToString();
                if (result.IsSuccess)
                {
                    tbxWriteFrequencyResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                }
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
        private void Button_Click_SetClosedLoop(object sender, RoutedEventArgs e)
        {
         
            CloseLoopMode closeLoopMode = (CloseLoopMode)Enum.Parse(typeof(CloseLoopMode), cbxSetClosedLoopMode.Text);
            HarmonicMode harmonicMode = (HarmonicMode)Enum.Parse(typeof(HarmonicMode), cbxSetClosedLoopHarmonicMode.Text);
            var result = dandick.SetClosedLoop(closeLoopMode, harmonicMode);
            tbxSetClosedLoopIsSuccess.Text = result.IsSuccess.ToString();
            tbxSetClosedLoopErrorCode.Text = result.ErrorCode.ToString();
            tbxSetClosedLoopMesseage.Text = result.Message.ToString();
            if (result.IsSuccess)
            {
                tbxSetClosedLoopResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
            }
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
                    channelsHarmonic=(ChannelsHarmonic)int.Parse(HarmonicsMultiChannelsValue.Text);
                }
                else
                {
                     channelsHarmonic = (ChannelsHarmonic)Enum.Parse(typeof(ChannelsHarmonic), cbxHarmonicChannels.Text);
                 
                }
            
                var result = dandick.WriteHarmonics(channelsHarmonic, harmonic);
                tbxWriteHarmonicsIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteHarmonicsErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteHarmonicsMesseage.Text = result.Message.ToString();

                if (result.IsSuccess)
                {
                    tbxWriteHarmonicsResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                }
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
                var result=dandick.WriteWattPower(channelWattPower, data);
                tbxWriteWattPowerIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteWattPowerErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteWattPowerMesseage.Text = result.Message.ToString();

                if (result.IsSuccess)
                {
                    tbxWriteWattPowerResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                }

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
                tbxWriteWattLessPowerIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteWattLessPowerErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteWattLessPowerMesseage.Text = result.Message.ToString();

                if (result.IsSuccess)
                {
                    tbxWriteWattLessPowerResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private CancellationTokenSource _cts;
        private void Button_Click_ReadACSourceData(object sender, RoutedEventArgs e)
        {
            gridData.Visibility = Visibility.Visible ;
            _cts = new CancellationTokenSource();
            //开启线程
            Task.Run(new Action(ReadingACSourceData), _cts.Token);
        }

        private void ReadingACSourceData()
        {
            while (!_cts.IsCancellationRequested)
            {
                var result = dandick.ReadACSourceData();
                Dispatcher.Invoke(() =>
                {
                    tbxReadACSourceDataIsSuccess.Text = result.IsSuccess.ToString();
                    tbxReadACSourceDataErrorCode.Text = result.ErrorCode.ToString();
                    tbxReadACSourceDataMesseage.Text = result.Message.ToString();
                    if (result.IsSuccess)
                    {
                        tbxReadACSourceDataResponse.Text = SoftBasic.ByteToHexString(result.Content,' ');
                    }
                    lbUA.Content = dandick.UA;
                    lbUB.Content = dandick.UB;
                    lbUC.Content = dandick.UC;
                    lbIA.Content = dandick.IA;
                    lbIB.Content= dandick.IB;
                    lbIC.Content= dandick.IC;
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
                //Thread.Sleep(500);
            }
        }

        private void Button_Click_StopReadACSourceData(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            gridData.Visibility=Visibility.Collapsed;
        }

      
    }
}
