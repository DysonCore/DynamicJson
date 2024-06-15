using System;
using System.Collections.Generic;
using DysonCore.DynamicJson.PolymorphicParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace DysonCore.DynamicJson.Tests.Runtime.PolymorphicParserTests
{
    [TestFixture]
    public class TypifyingPropertyTests 
    {
        private JsonSerializerSettings _settings;
    
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new PolymorphicConverter(UnknownTypeHandling.ReturnNull));
            _settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy { OverrideSpecifiedNames = false }));
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _settings = null;
        }
    
        [Test]
        public void DeserializeClasses_CompletesSuccessfully()
        {
            CoinReward coin = new CoinReward();
            GoldReward gold = new GoldReward();
            LimitedEditionReward limitedEdition = new LimitedEditionReward();
            RegularEditionReward regularEdition = new RegularEditionReward();
            WarriorBadge warriorBadge = new WarriorBadge();
            MageBadge mageBadge = new MageBadge();
            NewbieBadge newbieBadge = new NewbieBadge();

            string coinJson = JsonConvert.SerializeObject(coin, _settings);
            string goldJson = JsonConvert.SerializeObject(gold, _settings);
            string limitedEditionJson = JsonConvert.SerializeObject(limitedEdition, _settings);
            string regularEditionJson = JsonConvert.SerializeObject(regularEdition, _settings);
            string warriorBadgeJson = JsonConvert.SerializeObject(warriorBadge, _settings);
            string mageBadgeJson = JsonConvert.SerializeObject(mageBadge, _settings);
            string newbieBadgeJson = JsonConvert.SerializeObject(newbieBadge, _settings);

            Reward coinReward = JsonConvert.DeserializeObject<Reward>(coinJson, _settings);
            Reward goldReward = JsonConvert.DeserializeObject<Reward>(goldJson, _settings);
            Reward limitedEditionReward = JsonConvert.DeserializeObject<Reward>(limitedEditionJson, _settings);
            Reward regularEditionReward = JsonConvert.DeserializeObject<Reward>(regularEditionJson, _settings);
            Reward warriorBadgeReward = JsonConvert.DeserializeObject<Reward>(warriorBadgeJson, _settings);
            Reward mageBadgeReward = JsonConvert.DeserializeObject<Reward>(mageBadgeJson, _settings);
            Reward newbieBadgeReward = JsonConvert.DeserializeObject<Reward>(newbieBadgeJson, _settings);
            
            Assert.IsNotNull(coinReward);
            Assert.IsNotNull(goldReward);
            Assert.IsNotNull(limitedEditionReward);
            Assert.IsNotNull(regularEditionReward);
            Assert.IsNotNull(warriorBadgeReward);
            Assert.IsNotNull(mageBadgeReward);
            Assert.IsNotNull(newbieBadgeReward);
            
            Assert.IsInstanceOf<CoinReward>(coinReward);
            Assert.IsInstanceOf<GoldReward>(goldReward);
            Assert.IsInstanceOf<LimitedEditionReward>(limitedEditionReward);
            Assert.IsInstanceOf<RegularEditionReward>(regularEditionReward);
            Assert.IsInstanceOf<WarriorBadge>(warriorBadgeReward);
            Assert.IsInstanceOf<MageBadge>(mageBadgeReward);
            Assert.IsInstanceOf<NewbieBadge>(newbieBadgeReward);
        }
    
        [Test]
        public void DeserializeList_CompletesSuccessfully()
        {
            List<Reward> rewards = new List<Reward>
            {
                new CoinReward(),
                new GoldReward(),
                new LimitedEditionReward(),
                new RegularEditionReward(),
                new WarriorBadge(),
                new MageBadge(),
                new NewbieBadge()
            };

            string rewardsJson = JsonConvert.SerializeObject(rewards, _settings);
            List<Reward> deserializedRewards = JsonConvert.DeserializeObject<List<Reward>>(rewardsJson, _settings);

            for (int i = 0; i < rewards.Count; i++)
            {
                Assert.IsInstanceOf(rewards[i].GetType(), deserializedRewards[i]);
            }
        }
        
        [Test]
        public void DeserializeComposition_CompletesSuccessfully()
        {
            List<RewardWrapper> wrappers = new List<RewardWrapper>
            {
                new RewardWrapper{Reward = new CoinReward()},
                new RewardWrapper{Reward = new GoldReward()},
                new RewardWrapper{Reward = new NewbieBadge()},
                new RewardWrapper{Reward = new WarriorBadge()},
                new RewardWrapper{Reward = new MageBadge()},
                new RewardWrapper{Reward = new LimitedEditionReward()},
                new RewardWrapper{Reward = new RegularEditionReward()}
            };

            string rewardsJson = JsonConvert.SerializeObject(wrappers, _settings);
            List<RewardWrapper> deserializedWrappers = JsonConvert.DeserializeObject<List<RewardWrapper>>(rewardsJson, _settings);
            
            Assert.IsNotNull(deserializedWrappers);
            Assert.AreEqual(deserializedWrappers.Count, wrappers.Count);
            
            for (int i = 0; i < wrappers.Count; i++)
            {
                Assert.IsNotNull(deserializedWrappers[i]);
                Assert.IsNotNull(deserializedWrappers[i].Reward);
                Assert.IsInstanceOf(wrappers[i].Reward.GetType(), deserializedWrappers[i].Reward);
            }
        }
        
        [Test]
        public void DeserializeWrongType_NullValueHandling_ReturnsNull()
        {
            string rewardsJson = "[{\"badgeNumber\":105,\"RewardType\":\"badge\"},{\"badgeNumber\":106,\"RewardType\":\"badge\"},{\"RewardType\":\"badge\",\"badgeNumber\":107}]";
            Assert.DoesNotThrow(() =>
            {
                List<Reward> deserializedRewards = JsonConvert.DeserializeObject<List<Reward>>(rewardsJson, _settings);
                foreach (var reward in deserializedRewards)
                {
                    Assert.IsNull(reward);
                }
            });
        }
        
        [Test]
        public void DeserializeWrongTypeComposition_NullValueHandling_ReturnsNull()
        {
            string rewardsJson = "[{\"Reward\":{\"badgeNumber\":105,\"RewardType\":\"badge\"}}]";
            Assert.DoesNotThrow(() =>
            {
                List<RewardWrapper> deserializedRewards = JsonConvert.DeserializeObject<List<RewardWrapper>>(rewardsJson, _settings);
                foreach (var rewardWrapper in deserializedRewards)
                {
                    Assert.IsNull(rewardWrapper.Reward);
                }
            });
        }
        
        [Test]
        public void DeserializeListOfInterfaces_CompletesSuccessfully()
        {
            List<IAnimal> animals = new List<IAnimal>
            {
                new Mammal(),
                new Bird(),
                new Fish()
            };

            string animalJson = JsonConvert.SerializeObject(animals, _settings);
            List<IAnimal> deserializedAnimals = JsonConvert.DeserializeObject<List<IAnimal>>(animalJson, _settings);
            
            Assert.IsNotNull(deserializedAnimals);
            Assert.AreEqual(deserializedAnimals.Count, animals.Count);

            for (int i = 0; i < animals.Count; i++)
            {
                Assert.IsNotNull(deserializedAnimals[i]);
                Assert.IsInstanceOf(animals[i].GetType(), deserializedAnimals[i]);
            }
        }
        
        [Test]
        public void DeserializeWithClassQualifier_CompletesSuccessfully()
        {
            List<Colour> colours = new List<Colour>
            {
                new Red(),
                new Green(),
                new White()
            };

            string coloursJson = JsonConvert.SerializeObject(colours, _settings);
            List<Colour> deserializedColours = JsonConvert.DeserializeObject<List<Colour>>(coloursJson, _settings);
            
            Assert.IsNotNull(deserializedColours);
            Assert.AreEqual(deserializedColours.Count, colours.Count);

            for (int i = 0; i < colours.Count; i++)
            {
                Assert.IsNotNull(deserializedColours[i]);
                Assert.IsInstanceOf(colours[i].GetType(), deserializedColours[i]);
                Assert.AreEqual(colours[i].RGB, deserializedColours[i].RGB);
            }
        }
        
#region TestModel_Reward

        private class RewardWrapper
        {
            public Reward Reward { get; set; }
        }
        
        private abstract class Reward
        {
            [TypifyingProperty] 
            public abstract RewardType RewardType { get; }
        }

        private abstract class CurrencyReward : Reward
        {
            [TypifyingProperty] 
            public sealed override RewardType RewardType => RewardType.Currency;

            [TypifyingProperty]
            [JsonProperty("currencyType")]
            public abstract string Currency { get; }
        }

        private class GoldReward : CurrencyReward
        {
            [TypifyingProperty] 
            public override string Currency => "Gold";
        }

        private class CoinReward : CurrencyReward
        {
            [TypifyingProperty] 
            public override string Currency => "Coin";
        }

        private class NewbieBadge : Reward
        {
            [TypifyingProperty] 
            public sealed override RewardType RewardType => RewardType.Badge;

            [TypifyingProperty]
            [JsonProperty("badgeNumber")]
            public virtual int BadgeId => 100;
        }

        private class WarriorBadge : NewbieBadge
        {
            [TypifyingProperty] 
            public override int BadgeId => 101;
        }

        private class MageBadge : NewbieBadge
        {
            [TypifyingProperty] 
            public override int BadgeId => 102;
        }

        private abstract class SpecialReward : Reward
        {
            [TypifyingProperty] 
            public sealed override RewardType RewardType => RewardType.Special;
            [TypifyingProperty] 
            public abstract bool IsLimitedEdition { get; }
        }

        private class LimitedEditionReward : SpecialReward
        {
            [TypifyingProperty] 
            public sealed override bool IsLimitedEdition => true;
        }

        private class RegularEditionReward : SpecialReward
        {
            [TypifyingProperty] 
            public sealed override bool IsLimitedEdition => false;
        }

        private enum RewardType
        {
            Currency,
            Badge,
            Special
        }

#endregion
        
#region TestModel_Animal

        private interface IAnimal
        {
            [TypifyingProperty]
            [JsonProperty("type")]
            AnimalType AnimalType { get; }
        }
        
        private class Mammal : IAnimal
        {
            [TypifyingProperty(typeof(IAnimal))]
            public AnimalType AnimalType => AnimalType.Mammal;
        }
        
        private class Bird : IAnimal
        {
            [TypifyingProperty(typeof(IAnimal))]
            public AnimalType AnimalType => AnimalType.Bird;
        }

        private class Fish : IAnimal
        {
            [TypifyingProperty(typeof(IAnimal))]
            public AnimalType AnimalType => AnimalType.Fish;
        }

        private enum AnimalType
        {
            Mammal,
            Bird,
            Fish
        }
        
#endregion

#region TestModel_Colour

        private abstract class Colour
        {
            [TypifyingProperty]
            public abstract RGB RGB { get; }
        }

        private class Red : Colour
        {
            [TypifyingProperty]
            public override RGB RGB { get; } = new RGB(255, 0, 0);
        }
        
        private class Green : Colour
        {
            [TypifyingProperty]
            public override RGB RGB { get; } = new RGB(0, 255, 0);
        }
        
        private class White : Colour
        {
            [TypifyingProperty]
            public override RGB RGB { get; } = new RGB(255, 255, 255);
        }

        private class RGB
        {
            public int R { get; }
            public int G { get; }
            public int B { get; }

            public RGB(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                
                return obj.GetType() == this.GetType() && Equals((RGB)obj);
            }

            private bool Equals(RGB other)
            {
                return R == other.R && G == other.G && B == other.B;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(R, G, B);
            }
        }

#endregion

    }
}
