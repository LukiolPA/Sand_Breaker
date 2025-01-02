using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;
using System;


namespace Sand_Breaker.GameObjects
{
    public class Ball : Sprite
    {

        public enum BallState
        {
            Waiting,
            Wandering,
            Shot
        }


        public static int initialMinSize = 30;
        public static int initialMaxSize = 35;
        public static float initialLaunchSpeed { get; private set; } = 250;

        public static int MinSize {get; private set;} = initialMinSize;
        public static int MaxSize {get; private set;} = initialMaxSize;
        public static float LaunchSpeed {get; private set;} = initialLaunchSpeed;
        public static Vector2 relativeOffset { get; private set;} = new Vector2(0.5f, 0.5f);
        private Texture2D visitorTexture;
    
        private static readonly float rotationSpeed = (float)Math.PI * 0.5f;

        private readonly bool canMove;
        private readonly float waitingDuration = 5;
        private readonly float wanderingDuration = 3;
        private static readonly float wanderingSpeed = 20;
        private float wanderingTimer;
        private float waitingTimer;
        public BallState state = BallState.Waiting;

        private Vector2 velocity;
        public float speed = 0f;
        private Vector2 Direction
        {
            get
            {
                return speed == 0 ? Vector2.Zero : velocity / speed;
            }
            set
            {
                velocity = Vector2.Normalize(value) * speed;
            }
        }

        private static int initialDamage = 1;
        private static int damage = initialDamage;
        public int actualDamage;
        private static readonly Vector2 colliderRelativeSize = new Vector2(0.882f, 0.882f);
        public override Rectangle Collider => new Rectangle((int)((position.X - offset.X + 0.5f*size.X*(1-colliderRelativeSize.X))), (int)((position.Y - offset.Y + 0.5f*size.Y*(1-colliderRelativeSize.Y))), (int)(size.X * colliderRelativeSize.X), (int)(size.Y * colliderRelativeSize.Y));

        public Ball(Scene rootScene, Vector2 position, float size, float rotation=0, bool canMove = false, bool isGolden = false) : base(rootScene, "Sprites/Seashell", position, new Vector2(size, size))
        {
            this.rotation = rotation;
            color = isGolden ? Color.Gold : Color.SeaShell; //255 245 238
            velocity = Vector2.Zero;
            offset = relativeOffset * size;
            textureActualOrigin = offset * this.texture.Bounds.Size.ToVector2() / size;
            this.actualDamage = isGolden ? damage * 2 : damage;
            wanderingTimer = 0;
            waitingTimer = 0;
            this.canMove = canMove;
            if (canMove)
            {
                visitorTexture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/Crustacean");
                waitingDuration *= (float)(1 + new Random().NextDouble() * 0.75);
                wanderingDuration *= (float)(1 + new Random().NextDouble() * 0.5);
            }

        }


        public static void GainBonusSize(int amount)
        {
            MinSize += amount;
            MaxSize += amount;
        }

        public static void GainBonusSpeed(float amount)
        {
            LaunchSpeed += amount;
        }

        public static void GainDamage(int amount)
        {
            damage += amount;
        }

        public static void RemoveAllBonus()
        {
            MinSize = initialMinSize;
            MaxSize = initialMaxSize;
            LaunchSpeed = initialLaunchSpeed;
            damage = initialDamage;
        }

        public override void Update(float dt)
        {
            if (IsActive)
            {
                if(canMove) UpdateState(dt);
                if (state == BallState.Wandering) Wander(dt);
                if (state == BallState.Shot) Move(dt);
                foreach (Obstacle obstacle in rootScene.GetObjects<Obstacle>())
                {
                    ResolveCollisionWith<Obstacle>(obstacle);
                }
                ResolveCollisionWith<Exit>(rootScene.GetObjects<Exit>()[0]);
                ResolveCollisionWith<Paddle>(rootScene.GetObjects<Paddle>()[0]);
                ResolveCollisionWith<Danger>(rootScene.GetObjects<Danger>()[0]);
            }
        }

        public  void UpdateState(float dt)
        {
            if (state == BallState.Waiting)
            {
                waitingTimer += dt;
                if (waitingTimer >= waitingDuration)
                {
                    ChangeState(BallState.Wandering);
                }
            }
            else if (state == BallState.Wandering)
            {
                wanderingTimer += dt;
                if (wanderingTimer >= wanderingDuration)
                {
                    ChangeState(BallState.Waiting);
                }
            }
        }

        private void ChangeState(BallState newState)
        {
            if (newState == BallState.Waiting)
            {
                waitingTimer = 0;
                state = newState;
            }
            if (newState == BallState.Wandering)
            {
                rotation += (float)(new Random().NextDouble() * Math.PI * 0.5 + Math.PI * 0.25);
                wanderingTimer = 0;
                state = newState;
            }
        }

        public void Shoot(Vector2 direction)
        {
            speed = LaunchSpeed;
            Direction = direction;
            state = BallState.Shot;
        }
        public void Shoot()
        {
            Vector2 direction = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            Shoot(direction);
        }

        private void Move(float dt)
        {
            position += velocity * dt;
            rotation = (rotation + rotationSpeed * dt) % (2 * (float)Math.PI);
            BounceOnBounds();
        }

        private void Wander(float dt)
        {
            position += wanderingSpeed * new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation)) * dt;
            BounceOnBounds();
        }

        private void BounceOnBounds()
        {
            Rectangle bounds = ((GameScene)rootScene).LevelBounds;
            if (Collider.Left < bounds.Left)
            {
                position.X = bounds.Left + offset.X;
                if (state == BallState.Shot) velocity.X *= -1;
                else ChangeState(BallState.Waiting);
            }
            else if (Collider.Right >= bounds.Right)
            {
                position.X = bounds.Right + offset.X - size.X;
                if (state == BallState.Shot) velocity.X *= -1;
                else ChangeState(BallState.Waiting);
            }
        }

        public override void OnCollide(Sprite otherSprite, CollisionSide side)
        {            
            if (otherSprite is Paddle || otherSprite is Obstacle)
            {
                switch (side)
                {
                    case CollisionSide.Top:
                        position.Y = otherSprite.Collider.Bottom + offset.Y;
                        if(otherSprite is Paddle)
                        {
                            if (state == BallState.Shot) velocity.Y *= -1;
                            else Shoot(Center - otherSprite.Center); 

                        }
                        else
                        {
                            if (state == BallState.Shot) velocity.Y *= -1;
                            else ChangeState(BallState.Waiting);
                        } 
                        break;
                    case CollisionSide.Bottom:
                        position.Y = otherSprite.Collider.Top + offset.Y - Collider.Height;
                        if (otherSprite is Paddle)
                        {
                            if (state == BallState.Shot) Direction = (Center - otherSprite.Center);
                            else Shoot(Center - otherSprite.Center);
                        }
                        else
                        {
                            if (state == BallState.Shot) velocity.Y *= -1;
                            else ChangeState(BallState.Waiting);
                        }
                        break;
                    case CollisionSide.Left:
                        position.X = otherSprite.Collider.Right + offset.X;
                        if (otherSprite is Paddle)
                        {
                            if (state == BallState.Shot) velocity.X *= -1; 
                            else Shoot(Center - otherSprite.Center);
                        }
                        else
                        {
                            if (state == BallState.Shot) velocity.X *= -1;
                            else ChangeState(BallState.Waiting);
                        }
                        break;
                    case CollisionSide.Right:
                        position.X = otherSprite.Collider.Left + offset.X - Collider.Width;
                        if (otherSprite is Paddle)
                        {
                            if (state == BallState.Shot) velocity.X *= -1;
                            else Shoot(Center - otherSprite.Center);
                        }
                        else
                        {
                            if (state == BallState.Shot) velocity.X *= -1;
                            else ChangeState(BallState.Waiting);
                        }
                        break;
                    default:
                        break;
                }
            }

        }


        public override void Draw()
        {
            if (IsActive && texture != null)
            {
                spriteBatch.Draw(texture, new Rectangle((position).ToPoint(), size.ToPoint()), null, color, rotation, textureActualOrigin, SpriteEffects.None, 0);
                if(state == BallState.Wandering)
                {
                    spriteBatch.Draw(visitorTexture, new Rectangle((position).ToPoint(), size.ToPoint()), null, Color.White, rotation, textureActualOrigin, SpriteEffects.None, 0);
                }
            }
        }
    }
}
