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

        private void Button_Click_ReadACSourceRanges(object sender, RoutedEventArgs e)
        {
            if (!dandick.IsOpen())
            {
                MessageBox.Show("串口都没打开，你读个毛啊！");
                return;
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
    }
}
