![PolymorphicJson logo](https://github.com/DysonCore/PolymorphicJson/assets/39878275/44bc4a94-f9be-44c2-be59-28facebb7a7d)

**DynamicJson** is a **UnityEngine** specific **UPM package** that enhances the capabilities of `newtonsoft.json` by providing a straightforward, intuitive, and generalized deserialization of complex polymorphic models. As well as other tools to simplify workflows with **JSON**s. 

### Installation 
To add this package to your Unity project:

- _Open_ the **Package Manager**.
- _Press_ the **Add** button.
- _Select_ `Add package from git URL`.
- _Enter_ the URL: `https://github.com/DysonCore/DynamicJson.git`.

For a detailed guide, refer to [Unity How to Install a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

### Dependencies

This package requires the **Newtonsoft.Json** library to be installed.<br>
You can add it as a [UPM package](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM).  

### Converters

<details>
<summary class="summary-title">
Polymorphic converter
<summary class="summary-content">
Adds the ability to deserialize JSON with polymorphic content straight into c# instances without creating custom converters for each class. 

</summary>
</summary>

### Description

- `PolymorphicConverter` - provides custom JSON deserialization for objects annotated with `TypifyingProperty` attribute.
- `TypifyingPropertyAttribute` - designates a property for polymorphic deserialization as a qualifier. This attribute should be applied both on the property declaration and its value assignment.
-  `TypifiedPropertyAttribute` - allows to deserialize class members with the same `TypifyingPropertyAttribute` value as the main class.

### Usage

Start by marking the qualifying property (either `abstract` or `virtual`) in the base class with `[TypifyingProperty]` attribute.
```csharp
public abstract class Reward
{
    // Can be used in conjunction with the [JsonProperty] attribute. 
    [TypifyingProperty]
    public abstract string RewardType { get; }
}
```
In the derived classes, annotate the same property again and assign a unique value that identifies each class. 
```csharp
public class CurrencyReward : Reward
{
    [TypifyingProperty]
    // Tip: use the 'sealed' keyword if You don't intend 
    // to further inherit from this class and override the qualifying value.  
    public sealed override string RewardType => "currency";
    
    public int Amount { get; set; }
}
```
```csharp
public class BadgeReward : Reward
{
    [TypifyingProperty]
    public sealed override string RewardType => "badge";
    
    public string BadgeId { get; set; }
}
```
Lastly, integrate the `PolymorphicConverter` into  `JsonSerializer` or `JsonSerializerSettings` (preferred method).
```csharp
var settings = new JsonSerializerSettings();
settings.Converters.Add(new PolymorphicConverter());
```
```csharp
var serializer = new JsonSerializer();
serializer.Converters.Add(new PolymorphicConverter());
```
As another option, You can annotate the base class with `[JsonConverter(typeof(PolymorphicConverter))]` (though it is not a recommended approach).
```csharp
[JsonConverter(typeof(PolymorphicConverter))]
public abstract class Reward
{
    //fields and properties...
}
```
With this setup, You can execute the following:
```csharp
List<Reward> rewards = new List<Reward>
{
    new CurrencyReward{Amount = 100},
    new BadgeReward{BadgeId = "newbie_badge_01"}
};
            
string rewardsJson = JsonConvert.SerializeObject(rewards);
// rewardsJson value: [{"RewardType":"currency","Amount":100},{"RewardType":"badge","BadgeId":"newbie_badge_01"}]
List<Reward> deserializedRewards = JsonConvert.DeserializeObject<List<Reward>>(rewardsJson); 
// Note: The above has been tested with the [JsonConverter(typeof(PolymorphicConverter))] attribute applied to the Reward class.
}
```
Despite using the `abstract` `Reward` class for deserialization, the `deserializedRewards` list will correctly contain instances of the concrete `CurrencyReward` and `BadgeReward` classes.

### Complex Inheritance

**PolymorphicJson** is designed to address the challenges posed by intricate inheritance hierarchies in polymorphism.

Consider the scenario of an `abstract` `Animal` class. This base class has an `abstract` inheritor `Mammal`, which in turn can be inherited by other concrete classes like `Dog` or `Cat`.
```csharp
public abstract class Animal
{
    [TypifyingProperty]
    public abstract string AnimalType { get; }
}
```
```csharp
public abstract class Mammal : Animal
{
    [TypifyingProperty]
    public override abstract string AnimalType => "Mammal";

    [TypifyingProperty]
    public abstract string MammalType { get; }
}
```
```csharp
public class Dog : Mammal
{
    [TypifyingProperty]
    public sealed override string MammalType => "Dog";

    public string Breed { get; set; }
}
```
```csharp
public class Cat : Mammal
{
    [TypifyingProperty]
    public sealed override string MammalType => "Cat";

    public string Color { get; set; }
}
```

In this example:

-   `Animal` is the top-level base class with a qualifying attribute `AnimalType`.
-	`Mammal`, an `abstract` derived class, assigns the value `"Mammal"` to the `AnimalType` and introduces its own qualifier `MammalType`.
-	Concrete classes `Dog` and `Cat` further override `MammalType` qualifier to provide specific values.

When deserializing a list of `Animal`, **PolymorphicJson** will inspect the qualifiers and correctly instantiate `Dog` and `Cat` objects based on the provided JSON, even with such nested hierarchies.

### Typifying types

**PolymorphicJson** allows a great deal of flexibility when choosing the type for typifying property. Both `value types` and `reference types` which properly implement equality comparison are valid. I.e. `override bool Equals(object obj)` and `override int GetHashCode()`.

**\*Tip\*:** the most concise and convenient type for qualifying property is `enum` in combination with `Newtonsoft.StringEnumConverter` or `PolymorphicJson.SafeStringEnumConverter`. 

### Interface as Inheritance Root 

**PolymorphicJson** can't automatically find references between interface and derived classes. So if You are using interface as an inheritance root, You need to explicitly specify the interface type like so:

```csharp
public interface IAnimal
{
    [TypifyingProperty]
    AnimalType AnimalType { get; }
}
```

```csharp       
public class Mammal : IAnimal
{
    [TypifyingProperty(typeof(IAnimal))]
    public AnimalType AnimalType => AnimalType.Mammal;
}
```

### Unknown Value Handling
When `PolymorphicConverter` encounters unknown value under the `TypifyingPropertyAttribute` - it has 2 ways to handle it:

- `UnknownTypeHandling.ThrowError` - Throws `JsonSerializationException`. 
- `UnknownTypeHandling.ReturnNull` - Returns null for an object.

By default `UnknownTypeHandling.ThrowError` is used. To specify otherwise, pass `UnknownTypeHandling` `Enum` as a parameter in `PolymorphicConverter` constructor.  

```csharp
var settings = new JsonSerializerSettings();
settings.Converters.Add(new PolymorphicConverter(UnknownTypeHandling.ReturnNull));
```

### Typified Properties
The `TypifiedPropertyAttribute` allows to deserialize polymorphic class members with the same `TypifyingPropertyAttribute` value as the main class. 

Start by declaring the main class with `[TypifyingProperty]` and another root class of `IQuestProgress` as `[TypifiedProperty]`.
```csharp
public class Quest
{
    [TypifyingProperty]
    public QuestType QuestType { get; private set; }

    [TypifiedProperty]
    public IQuestProgress Progress { get; private set; }
}
```
```csharp
public enum QuestType
{
    Normal,
    Special
}
```

`IQuestProgress` class and its inheritors should have the same structure as a regular polymorphic hierarchy with `TypifyingPropertyAttribute`.

```csharp
public interface IQuestProgress
{
    [TypifyingProperty]
    QuestType QuestType { get; }
}
```
```csharp
private class NormalQuestProgress : IQuestProgress
{
    [TypifyingProperty(typeof(IQuestProgress))]
    public QuestType QuestType => QuestType.Normal;
}
```
```csharp
private class SpecialQuestProgress : IQuestProgress
{
    [TypifyingProperty(typeof(IQuestProgress))]
    public QuestType QuestType => QuestType.Special;
}
```

With this setup `PolymorphicConverter` will correctly deserialize `IQuestProgress` composite member of the `Quest` class by using `[TypifyingProperty] QuestType` value from the main class. 

### Cache initialization

`PolymorphicConverter` requires knowledge of potential derived types for accurate deserialization. Converter automatically scans assemblies which are referencing the **DynamicJson** assembly on script recompilation and pre-build process and creates a cache file under **Assets/Resources/DynamicJson** folder. 
<span class="span-warning">Do not edit, move or delete the cache file!</span> 

### Known limitations
-   Plain `[TypifyingProperty]` can not be used with the interface as an inheritance root! `[TypifyingProperty(typeof(Interface))]` should be used in derived classes instead.

### Remarks
-   `Newtonsoft.Json` does not support multiple `converters` on a single `class`. If you are using `PolymorphicConverter`'s attributes in your base `class` - make sure it will not be deserialized by another `converter`.
-   Although unit tests are covering the most common use cases, it is never a bad idea to test your polymorphic models and parsing correctness after initial implementation. 

</details>

***

<details>
<summary class="summary-title">
SafeStringEnum converter
<summary class="summary-content">
Adds the fallback for Enum deserialization. If the given json string has no corresponding Enum value - the default one will be used.

</summary>
</summary>

### Description

`SafeStringEnumConverter` is an inheritor of Newtonsoft `StringEnumConverter` and it is designed to safely handle Enum deserialization, providing additional support of fallbacks.

### Usage

Use the `DefaultEnumValueAttribute` to mark an enum member as the default value:

```csharp
public enum FoodType
{
    [DefaultEnumValue]
    Unknown,
    Pizza,
    Burger
    // other values...
}
```
```csharp
public class Plate
{
[JsonConverter(typeof(SafeStringEnumConverter))] //or you can add this converter to JsonSerializerSettings.
public FoodType Food { get; set; }
}

string json = "{\"Food\":\"Sushi\"}"; // sushi is not present in FoodType enum.
Plate plate = JsonConvert.DeserializeObject<Plate>(json);
// Plate.Food will be set to FoodType.Unknown.
```

In this example, an invalid enum value in the JSON string is safely converted to `FoodType.Unknown`.

</details>

***

<details>
<summary class="summary-title">
Injection converter (<span class="span-warning">experimental</span>)
<summary class="summary-content">
Adds the ability to inject data from a cache into the deserialized instance based on given identifiers. 

</summary>
</summary>

### Description

`InjectionConverter` gives an ability to inject data from `InjectionDataProviders` straight into `IInjectable<>` members of deserialized instance.<br><br>
This converter is not nearly as useful as other ones since any `IInjectable<TValue>` can be replaced by its plain identifier and the data can be retrieved from the cache after the deserialization is finished. But this "data retrieving" operations can occur quite frequently, so this converter can be a good "quality of life" improvement.

- `IInjectable<TValue>` - root `interface` for wrappers over any data model - `TValue`.
- `InjectionDataProvider<TIdentifier, TValue>` - root provider / cache class. Enforces implementation of methods for retrieving data - `TValue` by the identifier - `TIdentifier`. Its inheritors can be safely instantiated by any `Dependency Injector`. Only one `InjectionDataProvider` can be present at once for any `TValue` type.
- `EagerInjectable<TValue>` - concrete inheritor of `IInjectable<TValue>`. Will retrieve the data from its corresponding `InjectionDataProvider` as soon as its identifier is set.
- `LazyInjectable<TValue>` - concrete inheritor of `IInjectable<TValue>`. Will retrieve the data from its corresponding `InjectionDataProvider` only when its `TValue` value will be requested. 
- `InjectionConverter` - replaces any `IInjectable<>` with its identifier on `serialization` and puts the identifier into the `IInjectable<>` on `deserialization`.

### Usage

Lets create 2 models: `Weapon` and `WeaponConfig`. `WeaponConfigs` are predetermined, while `Weapons` are composite from `WeaponConfig` and `Name` property.

```csharp
public class WeaponConfig
{
    public string Id { get; private set; }
            
    public int Damage { get; private set; }

    public WeaponConfig(string id, int damage)
    {
        Id = id;
        Damage = damage;
    }
}
```
```csharp
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
```

The `WeaponConfig` is wrapped with `EagerInjectable<>`, meaning we can deserialize `Weapon` without full representation of `WeaponConfig`. Now lets make `InjectionDataProvider`.

```csharp
//identifier for WeaponConfig model is a string. But it can be any type. 
public class WeaponInjectionDataProvider : InjectionDataProvider<string, WeaponConfig>
{
    // Data cache.
    private readonly Dictionary<string, WeaponConfig> _data = new ();
    // Used by IInjectable<WeeaponConfig> models to resolve data from identifier.        
    public override WeaponConfig GetValue(string identifier)
    {
        _data.TryGetValue(identifier, out WeaponConfig config);
        return config;
    }
    // Used by InjectionConverter to get identifier for serialization.
    public override string GetIdentifier(WeaponConfig value)
    {
        return value.Id; // Specifies how the identifier is retrieved from the WeaponConfig model. 
    }
    // Is used to fill the cache with data for demonstration purposes.
    public void AddConfig(WeaponConfig config)
    {   
        _data[config.Id] = config;
    }
}
```

And that is it. Really.<br>
The only thing left is to plug `InjectionConverter` and test the results. This converter can work both directions (`serialization` and `deserialization`).

```csharp
// Create JSON Settings with InjectionConverter.
var settings = new JsonSerializerSettings();
settings.Converters.Add(new InjectionConverter());
// Instantiate data provider. It will be automatically added to the static registry of providers. 
// Also It can be instantiated by Dependency Injector such as Zenject, StrangeIoC, or any other. 
var weaponDataProvider = new WeaponInjectionDataProvider();
// Create weapon configs. 
WeaponConfig heavyBladeConfig = new WeaponConfig("heavy_blade_01", 100);
WeaponConfig daggerConfig = new WeaponConfig("dagger_01", 20);
// Populate providers cache with data (WeaponConfigs).            
weaponDataProvider.AddConfig(heavyBladeConfig);
weaponDataProvider.AddConfig(daggerConfig);
// Weapons are created manually to test serialization first.            
Weapon heavyWeapon = new Weapon(heavyBladeConfig, "Big spoon");
Weapon daggerWeapon = new Weapon(daggerConfig, "THE pencil");

List<Weapon> weapons = new()
{
    heavyWeapon,
    daggerWeapon,
};

string weaponsString = JsonConvert.SerializeObject(weapons, settings);
// serialized weaponsString value:
// [{"uid":"heavy_blade_01","name":"Big spoon"},{"uid":"dagger_01","name":"THE pencil"}]
// As you can see - it has no data about WeaponConfigs except its identifiers, i.e. uid.  

// Now we can deserialize it back and InjectionParser will get the WeaponConfig data from 
// WeaponDataProvider and set it into the Weapon instances. 
List<Weapon> deserializedWeapons = JsonConvert.DeserializeObject<List<Weapon>>(weaponsString, settings);
```

</details>

***

### Feedback and Contributions

Your feedback is invaluable to **DynamicJson** improvements. For bug reports, suggestions, feature requests, or contributions, please visit the GitHub repository.