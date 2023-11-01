# PolymorphicJson - An Extension for Newtonsoft.Json
**PolymorphicJson** is a UPM package that enhances the capabilities of `newtonsoft.json` by providing a straightforward, intuitive, and generalized deserialization flow for complex polymorphism. 

## Installation 
To add this package to your Unity project:

- _Open_ the **Package Manager**.
- _press_ the **Add** button.
- _Select_ `Add package from git URL`.
- _enter_ the URL: `https://github.com/DysonCore/PolymorphicJson.git`.

For a detailed guide, refer to [Unity How to Install a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

**Alternatively** You can place the `DysonCore.PolymorphicJson.dll` file into **\*\*Your_Unity_Project\*\*/Assets/Plugins** directory.

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
Despite using the abstract `Reward` class for deserialization, the `deserializedRewards` list will correctly contain instances of the concrete `CurrencyReward` and `BadgeReward` classes.

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

-	`Animal` is the top-level base class with a qualifying attribute `AnimalType`.
-	`Mammal`, an `abstract` derived class, assigns the value `"Mammal"` to the `AnimalType` and introduces its own qualifier `MammalType`.
-	Concrete classes `Dog` and `Cat` further override `MammalType` qualifier to provide specific values.

When deserializing a list of `Animal`, PolymorphicJson will inspect the qualifiers and correctly instantiate `Dog` and `Cat` objects based on the provided JSON, even with such nested hierarchies.

### Qualifying Properties

**PolymorphicJson** allows a great deal of flexibility when choosing qualifying properties. Both `value types` and `reference types` that implement the `IEquatable<T>` interface are valid. This offers a vast array of possibilities in defining polymorphic relationships.

**\*Tip\*:** the most concise and convenient type for qualifying property is `enum` in combination with `StringEnumConverter`. 

## Initialization and Performance

`PolymorphicJsonConverter` requires knowledge of potential derived types for accurate deserialization. **By default**, the converter will scan assemblies which are referencing the `PolymorphicJson` assembly. However, for enhanced initialization performance, You can specify assemblies in constructor:
```csharp
var converter = new PolymorphicJsonConverter(new [] { typeof(YourClass).Assembly });
// Note: creation of new PolymorphicJsonConverter instance will re-write converters static cache. 
```
Specifying assemblies directly can reduce the initialization time and garbage generation.

## Feedback and Contributions

Your feedback is invaluable to **PolymorphicJson** improvements. For bug reports, suggestions, feature requests, or contributions, please visit the GitHub repository.