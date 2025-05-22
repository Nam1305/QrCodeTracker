using System;
using System.IO.Ports;
using System.Drawing;
using System.Drawing.Imaging;
using QRCoder;
using ZXing;
using ZXing.QrCode;
class Program
{
    private static SerialPort _serialPort;
    static void Main(string[] args)
    {

        try
        {
            //_serialPort.Open();
            InitializeSerialPort("COM3");
            _serialPort.DataReceived += SerialPort_DataReceived;
            //string x = _serialPort.ReadExisting();
            Console.WriteLine("Đã mở cổng COM. Quét mã để nhận dữ liệu...");

            string scannedData = null;
            string filePath = @"C:\Users\vnintern01\Desktop\job_188552_modified.png";

            Console.ReadLine(); // Giữ ứng dụng chạy
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi: {ex.Message}");
        }
        //finally
        //{
        //    if (_serialPort.IsOpen)
        //        _serialPort.Close();
        //}

        //try
        //{
        //    // Khởi tạo và cấu hình cổng COM (thay đổi "COM3" thành cổng COM thích hợp)
        //    InitializeSerialPort("COM3");
        //    _serialPort.DataReceived += SerialPort_DataReceived;
        //    Console.WriteLine("Kết nối thành công với máy quét Denso GT20Q-SM!");
        //    Console.WriteLine("1. Bật đèn đỏ nháy");
        //    Console.WriteLine("2. Bật chế độ rung");
        //    Console.WriteLine("3. Bật cả đèn đỏ và rung");
        //    Console.WriteLine("4. Tắt đèn và rung");
        //    Console.WriteLine("0. Thoát");

        //    bool running = true;
        //    while (running)
        //    {
        //        Console.Write("Nhập lựa chọn của bạn: ");
        //        string choice = Console.ReadLine();

        //        switch (choice)
        //        {
        //            case "1":
        //                FlashRedLight();
        //                break;
        //            case "2":
        //                Vibrate();
        //                break;
        //            case "3":
        //                FlashRedLightAndVibrate();
        //                break;
        //            case "4":
        //                TurnOffIndicators();
        //                break;
        //            case "0":
        //                running = false;
        //                break;
        //            default:
        //                Console.WriteLine("Lựa chọn không hợp lệ!");
        //                break;
        //        }
        //    }

        //    // Đóng kết nối và giải phóng tài nguyên
        //    CloseSerialPort();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Lỗi: {ex.Message}");
        //}
    }

    private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string data = sp.ReadExisting();
        Console.WriteLine($"Nhận dữ liệu: {data}");
    }

    private static void InitializeSerialPort(string portName)
    {
        _serialPort = new SerialPort
        {
            PortName = portName,
            BaudRate = 19600,        // Tốc độ Baud (thay đổi nếu cần)
            DataBits = 8,           // Số bit dữ liệu
            Parity = Parity.None,   // Kiểm tra chẵn lẻ
            StopBits = StopBits.One // Số bit dừng
        };
        _serialPort.Open();
    }

    static void GenerateQRCode(string scannedData, string filePath)
    {
        try
        {
            // Khởi tạo QRCodeGenerator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(scannedData, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrCode.GetGraphic(5);
            // Lưu ảnh từ byte[] vào file PNG
            using (MemoryStream ms = new MemoryStream(qrCodeImage))
            {
                using (Bitmap qrBitmap = new Bitmap(ms))
                {
                    qrBitmap.Save(filePath, ImageFormat.Png);
                }
            }

            Console.WriteLine($"QR Code đã được lưu tại: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi tạo QR Code: {ex.Message}");
        }
    }




    private static void CloseSerialPort()
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }

    private static void FlashRedLight()
    {

        SendCommand("NG");
        Console.WriteLine("Đã bật đèn đỏ nháy!");
    }

    private static void Vibrate()
    {

        SendCommand("VO");
        Console.WriteLine("Đã bật chế độ rung!");
    }

    private static void FlashRedLightAndVibrate()
    {
        // Bật đèn đỏ trước
        FlashRedLight();

        // Đợi một chút
        Thread.Sleep(100);

        // Sau đó bật rung
        Vibrate();

        Console.WriteLine("Đã bật cả đèn đỏ và rung!");
    }


    private static void TurnOffIndicators()
    {

        SendCommand("LB");
        Console.WriteLine("Đã tắt tất cả đèn và rung!");
    }

    private static void SendCommand(string command)
    {
        if (_serialPort != null && _serialPort.IsOpen)
        {
            string fullCommand = command + "\r"; // Ví dụ: Thêm ký tự CR (Carriage Return)
            _serialPort.Write(fullCommand);
            Console.WriteLine($"Đã gửi lệnh: {fullCommand.Trim()}");
        }
        else
        {
            throw new InvalidOperationException("Cổng COM chưa được mở!");
        }
    }
    // Hàm gửi lệnh và nhận phản hồi (nếu có)
    private string SendCommandImage(string command)
    {
        if (!_serialPort.IsOpen)
        {
            throw new InvalidOperationException("Cổng COM chưa được mở.");
        }

        try
        {
            string fullCommand = command + "\r";
            _serialPort.Write(fullCommand);
            Console.WriteLine($"Đã gửi lệnh: {fullCommand.Trim()}");
            string response = _serialPort.ReadLine();  // Đọc phản hồi (có thể chỉ là ACK)
            Console.WriteLine($"Phản hồi: {response.Trim()}");
            return response;
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Hết thời gian chờ khi đọc phản hồi.");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi gửi/nhận lệnh: {ex.Message}");
            throw;
        }
    }
}