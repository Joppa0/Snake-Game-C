using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake_Game
{
    public partial class Form1 : Form
    {
        private string direction = "right";

        private int squareSize = 25;

        private List<Square> Snake = new List<Square>();
        private Square apple = new Square();

        private int maxWidth;
        private int maxHeight;

        private int score;
        private int highScore;

        private bool gameOver = true;

        Random rand = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        //Registrerar om en piltangent trycks ned, kollar vilken, och ställer sedan in ormen i rätt riktning
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && direction != "right")
            {
                direction = "left";
            }

            if (e.KeyCode == Keys.Right && direction != "left")
            {
                direction = "right";
            }

            if (e.KeyCode == Keys.Up && direction != "down")
            {
                direction = "up";
            }

            if (e.KeyCode == Keys.Down && direction != "up")
            {
                direction = "down";
            }

            //Startar ny runda om den nuvarande är över och enter trycks
            if (e.KeyCode == Keys.Enter && gameOver)
            {
                RestartGame();

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        //Kallar metoden RestartGame när startknappen trycks
        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        //Uppdaterar skärmen och ormens rörelse varje gång timern tickar ned
        private void GameTimerEvent(object sender, EventArgs e)
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Rör huvudet i rätt riktning
                if (i == 0)
                {
                    switch (direction)
                    {
                        case "left":
                            Snake[i].X--;
                            break;
                        case "right":
                            Snake[i].X++;
                            break;
                        case "up":
                            Snake[i].Y--;
                            break;
                        case "down":
                            Snake[i].Y++;
                            break;
                    }

                    //Avslutar rundan om ormen träffar väggen
                    if (Snake[i].X > maxWidth || Snake[i].X < 0 || Snake[i].Y > maxHeight || Snake[i].Y < 0)
                    {
                        GameOver();
                    }

                    //Kallar metoden EatApple om ormen kolliderar med ett äpple
                    if (Snake[i].X == apple.X && Snake[i].Y == apple.Y)
                    {
                        EatApple();
                    }
                    
                    //Kallar GameOver om ormens huvud rör någon del av svansen
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X && Snake[i].Y == Snake[j].Y)
                        {
                            GameOver();
                        }
                    }
                }

                //Rör ormens svans
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }

            //Ritar om hela canvas
            picCanvas.Invalidate();
        }

        //Ritar ut canvas
        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColor;
            Pen pen = new Pen(Color.White);

            //Ritar rutnätet
            for (int i = 0; i < picCanvas.Size.Height / squareSize; i++)
            {
                canvas.DrawLine(pen, i * squareSize, 0, i * squareSize, picCanvas.Height);
                canvas.DrawLine(pen, 0, i * squareSize, picCanvas.Width, i * squareSize);
            }

            //Ritar ut ormen
            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColor = Brushes.DarkGreen;
                }

                else
                {
                    snakeColor = Brushes.SpringGreen;
                }

                canvas.FillRectangle(snakeColor, Snake[i].X * squareSize, Snake[i].Y * squareSize, squareSize, squareSize);
            }

            //Ritar ut äpple
            canvas.FillRectangle(Brushes.Red, new Rectangle
            (
            apple.X * squareSize,
            apple.Y * squareSize,
            squareSize, squareSize
            ));
        }

        //Startar en ny runda
        private void RestartGame()
        {
            gameOver = false;

            maxWidth = picCanvas.Width / squareSize - 1;
            maxHeight = picCanvas.Height / squareSize - 1;

            direction = "right";

            //Alla objekt i listan för ormens huvud och svans raderas
            Snake.Clear();

            startButton.Enabled = false;
            RestartText.Enabled = false;
            RestartText.Visible = false;

            //Ställer om hur snabbt ormen rör sig
            gameTimer.Interval = 70; 

            //Ställer om poäng
            score = 0;
            txtScore.Text = "Score: " + score;

            //Ritar ut huvud och lägger till det i listan
            Square head = new Square { X = 0, Y = 0 };
            Snake.Add(head);

            //Ritar ut kropp och lägger till det i listan
            for (int i = 0; i < 6; i++)
            {
                Square body = new Square();
                Snake.Add(body);
            }

            SpawnApple();

            gameTimer.Start();
        }

        //Lägger ut ett äpple i en slumpmässig ruta
        private void SpawnApple()
        {
            bool badSpawn;
            int spawnX, spawnY;

            do
            {
                badSpawn = false;

                spawnX = rand.Next(2, maxWidth);
                spawnY = rand.Next(2, maxHeight);

                for (int i = 0; i < Snake.Count; i++)
                {
                    if (Snake[i].X == spawnX && Snake[i].Y == spawnY)
                    {
                        badSpawn = true;
                    }
                }
            } while (badSpawn == true);

            apple = new Square { X = spawnX, Y = spawnY };
        }

        private void EatApple()
        {
            //Poäng ökas när ett äpple äts
            score++;
            txtScore.Text = "Score: " + score;

            //Skapar ny del av svansen
            Square body = new Square
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);

            SpawnApple();

            //Spelet blir snabbare och snabbare för varje äpple som äts
            if (gameTimer.Interval > 10)
            {
                gameTimer.Interval--;
            }
        }

        //Lägger till poäng till highScore och möjliggör start av en ny runda
        private void GameOver()
        {
            gameTimer.Stop();
            startButton.Enabled = true;
            RestartText.Visible = true;
            RestartText.Enabled = true;

            gameOver = true;

            if (score > highScore)
            {
                highScore = score;

                txtHighScore.Text = "High Score: " + Environment.NewLine + highScore;
                txtHighScore.ForeColor = Color.Maroon;
                txtHighScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }
    }
}