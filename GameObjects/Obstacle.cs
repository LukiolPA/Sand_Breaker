
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;
using System;

namespace Sand_Breaker.GameObjects
{
    public class Obstacle : Sprite
    {
        private int health;
        private BonusType? bonus = null;
        private Texture2D bonusTexture;
        public static int nbFrames { get; private set; } = 5;

        private int CurrentFrame => health < nbFrames ? health-1 : nbFrames-1; 
        public Obstacle(Scene rootScene, Vector2 position, Vector2 size, int health = 1, BonusType? bonus = null) : base(rootScene, "Sprites/SandWall", position, size)
        {
            this.health = health;
            this.bonus = bonus;
            if (bonus != null) bonusTexture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/Bonus");
        }
        public Obstacle(Scene rootScene, Vector2 position, float scale, int health = 1, BonusType? bonus = null) : base(rootScene, "Sprites/SandWall", position, scale)
        {
            this.size = new Vector2(texture.Width * scale / nbFrames, texture.Height * scale);
            this.textureActualOrigin = offset * this.texture.Bounds.Size.ToVector2() / size;
            this.health = health;
            this.bonus = bonus;
            if (bonus != null) bonusTexture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/Bonus");
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            if(health<=0)
            {
                if(bonus is not null) ((GameScene)rootScene).GatheredBonus = this.bonus;
                Destroy();
            }
        }

        public override void OnCollide(Sprite otherSprite, CollisionSide side = CollisionSide.None)
        {
            if(side != CollisionSide.None)
            {
                if(otherSprite is Ball && ((Ball)otherSprite).state == Ball.BallState.Shot)
                {
                    TakeDamage(((Ball)otherSprite).actualDamage);
                }
            }
        }

        public override void Draw()
        {
            if (IsActive)
            {
                spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), size.ToPoint()), new Rectangle(new Point(CurrentFrame * texture.Width / nbFrames, 0), new Vector2(texture.Width / nbFrames, texture.Height).ToPoint()), color, 0, offset, SpriteEffects.None, 0);
                if(bonus != null)
                {
                    spriteBatch.Draw(bonusTexture, new Rectangle((position + size * 0.25f).ToPoint(), (size * 0.5f).ToPoint()), null, Color.Gold, 0, offset, SpriteEffects.None, 0);
                }
            }
        }
    }
}
