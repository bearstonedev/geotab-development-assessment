using System;
using Xunit;

namespace JokeGenerator.Tests
{
    public class GuardTests
    {
        [Fact]
        public void ShouldThrowIfNull()
        {
            Assert.Throws(typeof(ArgumentNullException), () => Guard.NotNull(null, "a null argument"));
        }

        [Fact]
        public void ShouldNotThrowIfNotNull()
        {
            var exception = Record.Exception(() => Guard.NotNull("I'm not null!", "a non-null argument"));
            Assert.Null(exception);
        }
    }
}