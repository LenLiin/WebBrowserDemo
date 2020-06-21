using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WebBrowserDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        WebBrowser wb;
        public MainWindow()
        {
            InitializeComponent();
        }


        private void register_Click(object sender, RoutedEventArgs e)
        {
            SetBrowSerCompatibilityMode(true);
            ReStart();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            SetBrowSerCompatibilityMode(false);
            ReStart();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ReStart();
        }

        private void ReStart()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown(); 
        }

        private void SetBrowSerCompatibilityMode(bool IsRegister)
        {
            //获取程序名称
            var fileName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
            if (string.Compare(fileName, "devenv.exe", true) == 0)//确定不是在vs中运行
                return;
            try
            {
                //设置浏览器仿真
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                        RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    key.SetValue(fileName, (uint)(IsRegister ? 11001 : 7000), RegistryValueKind.DWord);
                }

                //禁用阻塞本地Script脚本
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BLOCK_LMZ_SCRIPT",
                        RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    key.SetValue(fileName, (uint)(IsRegister ? 0 : 1), RegistryValueKind.DWord);
                }
                //禁用传统输入模式
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_NINPUT_LEGACYMODE",
                        RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    key.SetValue(fileName, (uint)(IsRegister ? 0 : 1), RegistryValueKind.DWord);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
