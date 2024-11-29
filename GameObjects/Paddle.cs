
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using static System.Formats.Asn1.AsnWriter;

namespace Sand_Breaker.GameObjects
{
    public class Paddle : Sprite
    {
        public static Vector2 defaultSpeed { get; private set; } = new Vector2(500f, 80f);
        public static Vector2 absoluteSpeed = defaultSpeed;
        public static float defaultScale { get; private set; } = 0.12f;
        public static float scale { get; private set; } = defaultScale;
        private static float lerp_amount = 0.2f;

        private int nbFrames = 6;
        private float animationDuration = 0.3f;
        private bool leftClawMoving = false;
        private float leftClawTimer = 0f;
        private bool rightClawMoving = false;
        private float rightClawTimer = 0f;

        private IMouseService mouse = ServiceLocator.Get<IMouseService>();
        private IKeyboardService keyboard = ServiceLocator.Get<IKeyboardService>();

        private int LeftClawFrame => -Math.Abs((int)Math.Floor((leftClawTimer * 2/animationDuration-1)*(nbFrames-1)))+nbFrames-1;
        private int RightClawFrame => -Math.Abs((int)Math.Floor((rightClawTimer * 2 / animationDuration - 1) * (nbFrames - 1))) + nbFrames - 1;

        private Vector2 targetPosition;
        public Vector2 Speed;

        private Rectangle MovementBounds;

        private static Vector2 colliderOffset;
        public override Rectangle Collider => new Rectangle((int)((position.X - offset.X) + colliderOffset.X), (int)((position.Y - offset.Y)+colliderOffset.Y), ((int)(size.X - colliderOffset.X)), (int)(size.Y - colliderOffset.Y));

        private Danger danger => rootScene.GetObjects<Danger>()[0];

        


        public Paddle(Scene rootScene, Vector2 position, float scale) : base(rootScene, "Sprites/Crabe", position)
        {
            this.size = new Vector2(texture.Width * scale * 2 / nbFrames, texture.Height * scale);
            GameScene scene = ((GameScene)rootScene);
            MovementBounds = new Rectangle(scene.LevelBounds.Left, scene.ExitArea.Top, scene.LevelBounds.Width, (int)danger.position.Y - MovementBounds.Top);
            this.textureActualOrigin = offset * this.texture.Bounds.Size.ToVector2() / size;
        }

        public Paddle(Scene rootScene, float scale) : this(rootScene, Vector2.Zero, scale)
        {
            this.position = new Vector2(MovementBounds.Center.X - size.X*0.5f, MovementBounds.Bottom - size.Y);
            
        }


        public Paddle(Scene rootScene) : this(rootScene, Paddle.scale)
        {
        }


        public static void GainBonusSpeed(Vector2 amount)
        {
            absoluteSpeed += amount;
        }
        public static void GainBonusSize(float scale)
        {
            scale *= scale;
        }

        public static void RemoveAllBonus()
        {
            absoluteSpeed = defaultSpeed;
            scale = defaultScale;
        }


        public override void Start()
        {
            targetPosition = position;
            colliderOffset = new Vector2(size.X * 0, size.Y * 0.237f);
        }

        public override void Update(float dt)
        {
            if (IsActive)
            {
                if (keyboard.KeyIsPressed(Keys.Up))
                    Speed.Y = -absoluteSpeed.Y;
                else if (keyboard.KeyIsPressed(Keys.Down))
                    Speed.Y = absoluteSpeed.Y;
                else Speed.Y = 0;

                if(keyboard.KeyIsPressed(Keys.Left))
                    Speed.X = -absoluteSpeed.X;
                else if(keyboard.KeyIsPressed(Keys.Right))
                    Speed.X = absoluteSpeed.X;
                else Speed.X = 0;

                MovementBounds.Height = (int)danger.Collider.Top - MovementBounds.Top;

                Move(dt);
                AnimateClaw(dt);
                foreach (Obstacle obstacle in rootScene.GetObjects<Obstacle>())
                {
                    ResolveCollisionWith<Obstacle>(obstacle);
                }
            }
        }

        public void Move(float dt)
        {
            targetPosition += Speed * dt;
            targetPosition = Vector2.Clamp(targetPosition, MovementBounds.Location.ToVector2() + offset, (MovementBounds.Location + MovementBounds.Size).ToVector2() + offset - size);
            if(dt>0) position = Vector2.Lerp(position, targetPosition, lerp_amount);
        }

        public void AnimateClaw(float dt)
        {
            if (leftClawMoving)
            {
                leftClawTimer += dt;
                if (leftClawTimer >= animationDuration)
                {
                    leftClawMoving = false;
                }
            }
            if (rightClawMoving)
            {
                rightClawTimer += dt;
                if (rightClawTimer >= animationDuration)
                {
                    rightClawMoving = false;
                }
            }
        }


        public void CollideBlocks(Obstacle[,] blocks_array)
        {
            for (int i = 0; i < blocks_array.GetLength(0); i++)
            {
                for (int j = 0; j < blocks_array.GetLength(1); j++)
                {
                    if (Collider.Intersects(blocks_array[i, j].Collider))
                    {
                        IsActive = false;
                    }
                }
            }
        }


        public void ManageCollisions(Obstacle[,] blocks_array)
        {
            CollideBlocks(blocks_array);
        }


        public override void OnCollide(Sprite otherSprite, CollisionSide side)
        {
            if(otherSprite is Obstacle)
            {
                switch (side)
                {
                    case CollisionSide.None:
                    case CollisionSide.Top:
                        position.Y = otherSprite.Collider.Bottom + offset.Y - colliderOffset.Y;
                        break;
                    case CollisionSide.Bottom:
                        position.Y = otherSprite.Collider.Top - offset.Y - Collider.Height - colliderOffset.Y;
                        break;
                    case CollisionSide.Left:
                        position.X = otherSprite.Collider.Right + offset.X;
                        break;
                    case CollisionSide.Right:
                        position.X = otherSprite.Collider.Left - offset.X - Collider.Width;
                        break;
                    default:
                        break;
                }
                targetPosition = position;
            }

            else if (otherSprite is Ball && side == CollisionSide.Bottom)
            {
                if(otherSprite.Collider.Center.X < Collider.Center.X)
                {
                    leftClawTimer = 0;
                    leftClawMoving = true;
                }
                else
                {
                    rightClawTimer = 0;
                    rightClawMoving = true;
                }
            }
        }


        public override void Draw()
        { 
            if (IsActive && texture != null)
            {
                spriteBatch.Draw(texture, new Rectangle(position.ToPoint(), new Vector2(size.X * 0.5f, size.Y).ToPoint()), new Rectangle(new Point(LeftClawFrame*texture.Width/nbFrames, 0), new Vector2(texture.Width/nbFrames, texture.Height).ToPoint()), color, 0, offset, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(texture, new Rectangle((position+new Vector2(size.X*0.5f-1, 0)).ToPoint(), new Vector2(size.X*0.5f, size.Y).ToPoint()), new Rectangle(new Point(RightClawFrame * texture.Width / nbFrames, 0), new Vector2(texture.Width / nbFrames, texture.Height).ToPoint()), color, 0, offset, SpriteEffects.None, 0);
            }
        }
    }
}