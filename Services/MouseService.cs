

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sand_Breaker.Services
{

    public interface IMouseService
    {
        public bool MouseIsClicked(MouseButton button);
        public bool MouseJustClicked(MouseButton button);
        public bool MouseJustReleased(MouseButton button);
        public bool MouseHovers(Rectangle rectangle);

    }

    public enum MouseButton
    {
        Left,
        Right
    }

    public sealed class MouseService : IMouseService
    {
        private InputManager inputManager;


        public MouseService()
        {
            inputManager = ServiceLocator.Get<InputManager>();
            ServiceLocator.Register<IMouseService>(this);
        }

        public bool MouseIsClicked(MouseButton button) => inputManager.MouseIsClicked(button);
        public bool MouseJustClicked(MouseButton button) => inputManager.MouseJustClicked(button);
        public bool MouseJustReleased(MouseButton button) => inputManager.MouseJustReleased(button);
        public bool MouseHovers(Rectangle rectangle) => inputManager.MouseHovers(rectangle);

    }
}
