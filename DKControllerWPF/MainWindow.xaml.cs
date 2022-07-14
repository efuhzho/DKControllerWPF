using DKCommunication.BasicFramework;
using DKCommunication.Dandick.DK81Series;
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
            ReadACSourceRangesIsSuccess.Text=d.IsSuccess.ToString();
            ReadACSourceRangesErrorCode.Text=d.ErrorCode.ToString();
            ReadACSourceRangesMesseage.Text = d.Message;
            if (d.IsSuccess)
            {
                ReadACSourceRangesResponse.Text = SoftBasic.ByteToHexString(d.Content,' ');
                txbURangeCount.Text=dandick.ACU_RangesCount.ToString();
                txbIRangeCount.Text = dandick.ACI_RangesCount.ToString();
                txbIProtectRangeCount.Text = dandick.IProtectRangesCount.ToString();
                txbURangsASingle.Text=dandick.URanges_Asingle.ToString();
                txbIRangsASingle.Text = dandick.IRanges_Asingle.ToString();
                txbIProtectRangsASingle.Text = dandick.IProtectRanges_Asingle.ToString();
                cbxACURangs.ItemsSource = dandick.ACU_RangesList;                
                cbxACIRangs.ItemsSource = dandick.ACI_RangesList;
                cbxACIRangs.SelectedIndex = 0;
                cbxACURangs.SelectedIndex = 0;
            }
        }

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
    }
}
