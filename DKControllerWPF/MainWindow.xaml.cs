using DKCommunication.BasicFramework;
using DKCommunication.Dandick.DK81Series;
using DKCommunication.Core;
using System;
using System.IO.Ports;
using System.Windows;

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

            //初始化接线模式设置列表框
            cbxSetWireMode.ItemsSource = Enum.GetNames(typeof(WireMode));
            cbxSetWireMode.SelectedIndex = (int)dandick.WireMode;

        }

        /// <summary>
        /// Handshake
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你连个嘚啊！");
                return;
            }
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
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个毛啊！");
                return;
            }
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
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚DCSourceRanges啊！");
                return;
            }
            var d = dandick.ReadDCSourceRanges();
            ReadDCSourceRangesIsSuccess.Text = d.IsSuccess.ToString();
            ReadDCSourceRangesErrorCode.Text = d.ErrorCode.ToString();
            ReadDCSourceRangesMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                ReadDCSourceRangesResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');

                txbDCURangeCount.Text = dandick.DCI_RangesCount.ToString();
                txbDCIRangeCount.Text = dandick.DCI_RangesCount.ToString();
                cbxDCURangs.ItemsSource = dandick.DCU_Ranges;
                cbxDCIRangs.ItemsSource = dandick.DCU_Ranges;
            }
        }

        /// <summary>
        /// 读取直流表档位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_ReadDCMeterRanges(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚DCMeterRanges啊！");
                return;
            }
            var d = dandick.ReadDCMeterRanges();
            ReadDCMeterRangesIsSuccess.Text = d.IsSuccess.ToString();
            ReadDCMeterRangesErrorCode.Text = d.ErrorCode.ToString();
            ReadDCMeterRangesMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                ReadDCSourceRangesResponse.Text = SoftBasic.ByteToHexString(d.Content, ' ');

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
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }
            var d = dandick.SetDisplayPage((DisplayPage)Enum.Parse(typeof(DisplayPage), cbxSetDisplayPage.SelectedItem.ToString()));
            tbxSetDisplayPageErrorCode.Text = d.ErrorCode.ToString();
            tbxSetDisplayPageIsSuccess.Text = d.IsSuccess.ToString();
            tbxSetDisplayPageMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                tbxSetDisplayPage.Text = SoftBasic.ByteToHexString(d.Content, ' ');
            }
            tbxSetDisplayPage.Text = dandick.DisplayPage.ToString();

        }

        /// <summary>
        /// 交流源关闭命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_StopACSource(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }

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
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }

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
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }
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
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }
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
            tabControl.TabStripPlacement=System.Windows.Controls.Dock.Left;
        }

        /// <summary>
        /// 设置交流源幅度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_WriteACSourceAmplitude(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }
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
                var result=dandick.WriteACSourceAmplitude(amplitude);
                tbxWriteACSourceAmplitudeIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteACSourceAmplitudeErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteACSourceAmplitudeMesseage.Text = result.Message.ToString();
                if (result.IsSuccess)
                {
                    tbxWriteACSourceAmplitudeResponse.Text = SoftBasic.ByteToHexString(result.Content);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           
        }

        private void Button_Click_WritePhase(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }
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
                    tbxWriteACSourceAmplitudeResponse.Text = SoftBasic.ByteToHexString(result.Content);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_WriteFrequency(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个嘚啊！");
                return;
            }
            try
            {                
                float f = float.Parse(txbWriteFrequency.Text);
                float fc= float.Parse(txbWriteFrequencyC.Text);               

                var result = dandick.WriteFrequency(f,fc);
                tbxWriteFrequencyIsSuccess.Text = result.IsSuccess.ToString();
                tbxWriteFrequencyErrorCode.Text = result.ErrorCode.ToString();
                tbxWriteFrequencyMesseage.Text = result.Message.ToString();
                if (result.IsSuccess)
                {
                    tbxWriteACSourceAmplitudeResponse.Text = SoftBasic.ByteToHexString(result.Content);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }
}
