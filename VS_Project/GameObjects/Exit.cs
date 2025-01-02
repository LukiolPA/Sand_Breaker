using Microsoft.Xna.Framework;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;

namespace Sand_Breaker.GameObjects
{
    public class Exit : Sprite
    {

        public override Rectangle Collider => new Rectangle((int)(position.X - offset.X), (int)(position.Y - offset.Y), (int)size.X, (int)(size.Y*0.8f));

        public Exit(Scene rootScene, Vector2 position, Vector2 size) : base(rootScene, "Sprites/Beach", position, size)
        {
        }
        public override void Update(float dt)
        {
            if (IsActive)
            {
                ResolveCollisionWith(rootScene.GetObjects<Paddle>()[0]);
            }

        }

        public override void OnCollide(Sprite otherSprite, CollisionSide side)
        {
            if(otherSprite is Paddle)
            {
                ServiceLocator.Get<GameController>().LevelUp(((GameScene)rootScene).GatheredBonus);
            }
            else if(otherSprite is Ball)
            {
                otherSprite.Destroy();
            }
        }



    }
}
