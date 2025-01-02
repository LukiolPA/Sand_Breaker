using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Sand_Breaker.Services;
using Sand_Breaker.Scenes;


namespace Sand_Breaker.GameObjects
{
    public enum CollisionSide //NB : side is relative to the object calling the collision method.
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }

    public class Sprite : Object
    {

        protected Texture2D texture;
        protected Vector2 textureActualOrigin;
        protected static SpriteBatch spriteBatch = ServiceLocator.Get<SpriteBatch>();

        public Color color;
        public Vector2 size { get; protected set; }
        public Vector2 offset { get; protected set; }
        public float rotation { get; protected set; }

        public virtual Rectangle Collider => new Rectangle((int)(position.X - offset.X), (int)(position.Y - offset.Y), (int)size.X, (int)size.Y);
        public Vector2 Center => new Vector2(position.X - offset.X + size.X / 2, position.Y - offset.Y + size.Y / 2);

        public Sprite(Scene rootScene, string texture, Color color, Vector2 position, Vector2 size, Vector2 offset, float rotation = 0) : base(rootScene, position)
        {
            this.color = color;
            this.size = size;
            this.offset = offset;
            this.rotation = rotation;
            if(texture != "")
            {
                this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>(texture);
                this.textureActualOrigin = offset * this.texture.Bounds.Size.ToVector2() / size;
            }
        }

        public Sprite(Scene rootScene, string texture, Color color, Vector2 position, Vector2 size) : this(rootScene, texture, color, position, size, Vector2.Zero) { }


        public Sprite(Scene rootScene, string texture, Vector2 position, Vector2 size, Vector2 offset) : this(rootScene, texture, Color.White, position, size, offset) { }
        public Sprite(Scene rootScene, string texture, Vector2 position, Vector2 size) : this(rootScene, texture, position, size, Vector2.Zero) { }
        public Sprite(Scene rootScene, string texture, Vector2 position) : this(rootScene, texture, position, Vector2.One)
        {
            if (texture != "")
            {
                this.size = new Vector2(this.texture.Width, this.texture.Height);
                this.textureActualOrigin = offset;
            }
        }



        public Sprite(Scene rootScene, string texture, Color color, Vector2 position, float scale, Vector2 offset, float rotation = 0) : this(rootScene, texture, position)
        {
            this.color = color;
            if (texture != "")
            {
                this.size = new Vector2(this.texture.Width * scale, this.texture.Height * scale);
                this.textureActualOrigin = offset * this.texture.Bounds.Size.ToVector2() / size;
            }
        }

        public Sprite(Scene rootScene, string texture, Vector2 position, float scale, Vector2 offset) : this(rootScene, texture, Color.White, position, scale, offset) { }
        public Sprite(Scene rootScene, string texture, Vector2 position, float scale = 1) : this(rootScene, texture, position, scale, Vector2.Zero) { }



        protected virtual bool isColliding<T>(T other_sprite) where T : Sprite
        {
            if (!IsActive || !other_sprite.IsActive) return false;
            if (other_sprite == this) throw new InvalidOperationException("Object should not collide with itself");

            return Collider.Intersects(other_sprite.Collider);
        }

        protected CollisionSide GetCollisionSideWith<T>(T other_sprite) where T : Sprite
        {
            if (isColliding(other_sprite))
            {
                float depthX = Math.Min(Collider.Right - other_sprite.Collider.Left, other_sprite.Collider.Right - Collider.Left);
                float depthY = Math.Min(Collider.Bottom - other_sprite.Collider.Top, other_sprite.Collider.Bottom - Collider.Top);

                if (depthX < depthY) //horizontal bounce
                {
                    if (Collider.Right > other_sprite.Collider.Left && Collider.Left < other_sprite.Collider.Left)
                    {
                        return CollisionSide.Right;
                    }
                    else if (Collider.Left < other_sprite.Collider.Right && Collider.Right > other_sprite.Collider.Right)
                    {
                        return CollisionSide.Left;
                    }
                }
                else //vertical bounce
                {
                    if (Collider.Bottom > other_sprite.Collider.Top && Collider.Top < other_sprite.Collider.Top)
                    {
                        return CollisionSide.Bottom;
                    }
                    else if (Collider.Top < other_sprite.Collider.Bottom && Collider.Bottom > other_sprite.Collider.Bottom) //bottom side bounce
                    {
                        return CollisionSide.Top;
                    }
                }
            }
            return CollisionSide.None;
        }

        protected virtual void ResolveCollisionWith<T>(T otherSprite) where T : Sprite
        {
            CollisionSide side = GetCollisionSideWith<T>(otherSprite);
            if (side != CollisionSide.None)
            {
                this.OnCollide(otherSprite, side);
                otherSprite.OnCollide(this, side);
            }
        }
        public virtual void OnCollide(Sprite otherSprite, CollisionSide side) { }
        public virtual void Destroy()
        {
            IsActive = false;
            isFree = true;
        }


        public override void Draw()
        {
            if (IsActive && texture != null)
            {
                spriteBatch.Draw(texture, new Rectangle((position).ToPoint(), size.ToPoint()), null, color, rotation, textureActualOrigin, SpriteEffects.None, 0);
            }
        }
    }
}
