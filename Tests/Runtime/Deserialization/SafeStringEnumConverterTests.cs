using System.Collections.Generic;
using DysonCore.PolymorphicJson.Attributes;
using DysonCore.PolymorphicJson.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Tests.Runtime.Deserialization
{
    public class SafeStringEnumConverterTests
    {
        private JsonSerializerSettings _settings;
    
        [SetUp]
        public void SetUp()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new SafeStringEnumConverter(new CamelCaseNamingStrategy { OverrideSpecifiedNames = false }));
        }

        [Test]
        public void DeserializeEnums_CompletesSuccessfully()
        {
            List<Food> foodList = new List<Food> { Food.Pasta, Food.Pizza, Food.Unknown };

            string foodListString = JsonConvert.SerializeObject(foodList, _settings);

            List<Food> deserializedFoodList = JsonConvert.DeserializeObject<List<Food>>(foodListString, _settings);

            for (int i = 0; i < deserializedFoodList.Count; i++)
            {
                Assert.IsNotNull(deserializedFoodList[i]);
                Assert.AreEqual(foodList[i], deserializedFoodList[i]);
            }
        }
        
        [Test]
        public void DeserializeEnumsWithWrongValue_CompletesSuccessfully()
        {
            List<Food> foodList = new List<Food> { Food.Pasta, Food.Pizza, Food.Unknown };

            string foodListString = "[\"pasta\",\"pizza\",\"burger\"]";

            List<Food> deserializedFoodList = JsonConvert.DeserializeObject<List<Food>>(foodListString, _settings);

            for (int i = 0; i < deserializedFoodList.Count; i++)
            {
                Assert.IsNotNull(deserializedFoodList[i]);
                Assert.AreEqual(foodList[i], deserializedFoodList[i]);
            }
        }
        
        [Test]
        public void DeserializeEnumWithWrongValue_CompletesSuccessfully()
        {
            Food food = Food.Unknown;

            string foodString = "\"burger\"";
            
            Food deserializedFood = JsonConvert.DeserializeObject<Food>(foodString, _settings);
            
            Assert.AreEqual(food, deserializedFood);
        }
        
        private enum Food
        {
            [DefaultEnumValue]
            Unknown = 0,
            Pizza,
            Pasta
        }
    }
}