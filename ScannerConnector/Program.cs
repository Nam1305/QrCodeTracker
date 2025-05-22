using System;
using System.Threading.Tasks;

namespace ScannerApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Chương trình đọc mã quét từ Denso GT20Q-SM (USB Keyboard Interface)");
                Console.WriteLine("Quét mã để hiển thị thông tin. Nhấn ESC để thoát.");

                while (true)
                {
                    // Đọc dữ liệu từ bàn phím (máy quét gửi như thao tác gõ phím)
                    string? scannedData = await Task.Run(() => Console.ReadLine());

                    // Kiểm tra dữ liệu quét
                    if (!string.IsNullOrEmpty(scannedData))
                    {
                        Console.WriteLine($"Mã quét được: {scannedData}");
                        Console.WriteLine($"Thời gian: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        Console.WriteLine("-------------------");
                    }

                    // Kiểm tra phím ESC để thoát
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                        {
                            Console.WriteLine("Thoát chương trình.");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
}