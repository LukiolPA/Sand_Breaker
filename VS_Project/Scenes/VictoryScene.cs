
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sand_Breaker.GameObjects;
using Sand_Breaker.Services;
using Sand_Breaker.UI;
using System;
using System.Collections.Generic;

namespace Sand_Breaker.Scenes
{

    public class VictoryScene : Scene
    {
        public override void Load(params object[] data)
        {
            AddObject(new TextObject(this, new Vector2(screen.Width * 0.5f, screen.Height * 0.3f), ServiceLocator.Get<IAssetsService>().Get<SpriteFont>("Fonts/MainFont64"), Color.DeepSkyBlue, "Congratulations!"));
            AddObject(new TextObject(this, new Vector2(screen.Width * 0.5f, screen.Height * 0.5f), ServiceLocator.Get<IAssetsService>().Get<SpriteFont>("Fonts/MainFont32"), Color.DeepSkyBlue, $"You've beaten Level {ServiceLocator.Get<GameController>().MaxLevel}, finishing the game!"));
            AddObject(new TextObject(this, new Vector2(screen.Width * 0.5f, screen.Height * 0.7f), ServiceLocator.Get<IAssetsService>().Get<SpriteFont>("Fonts/MainFont48"), Color.DeepSkyBlue, "Press space to restart"));
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (keyboard.KeyJustReleased(Keys.Space))
            {
                sceneManager.LoadScene<GameScene>();
            }
        }
    }

 }
