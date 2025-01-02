
using Microsoft.Xna.Framework.Input;

namespace Sand_Breaker.Services
{


    public interface IKeyboardService
    {
        public bool KeyIsPressed(Keys key);
        public bool KeyJustPressed(Keys key);
        public bool KeyJustReleased(Keys key);


    }
    
    public sealed class KeyboardService : IKeyboardService
    {
        private InputManager inputManager;

        public KeyboardService()
        {
            inputManager = ServiceLocator.Get<InputManager>();
            ServiceLocator.Register<IKeyboardService>(this);
        }

        public bool KeyIsPressed(Keys key) => inputManager.KeyIsPressed(key);
        public bool KeyJustPressed(Keys key) => inputManager.KeyJustPressed(key);
        public bool KeyJustReleased(Keys key) => inputManager.KeyJustReleased(key);


    }
}
