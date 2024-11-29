using Microsoft.Xna.Framework;
using Sand_Breaker.Scenes;

namespace Sand_Breaker.GameObjects
{
    public abstract class Object
    {
        private bool isActive;

        public Scene rootScene { get; private set; }
        public Vector2 position;
        public bool isFree = false;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (isActive) OnEnable();
                    else OnDisable();
                }
            }
        }

        public Object(Scene rootScene, Vector2 position, bool isEnable = true)
        {
            this.rootScene = rootScene;
            this.position = position;
            this.isActive = isEnable;
        }

        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        public virtual void Start() { }
        public virtual void OnFree() { }
        public virtual void Update(float dt) { }
        public virtual void Draw() { }


    }
}
