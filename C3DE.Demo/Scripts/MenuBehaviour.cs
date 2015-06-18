﻿using C3DE.Components;
using C3DE.UI;
using C3DE.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace C3DE.Demo.Scripts
{
    public class MenuBehaviour : Behaviour
    {
        public static int ButtonWidth = 240;
        public static int ButtonHeight = 55;

        class DemoWidget
        {
            private Rectangle _rect;
            private string _text;
            private int _index;

            public DemoWidget(string text, int index)
            {
                _text = text;
                _index = index;
                _rect = new Rectangle(0, 0, ButtonWidth, ButtonHeight);
            }

            public void SetPosition(float x, float y)
            {
                _rect.X = (int)x;
                _rect.Y = (int)y;
            }

            public void Draw(GUI gui)
            {
                if (gui.Button(ref _rect, _text))
                    Application.LoadLevel(_index);
            }
        }

        private Rectangle _backgroundRect;
        private Texture2D _background;
        private Vector2 _titleRect;
        private Vector2 _footerRect;
        private float _titleSize;
        private float _margin;
        private DemoWidget[] _demos;

        public override void Start()
        {
            _margin = 15;

            _background = GraphicsHelper.CreateGradiantTexture(Color.LightSteelBlue, Color.Linen, Screen.VirtualWidth, Screen.VirtualHeight);
            _backgroundRect = new Rectangle(0, 0, Screen.VirtualWidth, Screen.VirtualHeight);

            var tempVec2 = GUI.Skin.Font.MeasureString("C3DE Demos");
            _titleSize = 2.0f;
            _titleRect = new Vector2(Screen.VirtualWidthPerTwo - tempVec2.X * _titleSize / 2, tempVec2.Y);

            tempVec2 = GUI.Skin.Font.MeasureString("Gets the source : https://github.com/demonixis/C3DE");
            _footerRect = new Vector2(Screen.VirtualWidthPerTwo - tempVec2.X / 2, Screen.VirtualHeight - tempVec2.Y - 5);

            _demos = new DemoWidget[Application.SceneManager.Count - 1];

            for (int i = 0; i < _demos.Length; i++)
                _demos[i] = new DemoWidget(Application.SceneManager[i + 1].Name, i + 1);

            float x = Screen.VirtualWidthPerTwo - ButtonWidth / 2;
            float y = Screen.VirtualHeightPerTwo - ((ButtonHeight + _margin) * _demos.Length) / 2;

            for (int i = 0; i < _demos.Length; i++)
                _demos[i].SetPosition(x, y + i * (ButtonHeight + _margin));
        }

        public override void Update()
        {
            if (Input.Keys.JustPressed(Keys.Escape) || Input.Gamepad.JustPressed(Buttons.Back))
                Application.Quit();
        }

        public override void OnGUI(GUI gui)
        {
            gui.DrawTexture(ref _backgroundRect, _background);
            gui.Label(ref _titleRect, "C3DE Demos", _titleSize);

            for (int i = 0, l = _demos.Length; i < l; i++)
                _demos[i].Draw(gui);

            gui.Label(ref _footerRect, "Gets the source : https://github.com/demonixis/C3DE");
        }
    }
}