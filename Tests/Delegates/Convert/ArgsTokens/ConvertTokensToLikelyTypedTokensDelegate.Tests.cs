using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

using Interfaces.Delegates.Convert;

using Controllers.Instances;

using Models.ArgsTokens;

namespace Delegates.Convert.ArgsTokens.Tests
{
    public class ConvertTokensToLikelyTypedTokensDelegateTests
    {
        private IConvertAsyncDelegate<IEnumerable<string>, IAsyncEnumerable<(string, Tokens)>> convertTokensToLikelyTypedTokensDelegate;
        private Models.Status.Status testStatus = new Models.Status.Status();

        public ConvertTokensToLikelyTypedTokensDelegateTests()
        {
            var singletonInstancesController = new SingletonInstancesController(true);

            this.convertTokensToLikelyTypedTokensDelegate = singletonInstancesController.GetInstance(
                typeof(ConvertTokensToLikelyTypedTokensDelegate))
                as ConvertTokensToLikelyTypedTokensDelegate;
        }

        private async Task<List<(string, Tokens)>> ConvertTokensToLikelyTypedTokens(params string[] tokens)
        {
            var likelyTypedTokens = new List<(string, Tokens)>();
            await foreach (var likelyTypedToken in convertTokensToLikelyTypedTokensDelegate.ConvertAsync(tokens, testStatus))
                likelyTypedTokens.Add(likelyTypedToken);
            
            return likelyTypedTokens;
        }

        [Theory]
        [InlineData("download", "productimages")]
        [InlineData("download", "accountproductimages")]
        [InlineData("download", "screenshots")]
        [InlineData("download", "productfiles")]
        [InlineData("prepare", "productimages")]
        [InlineData("prepare", "accountproductimages")]
        [InlineData("prepare", "screenshots")]
        [InlineData("prepare", "productfiles")]
        [InlineData("update", "products")]
        [InlineData("update", "gameproductdata")]
        [InlineData("update", "accountproducts")]
        [InlineData("update", "apiproducts")]
        [InlineData("update", "gamedetails")]
        [InlineData("update", "updated")]
        [InlineData("update", "wishlisted")]
        [InlineData("update", "screenshots")]
        public async void CanConvertTokensToLikelyTypedTokensDelegateMethodTitlesCollectionTitles(params string[] tokens)
        {
            var likelyTypedTokens = await ConvertTokensToLikelyTypedTokens(tokens);

            Assert.NotEmpty(likelyTypedTokens);
            Assert.Equal(2, likelyTypedTokens.Count());
            Assert.Equal(Tokens.MethodTitle, likelyTypedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.CollectionTitle, likelyTypedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("authorize", "--username")]
        [InlineData("authorize", "--password")]
        [InlineData("download", "--id")]
        [InlineData("download", "--os")]
        [InlineData("download", "--lang")]
        [InlineData("prepare", "--id")]
        [InlineData("prepare", "--os")]
        [InlineData("prepare", "--lang")]
        [InlineData("update", "--id")]
        public async void CanConvertTokensToLikelyTypedTokensDelegateMethodTitlesParameterTitles(params string[] tokens)
        {
            var likelyTypedTokens = await ConvertTokensToLikelyTypedTokens(tokens);

            Assert.NotEmpty(likelyTypedTokens);
            Assert.Equal(2, likelyTypedTokens.Count());
            Assert.Equal(Tokens.MethodTitle, likelyTypedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.ParameterTitle, likelyTypedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("productimages", "download")]
        [InlineData("productimages", "prepare")]
        [InlineData("products", "update")]
        public async void CanConvertTokensToLikelyTypedTokensDelegateWrongOrderOfValidTokensProducesLikelyParameterValues(params string[] tokens)
        {
            var likelyTypedTokens = await ConvertTokensToLikelyTypedTokens(tokens);

            Assert.NotEmpty(likelyTypedTokens);
            Assert.Equal(2, likelyTypedTokens.Count());
            Assert.Equal(Tokens.CollectionTitle, likelyTypedTokens.ElementAt(0).Item2);
            Assert.Equal(Tokens.LikelyParameterValue, likelyTypedTokens.ElementAt(1).Item2);
        }

        [Theory]
        [InlineData("")]
        [InlineData("arbitrarystring")]
        public async void CanConvertTokensToLikelyTypedTokensDelegateRandomInputProducesLikelyParameterValues(params string[] tokens)
        {
            var likelyTypedTokens = await ConvertTokensToLikelyTypedTokens(tokens);

            Assert.NotEmpty(likelyTypedTokens);
            Assert.Single(likelyTypedTokens);
            Assert.Equal(Tokens.LikelyParameterValue, likelyTypedTokens.ElementAt(0).Item2);
        }
    }
}