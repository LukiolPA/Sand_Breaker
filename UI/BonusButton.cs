using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sand_Breaker.GameObjects;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;
using System;
namespace Sand_Breaker.UI
{

    public interface IClickable
    {
        public bool IsClicked(IMouseService mouse);
        public bool IsHovered(IMouseService mouse);
        public void OnClick();

        public IMouseService Mouse { get; }
    }


    public class BonusButton : Sprite, IClickable
    {
        private BonusType type;
        private Texture2D backgroundTexture;
        private static Color backgroundMainColor = Color.SteelBlue;
        private static Color backgroundAlternativeColor = Color.MediumAquamarine;
        private Color backgroundColor;
        private IMouseService mouse;
        public bool isSelected;


        public IMouseService Mouse => mouse;

        public BonusButton(Scene rootScene, Vector2 position, float scale, BonusType type, bool isSelected = false) : base(rootScene, "", position, scale)
        {
            this.type = type;
            this.isSelected = isSelected;
            mouse = ServiceLocator.Get<IMouseService>();
            switch (type)
            {
                case BonusType.PaddleFaster:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/PaddleFasterIcon");
                    break;
                case BonusType.PaddleBigger:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/PaddleBiggerIcon");
                    break;
                case BonusType.BallFaster:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/BallFasterIcon");
                    break;
                case BonusType.BallBigger:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/BallBiggerIcon");
                    break;
                case BonusType.MoreShells:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/MoreShellsIcon");
                    break;
                case BonusType.MoreLuck:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/MoreLuckIcon");
                    break;
                case BonusType.MoreDamage:
                    this.texture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/MoreDamageIcon");
                    break;
            }

            this.size = new Vector2(this.texture.Width * scale, this.texture.Height * scale);
            this.offset = new Vector2(0.5f * size.X, 0.5f * size.Y);
            this.textureActualOrigin = offset * this.texture.Bounds.Size.ToVector2() / size;
            backgroundTexture = ServiceLocator.Get<IAssetsService>().Get<Texture2D>("Sprites/BonusIcons/BonusBackground");
            backgroundColor = backgroundMainColor;
        }

        public override void Update(float dt)
        {
            backgroundColor = isSelected ? backgroundAlternativeColor : backgroundMainColor;
        }

        public bool IsClicked(IMouseService mouse)
        {
            if (IsHovered(mouse))
            {
                return mouse.MouseJustReleased(MouseButton.Left);
            }
            else return false;
        }
        public bool IsHovered(IMouseService mouse)
        {
            return mouse.MouseHovers(Collider);
        }

        public void OnClick()
        {
            ((BonusScene)rootScene).chosenBonus = type;
        }

        public override void Draw()
        {

            if (IsActive && texture != null)
            {
                spriteBatch.Draw(backgroundTexture, new Rectangle((position).ToPoint(), size.ToPoint()), null, backgroundColor, rotation, textureActualOrigin, SpriteEffects.None, 0);
            }
            base.Draw();
        }
    }
}
