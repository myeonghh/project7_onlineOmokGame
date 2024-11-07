using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_omok
{
    public partial class OmokMain : Form
    {
        private int margin = 40;
        private int 눈Size = 30;
        private int 돌Size = 28;
        private int 화점Size = 10;

        Graphics g;
        Pen pen;
        Brush wBrush, bBrush;

        enum STONE {none, black, white};
        STONE[,] stoneBoard = new STONE[19, 19];
        bool flag = false; // false:흑, true:백

        public OmokMain()
        {
            InitializeComponent();

            panel1.BackColor = Color.Orange;
            pen = new Pen(Color.Black);
            wBrush = new SolidBrush(Color.White);
            bBrush = new SolidBrush(Color.Black);

            panel1.ClientSize = new Size(2 * margin + 18 * 눈Size, 2 * margin + 18 * 눈Size + menuStrip1.Height);

            // panel1의 Paint 이벤트에 DrawBoard 메서드 연결
            panel1.Paint += panel1_Paint;

            //// panel1의 초기 페인팅 강제 호출
            //panel1.Invalidate();
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
            int x = (e.X - margin + 눈Size / 2) / 눈Size;
            int y = (e.Y - margin + 눈Size / 2) / 눈Size;

            if (stoneBoard[x, y] != STONE.none)
                return;

            Rectangle r = new Rectangle(margin + 눈Size * x - 돌Size / 2, margin + 눈Size * y - 돌Size / 2, 돌Size, 돌Size);

            // 검은돌 차례
            if (flag == false)
            {
                Bitmap bmp = new Bitmap("../../images/black.png");
                g.DrawImage(bmp, r);
                flag = true;
                stoneBoard[x, y] = STONE.black;
            }
            // 흰돌 차례
            else
            {
                Bitmap bmp = new Bitmap("../../images/white.png");
                g.DrawImage(bmp, r);
                flag = false;
                stoneBoard[x, y] = STONE.white;
            }

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
            DialogResult res = MessageBox.Show(stoneBoard[x, y].ToString().ToUpper()
              + " Wins!\n새로운 게임을 시작할까요?", "게임 종료", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
                NewGame();
            else if (res == DialogResult.No)
                this.Close();
        }

        // 새로운 게임을 시작(초기화)
        private void NewGame()
        {
            flag = false;

            for (int x = 0; x < 19; x++)
                for (int y = 0; y < 19; y++)
                    stoneBoard[x, y] = STONE.none;

            panel1.Refresh();
            DrawBoard();
            DrawStones();
        }

        private void DrawBoard()
        {
            for (int i = 0; i < 19; i++)
            {
                g = panel1.CreateGraphics();
                g.DrawLine(pen, new Point(margin + i * 눈Size, margin),
                    new Point(margin + i * 눈Size, margin + 18 * 눈Size)); // 세로선
                g.DrawLine(pen, new Point(margin, margin + i * 눈Size),
                    new Point(margin + 18 * 눈Size, margin + i * 눈Size)); // 가로선
            }

            // 화점 그리기 (3, 9, 15)
            for (int x = 3; x <= 15; x+=6)
                for (int y = 3; y <= 15; y+=6)
                {
                    g.FillEllipse(bBrush, 
                        margin + 눈Size * x - 화점Size/2, 
                        margin + 눈Size * y - 화점Size/2, 
                        화점Size, 화점Size);
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void DrawStones()
        {
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    if (stoneBoard[x, y] == STONE.black)
                    {
                        Bitmap bmp = new Bitmap("../../images/black.png");
                        Rectangle r = new Rectangle(
                            margin + 눈Size * x - 돌Size / 2,
                            margin + 눈Size * y - 돌Size / 2,
                            돌Size, 돌Size);
                        g.DrawImage(bmp, r);
                    }
                    else if (stoneBoard[x, y] == STONE.white)
                    {
                        Bitmap bmp = new Bitmap("../../images/white.png");
                        Rectangle r = new Rectangle(
                            margin + 눈Size * x - 돌Size / 2,
                            margin + 눈Size * y - 돌Size / 2,
                            돌Size, 돌Size);
                        g.DrawImage(bmp, r);
                    }
                }
            }
        }
    }
}
