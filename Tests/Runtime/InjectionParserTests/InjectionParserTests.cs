using System.Collections.Generic;
using DysonCore.DynamicJson.InjectionParser;
using Newtonsoft.Json;
using NUnit.Framework;

namespace DysonCore.DynamicJson.Tests.Runtime
{
    [TestFixture]
    public class InjectionParserTests
    {
        private JsonSerializerSettings _settings;
        private WeaponInjectionDataProvider _weaponDataProvider;

        private const string HeavyBladeId = "heavy_blade_01";
        private const string DaggerId = "dagger_01";
        private const string ShortSwordId = "short_sword_01";
        private const string CursedShortSwordId = "short_sword_02";
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new InjectionConverter());

            _weaponDataProvider = new WeaponInjectionDataProvider();

            WeaponConfig heavyBladeConfig = new WeaponConfig(HeavyBladeId, 100);
            WeaponConfig daggerConfig = new WeaponConfig(DaggerId, 20);
            WeaponConfig shortSwordConfig = new WeaponConfig(ShortSwordId, 35);
            WeaponConfig cursedShortSwordConfig = new WeaponConfig(CursedShortSwordId, 50);
            
            _weaponDataProvider.AddConfig(heavyBladeConfig);
            _weaponDataProvider.AddConfig(daggerConfig);
            _weaponDataProvider.AddConfig(shortSwordConfig);
            _weaponDataProvider.AddConfig(cursedShortSwordConfig);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _settings = null;
            _weaponDataProvider = null;
        }
        
        [Test]
        public void DeserializeModelsWithInjection_CompletesSuccessfully()
        {
            WeaponConfig heavyBladeConfig = _weaponDataProvider.GetValue(HeavyBladeId);
            WeaponConfig daggerConfig = _weaponDataProvider.GetValue(DaggerId);
            WeaponConfig shortSwordConfig = _weaponDataProvider.GetValue(ShortSwordId);
            
            Weapon heavyWeapon = new Weapon(heavyBladeConfig, "Big spoon");
            Weapon daggerWeapon = new Weapon(daggerConfig, "THE pencil");
            Weapon shortWeapon = new Weapon(shortSwordConfig, "Long boy");

            string heavyWeaponString = JsonConvert.SerializeObject(heavyWeapon, _settings);
            string daggerWeaponString = JsonConvert.SerializeObject(daggerWeapon, _settings);
            string shortWeaponString = JsonConvert.SerializeObject(shortWeapon, _settings);

            Weapon deserializedHeavyWeapon = JsonConvert.DeserializeObject<Weapon>(heavyWeaponString, _settings);
            Weapon deserializedDaggerWeapon = JsonConvert.DeserializeObject<Weapon>(daggerWeaponString, _settings);
            Weapon deserializedShortWeapon = JsonConvert.DeserializeObject<Weapon>(shortWeaponString, _settings);
            
            Assert.IsNotNull(deserializedHeavyWeapon);
            Assert.IsNotNull(deserializedDaggerWeapon);
            Assert.IsNotNull(deserializedShortWeapon);
            
            Assert.IsNotNull(deserializedHeavyWeapon.Config);
            Assert.IsNotNull(deserializedDaggerWeapon.Config);
            Assert.IsNotNull(deserializedShortWeapon.Config);
            
            Assert.AreEqual(heavyWeapon.Config.Value.Id, deserializedHeavyWeapon.Config.Value.Id);
            Assert.AreEqual(daggerWeapon.Config.Value.Id, deserializedDaggerWeapon.Config.Value.Id);
            Assert.AreEqual(shortWeapon.Config.Value.Id, deserializedShortWeapon.Config.Value.Id);
        }

        [Test]
        public void DeserializeListWithInjection_CompletesSuccessfully()
        {
            WeaponConfig heavyBladeConfig = _weaponDataProvider.GetValue(HeavyBladeId);
            WeaponConfig daggerConfig = _weaponDataProvider.GetValue(DaggerId);
            WeaponConfig shortSwordConfig = _weaponDataProvider.GetValue(ShortSwordId);
            WeaponConfig cursedShortSwordConfig = _weaponDataProvider.GetValue(CursedShortSwordId);
            
            Weapon heavyWeapon = new Weapon(heavyBladeConfig, "Big spoon");
            Weapon daggerWeapon = new Weapon(daggerConfig, "THE pencil");
            Weapon shortWeapon = new Weapon(shortSwordConfig, "Long boy");
            Weapon cursedWeapon = new Weapon(cursedShortSwordConfig, "Stinky stick");

            List<Weapon> weapons = new()
            {
                heavyWeapon,
                daggerWeapon,
                shortWeapon,
                cursedWeapon
            };

            string weaponsString = JsonConvert.SerializeObject(weapons, _settings);

            List<Weapon> deserializedWeapons = JsonConvert.DeserializeObject<List<Weapon>>(weaponsString, _settings);
            
            Assert.IsNotNull(deserializedWeapons);
            Assert.IsTrue(weapons.Count == deserializedWeapons.Count);

            for (int i = 0; i < weapons.Count; i++)
            {
                Assert.IsNotNull(deserializedWeapons[i]);
                Assert.IsNotNull(deserializedWeapons[i].Config);
                Assert.AreEqual(weapons[i].Config.Value.Id, deserializedWeapons[i].Config.Value.Id);
            }
        }
        
#region TestModel_Weapons
        
        public class WeaponConfig
        {
            [JsonProperty("uid")]
            public string Id { get; private set; }
            
            [JsonProperty("damage")] 
            public int Damage { get; private set; }

            public WeaponConfig(string id, int damage)
            {
                Id = id;
                Damage = damage;
            }

            [JsonConstructor]
            private WeaponConfig() { }
        }

        public class Weapon
        {
            [JsonProperty("uid")]
            public EagerInjectable<WeaponConfig> Config { get; private set; }

            [JsonProperty("name")]
            public string Name { get; private set; }

            public Weapon(WeaponConfig config, string name)
            {
                Config = new EagerInjectable<WeaponConfig>(config);
                Name = name;
            }

            [JsonConstructor]
            private Weapon() { }
        }

        private class WeaponInjectionDataProvider : InjectionDataProvider<string, WeaponConfig>
        {
            private readonly Dictionary<string, WeaponConfig> _data = new ();
            
            public override WeaponConfig GetValue(string identifier)
            {
                _data.TryGetValue(identifier, out WeaponConfig config);
                return config;
            }

            public override string GetIdentifier(WeaponConfig value)
            {
                return value.Id;
            }

            public void AddConfig(WeaponConfig config)
            {
                _data[config.Id] = config;
            }
        }
        
#endregion

    }
}