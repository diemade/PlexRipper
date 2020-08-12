﻿using System.Text.Json;
using System.Text.Json.Serialization;
using PlexRipper.BaseTests;
using Shouldly;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PlexApi.UnitTests
{
    public class JsonConverterTests
    {
        private BaseContainer Container { get; }

        public JsonConverterTests(ITestOutputHelper output)
        {
            BaseDependanciesTest.Setup(output);
            Container = new BaseContainer();
        }

        [Fact]
        public void ShouldReturnValidLongFromJsonString_WhenJsonStringIsValidLong()
        {
            // Arrange
            var jsonString = "{\"contentChangedAt\" : \"2247395385730901353\"}";
            
            // Act
            var longValue = JsonSerializer.Deserialize<TestDTO>(jsonString);

            // Assert
            longValue.ShouldNotBeNull();
            longValue.ContentChangedAt.ShouldBeGreaterThanOrEqualTo(long.MinValue);
            longValue.ContentChangedAt.ShouldBeLessThanOrEqualTo(long.MaxValue);
        }   
        
      }
}