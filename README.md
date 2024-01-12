# _PolymorphicJson_ - An Extension for Newtonsoft.Json

![PolymorphicJson logo](https://github.com/DysonCore/PolymorphicJson/assets/39878275/44bc4a94-f9be-44c2-be59-28facebb7a7d)

**PolymorphicJson** is a UPM package that enhances the capabilities of `newtonsoft.json` by providing a straightforward, intuitive, and generalized deserialization flow for complex polymorphism. 

## Installation 
To add this package to your Unity project:

- _Open_ the **Package Manager**.
- _press_ the **Add** button.
- _Select_ `Add package from git URL`.
- _enter_ the URL: `https://github.com/DysonCore/PolymorphicJson.git`.

For a detailed guide, refer to [Unity How to Install a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

### Dependencies

This package requires the **Newtonsoft.Json** library to be installed. 

If You haven’t already, You can add it as a [UPM package](https://github.com/applejag/Newtonsoft.Json-for-Unity/wiki/Install-official-via-UPM).  


## Description

- `PolymorphicJsonConverter` - Provides custom JSON deserialization for objects annotated with `TypifyingProperty` attribute.
- `TypifyingPropertyAttribute` - Designates a property for polymorphic deserialization as a qualifier. This attribute should be applied both on the property declaration and its value assignment. 

## Usage

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
Lastly, integrate the `PolymorphicJsonConverter` into  `JsonSerializer` or `JsonSerializerSettings` (preferred method).
```csharp
var settings = new JsonSerializerSettings();
settings.Converters.Add(new PolymorphicJsonConverter());
```
```csharp
var serializer = new JsonSerializer();
serializer.Converters.Add(new PolymorphicJsonConverter());
```
As another option, You can annotate the base class with `[JsonConverter(typeof(PolymorphicJsonConverter))]` (though it is not a recommended approach).
```csharp
[JsonConverter(typeof(PolymorphicJsonConverter))]
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
// Note: The above has been tested with the [JsonConverter(typeof(PolymorphicJsonConverter))] attribute applied to the Reward class.
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

## Typifying types

**PolymorphicJson** allows a great deal of flexibility when choosing the type for typifying property. Both `value types` and `reference types` which implement the `IEquatable<T>` interface are valid.

**\*Tip\*:** the most concise and convenient type for qualifying property is `enum` in combination with `StringEnumConverter`. 

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
When `TypifyingPropertyAttribute` encounters unknown value in qualifying property - it has 2 ways to handle it:

- `UnknownTypeHandling.ThrowError` - Throws `JsonSerializationException`. 
- `UnknownTypeHandling.ReturnNull` - Returns null for an object.

By default `UnknownTypeHandling.ThrowError` is used. To specify otherwise, pass `UnknownTypeHandling` `enum` as a parameter in `TypifyingPropertyAttribute` constructor in the root class.  

```csharp
public abstract class Animal
{
    [TypifyingProperty(UnknownTypeHandling.ReturnNull)]
    public abstract string AnimalType { get; }
}
```

## Typified Properties
The `TypifiedPropertyAttribute` allows You to deserialize polymorphic class members with the same `TypifyingPropertyAttribute` value as the main class. 

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

With this setup `PolymorphicJsonConverter` will correctly deserialize `IQuestProgress` composite member of the `Quest` class by using `[TypifyingProperty] QuestType` value from the main class. 

## Initialization and Performance

`PolymorphicJsonConverter` requires knowledge of potential derived types for accurate deserialization. **By default**, the converter will scan assemblies which are referencing the **PolymorphicJson** assembly. However, for enhanced initialization performance, You can specify assemblies in constructor (single or array):
```csharp
var converter = new PolymorphicJsonConverter(Assembly.GetExecutingAssembly());
// Note: creation of new PolymorphicJsonConverter instance will re-write converters static cache. 
```
Specifying assemblies directly can reduce the initialization time and garbage generation.

## Known limitations
-   Plain `[TypifyingProperty]` can not be used with the interface as an inheritance root! `[TypifyingProperty(typeof(Interface))]` should be used in derived classes instead.

## Remarks
-   Although unit tests are covering the most common use cases, it is never a bad idea to test Your polymorphic models and parsing correctness after initial implementation. 


## SafeStringEnumConverter

`SafeStringEnumConverter` is an inheritor of Newtonsoft `StringEnumConverter` and it is designed to safely handle enum deserialization, providing additional support for default values via a custom `DefaultEnumValueAttribute`.

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

In this example, an invalid enum value in the JSON string is safely converted to the default `FoodType.Unknown`.


## Feedback and Contributions

Your feedback is invaluable to **PolymorphicJson** improvements. For bug reports, suggestions, feature requests, or contributions, please visit the GitHub repository.
