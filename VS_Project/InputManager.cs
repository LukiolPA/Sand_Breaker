

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sand_Breaker.Services;
using System;

namespace Sand_Breaker
{
    public class InputManager
    {
        public KeyboardState keyboardState;
        public KeyboardState oldKeyboardState;
        public MouseState mouseState;
        public MouseState oldMouseState;

        public InputManager()
        {
            keyboardState = Keyboard.GetState();
            oldKeyboardState = keyboardState;
            mouseState = Mouse.GetState();
            oldMouseState = mouseState;
            ServiceLocator.Register<InputManager>(this);
        }

        public bool KeyIsPressed(Keys key) => keyboardState.IsKeyDown(key);
        public bool KeyJustPressed(Keys key) => keyboardState.IsKeyDown(key) && !oldKeyboardState.IsKeyDown(key);
        public bool KeyJustReleased(Keys key) => keyboardState.IsKeyUp(key) && oldKeyboardState.IsKeyDown(key);

        public bool MouseIsClicked(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return mouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return mouseState.RightButton == ButtonState.Pressed;
                default:
                    throw new InvalidOperationException($"Mouse button {button} does not exist");
            }
        }

        public bool MouseJustClicked(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return mouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released;
                case MouseButton.Right:
                    return mouseState.RightButton == ButtonState.Pressed && oldMouseState.RightButton == ButtonState.Released;
                default:
                    throw new InvalidOperationException($"Mouse button {button} does not exist");
            }
        }

        public bool MouseJustReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return mouseState.LeftButton == ButtonState.Released && oldMouseState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return mouseState.RightButton == ButtonState.Released && oldMouseState.RightButton == ButtonState.Pressed;
                default:
                    throw new InvalidOperationException($"Mouse button {button} does not exist");
            }
        }

        public bool MouseHovers(Rectangle rectangle) => rectangle.Contains(mouseState.X, mouseState.Y);
    }
}
