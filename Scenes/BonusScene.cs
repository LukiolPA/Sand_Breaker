
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sand_Breaker.GameObjects;
using Sand_Breaker.Services;
using Sand_Breaker.UI;
using System;
using System.Collections.Generic;

namespace Sand_Breaker.Scenes
{
    public enum BonusType
    {
        PaddleFaster,
        PaddleBigger,
        BallFaster,
        BallBigger,
        MoreShells,
        MoreLuck,
        MoreDamage,
    }

    public class BonusScene : Scene
    {
        private IMouseService mouse = ServiceLocator.Get<IMouseService>();
        private int nbBonusMini = 2;

        private BonusButton selectedBonus;
        public BonusType chosenBonus { private get; set; }
        public override void Load(params object[] data)
        {
            if (data.Length == 0 || !(data[0] is null || data[0] is BonusType))
            {
                throw new ArgumentException($"Param data[0] should be a nullable BonusType");
            }
            AddObject(new TextObject(this, new Vector2(screen.Width*0.5f, screen.Height*0.7f), ServiceLocator.Get<IAssetsService>().Get<SpriteFont>("Fonts/MainFont48"), Color.DeepSkyBlue, "Choose a permanent bonus"));
            CreateBonusButton(((BonusType?)data[0]));


        }

        private void CreateBonusButton(BonusType? additionalBonus)
        {
            List<BonusType> offeredBonus = offerRandomBonus(additionalBonus);
            if (additionalBonus is not null)
            {
                BonusButton btn = new BonusButton(this, new Vector2(screen.Width * 0.5f, screen.Height * 0.3f), 0.5f, additionalBonus.Value);
                AddObject(btn);
                AddObject(new Sprite(this, "Sprites/BonusIcons/Bonus", Color.Gold, new Vector2(screen.Width * 0.5f-23, screen.Height * 0.18f), 0.1f, Vector2.Zero));
            }

            selectedBonus = new BonusButton(this, new Vector2(screen.Width * 0.2f, screen.Height * 0.3f), 0.5f, offeredBonus[0], true);
            AddObject(selectedBonus);
            
            AddObject(new BonusButton(this, new Vector2(screen.Width*0.8f, screen.Height*0.3f), 0.5f, offeredBonus[1]));

        }


        private List<BonusType> offerRandomBonus(BonusType? additionalBonus)
        {
            List<BonusType> listToPickFrom = new List<BonusType>();
            listToPickFrom.AddRange(Enum.GetValues<BonusType>());
            List<BonusType> bonusPicked = new List<BonusType>();

            if(additionalBonus != null)
            {
                listToPickFrom.Remove(additionalBonus.Value);
            }
            for(int i = 0; i< nbBonusMini; i++)
            {
                if (listToPickFrom.Count >= 0)
                {
                    BonusType randomBonus = listToPickFrom[new Random().Next(listToPickFrom.Count)];
                    bonusPicked.Add(randomBonus);
                    listToPickFrom.Remove(randomBonus);
                }
            }

            return bonusPicked;
        }

        private void ManageBonusSelection()
        {
            List<BonusButton> all_buttons = GetObjects<BonusButton>();
            if (ManageMouseSelection(all_buttons)) return;
            else ManageKeyboardSelection(all_buttons);
            
        }

        private void ManageKeyboardSelection(List<BonusButton> all_buttons)
        {
            if (keyboard.KeyJustPressed(Keys.Left))
            {
                for(int i=0; i<all_buttons.Count; i++)
                {
                    if (all_buttons[i] == selectedBonus)
                    {
                        if (i < all_buttons.Count - 1) ChangeSelectedBonus(all_buttons[i + 1]);
                        else ChangeSelectedBonus(all_buttons[0]);
                        return;
                    }
                }
            }
            else if (keyboard.KeyJustPressed(Keys.Right))
            {
                for (int i = 0; i < all_buttons.Count; i++)
                {
                    if (all_buttons[i] == selectedBonus)
                    {
                        if (i > 0) ChangeSelectedBonus(all_buttons[i-1]);
                        else ChangeSelectedBonus(all_buttons[all_buttons.Count - 1]);
                        return;
                    }
                }
            }
        }

        private bool ManageMouseSelection(List<BonusButton> all_buttons)
        {
            foreach (BonusButton button in all_buttons)
            {
                if (button != selectedBonus && button.IsHovered(mouse))
                {
                    ChangeSelectedBonus(button);
                }
            }
            return false;
        }
        
        private void ChangeSelectedBonus(BonusButton bonus)
        {
            selectedBonus.isSelected = false;
            bonus.isSelected = true;
            selectedBonus = bonus;
        }


        public override void Update(float dt)
        {
            base.Update(dt);
            ManageBonusSelection();
            if (keyboard.KeyJustReleased(Keys.Space) || keyboard.KeyJustReleased(Keys.Enter))
            {
                selectedBonus.OnClick();
                sceneManager.LoadScene<GameScene>(chosenBonus);
            }
            foreach (BonusButton button in GetObjects<BonusButton>())
            {
                if (button.IsClicked(mouse))
                {
                    button.OnClick();
                    sceneManager.LoadScene<GameScene>(chosenBonus);
                }
            }
        }
    }

 }
