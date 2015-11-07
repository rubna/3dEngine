using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;

namespace _3dEngine
{
    internal class InputHelper
    {
        private static InputHelper instance;
        public static InputHelper Instance
        {
            get { return instance ?? (instance = new InputHelper()); }
        }

        private KeyboardState keyBoardState, lastKeyboardState;
        private MouseState mouseState, lastMouseState;

        private InputHelper()
        {
            TouchPanel.EnableMouseTouchPoint = true;
            TouchPanel.EnabledGestures = GestureType.FreeDrag;
        }

        public void Update()
        {
            lastKeyboardState = keyBoardState;
            lastMouseState = mouseState;

            keyBoardState = Keyboard.GetState();
            mouseState = Mouse.GetState();
        }
        

        public bool KeyboardCheckDown(Keys k)
        {
            return keyBoardState.IsKeyDown(k);
        }
        public bool KeyboardCheckReleased(Keys k)
        {
            return keyBoardState.IsKeyUp(k) && lastKeyboardState.IsKeyDown(k);
        }
        public bool KeyboardCheckPressed(Keys k)
        {
            return keyBoardState.IsKeyDown(k) && lastKeyboardState.IsKeyUp(k);
        }

        public Vector2 MousePosition()
        {
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public Vector2 MouseLastPosition()
        {
            return new Vector2(lastMouseState.X, lastMouseState.Y);
        }

        public Vector2 MouseSpeed()
        {
            return MousePosition() - MouseLastPosition();
        }

        private ButtonState GetButtonState(MouseState state, MouseButton button)
        {
            switch (button)
            {
                case MouseButton.LeftButton:
                    return state.LeftButton;
                case MouseButton.MiddleButton:
                    return state.MiddleButton;
                case MouseButton.RightButton:
                    return state.RightButton;
                default:
                    return state.LeftButton;
            }
        }

        public bool MouseCheckPressed(MouseButton button)
        {
            return ButtonPressed(GetButtonState(mouseState, button), GetButtonState(lastMouseState, button));
        }
        public bool MouseCheckReleased(MouseButton button)
        {
            return ButtonReleased(GetButtonState(mouseState, button), GetButtonState(lastMouseState, button));
        }
        public bool MouseCheckDown(MouseButton button)
        {
            return ButtonDown(GetButtonState(mouseState, button));
        }

        public float MouseScrollDelta()
        {
            return mouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
        }
        private bool ButtonPressed(ButtonState buttonState, ButtonState lastButtonState)
        {
            return lastButtonState == ButtonState.Released && buttonState == ButtonState.Pressed;
        }
        private bool ButtonReleased(ButtonState buttonState, ButtonState lastButtonState)
        {
            return lastButtonState == ButtonState.Pressed && buttonState == ButtonState.Released;
        }
        private bool ButtonDown(ButtonState buttonState)
        {
            return buttonState == ButtonState.Pressed;
        }
    }

    public enum MouseButton
    {
        LeftButton, RightButton, MiddleButton
    }
}
