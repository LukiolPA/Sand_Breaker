
using Sand_Breaker.Scenes;
using Sand_Breaker.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Sand_Breaker
{
    public class GameController
    {
        private string folderPath = "Levels";
        private ISceneManager sceneManager;

        public int CurrentLevel { get; private set; } = 1;
        public int MaxLevel { get; private set; } = 1;

        public GameController()
        {
            ServiceLocator.Register<GameController>(this);
            sceneManager = ServiceLocator.Get<ISceneManager>();
            MaxLevel = CountLevels();
        }

        private void reset()
        {
            CurrentLevel = 1;
            GameScene.RemoveAllBonus();
        }


        public void LevelUp(BonusType? gatheredBonus)
        {
            if (CurrentLevel < MaxLevel)
            {
                CurrentLevel++;
                sceneManager.LoadScene<BonusScene>(gatheredBonus);
            }
            else
            {
                sceneManager.LoadScene<VictoryScene>();
                reset();
            }
        }

        public void GameOver()
        {
            sceneManager.LoadScene<DefeatScene>();
            reset();
        }

        public char[,] GetObstacleGrid()
        {
            string path = $"{folderPath}/Level{CurrentLevel}.txt";
            List<string> lines = new List<string>();
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while((line = sr.ReadLine()) != null)
                    lines.Add(line);
            }

            int nbRows = lines.Count;
            int nbColumns = lines[0].Length;
            char[,] grid = new char[nbColumns, nbRows];

            for(int i=0; i < nbRows; i++)
            {
                for(int j=0; j < nbColumns; j++)
                {
                    grid[j, i] = lines[i][j];
                }
            }
            return grid;
        }

        private int CountLevels()
        {
            if(Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath, "Level*.txt");
                return files.Length;
            }
            else throw new DirectoryNotFoundException($"Directory {folderPath} does not exist");
        }
    }
}
