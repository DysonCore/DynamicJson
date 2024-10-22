using System.Collections.Generic;
using DysonCore.DynamicJson.PolymorphicParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace DysonCore.DynamicJson.Tests.Runtime
{
    [TestFixture]
    public class TypifiedPropertyTests
    {
        private JsonSerializerSettings _settings;
    
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new PolymorphicConverter());
            _settings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy { OverrideSpecifiedNames = false }));
        }
        
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _settings = null;
        }
        
        [Test]
        public void DeserializeCompositeClasses_CompletesSuccessfully()
        {
            Quest normalQuest = new Quest(new NormalQuestProgress());
            Quest specialQuest = new SpecialQuest(new SpecialQuestProgress());

            string normalQuestString = JsonConvert.SerializeObject(normalQuest, _settings);
            string specialQuestString = JsonConvert.SerializeObject(specialQuest, _settings);

            Quest deserializedNormalQuest = JsonConvert.DeserializeObject<Quest>(normalQuestString, _settings);
            Quest deserializedSpecialQuest = JsonConvert.DeserializeObject<Quest>(specialQuestString, _settings);
            
            Assert.IsNotNull(deserializedNormalQuest);
            Assert.IsNotNull(deserializedSpecialQuest);
            
            Assert.IsNotNull(deserializedNormalQuest.Progress);
            Assert.IsNotNull(deserializedSpecialQuest.Progress);
            
            Assert.IsInstanceOf<NormalQuestProgress>(deserializedNormalQuest.Progress);
            Assert.IsInstanceOf<SpecialQuestProgress>(deserializedSpecialQuest.Progress);
        }
        
        [Test]
        public void DeserializeCompositeClass_CompletesSuccessfully()
        {
            Reward defaultReward = new Reward(new RewardData());
            Reward normalReward = new Reward(new NormalRewardData());
            Reward superReward = new Reward(new SuperRewardData());

            string defaultRewardString = JsonConvert.SerializeObject(defaultReward, _settings);
            string normalRewardString = JsonConvert.SerializeObject(normalReward, _settings);
            string superRewardString = JsonConvert.SerializeObject(superReward, _settings);

            Reward deserializedDefaultReward = JsonConvert.DeserializeObject<Reward>(defaultRewardString, _settings);
            Reward deserializedNormalReward = JsonConvert.DeserializeObject<Reward>(normalRewardString, _settings);
            Reward deserializedSuperReward = JsonConvert.DeserializeObject<Reward>(superRewardString, _settings);
            
            Assert.IsNotNull(deserializedDefaultReward);
            Assert.IsNotNull(deserializedNormalReward);
            Assert.IsNotNull(deserializedSuperReward);
            
            Assert.IsNotNull(deserializedDefaultReward.RewardData);
            Assert.IsNotNull(deserializedNormalReward.RewardData);
            Assert.IsNotNull(deserializedSuperReward.RewardData);
            
            Assert.IsInstanceOf<RewardData>(deserializedDefaultReward.RewardData);
            Assert.IsInstanceOf<NormalRewardData>(deserializedNormalReward.RewardData);
            Assert.IsInstanceOf<SuperRewardData>(deserializedSuperReward.RewardData);
        }
        
        [Test]
        public void DeserializeCompositeList_CompletesSuccessfully()
        {
            List<Quest> quests = new List<Quest>
            {
                new Quest(new NormalQuestProgress()),
                new SpecialQuest(new SpecialQuestProgress())
            };
            
            string questsString = JsonConvert.SerializeObject(quests, _settings);
            List<Quest> deserializedQuestList = JsonConvert.DeserializeObject<List<Quest>>(questsString, _settings);
            
            Assert.IsNotNull(deserializedQuestList);
            Assert.AreEqual(quests.Count, deserializedQuestList.Count);
            
            for (int i = 0; i < deserializedQuestList.Count; i++)
            {
                Assert.IsNotNull(deserializedQuestList[i]);
                Assert.IsNotNull(deserializedQuestList[i].Progress);
                Assert.IsInstanceOf(quests[i].GetType(),  deserializedQuestList[i]);
                Assert.IsInstanceOf(quests[i].Progress.GetType(),  deserializedQuestList[i].Progress);
            }
        }
        
        [Test]
        public void DeserializeTypifiedWithInterface_CompletesSuccessfully()
        {
            TequilaDescription tequilaDescription = new TequilaDescription("throat burner");
            RumDescription rumDescription = new RumDescription("pure sand", "refreshing as Sahara desert");
            
            HardDrink tequila = new HardDrink("tequila", tequilaDescription);
            HardDrink rum = new HardDrink("rum", rumDescription);
            
            List<HardDrink> drinks = new List<HardDrink>
            {
                tequila,
                rum
            };
            
            string drinksList = JsonConvert.SerializeObject(drinks, _settings);
            List<HardDrink> deserializedDrinksList = JsonConvert.DeserializeObject<List<HardDrink>>(drinksList, _settings);
            
            Assert.IsNotNull(deserializedDrinksList);
            Assert.AreEqual(drinks.Count, deserializedDrinksList.Count);
            
            for (int i = 0; i < deserializedDrinksList.Count; i++)
            {
                Assert.IsNotNull(deserializedDrinksList[i]);
                Assert.IsNotNull(deserializedDrinksList[i].Type);
                Assert.IsInstanceOf(drinks[i].GetType(),  deserializedDrinksList[i]);
                Assert.IsInstanceOf(drinks[i].Description.GetType(),  deserializedDrinksList[i].Description);
            }
        }
        
        
#region TestModel_Quests

        private class Quest
        {
            [TypifyingProperty] 
            [JsonProperty] 
            public virtual QuestType QuestType => QuestType.Normal;

            [TypifiedProperty]
            [JsonProperty]
            public IQuestProgress Progress { get; protected set; }

            internal Quest(IQuestProgress progress)
            {
                Progress = progress;
            }

            internal Quest(){}
        }
        
        private class SpecialQuest : Quest
        {
            [TypifyingProperty]
            public override QuestType QuestType => QuestType.Special;

            internal SpecialQuest(IQuestProgress progress)
            {
                Progress = progress;
            }

            [JsonConstructor]
            internal SpecialQuest(){}
        }
        
        private interface IQuestProgress
        {
            [TypifyingProperty]
            [JsonIgnore]
            QuestType QuestType { get; }
        }
        
        private class NormalQuestProgress : IQuestProgress
        {
            [TypifyingProperty(typeof(IQuestProgress))]
            public QuestType QuestType => QuestType.Normal;
        }
        
        private class TemporaryQuestProgress : IQuestProgress
        {
            [TypifyingProperty(typeof(IQuestProgress))]
            public QuestType QuestType => QuestType.Temporary;
        }

        private class SpecialQuestProgress : IQuestProgress
        {
            [TypifyingProperty(typeof(IQuestProgress))]
            public QuestType QuestType => QuestType.Special;
        }

        private enum QuestType
        {
            Normal,
            Temporary,
            Special
        }

#endregion

#region TestModel_Rewards

        private class Reward
        {
            [TypifyingProperty]
            [JsonProperty]
            public string Type { get; private set; }

            [JsonProperty("rewardData")] 
            [TypifiedProperty]
            public RewardData RewardData { get; private set; }

            internal Reward(RewardData rewardData)
            {
                RewardData = rewardData;
                Type = rewardData.Type;
            }

            internal Reward(string type)
            {
                Type = type;
            }

            internal Reward() { }
        }

        private class RewardData
        {
            [JsonIgnore]
            [TypifyingProperty]
            public virtual string Type => "default";
        }

        private class SuperRewardData : RewardData
        {
            [TypifyingProperty] 
            public override string Type => "superRewardData";
        }
        
        private class NormalRewardData : RewardData
        {
            [TypifyingProperty] 
            public override string Type => "normalRewardData";
        }
    
#endregion

#region TestModel_Drinks

        private interface IDrink
        {
            public string Type { get; }

            public DrinkDescription Description { get; }
        }

        private class SoftDrink : IDrink
        {
            [JsonProperty]
            public string Type { get; }

            [JsonProperty] 
            public bool IsChilled { get; private set; }
            
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public DrinkDescription Description { get; }

            private SoftDrink() { }

            public SoftDrink(bool isChilled = true)
            {
                IsChilled = isChilled;
            }
        }

        private class HardDrink : IDrink
        {
            [JsonProperty]
            [TypifyingProperty]
            public string Type { get; }

            [JsonProperty]
            [TypifiedProperty]
            public DrinkDescription Description { get; private set; }
            
            private HardDrink() { }

            public HardDrink(string type, DrinkDescription description)
            {
                Type = type;
                Description = description;
            }

        }

        private abstract class DrinkDescription
        {
            [JsonProperty]
            [TypifyingProperty] 
            public abstract string Type { get; }

            [JsonProperty] 
            public string Name { get; protected set; }

            protected DrinkDescription()
            {
            }

            protected DrinkDescription(string name)
            {
                Name = name;
            }
        }

        private class TequilaDescription : DrinkDescription
        {
            [TypifyingProperty]
            public override string Type => "tequila";

            private TequilaDescription() { }

            public TequilaDescription(string name) : base(name) { }
        }
        
        private class RumDescription : DrinkDescription
        {
            [TypifyingProperty] 
            public override string Type => "rum";

            public string Description { get; private set; }

            private RumDescription() { }

            public RumDescription(string name, string description) : base(name)
            {
                Description = description;
            }
        }

#endregion
    }
}