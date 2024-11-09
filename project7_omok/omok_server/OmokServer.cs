using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace omok_server
{
    internal class OmokServer
    {

        // 접속한 클라이언트 정보 저장하는 클래스
        private class Client
        {
            public TcpClient socket { get; set; } // 소켓 디스크립터
            public string nickname { get; set; } // 닉네임
            public STATUS status { get; set; }   // 상태 (대기중, 게임중)
            public string level { get; set; }

            public Client(TcpClient socket, string nickname, STATUS status, string level)
            {
                this.socket = socket;
                this.nickname = nickname;
                this.status = status;
                this.level = level;
            }
        }

        private List<Client> clientList = new List<Client>(); // 유저 정보 구조체 리스트
        private TcpListener server; // 서버 소켓을 생성하고 클라이언트 연결을 기다리는 역할
        private List<TcpClient> clients = new List<TcpClient>(); // 연결된 클라이언트를 저장
        private readonly object clientLock = new object(); // 다중 스레드에서 클라이언트 리스트 보호용 lock 객체
        private int roomNum = 1;

        enum ACT {LOGIN, USERLIST, MATCHING, PUTSTONE, RESTART, QUIT};
        enum STATUS {WAITING, PLAYING};
        enum STONE { NONE, BLACK, WHITE };


        // 서버 시작 메서드
        public void StartServer(string ip, int port)
        {
            server = new TcpListener(IPAddress.Parse(ip), port); // 지정된 IP와 포트로 서버 소켓 생성
            server.Start(); // 서버 시작
            Console.WriteLine($"서버가 {ip}:{port}에서 시작되었습니다.");

            // 클라이언트 연결을 기다리는 스레드 생성
            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.IsBackground = true; // 백그라운드 스레드로 설정
            acceptThread.Start(); // 스레드 시작
        }

        // 클라이언트 연결을 처리하는 메서드
        private void AcceptClients()
        {
            while (true)
            {
                TcpClient clientSocket = server.AcceptTcpClient(); // 클라이언트 연결을 기다림
                lock (clientLock) // 다중 스레드 환경에서 안전하게 클라이언트를 추가
                {
                    clients.Add(clientSocket); // 연결된 클라이언트를 리스트에 추가
                }
                Console.WriteLine($"클라이언트가 연결되었습니다. 현재 클라이언트 수: {clients.Count}");

                // 클라이언트 데이터 수신을 처리하는 스레드 생성
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.IsBackground = true; // 백그라운드 스레드로 설정
                clientThread.Start(); // 스레드 시작
            }
        }

        // 클라이언트와의 통신을 처리하는 메서드
        private void HandleClient(TcpClient clientSocket)
        {
            NetworkStream stream = clientSocket.GetStream(); // 클라이언트와의 데이터 송수신을 위한 스트림
            byte[] buffer = new byte[1024]; // 데이터를 저장할 버퍼
            while (true) // 클라이언트가 연결된 동안 계속 실행
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length); // 데이터를 읽음
                    if (bytesRead > 0) // 데이터가 있으면
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead); // UTF-8로 디코딩
                        Console.WriteLine($"수신: {data}"); // 수신된 데이터 출력

                        string[] parts = data.Split('/');                
                        int actType = int.Parse(parts[0]);
                        string senderNick = parts[1];
                        string msg = parts[2];
                        string receiverNick = parts[3];
                        
                        ServerOperate(clientSocket, actType, senderNick, msg, receiverNick);                  
                    }
                }
                catch
                {
                    lock (clientLock) // 클라이언트 리스트 보호
                    {
                        clients.Remove(clientSocket); // 연결이 끊긴 클라이언트를 리스트에서 제거
                        clientList.RemoveAll(c => c.socket == clientSocket);
                    }
                    Console.WriteLine("클라이언트가 연결을 종료했습니다.");
                    break; // 루프 종료
                }
            }
        }

        private void ServerOperate(TcpClient clientSocket, int type, string senderNick, string msg, string receiverNick)
        {
            ACT actType = (ACT)type;

            switch (actType)
            {
                case ACT.LOGIN:
                    string level = msg;
                    bool exists = clientList.Any(c => c.nickname == senderNick);
                    if (exists)
                    {
                        SendMessage(clientSocket, (int)ACT.LOGIN, "nickDup");
                        return;
                    }
                    else
                    {
                        SendMessage(clientSocket, (int)ACT.LOGIN, "loginSuccess");
                        clientList.Add(new Client(clientSocket, senderNick, STATUS.WAITING, level));
                        Console.WriteLine($"클라이언트 추가: {senderNick} {level}, 상태: 대기중");
                        SendUserListInfo(clientSocket, senderNick);
                    }
                    break;
                case ACT.USERLIST:
                    SendUserListInfo(clientSocket, senderNick);
                    break;
                case ACT.MATCHING:
                    SendMatchingInfo(clientSocket, senderNick, msg, receiverNick);
                    break;
                case ACT.PUTSTONE:
                    string putStoneInfo = msg;
                    foreach (Client client in clientList)
                    {
                        if (client.nickname == receiverNick)
                        {
                            SendMessage(client.socket, (int)ACT.PUTSTONE, putStoneInfo);
                        }
                    }
                    break;
                case ACT.RESTART:
                    foreach (Client client in clientList)
                    {
                        if (client.nickname == receiverNick)
                        {
                            SendMessage(client.socket, (int)ACT.RESTART);
                        }
                    }
                    break;
                case ACT.QUIT:
                    foreach (Client client in clientList)
                    {
                        if (client.nickname == receiverNick)
                        {
                            SendMessage(client.socket, (int)ACT.QUIT);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void SendMatchingInfo(TcpClient clientSocket, string senderNick, string msg, string receiverNick)
        {
            string action = msg;
            
            if (action == "request")
            {
                bool check = false;
                foreach (Client client in clientList)
                {
                    if (client.nickname == receiverNick)
                    {
                        SendMessage(client.socket, (int)ACT.MATCHING, "request", senderNick);
                        check = true;
                        break;
                    }
                }

                if (!check)
                {
                    SendMessage(clientSocket, (int)ACT.MATCHING, "none");
                }
            }
            else if (action == "deny")
            {
                foreach (Client client in clientList)
                {
                    if (client.nickname == receiverNick)
                    {
                        SendMessage(client.socket, (int)ACT.MATCHING, "deny", senderNick);
                        break;
                    }
                }
            }
            else if (action == "accept")
            {
                // 흑과 백을 랜덤으로 결정
                List<string> stones = new List<string> { "black", "white" };
                Random rand = new Random();
                int index;

                foreach (Client client in clientList)
                {
                    if (client.nickname == receiverNick || client.nickname == senderNick)
                    {
                        client.status = STATUS.PLAYING;
                        index = rand.Next(stones.Count); // 돌 랜덤 선택
                        SendMessage(client.socket, (int)ACT.MATCHING, "accept", stones[index]);
                        stones.RemoveAt(index); // 할당된 돌 제거
                    }
                }

            }
        }


        private void SendUserListInfo(TcpClient clientSocket, string nickname)
        {
            StringBuilder userData = new StringBuilder();
            foreach (Client client in clientList)
            {
                if (client.status == STATUS.WAITING && client.nickname != nickname)
                {
                    userData.AppendLine($"{client.nickname},{client.level}"); // 구분자로 연결
                }                
            }
            string finalData = userData.ToString().Trim();
            SendMessage(clientSocket, (int)ACT.USERLIST, finalData);
        }

        private void SendMessage(TcpClient clientSocket, int type, string msg="", string sender="")
        {
            lock (clientLock) // 다중 스레드 환경에서 클라이언트 리스트 보호
            {
                try
                {
                    NetworkStream stream = clientSocket.GetStream(); // 클라이언트와의 스트림 가져오기
                    string fullmsg = $"{type.ToString()}/{msg}/{sender}";
                    byte[] data = Encoding.UTF8.GetBytes(fullmsg); // 메시지를 UTF-8로 인코딩
                    stream.Write(data, 0, data.Length); // 데이터를 클라이언트에게 전송
                    Console.WriteLine($"발신 : {fullmsg}");
                }
                catch
                {
                    lock (clientLock) // 클라이언트 리스트 보호
                    {
                        clients.Remove(clientSocket); // 연결이 끊긴 클라이언트를 리스트에서 제거
                        clientList.RemoveAll(c => c.socket == clientSocket);
                    }
                    Console.WriteLine("클라이언트에게 메시지를 보내지 못했습니다.");
                }
                
            }
        }

        // 서버를 중지하는 메서드
        public void StopServer()
        {
            server.Stop(); // 서버 중지
            Console.WriteLine("서버가 종료되었습니다.");
        }
    }
}
