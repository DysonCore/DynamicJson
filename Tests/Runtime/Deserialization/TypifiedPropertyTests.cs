using System.Collections.Generic;
using DysonCore.PolymorphicJson.Attributes;
using DysonCore.PolymorphicJson.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Tests.Runtime.Deserialization
{
    public class TypifiedPropertyTests
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
        public void DeserializeCompositeList_CompletesSuccessfully()
        {
            List<Quest> quests = new List<Quest>
            {
                new Quest(new NormalQuestProgress()),
                new SpecialQuest(new SpecialQuestProgress())
            };
            
            string questsString = JsonConvert.SerializeObject(quests, _settings);
            List<Quest> deserializedQuestList = JsonConvert.DeserializeObject<List<Quest>>(questsString, _settings);
            
            for (int i = 0; i < deserializedQuestList.Count; i++)
            {
                Assert.IsNotNull(deserializedQuestList[i]);
                Assert.IsNotNull(deserializedQuestList[i].Progress);
                Assert.IsInstanceOf(quests[i].GetType(),  deserializedQuestList[i]);
                Assert.IsInstanceOf(quests[i].Progress.GetType(),  deserializedQuestList[i].Progress);
            }
        }
        
        
        #region TestModels_Quests

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
    }
}