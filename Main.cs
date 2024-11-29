using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;

namespace Sand_Breaker
{

    public interface IMain
    {
        public void Exit();
    }


    public class Main : Game, IMain
    {
        private GraphicsDeviceManager _graphics;
        private SceneManager sceneManager;
        private InputManager inputManager;
        private SpriteBatch _spriteBatch;
        private AssetsService assetsService;
        private ScreenService screenService;



        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            ServiceLocator.Register<IMain>(this);
            ServiceLocator.Register<GraphicsDeviceManager>(_graphics);
            ServiceLocator.Register<ContentManager>(Content);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ServiceLocator.Register<SpriteBatch>(_spriteBatch);


            assetsService = new AssetsService();

            screenService = new ScreenService();
            screenService.SetSize(1000, 750);
            sceneManager = new SceneManager();

            inputManager = new InputManager();
            new MouseService();
            new KeyboardService();
            new GameController();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            LoadTextures();
            sceneManager.LoadScene<StartingScene>();
        }

        private void LoadTextures()
        {
            assetsService.Load<Texture2D>("Sprites/Seashell");
            assetsService.Load<Texture2D>("Sprites/Crustacean");
            assetsService.Load<Texture2D>("Sprites/Crabe");
            assetsService.Load<Texture2D>("Sprites/SandWall");
            assetsService.Load<Texture2D>("Sprites/Wall");
            assetsService.Load<Texture2D>("Sprites/SandTexture");
            assetsService.Load<Texture2D>("Sprites/Beach");
            assetsService.Load<Texture2D>("Sprites/Danger");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/Bonus");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/BallBiggerIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/BallFasterIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/MoreLuckIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/MoreShellsIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/MoreDamageIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/PaddleBiggerIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/PaddleFasterIcon");
            assetsService.Load<Texture2D>("Sprites/BonusIcons/BonusBackground");
            assetsService.Load<SpriteFont>("Fonts/MainFont32");
            assetsService.Load<SpriteFont>("Fonts/MainFont48");
            assetsService.Load<SpriteFont>("Fonts/MainFont64");
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            inputManager.keyboardState = Keyboard.GetState();
            inputManager.mouseState = Mouse.GetState();


            sceneManager.Update(dt);


            inputManager.oldKeyboardState = inputManager.keyboardState;
            inputManager.oldMouseState = inputManager.mouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);
            _spriteBatch.Begin();
            sceneManager.Draw();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
