using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace DnsChanger
{
    class Program
    {
        static void Main(string[] args)
        {
            string networkInterfaceName = GetActiveNetworkInterfaceName();

            if (string.IsNullOrEmpty(networkInterfaceName))
            {
                Console.WriteLine("Aktif bir ağ bağlantısı bulunamadı.");
                return;
            }

            Console.WriteLine("Erişim Engeli Kaldırılıyor");
            SetDns(networkInterfaceName, "8.8.8.8", "8.8.4.4");

            Console.WriteLine("Erişim Engeli Kaldırıldı. Çıkış yapmak için bir tuşa basın...");
            Console.ReadKey();

            Console.WriteLine("Eski DNS ayarları geri yükleniyor...");
            ResetDns(networkInterfaceName);

            Console.WriteLine("Eski DNS ayarları geri yüklendi.");
        }

        static string GetActiveNetworkInterfaceName()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                {
                    return ni.Name;
                }
            }
            return null;
        }

        static void SetDns(string networkInterface, string dns1, string dns2)
        {
            ExecuteCommand($"netsh interface ip set dns \"{networkInterface}\" static {dns1}");
            ExecuteCommand($"netsh interface ip add dns \"{networkInterface}\" {dns2} index=2");
        }

        static void ResetDns(string networkInterface)
        {
            ExecuteCommand($"netsh interface ip set dns \"{networkInterface}\" dhcp");
        }

        static void ExecuteCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/C " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = true,
                Verb = "runas" // Yönetici izni ile çalıştır
            };

            try
            {
                Process.Start(psi).WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Komut çalıştırılamadı: {ex.Message}");
            }
        }
    }
}
