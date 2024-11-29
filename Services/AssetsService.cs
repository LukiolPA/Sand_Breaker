
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace Sand_Breaker.Services
{
    public interface IAssetsService
    {
        public T Get<T>(string assetName);
    }

    public sealed class AssetsService : IAssetsService
    {
        private Dictionary<string, object> assetMap = new Dictionary<string, object>();
        private ContentManager contentManager;

        public AssetsService()
        {
            contentManager = ServiceLocator.Get<ContentManager>();
            ServiceLocator.Register<IAssetsService>(this);
        }

        public void Load<T>(string assetName)
        {
            if (assetMap.ContainsKey(assetName)) throw new InvalidOperationException($"{assetName} already loaded");
            assetMap[assetName] = contentManager.Load<T>(assetName);
        }


            
        public T Get<T>(string assetName)
        {
            if (!assetMap.ContainsKey(assetName))
                throw new InvalidOperationException($"Assets service : {assetName} of type {typeof(T)} not found.");
            else
                return (T)assetMap[assetName];
        }
    }
}
