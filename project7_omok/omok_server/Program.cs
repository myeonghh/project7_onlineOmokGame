using System;
using System.Collections.Generic;
using System.Net; 
using System.Net.Sockets;
using System.Text; 
using System.Threading;

namespace omok_server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 오목 서버 시작
            OmokServer server = new OmokServer();
            server.StartServer("127.0.0.1", 12345); // 로컬 IP와 포트 설정

            Console.WriteLine("서버가 실행 중입니다. 종료하려면 Enter 키를 누르세요..");
            Console.ReadLine(); // Enter 입력 대기

            server.StopServer(); // 서버 종료
            Console.WriteLine("서버가 종료되었습니다.");
        }
    }
}
