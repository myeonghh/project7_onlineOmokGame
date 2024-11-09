using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace project_omok
{
    internal class TcpClientManager
    {
        private TcpClient client; // 서버와 연결하는 소켓
        private NetworkStream stream; // 데이터 송수신 스트림
        private Thread receiveThread; // 데이터 수신을 처리하는 스레드

        // 수신된 데이터를 전달하기 위한 이벤트
        public event Action<string> OnDataReceived;

        // 생성자
        public TcpClientManager()
        {
        }

        // 서버에 연결
        public bool Connect(string serverIp, int serverPort)
        {
            try
            {
                client = new TcpClient(); // TCP 클라이언트 생성
                client.Connect(serverIp, serverPort); // 서버에 연결
                stream = client.GetStream(); // 서버와의 데이터 송수신을 위한 스트림 가져오기
                Console.WriteLine($"서버에 연결되었습니다. {serverIp}:{serverPort}");

                // 데이터를 수신하는 스레드 시작
                receiveThread = new Thread(ReceiveData);
                receiveThread.IsBackground = true; // 백그라운드 스레드로 설정
                receiveThread.Start(); // 스레드 시작
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"서버에 연결할 수 없습니다: {ex.Message}");
                return false;
            }
        }

        // 데이터를 서버로 전송
        public void SendData(int actType, string sender="", string msg="", string receiver="")
        {
            try
            {
                string data = $"{actType.ToString()}/{sender}/{msg}/{receiver}"; 
                byte[] bytes = Encoding.UTF8.GetBytes(data); // UTF-8로 인코딩
                stream.Write(bytes, 0, bytes.Length); // 데이터를 서버로 전송
            }
            catch (Exception ex)
            {
                Console.WriteLine($"데이터 전송 실패: {ex.Message}");
            }
        }

        // 데이터를 서버로부터 수신
        private void ReceiveData()
        {
            byte[] buffer = new byte[1024]; // 데이터를 저장할 버퍼
            while (true) // 연결된 동안 계속 실행
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length); // 서버로부터 데이터 읽기
                    if (bytesRead > 0) // 데이터가 있으면
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead); // UTF-8로 디코딩
                        OnDataReceived?.Invoke(data); // 데이터 수신 이벤트 호출
                    }
                }
                catch
                {
                    Console.WriteLine("서버 연결이 종료되었습니다."); // 연결 종료 로그 출력
                    break; // 루프 종료
                }
            }
        }

        // 연결 종료
        public void Disconnect()
        {
            try
            {
                receiveThread?.Abort(); // 수신 스레드 종료
                stream?.Close(); // 스트림 닫기
                client?.Close(); // 클라이언트 소켓 닫기
                Console.WriteLine("서버와의 연결이 종료되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"연결 종료 실패: {ex.Message}");
            }
        }
    }
}
