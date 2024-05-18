using GdUnit4;
using Godot;
using MPewsey.ManiaMapGodot.Exceptions;
using System;
using System.Threading.Tasks;

namespace MPewsey.ManiaMapGodot.Tests
{
    [TestSuite]
    public class TestAsyncResourceLoader
    {
        private const string EmptyScene = "uid://m8fcccufss1";

        [TestCase]
        public async Task TestLoadAsync()
        {
            var task = AsyncResourceLoader.LoadAsync<PackedScene>(EmptyScene, cacheMode: ResourceLoader.CacheMode.Ignore);
            await task;
            Assertions.AssertThat(task.IsCompleted).IsTrue();
            Assertions.AssertThat(task.IsCompletedSuccessfully).IsTrue();
            Assertions.AssertThat(task.Result != null).IsTrue();
        }

        [TestCase]
        public async Task TestLoadAsyncThrowsErrorOnNonExistentFilePath()
        {
            var exceptionThrown = false;

            try
            {
                await AsyncResourceLoader.LoadAsync<Resource>("res://bad_file_path.tres");
            }
            catch (Exception exception)
            {
                Assertions.AssertThat(exception).IsInstanceOf<ThreadedResourceRequestException>();
                exceptionThrown = true;
            }

            Assertions.AssertBool(exceptionThrown).IsTrue();
        }
    }
}