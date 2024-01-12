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
    public class TypifyingPropertyTests 
    {
        private JsonSerializerSettings _settings;
    
        [SetUp]
        public void SetUp()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new PolymorphicJsonConverter());
            _settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy { OverrideSpecifiedNames = false }));
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
            
            for (int i = 0; i < wrappers.Count; i++)
            {
                Assert.IsInstanceOf(wrappers[i].Reward.GetType(), deserializedWrappers[i].Reward);
            }
        }
        
        [Test]
        public void DeserializeWrongType_NullValueHandling_ReturnNull_CompletesSuccessfully()
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
        public void DeserializeWrongTypeComposition_NullValueHandling_ReturnNull_CompletesSuccessfully()
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

            for (int i = 0; i < animals.Count; i++)
            {
                Assert.IsInstanceOf(animals[i].GetType(), deserializedAnimals[i]);
            }
        }
        
        #region TestModels_Reward

        private class RewardWrapper
        {
            public Reward Reward { get; set; }
        }
        
        private abstract class Reward
        {
            [TypifyingProperty] public abstract RewardType RewardType { get; }
        }

        private abstract class CurrencyReward : Reward
        {
            [TypifyingProperty] public sealed override RewardType RewardType => RewardType.Currency;

            [TypifyingProperty]
            [JsonProperty("currencyType")]
            public abstract string Currency { get; }
        }

        private class GoldReward : CurrencyReward
        {
            [TypifyingProperty] public override string Currency => "Gold";
        }

        private class CoinReward : CurrencyReward
        {
            [TypifyingProperty] public override string Currency => "Coin";
        }

        private class NewbieBadge : Reward
        {
            [TypifyingProperty] public sealed override RewardType RewardType => RewardType.Badge;

            [TypifyingProperty(UnknownTypeHandling.ReturnNull)]
            [JsonProperty("badgeNumber")]
            public virtual int BadgeId => 100;
        }

        private class WarriorBadge : NewbieBadge
        {
            [TypifyingProperty] public override int BadgeId => 101;
        }

        private class MageBadge : NewbieBadge
        {
            [TypifyingProperty] public override int BadgeId => 102;
        }

        private abstract class SpecialReward : Reward
        {
            [TypifyingProperty] public sealed override RewardType RewardType => RewardType.Special;
            [TypifyingProperty] public abstract bool IsLimitedEdition { get; }
        }

        private class LimitedEditionReward : SpecialReward
        {
            [TypifyingProperty] public sealed override bool IsLimitedEdition => true;
        }

        private class RegularEditionReward : SpecialReward
        {
            [TypifyingProperty] public sealed override bool IsLimitedEdition => false;
        }

        private enum RewardType
        {
            Currency,
            Badge,
            Special
        }

        #endregion
        
        #region TestModels_Animal

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

    }
}
