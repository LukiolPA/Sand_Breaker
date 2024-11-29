
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sand_Breaker.GameObjects;
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;

namespace Sand_Breaker.UI
{
    public class TextObject : Object
    {

        private string text;
        private Color color;
        private SpriteFont font;
        private Vector2 offset;
        private float scale;


        private static SpriteBatch spriteBatch = ServiceLocator.Get<SpriteBatch>();
        public TextObject(Scene rootScene, Vector2 position, SpriteFont font, Color color, float scale, Vector2 offset, string text = "Text") : base(rootScene, position)
        {
            this.text = text;
            this.color = color;
            this.font = font;
            this.offset = offset;
            this.scale = scale;
        }


        public TextObject(Scene rootScene, Vector2 position, SpriteFont font, Color color, string text = "Text", float scale = 1) : this(rootScene, position, font, color, scale, Vector2.Zero, text)
        {
            offset = font.MeasureString(text) * 0.5f * scale;
        }
        public TextObject(Scene rootScene, Vector2 position, SpriteFont font, string text = "Text", float scale = 1) : this(rootScene, position, font, Color.Black, scale, Vector2.Zero, text) { }


        public override void Draw()
        {
            spriteBatch.DrawString(font, text, position, color, 0, offset, scale, SpriteEffects.None, 0);
        }
    }
}
