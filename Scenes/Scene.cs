using Microsoft.Xna.Framework.Input;
using Sand_Breaker.GameObjects;
using Sand_Breaker.Services;
using System.Collections.Generic;

namespace Sand_Breaker.Scenes
{
    public abstract class Scene
    {
        protected IScreenService screen = ServiceLocator.Get<IScreenService>();
        protected ISceneManager sceneManager = ServiceLocator.Get<ISceneManager>();
        protected IKeyboardService keyboard = ServiceLocator.Get<IKeyboardService>();
        private List<Object> objectsList = new List<Object>();

        public virtual void Load(params object[] data) { }
        public virtual void Unload() { }

        public virtual void Update(float dt)
        {
            if (keyboard.KeyIsPressed(Keys.Escape))
                ServiceLocator.Get<IMain>().Exit();
            foreach (Object obj in objectsList)
            {
                if(obj.IsActive)
                {
                    obj.Update(dt);
                }
            }
            for (int i = objectsList.Count - 1; i >= 0; i--)
            {
                if (objectsList[i].isFree)
                {
                    objectsList[i].OnFree();
                    objectsList.RemoveAt(i);
                }
            }
        }
        public void Draw()
        {
            foreach (Object obj in objectsList)
            {
                if (obj.IsActive)
                {
                    obj.Draw();
                }
            }
        }


        public void AddObject(Object obj)
        {
            if(obj != null)
            {
                obj.Start();
                objectsList.Add(obj);
            }
        }

        public List<T> GetObjects<T>() where T : Object
        {
            List<T> object_list = new List<T>();

            foreach(Object obj in objectsList)
            {
                if(obj is T typedObject)
                {
                    object_list.Add(typedObject);
                }
            }
            
            return object_list;
        }
    }
}
