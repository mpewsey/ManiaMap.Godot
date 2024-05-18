using Godot;
using MPewsey.ManiaMapGodot.Exceptions;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot
{
    public static class AsyncResourceLoader
    {
        private static int _threadStatusCheckDelay = 16;
        public static int ThreadStatusCheckDelay { get => _threadStatusCheckDelay; set => _threadStatusCheckDelay = Mathf.Max(value, 1); }

        public static async Task<T> LoadAsync<T>(string path, string typeHint = "", bool useSubThreads = false,
            ResourceLoader.CacheMode cacheMode = ResourceLoader.CacheMode.Reuse) where T : Resource
        {
            var error = ResourceLoader.LoadThreadedRequest(path, typeHint, useSubThreads, cacheMode);

            if (error != Error.Ok)
                throw new ThreadedResourceRequestException($"Error occured while requesting resource: (Error = {error}, Path = {path})");

            var status = ResourceLoader.LoadThreadedGetStatus(path);

            while (status != ResourceLoader.ThreadLoadStatus.Loaded)
            {
                if (status != ResourceLoader.ThreadLoadStatus.InProgress)
                    throw new ThreadedResourceRequestException($"Thread load status error: (Error = {status}, Path = {path})");

                await Task.Delay(ThreadStatusCheckDelay);
                status = ResourceLoader.LoadThreadedGetStatus(path);
            }

            return (T)ResourceLoader.LoadThreadedGet(path);
        }
    }
}