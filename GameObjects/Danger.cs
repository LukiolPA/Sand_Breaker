using Microsoft.Xna.Framework;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;

namespace Sand_Breaker.GameObjects
{
    public class Danger : Sprite
    {
        private float speed;
        private Vector2 colliderOffset;
        public override Rectangle Collider => new Rectangle((int)((position.X - offset.X) + colliderOffset.X), (int)((position.Y - offset.Y) + colliderOffset.Y), (int)(size.X - colliderOffset.X), (int)(size.Y - colliderOffset.Y));

        public Danger(Scene rootScene, float speed, Vector2 position, Vector2 size) : base(rootScene, "Sprites/Danger", position, size)
        {
            color = new Color(42, 61, 42);
            colliderOffset = new Vector2(size.X * 0, size.Y * 0.077f);
            this.speed = speed;
        }
        public override void Update(float dt)
        {
            if (IsActive)
            {
                position -= new Vector2(0, speed) * dt;
                ResolveCollisionWith(rootScene.GetObjects<Paddle>()[0]);
                foreach(Obstacle obstacle in rootScene.GetObjects<Obstacle>())
                {
                    ResolveCollisionWith(obstacle);
                }
            }

        }

        public override void OnCollide(Sprite otherSprite, CollisionSide side)
        {
            if(otherSprite is Paddle)
            {
                ServiceLocator.Get<GameController>().GameOver();
            }
            else if(otherSprite is Obstacle || otherSprite is Ball)
            {
                otherSprite.Destroy();
            }
        }



    }
}
