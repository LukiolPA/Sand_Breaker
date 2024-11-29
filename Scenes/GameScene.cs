using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sand_Breaker.GameObjects;
using Sand_Breaker.Services;
using Sand_Breaker.UI;
using System;
using System.Linq;

namespace Sand_Breaker.Scenes
{
    public class GameScene : Scene
    {
        private Random rng;
        private GameController gameController = ServiceLocator.Get<GameController>();

        private IAssetsService assets;
        private Rectangle leftWall;
        private Rectangle rightWall;
        private Rectangle obstaclesArea;
        private Rectangle dangerStartingArea;


        private static double ballCanMoveProba = 0.15;
        private static int initialNbBalls = 2;
        private static int nbBalls = initialNbBalls;
        private float ballRespawnDelay;
        private float ballRespawnTimer;
        private static double initialLuck = 0.1;
        private static double luck = initialLuck;
        private bool Pause;


        public Rectangle ExitArea;
        public Rectangle LevelBounds;
        public Rectangle BallPlacingArea;
        public BonusType? GatheredBonus = null;


        public GameScene()
        {
            rng = new Random();
            assets = ServiceLocator.Get<IAssetsService>();
            Pause = false;
            ballRespawnDelay = 7f;

            ExitArea = new Rectangle(screen.LeftBorder, screen.TopBorder, screen.Width, (int)(screen.Height*0.1));
            LevelBounds = new Rectangle((int)(screen.LeftBorder + 0.2 * screen.Width), screen.TopBorder+ExitArea.Height, (int)(screen.Width * 0.6), screen.Height-ExitArea.Height);
            leftWall = new Rectangle(screen.LeftBorder, LevelBounds.Top, (screen.Width-LevelBounds.Width)/2, LevelBounds.Height);
            rightWall = new Rectangle(LevelBounds.Right, LevelBounds.Top, (screen.Width - LevelBounds.Width) / 2, LevelBounds.Height);
            obstaclesArea = new Rectangle(LevelBounds.Location, new Point(LevelBounds.Width, (int)(screen.Height * 0.4f)));
            dangerStartingArea = new Rectangle(LevelBounds.Left, (int)(screen.Height*0.9f), LevelBounds.Width, screen.Height);
            BallPlacingArea = new Rectangle((screen.TopLeftCorner + new Vector2(screen.Width * 0.3f, screen.Height * 0.5f)).ToPoint(), new Point((int)(screen.Width * 0.4f), (int)(screen.Height * 0.3f)));
        }


        public override void Load(params object[] data)
        {
            if(data.Length == 1)
            {
                if (data[0] is BonusType) ApplyBonus((BonusType)data[0]);
                else throw new InvalidOperationException("data sent when loading function should be a BonusType");
            }
            CreateScenery();
            CreateGameplayObjects();
        }


        private void ApplyBonus(BonusType bonus)
        {
            switch (bonus)
            {
                case BonusType.PaddleFaster:
                    Paddle.GainBonusSpeed(new Vector2(100, 16));
                    break;
                case BonusType.PaddleBigger:
                    Paddle.GainBonusSize(1.2f);
                    break;
                case BonusType.BallFaster:
                    Ball.GainBonusSpeed(50);
                    break;
                case BonusType.BallBigger:
                    Ball.GainBonusSize(7);
                    break;
                case BonusType.MoreShells:
                    nbBalls += 1;
                    break;
                case BonusType.MoreLuck:
                    luck = Math.Min(luck + 0.2, 1);
                    break;
                case BonusType.MoreDamage:
                    Ball.GainDamage(1);
                    break;
                default:
                    throw new ArgumentException($"The bonus named {bonus.ToString()} was not implemented");
            }
        }

        public static void RemoveAllBonus()
        {
            luck = initialLuck;
            nbBalls = initialNbBalls;
            Paddle.RemoveAllBonus();
            Ball.RemoveAllBonus();
        }

        private void CreateScenery()
        {
            FillRectangleWith(ExitArea, "Sprites/SandTexture", 0.4f);
            AddObject(new Exit(this, ExitArea.Location.ToVector2(), ExitArea.Size.ToVector2()));
            FillRectangleWith(leftWall, "Sprites/Wall", 0.4f);
            FillRectangleWith(LevelBounds, "Sprites/SandTexture", 0.4f);
            FillRectangleWith(rightWall, "Sprites/Wall", 0.4f);
            AddObject(new TextObject(this, new Vector2(leftWall.X + leftWall.Width*0.5f, screen.Height * 0.95f), assets.Get<SpriteFont>("Fonts/MainFont32"), Color.Aqua, $"Level {gameController.CurrentLevel}"));
        }
        private void CreateGameplayObjects()
        {
            addObstacles(obstaclesArea);
            AddObject(new Danger(this, 5, dangerStartingArea.Location.ToVector2(), dangerStartingArea.Size.ToVector2()));
            AddObject(new Paddle(this));
            CreateBalls(nbBalls);
        }


        private void CreateBalls(int number = 1)
        {
            Obstacle last_obstacle = GetObjects<Obstacle>().LastOrDefault<Obstacle>();
            int top_bound = last_obstacle is null ? ExitArea.Bottom : last_obstacle.Collider.Bottom;
            Rectangle bounds = new Rectangle(LevelBounds.X, LevelBounds.Y, LevelBounds.Width, GetObjects<Danger>()[0].Collider.Top - (int)GetObjects<Paddle>()[0].Collider.Height*2 - LevelBounds.Y);
            int max_iteration = number * 100;
            int i = 0;
            while(i < max_iteration && number > 0)
            {
                int size = rng.Next(Ball.MinSize, Ball.MaxSize);
                if (bounds.Width > size && bounds.Height > size)
                {
                    Point location = bounds.Location + (Ball.relativeOffset*size).ToPoint() + new Point(rng.Next(bounds.Width - size), rng.Next(bounds.Height - size));
                    Rectangle area = new Rectangle(location, new Point(size, size));
                    if (areaIsEmpty(area))
                    {
                        float randomAngle = (float)rng.Next() % (2 * (float)Math.PI);
                        AddObject(new Ball(this, location.ToVector2(), size, randomAngle, rng.NextDouble() < ballCanMoveProba, rng.NextDouble() < luck));
                        number--; //don't panic: 'number' is passed by copy

                    }
                }
                i++;
            }
        }

        private void RefillBalls(float dt)
        {
            if(GetObjects<Ball>().Count < nbBalls)
            {
                ballRespawnTimer += dt;
                if(ballRespawnTimer > ballRespawnDelay)
                {
                    CreateBalls();
                    ballRespawnTimer = 0;
                }
            }
        }

        private bool areaIsEmpty(Rectangle area)
        {
            foreach (Ball otherBall in GetObjects<Ball>())
            {
                if (area.Intersects(otherBall.Collider)) return false;
            }
            Rectangle paddleArea = GetObjects<Paddle>()[0].Collider;
            if (area.Intersects(paddleArea)) return false;
            foreach(Obstacle obstacle in GetObjects<Obstacle>())
            {
                Rectangle obstacle_area_and_top = new Rectangle(obstacle.Collider.X, LevelBounds.Y, obstacle.Collider.Width, obstacle.Collider.Bottom - LevelBounds.Y);
                if(area.Intersects(obstacle_area_and_top)) return false;
                Rectangle ball_area_and_sides = new Rectangle(area.X - paddleArea.Width, area.Y, area.Width + 2*paddleArea.Width, area.Height);
                if(ball_area_and_sides.Intersects(obstacle.Collider)) return false;
            }
            return true;
        }


        private Vector2 getRandomPosition(Rectangle bounds)
            => new Vector2(bounds.Left + (float)rng.NextDouble() * bounds.Width, bounds.Top + (float)rng.NextDouble() * bounds.Height);


        private void addObstacles(Rectangle bounds)
        {
            char[,] grid = ServiceLocator.Get<GameController>().GetObstacleGrid();
            Texture2D texture = assets.Get<Texture2D>("Sprites/SandWall");

            int nbColumns = grid.GetLength(0);
            int nbRows = grid.GetLength(1);

            float obstacleWidth = bounds.Width / nbColumns;
            float obstacleHeight = obstacleWidth * texture.Height * Obstacle.nbFrames / texture.Width;

            for (int i=0; i< nbRows; i++)
            {
                for(int j=0; j< nbColumns; j++)
                {
                    if (grid[j,i] != 0)
                    {
                        AddObject(CharToObstacle(grid[j, i], i, j, bounds, obstacleWidth, obstacleHeight));
                    }
                }
            }
        }


        private Obstacle CharToObstacle(char character, int i, int j, Rectangle bounds, float obstacleWidth, float obstacleHeight)
        {
            if (character >= '1' && character <= '9') return new Obstacle(this, new Vector2(bounds.Left + j * obstacleWidth, bounds.Top + i * obstacleHeight), new Vector2(obstacleWidth, obstacleHeight), character - '0');
            else if (character >= 'A' && character <= 'Z')
            {
                BonusType randomBonus = (BonusType)new Random().Next(Enum.GetNames<BonusType>().Length);
                return new Obstacle(this, new Vector2(bounds.Left + j * obstacleWidth, bounds.Top + i * obstacleHeight), new Vector2(obstacleWidth, obstacleHeight), character - 'A' + 1, randomBonus);
            }
            else return null;
        }


        private void FillRectangleWith(Rectangle rectangle, string textureName, float scale)
        {
            Texture2D texture = assets.Get<Texture2D>(textureName);
            for(int x=rectangle.Left; x<rectangle.Right; x+= (int)(texture.Width*scale))
            {
                for(int y=rectangle.Top; y<rectangle.Bottom; y+= (int)(texture.Height * scale))
                {
                    AddObject(new Sprite(this, textureName, new Vector2(x,y), scale));
                }
            }
        }


        public override void Update(float dt)
        {
            if (keyboard.KeyJustPressed(Keys.P)) Pause = !Pause;
            if (Pause) dt = 0;
            base.Update(dt);
            RefillBalls(dt);
        }
    }
}
