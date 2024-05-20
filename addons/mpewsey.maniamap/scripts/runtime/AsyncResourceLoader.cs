using Godot;
using MPewsey.ManiaMapGodot.Exceptions;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot
{
    /// <summary>
    /// Contains methods for loading Resources asynchronously.
    /// </summary>
    public static class AsyncResourceLoader
    {
        private static int _threadStatusCheckDelay = 16;
        /// <summary>
        /// The delay time in milliseconds between each resource load status check.
        /// </summary>
        public static int ThreadStatusCheckDelay { get => _threadStatusCheckDelay; set => _threadStatusCheckDelay = Mathf.Max(value, 1); }

        /// <summary>
        /// Loads a Resource from file asynchronously.
        /// </summary>
        /// <param name="path">The resource path.</param>
        /// <exception cref="ThreadedResourceRequestException">Thrown if an error occurs while loading.</exception>
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