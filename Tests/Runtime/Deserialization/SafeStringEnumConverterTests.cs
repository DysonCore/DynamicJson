using System.Collections.Generic;
using DysonCore.PolymorphicJson.Attributes;
using DysonCore.PolymorphicJson.Converters;
using DysonCore.PolymorphicJson.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            _settings.Converters.Add(new PolymorphicConverter());
            _settings.Converters.Add(new SafeStringEnumConverter(new CamelCaseNamingStrategy { OverrideSpecifiedNames = false }));
        }

        [Test]
        public void DeserializeEnums_CompletesSuccessfully()
        {
            List<Food> foodList = new List<Food>
            {
                Food.Pasta, 
                Food.Pizza, 
                Food.Unknown
            };

            string foodListString = JsonConvert.SerializeObject(foodList, _settings);

            List<Food> deserializedFoodList = JsonConvert.DeserializeObject<List<Food>>(foodListString, _settings);
            
            Assert.IsNotNull(deserializedFoodList);
            Assert.IsTrue(deserializedFoodList.Count == foodList.Count);

            for (int i = 0; i < deserializedFoodList.Count; i++)
            {
                Assert.IsNotNull(deserializedFoodList[i]);
                Assert.AreEqual(foodList[i], deserializedFoodList[i]);
            }
        }
        
        [Test]
        public void DeserializeEnumsWithWrongValue_CompletesSuccessfully()
        {
            List<Food> foodList = new List<Food>
            {
                Food.Pasta, 
                Food.Pizza, 
                Food.Unknown
            };
            //burger is not present in Food enum
            string foodListString = "[\"pasta\",\"pizza\",\"burger\"]";

            List<Food> deserializedFoodList = JsonConvert.DeserializeObject<List<Food>>(foodListString, _settings);
            
            Assert.IsNotNull(deserializedFoodList);
            Assert.IsTrue(deserializedFoodList.Count == foodList.Count);

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
            //burger is not present in Food enum
            string foodString = "\"burger\"";
            
            Food deserializedFood = JsonConvert.DeserializeObject<Food>(foodString, _settings);
            
            Assert.AreEqual(food, deserializedFood);
        }
        
        [Test]
        public void IntegrationWithPolymorphicConverter_DeserializeWithCorrectEnums_CompletesSuccessfully()
        {
            List<Plate> plates = new List<Plate>
            {
                new PastaPlate(),
                new PizzaPlate(),
                new UnknownPlate()
            };

            string platesString = JsonConvert.SerializeObject(plates, _settings);
            
            List<Plate> deserializedPlates = JsonConvert.DeserializeObject<List<Plate>>(platesString, _settings);
            
            Assert.IsNotNull(deserializedPlates);
            Assert.IsTrue(deserializedPlates.Count == plates.Count);

            for (int i = 0; i < deserializedPlates.Count; i++)
            {
                Assert.IsNotNull(deserializedPlates[i]);
                Assert.IsInstanceOf(plates[i].GetType(), deserializedPlates[i]);
                Assert.AreEqual(plates[i].FoodType, deserializedPlates[i].FoodType);
            }
        }
        
        [Test]
        public void IntegrationWithPolymorphicConverter_DeserializeWithWrongEnums_CompletesSuccessfully()
        {
            List<Plate> plates = new List<Plate>
            {
                new PastaPlate(),
                new PizzaPlate(),
                new UnknownPlate()
            };
            //burger is not present in Food enum
            string platesString = "[{\"FoodType\":\"pasta\"},{\"FoodType\":\"pizza\"},{\"FoodType\":\"burger\"}]";
            
            List<Plate> deserializedPlates = JsonConvert.DeserializeObject<List<Plate>>(platesString, _settings);
            
            Assert.IsNotNull(deserializedPlates);
            Assert.IsTrue(deserializedPlates.Count == plates.Count);

            for (int i = 0; i < deserializedPlates.Count; i++)
            {
                Assert.IsNotNull(deserializedPlates[i]);
                Assert.IsInstanceOf(plates[i].GetType(), deserializedPlates[i]);
                Assert.AreEqual(plates[i].FoodType, deserializedPlates[i].FoodType);
            }
        }
        
        [Test]
        public void RegularStringEnumConverter_DeserializeWithWrongEnums_ThrowsError()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new PolymorphicConverter());
            settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy { OverrideSpecifiedNames = false }));
            
            //burger is not present in Food enum
            string platesString = "[{\"FoodType\":\"pasta\"},{\"FoodType\":\"pizza\"},{\"FoodType\":\"burger\"}]";
            //regular Newtonsoft.StringEnumConverter fails to convert burger to any Food enum value and throws an error
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<List<Plate>>(platesString, settings));
        }

#region TestModels_Foods

        private enum Food
        {
            [DefaultEnumValue]
            Unknown = 0,
            Pizza,
            Pasta
        }

        private abstract class Plate
        {
            [TypifyingProperty(UnknownTypeHandling.ReturnNull)]
            public abstract Food FoodType { get; }
        }

        private class PizzaPlate : Plate
        {
            [TypifyingProperty]
            public override Food FoodType => Food.Pizza;
        }

        private class PastaPlate : Plate
        {
            [TypifyingProperty]
            public override Food FoodType => Food.Pasta;
        }

        private class UnknownPlate : Plate
        {
            [TypifyingProperty]
            public override Food FoodType => Food.Unknown;
        }

#endregion
    }
}