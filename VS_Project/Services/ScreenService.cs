
using Microsoft.Xna.Framework;

namespace Sand_Breaker.Services
{
    public interface IScreenService
    {
        public int Width { get; }
        public int Height { get; }

        public int TopBorder { get; }
        public int BottomBorder { get; }
        public int LeftBorder { get; }
        public int RightBorder { get; }


        public Rectangle Rectangle { get; }

        public Vector2 TopLeftCorner { get; }
        public Vector2 TopRightCorner { get; }
        public Vector2 BottomLeftCorner { get; }
        public Vector2 BottomRightCorner { get; }
        public Vector2 Center { get; }
    }

    public sealed class ScreenService : IScreenService
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        public ScreenService()
        {
            this.graphicsDeviceManager = ServiceLocator.Get<GraphicsDeviceManager>();
            ServiceLocator.Register<IScreenService>(this);
        }

        public int Width => graphicsDeviceManager.PreferredBackBufferWidth;
        public int Height => graphicsDeviceManager.PreferredBackBufferHeight;

        public int TopBorder => 0;
        public int BottomBorder => TopBorder + Height;
        public int LeftBorder => 0;
        public int RightBorder => LeftBorder + Width;

        public Rectangle Rectangle => new Rectangle(LeftBorder, TopBorder, Width, Height);

        public Vector2 TopLeftCorner => new Vector2(LeftBorder, TopBorder);
        public Vector2 TopRightCorner => new Vector2(RightBorder, TopBorder);
        public Vector2 BottomLeftCorner => new Vector2(LeftBorder, BottomBorder);
        public Vector2 BottomRightCorner => new Vector2(RightBorder, BottomBorder);
        public Vector2 Center => TopLeftCorner + 0.5f * new Vector2(Width, Height);




        public void SetSize(int screenWidth, int screenHeight)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = screenWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = screenHeight;
            graphicsDeviceManager.ApplyChanges();
        }


    }
}
