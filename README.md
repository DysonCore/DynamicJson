# PolymorphicJson - extension for Newtonsoft.Json
**PolymorphicJson** is a UPM package which extends capabilities of `newtonsoft.json` by adding simple, intuitive and generalized deserialization for complex polymorphism. 

## Installation 
To add this package to Unity project, you should:

- _open_ **Package Manager**
- _press_ **Add** button
- _select_ `Add package from git URL`
- _enter_ url: `https://github.com/DysonCore/PolymorphicJson.git`

For detailed explanation - visit [Unity How to Install a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

**Alternatively** you can put `DysonCore.PolymorphicJson.dll` file to **(Your Unity Project)/Assets/Plugins** folder.

## Description

- `PolymorphicJsonConverter` - Provides custom JSON deserialization for objects marked with `TypifyingProperty` attribute.
- `TypifyingPropertyAttribute` - Marks property for polymorphic deserialization as a qualifier. Should be used on property declaration and value assignment. 

## How to use

Start by annotating qualifying property (`abstract` or `virtual`) in the base class with `[TypifyingProperty]` attribute.
```csharp
public abstract class Reward
{
    [TypifyingProperty] 
    // Can be used together with [JsonProperty] attribute. 
    public abstract string RewardType { get; }
}
```
Then, in the inheritor class(_es_), annotate this property one more time and assign value which determines this class. 
```csharp
public class CurrencyReward : Reward
{
    [TypifyingProperty]
    // It is recommended to add sealed keyword if you do not want to
    // inherit from this class and re-override this value. 
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
And that is pretty much it. All it is left to do is to add `PolymorphicJsonConverter` to `JsonSerializer` or `JsonSerializerSettings` (preferred).
```csharp
var settings = new JsonSerializerSettings();
settings.Converters.Add(new PolymorphicJsonConverter());
```
```csharp
var serializer = new JsonSerializer();
serializer.Converters.Add(new PolymorphicJsonConverter());
```
Alternatively, you can annotate base class(_es_) with `[JsonConverter(typeof(PolymorphicJsonConverter))]` (not recommended).
```csharp
[JsonConverter(typeof(PolymorphicJsonConverter))]
public abstract class Reward
{
    //fields and properties...
}
```
And now you can run something like this:
```csharp
List<Reward> rewards = new List<Reward>
{
    new CurrencyReward{Amount = 100},
    new BadgeReward{BadgeId = "newbie_badge_01"}
};
            
string rewardsJson = JsonConvert.SerializeObject(rewards);
// rewardsJson = [{"RewardType":"currency","Amount":100},{"RewardType":"badge","BadgeId":"newbie_badge_01"}]
List<Reward> deserializedRewards = JsonConvert.DeserializeObject<List<Reward>>(rewardsJson); //tested with [JsonConverter(typeof(PolymorphicJsonConverter))] attribute applied to Reward class
}
```
Although we used abstract `Reward` class as a type for deserialization, `deserializedRewards` list will contain objects of concrete `CurrencyReward` and `BadgeReward` classes. 
