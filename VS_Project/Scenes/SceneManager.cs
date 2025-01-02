using Microsoft.Xna.Framework.Graphics;
using Sand_Breaker.Services;
using System;
using System.Collections.Generic;

namespace Sand_Breaker.Scenes
{

    public interface ISceneManager
    {
        public void LoadScene<T>(params object[] data) where T : Scene, new();
    }

    public sealed class SceneManager : ISceneManager
    {
        private Scene currentScene;


        public SceneManager()
        {
            ServiceLocator.Register<ISceneManager>(this);
        }

        public void LoadScene<T>(params object[] data) where T: Scene, new()
        {
                currentScene?.Unload();
                currentScene = new T();
                currentScene.Load(data);
        }

        public void Update(float dt) => currentScene?.Update(dt);
        public void Draw() => currentScene?.Draw();
    }
}
