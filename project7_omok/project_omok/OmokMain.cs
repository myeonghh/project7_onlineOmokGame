using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace project_omok
{
    public partial class OmokMain : Form
    {
        private TcpClientManager clientManager; // tcp통신 클래스 객체 선언
        private bool connectSuccess;
        private string ip = "127.0.0.1";
        private int port = 12345;
        private string myNickname;
        private string myLevel;
        private string opponentNickname;
        private string opponentLevel;

        enum ACT {LOGIN, USERLIST, MATCHING, PUTSTONE, RESTART, QUIT, GIVEUP, CHAT, LOGOUT};

        private int margin = 40;
        private int gridSize = 30;
        private int stoneSize = 28;
        private int dotSize = 10;

        Graphics g;
        Pen pen;
        Brush wBrush, bBrush;

        enum STONE {NONE, BLACK, WHITE};
        STONE[,] stoneBoard = new STONE[19, 19];
        STONE myStone = STONE.NONE;
        bool myTurn = false;
        bool gameover = false;
        bool myRestart = false;
        bool opponentRestart = false;
        bool showSequence = false;

        int stoneCnt = 1; // 수순
        Font font = new Font("맑은 고딕", 10);  // 수순 출력용
        int[,] stoneSequenceList = new int[19, 19];

        public OmokMain()
        {
            InitializeComponent();
            panel1.BackColor = Color.BurlyWood;
            pen = new Pen(Color.Black);
            wBrush = new SolidBrush(Color.White);
            bBrush = new SolidBrush(Color.Black);

            panel1.ClientSize = new Size(2 * margin + 18 * gridSize, 2 * margin + 18 * gridSize + menuStrip1.Height);

            // panel1의 Paint 이벤트에 DrawBoard 메서드 연결
            panel1.Paint += panel1_Paint;
            gameoverPanel.Visible = false;
            this.Cursor = Cursors.Default;

            // 서버 연결
            clientManager = new TcpClientManager(); // 서버 IP와 포트
            connectSuccess = clientManager.Connect(ip, port);
            if (connectSuccess)
            {
                severConnectLabel.Text = $"서버연결 성공  [ IP: {ip} PORT: {port} ]";
                clientManager.OnDataReceived += HandleServerData; // 서버에서 수신한 데이터 처리
            }
            else
            {
                severConnectLabel.Text = "서버연결 실패";
            }
        }

        // 수순을 돌의 중앙에 써줍니다
        private void DrawStoneSequence(int cnt, Brush color, Rectangle r)
        {
            if (cnt == 0)
                return;

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            g.DrawString(cnt.ToString(), font, color, r, stringFormat);
        }

        private void UpdateUI(Action uiAction)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(uiAction);
            }
            else
            {
                uiAction();
            }
        }

        private void HandleServerData(string data)
        {
            // 서버에서 받은 데이터 처리
            string[] parts = data.Split('/');
            ACT actType = (ACT)int.Parse(parts[0]);
            string msg = parts[1];
            string senderNick = parts[2];

            UpdateUI(() =>
            {
                switch (actType)
                {
                    case ACT.LOGIN:
                        if (msg == "nickDup")
                        {
                            MessageBox.Show(this, "이미 사용중인 닉네임입니다.");
                        }
                        else
                        {
                            nickNameText.Clear();
                            levelComboBox.SelectedIndex = 0;
                            tabControlMain.SelectedIndex = 1;                       
                        }
                        break;
                    case ACT.USERLIST:
                        MakeUserList(msg);             
                        break;
                    case ACT.MATCHING:
                        MatchingControl(msg, senderNick);
                        break;
                    case ACT.PUTSTONE:
                        OpponentStoneControl(msg);
                        break;
                    case ACT.RESTART:
                        opponentRestart = true;
                        if (myRestart)
                        {
                            myStone = myStone == STONE.WHITE ? STONE.BLACK : STONE.WHITE;
                            NewGame();
                        }                       
                        break;
                    case ACT.QUIT:
                        clientManager.SendData((int)ACT.USERLIST, myNickname);
                        chatRichBox.Clear();
                        tabControlMain.SelectedIndex = 1;
                        break;
                    case ACT.GIVEUP:
                        turnLabel.Text = "게임종료";
                        gameover = true;
                        gameoverImg.Image = Properties.Resources.win;
                        gameoverPanel.Visible = true;
                        break;
                    case ACT.CHAT:
                        chatRichBox.AppendText($"[{senderNick}] : {msg}\n");
                        chatRichBox.SelectionStart = chatRichBox.Text.Length; // 텍스트의 끝으로 커서를 이동
                        chatRichBox.ScrollToCaret(); // 커서 위치로 스크롤
                        break;
                    default:
                        break;
                }
            });
        }

        private void OpponentStoneControl(string msg)
        {
            //$"{(int)STONE.BLACK},{x},{y}"
            string[] msgParts = msg.Split(',');
            STONE stone = (STONE)int.Parse(msgParts[0]);
            int x = int.Parse(msgParts[1]);
            int y = int.Parse(msgParts[2]);

            Rectangle r = new Rectangle(margin + gridSize * x - stoneSize / 2, margin + gridSize * y - stoneSize / 2, stoneSize, stoneSize);

            // 상대방이 흑돌인 경우
            if (stone == STONE.BLACK)
            {
                Bitmap bmp = new Bitmap("../../images/black.png");
                g.DrawImage(bmp, r);
                myTurn = true;
                stoneBoard[x, y] = STONE.BLACK;
                stoneSequenceList[x, y] = stoneCnt;
                if (showSequence)
                {
                    DrawStoneSequence(stoneCnt, Brushes.White, r);
                }
                stoneCnt++;
                
            }
            // 상대방이 백돌인 경우
            else if (stone == STONE.WHITE)
            {
                Bitmap bmp = new Bitmap("../../images/white.png");
                g.DrawImage(bmp, r);
                myTurn = true;
                stoneBoard[x, y] = STONE.WHITE;
                stoneSequenceList[x, y] = stoneCnt;
                if (showSequence)
                {
                    DrawStoneSequence(stoneCnt, Brushes.Black, r);
                }
                stoneCnt++;
            }
            turnLabel.Text = "내 차례입니다.";
            checkOmok(x, y);

        }

        private void MatchingControl(string msg, string sender)
        {
            if (msg == "none")
            {
                MessageBox.Show(this, "해당 유저가 존재하지 않거나 게임중입니다." +
                    "\n\t   유저목록을 새로고침하고 다시 선택하세요.");
            }
            else if (msg == "request")
            {
                string[] senderParts = sender.Split(',');
                string senderNick = senderParts[0];
                string senderLevel = senderParts[1];

                DialogResult res = MessageBox.Show(this, $"'[{senderLevel}] {senderNick}'님이 당신에게 대전을 요청했습니다.\n\t   수락하시겠습니까?",
                    "대전 요청", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    clientManager.SendData((int)ACT.MATCHING, myNickname, "accept", senderNick);
                    opponentNickname = senderNick;
                    opponentLevel = senderLevel;
                }                    
                else if (res == DialogResult.No)
                {
                    clientManager.SendData((int)ACT.MATCHING, myNickname, "deny", senderNick);
                }                    
            }
            else if (msg == "deny")
            {
                MessageBox.Show(this, $"'{sender}'님이 당신의 요청을 거절했습니다.");
            }
            else if (msg == "accept")
            {
                string stone = sender;
                myStone = stone == "white" ? STONE.WHITE : STONE.BLACK;               
                SelectProfileImage(playerImg1, myLevel);
                SelectProfileImage(playerImg2, opponentLevel);               
                playerLabel1.Text = $"[{myLevel}]  {myNickname}";
                playerLabel2.Text = $"[{opponentLevel}]  {opponentNickname}";               
                NewGame();
                tabControlMain.SelectedIndex = 2;                
            }
        }

        private void SelectProfileImage(PictureBox pictureBox, string level)
        {
            switch (level)
            {
                case "초보":
                    pictureBox.Image = Properties.Resources.kid;
                    break;
                case "중수":
                    pictureBox.Image = Properties.Resources.person;
                    break;
                case "고수":
                    pictureBox.Image = Properties.Resources.sedol;
                    break;
                case "알파고":
                    pictureBox.Image = Properties.Resources.alphago;
                    break;
                default:
                    break;
            }
        }

        private void MakeUserList(string msg)
        {
            userListView.Items.Clear(); // 기존 유저 목록 초기화
            if (string.IsNullOrEmpty(msg)) return;
          
            string[] userInfos = msg.Split('\n');
            foreach (string userInfo in userInfos)
            {
                string[] userDetails = userInfo.Split(',');

                string userNickname = userDetails[0];
                string userLevel = userDetails[1];
                Console.WriteLine($"User: {userNickname}, {userLevel}");

                // ListViewItem 생성 및 추가
                ListViewItem item = new ListViewItem(userNickname); // 첫 번째 컬럼 (닉네임)
                item.SubItems.Add(userLevel);                       // 두 번째 컬럼 (실력)
                userListView.Items.Add(item);
            }
        }
        private void sequenceShow_Click(object sender, EventArgs e)
        {
            showSequence = true;
            DrawStones();
        }

        private void sequenceHide_Click(object sender, EventArgs e)
        {
            showSequence = false;
            DrawStones();
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            clientManager.SendData((int)ACT.LOGOUT, myNickname);
            showSequence = false;
            tabControlMain.SelectedIndex = 0;
        }

        private void sendChatBtn_Click(object sender, EventArgs e)
        {
            string message = chatTextBox.Text.Trim(); // 입력된 텍스트 가져오기

            if (string.IsNullOrEmpty(message))
                return;

            clientManager.SendData((int)ACT.CHAT, myNickname, message, opponentNickname);
            // RichTextBox에 자신의 메시지 추가
            chatRichBox.AppendText($"[{myNickname}] : {message}\n");
            chatRichBox.SelectionStart = chatRichBox.Text.Length; // 텍스트의 끝으로 커서를 이동
            chatRichBox.ScrollToCaret(); // 커서 위치로 스크롤
            chatTextBox.Clear(); // TextBox 내용 비우기


        }

        private void chatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // 엔터 입력 시 기본 행동(삑 소리)을 방지                
                string message = chatTextBox.Text.Trim(); // 입력된 텍스트 가져오기

                if (string.IsNullOrEmpty(message))
                    return;

                clientManager.SendData((int)ACT.CHAT, myNickname, message, opponentNickname);
                // RichTextBox에 자신의 메시지 추가
                chatRichBox.AppendText($"[{myNickname}] : {message}\n");
                chatRichBox.SelectionStart = chatRichBox.Text.Length; // 텍스트의 끝으로 커서를 이동
                chatRichBox.ScrollToCaret(); // 커서 위치로 스크롤
                chatTextBox.Clear(); // TextBox 내용 비우기
            }
        }

        // 접속 버튼 눌렀을 때 실행되는 메서드
        private void serverJoinBtn_Click(object sender, EventArgs e)
        {
            myNickname = nickNameText.Text.Trim();
            myLevel = levelComboBox.Text.Trim();


            if (string.IsNullOrEmpty(myNickname))
            {
                MessageBox.Show(this, "닉네임을 입력하세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                clientManager.SendData((int)ACT.LOGIN, myNickname, myLevel);
            }
        }

        private void giveUpBtn_Click(object sender, EventArgs e)
        {
            if (gameover)
                return;

            clientManager.SendData((int)ACT.GIVEUP, myNickname, "giveUp", opponentNickname);
            turnLabel.Text = "게임종료";
            gameover = true;
            gameoverImg.Image = Properties.Resources.lose;
            gameoverPanel.Visible = true;
        }

        private void restartBtn_Click(object sender, EventArgs e)
        {
            myRestart = true;
            clientManager.SendData((int)ACT.RESTART, myNickname, "restart", opponentNickname);
            if (opponentRestart)
            {
                myStone = myStone == STONE.WHITE ? STONE.BLACK : STONE.WHITE;               
                NewGame();
                return;
            }
            restartWaitLabel.Text = "상대 선택을 기다리는 중..";
        }

        private void toBackBtn_Click(object sender, EventArgs e)
        {
            clientManager.SendData((int)ACT.QUIT, myNickname, "quit", opponentNickname);
            clientManager.SendData((int)ACT.USERLIST, myNickname);
            chatRichBox.Clear();
            tabControlMain.SelectedIndex = 1;

        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            clientManager.SendData((int)ACT.USERLIST, myNickname);
        }

        private void matchingBtn_Click(object sender, EventArgs e)
        {
            // 유저리스트뷰에서 선택된 항목이 있는지 확인
            if (userListView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = userListView.SelectedItems[0];
                string selectedUserNick = selectedItem.Text;
                opponentLevel = selectedItem.SubItems[1].Text;
                opponentNickname = selectedUserNick;
                clientManager.SendData((int)ACT.MATCHING, $"{myNickname},{myLevel}", "request", selectedUserNick);                
            }
            else
            {
                MessageBox.Show(this, "대전 할 유저를 선택하세요!");
            }

        }

        // panel1의 Paint 이벤트 핸들러
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            DrawBoard();
            DrawStones();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // e.X, e.Y 는 마우스에서 전달하는 좌표
            // x, y 는 변환된 stoneBoard 좌표
            int x = (e.X - margin + gridSize / 2) / gridSize;
            int y = (e.Y - margin + gridSize / 2) / gridSize;

            if (stoneBoard[x, y] != STONE.NONE)
                return;
            if (!myTurn)
                return;
            if (gameover)
                return;

            Rectangle r = new Rectangle(margin + gridSize * x - stoneSize / 2, margin + gridSize * y - stoneSize / 2, stoneSize, stoneSize);

            // 검은돌 차례
            if (myStone == STONE.BLACK)
            {
                Bitmap bmp = new Bitmap("../../images/black.png");
                g.DrawImage(bmp, r);
                myTurn = false;
                stoneBoard[x, y] = STONE.BLACK;
                stoneSequenceList[x, y] = stoneCnt;
                if (showSequence)
                {
                    DrawStoneSequence(stoneCnt, Brushes.White, r);
                }
                stoneCnt++;
                clientManager.SendData((int)ACT.PUTSTONE, myNickname, $"{(int)STONE.BLACK},{x},{y}", opponentNickname);
            }
            // 흰돌 차례
            else if (myStone == STONE.WHITE)
            {
                Bitmap bmp = new Bitmap("../../images/white.png");
                g.DrawImage(bmp, r);
                myTurn = false;
                stoneBoard[x, y] = STONE.WHITE;
                stoneSequenceList[x, y] = stoneCnt;
                if (showSequence)
                {
                    DrawStoneSequence(stoneCnt, Brushes.Black, r);
                }
                stoneCnt++;
                clientManager.SendData((int)ACT.PUTSTONE, myNickname, $"{(int)STONE.WHITE},{x},{y}", opponentNickname);
            }
            turnLabel.Text = "상대 차례입니다.";
            checkOmok(x, y);
        }

        // 오목인지 체크하는 메소드
        private void checkOmok(int x, int y)
        {
            int cnt = 1;

            // 오른쪽 방향
            for (int i = x + 1; i <= 18; i++)
                if (stoneBoard[i, y] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            // 왼쪽방향
            for (int i = x - 1; i >= 0; i--)
                if (stoneBoard[i, y] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            if (cnt == 5)
            {
                OmokComplete(x, y);
                return;
            }

            cnt = 1;

            // 아래 방향
            for (int i = y + 1; i <= 18; i++)
                if (stoneBoard[x, i] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            // 위 방향
            for (int i = y - 1; i >= 0; i--)
                if (stoneBoard[x, i] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            if (cnt == 5)
            {
                OmokComplete(x, y);
                return;
            }

            cnt = 1;

            // 대각선 오른쪽 위방향
            for (int i = x + 1, j = y - 1; i <= 18 && j >= 0; i++, j--)
                if (stoneBoard[i, j] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            // 대각선 왼쪽 아래 방향
            for (int i = x - 1, j = y + 1; i >= 0 && j <= 18; i--, j++)
                if (stoneBoard[i, j] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            if (cnt == 5)
            {
                OmokComplete(x, y);
                return;
            }

            cnt = 1;

            // 대각선 왼쪽 위방향
            for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--, j--)
                if (stoneBoard[i, j] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            // 대각선 오른쪽 아래 방향
            for (int i = x + 1, j = y + 1; i <= 18 && j <= 18; i++, j++)
                if (stoneBoard[i, j] == stoneBoard[x, y])
                    cnt++;
                else
                    break;

            if (cnt == 5)
            {
                OmokComplete(x, y);
                return;
            }
        }

        // 오목이 되었을 떄 처리하는 루틴 
        private void OmokComplete(int x, int y)
        {
            turnLabel.Text = "게임종료";
            gameover = true;
            gameoverImg.Image = myStone == stoneBoard[x, y] ? Properties.Resources.win : Properties.Resources.lose;
            gameoverPanel.Visible = true;
        }

        // 새로운 게임을 시작(초기화)
        private void NewGame()
        {
            restartWaitLabel.Text = "";            
            gameoverPanel.Visible = false;           
            gameover = false;
            myRestart = false;
            opponentRestart = false;
            myTurn = myStone == STONE.WHITE ? false : true;
            turnLabel.Text = $"{(myTurn ? "내 차례입니다." : "상대 차례입니다.")}";
            player1StoneImg.Image = myStone == STONE.WHITE ? Properties.Resources.white : Properties.Resources.black;
            player2StoneImg.Image = myStone == STONE.WHITE ? Properties.Resources.black : Properties.Resources.white;
            stoneCnt = 1;

            for (int x = 0; x < 19; x++)
                for (int y = 0; y < 19; y++)
                    stoneBoard[x, y] = STONE.NONE;

            for (int x = 0; x < 19; x++)
                for (int y = 0; y < 19; y++)
                    stoneSequenceList[x, y] = 0;

            panel1.Refresh();
            DrawBoard();
            DrawStones();
        }

        private void DrawBoard()
        {
            for (int i = 0; i < 19; i++)
            {
                g = panel1.CreateGraphics();
                g.DrawLine(pen, new Point(margin + i * gridSize, margin),
                    new Point(margin + i * gridSize, margin + 18 * gridSize)); // 세로선
                g.DrawLine(pen, new Point(margin, margin + i * gridSize),
                    new Point(margin + 18 * gridSize, margin + i * gridSize)); // 가로선
            }

            // 화점 그리기 (3, 9, 15)
            for (int x = 3; x <= 15; x+=6)
                for (int y = 3; y <= 15; y+=6)
                {
                    g.FillEllipse(bBrush, 
                        margin + gridSize * x - dotSize/2, 
                        margin + gridSize * y - dotSize/2, 
                        dotSize, dotSize);
                    
                }
        }

        private void DrawStones()
        {
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    if (stoneBoard[x, y] == STONE.BLACK)
                    {
                        Bitmap bmp = new Bitmap("../../images/black.png");
                        Rectangle r = new Rectangle(
                            margin + gridSize * x - stoneSize / 2,
                            margin + gridSize * y - stoneSize / 2,
                            stoneSize, stoneSize);
                        g.DrawImage(bmp, r);
                        if (showSequence)
                        {
                            int cnt = stoneSequenceList[x, y];
                            DrawStoneSequence(cnt, Brushes.White, r);
                        }

                    }
                    else if (stoneBoard[x, y] == STONE.WHITE)
                    {
                        Bitmap bmp = new Bitmap("../../images/white.png");
                        Rectangle r = new Rectangle(
                            margin + gridSize * x - stoneSize / 2,
                            margin + gridSize * y - stoneSize / 2,
                            stoneSize, stoneSize);
                        g.DrawImage(bmp, r);
                        if (showSequence)
                        {
                            int cnt = stoneSequenceList[x, y];
                            DrawStoneSequence(cnt, Brushes.Black, r);
                        }
                    }
                }
            }
        }
    }
}
