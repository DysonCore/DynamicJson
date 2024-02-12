using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DysonCore.PolymorphicJson.Utils;
using Newtonsoft.Json;

namespace DysonCore.PolymorphicJson.PolymorphicConverter
{
    /// <summary>
    /// Provides a centralized data provider for managing <see cref="PropertyData"/> mappings used by polymorphic converter.
    /// It handles mappings of abstract types and their concrete implementations to facilitate correct type resolution during deserialization.
    /// </summary>
    internal static class PropertyDataProvider
    {
        /// <summary>
        /// Stores mappings from base types to their corresponding <see cref="TypifyingPropertyData"/>.
        /// Used to resolve the correct type during JSON deserialization based on <see cref="TypifyingPropertyAttribute"/>.
        /// </summary>
        internal static Dictionary<Type, TypifyingPropertyData> BaseToPropertyData { get; } = new ();
        /// <summary>
        /// Temporal list of tuples containing abstract types of secondary inheritance (when abstract class assigns value to the abstract property of base class) and associated <see cref="TypifyingPropertyData"/>.
        /// Used to store additional data required for <see cref="PostProcessAbstractClasses"/>. Gets cleared when <see cref="InitializeCache"/> is finished.
        /// </summary>
        private static readonly List<(Type abstractType, TypifyingPropertyData propertyData)> AbstractDefiningData = new ();
        /// <summary>
        /// Temporal dictionary of types to a list of <see cref="TypifiedPropertyData"/>, storing information for types that have been marked with <see cref="TypifiedPropertyAttribute"/>.
        /// Used to store additional data required for <see cref="PostProcessTypifiedProperties"/>. Gets cleared when <see cref="InitializeCache"/>> is finished.
        /// </summary>
        private static readonly Dictionary<Type, List<TypifiedPropertyData>> TypifiedDefiningData = new ();
        /// <summary>
        /// Indicates whether the PropertyDataProvider has been initialized, to prevent redundant initializations.
        /// </summary>
        private static bool _initialized;
        
        /// <summary>
        /// Initializes the <see cref="PropertyDataProvider"/> with data from the specified assemblies.
        /// Scans the provided assemblies to build the data mappings required for polymorphic deserialization.
        /// </summary>
        /// <param name="assembliesToUse">The assemblies to scan for <see cref="TypifyingPropertyAttribute"/> and <see cref="TypifiedPropertyAttribute"/>. If left empty - all the assemblies which are referencing this assembly will be scanned.</param>
        internal static void Initialize(params Assembly[] assembliesToUse)
        {
            if (assembliesToUse.Length > 0)
            {
                BaseToPropertyData.Clear();
                InitializeCache(assembliesToUse);
            }
            else
            {
                if (!_initialized)
                {
                    InitializeCache(Assembly.GetExecutingAssembly().GetReferencingAssemblies());
                }
            }

            _initialized = true;
        }
        
        /// <summary>
        /// Initializes provider by scanning the given assemblies for types with the <see cref="TypifyingPropertyAttribute"/>
        /// and populates the <see cref="BaseToPropertyData"/> dictionary.
        /// </summary>
        /// <param name="assemblies">List of assemblies to scan.</param>
        private static void InitializeCache(params Assembly[] assemblies)
        {
            AbstractDefiningData.Clear();
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type classType in assembly.GetTypes())
                {
                    foreach (PropertyInfo propertyInfo in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        ProcessProperty(propertyInfo, classType);
                    }
                }
            }

            PostProcessAbstractClasses();
            PostProcessTypifiedProperties();
            
            AbstractDefiningData.Clear();
            TypifiedDefiningData.Clear();
        }

        /// <summary>
        /// Processes a property to determine whether it has <see cref="TypifyingPropertyAttribute"/> or <see cref="TypifiedPropertyAttribute"/> and ensures that it is not marked as both.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property to process.</param>
        /// <param name="classType">The Type of the class that the property belongs to.</param>
        /// <exception cref="Exception">Throws the <see cref="Exception"/> if single property marked with both <see cref="TypifyingPropertyAttribute"/> and <see cref="TypifiedPropertyAttribute"/></exception>
        private static void ProcessProperty(PropertyInfo propertyInfo, Type classType)
        {
            TypifyingPropertyAttribute typifyingAttribute = propertyInfo.GetCustomAttribute<TypifyingPropertyAttribute>();
            TypifiedPropertyAttribute typifiedAttribute = propertyInfo.GetCustomAttribute<TypifiedPropertyAttribute>();

            bool isTypifying = typifyingAttribute != null;
            bool isTypified = typifiedAttribute != null;

            if (isTypifying && isTypified)
            {
                throw new Exception($"[{nameof(PropertyDataProvider)}.{nameof(ProcessProperty)}] {classType.Name}.{propertyInfo.Name} property has {nameof(TypifyingPropertyAttribute)} and {nameof(TypifiedPropertyAttribute)} at the same time!");
            }

            if (isTypifying)
            {
                ProcessTypifyingProperty(propertyInfo, classType, typifyingAttribute);
            }

            if (isTypified)
            {
                ProcessTypifiedProperty(propertyInfo, classType, typifiedAttribute);
            }
        }
        
        /// <summary>
        /// Processes properties marked with <see cref="TypifyingPropertyAttribute"/> to establish their role in polymorphic deserialization.
        /// This method also ensures the consistency of typifying properties in base and derived classes by validating
        /// the attribute's presence in the base class, thereby supporting correct type resolution in polymorphic scenarios.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object of the property with <see cref="TypifyingPropertyAttribute"/></param>
        /// <param name="classType">The Type of the class that contains the current property with <see cref="TypifyingPropertyAttribute"/>.</param>
        /// <param name="typifyingAttribute">The <see cref="TypifyingPropertyAttribute"/> applied to the property.</param>
        private static void ProcessTypifyingProperty(PropertyInfo propertyInfo, Type classType, TypifyingPropertyAttribute typifyingAttribute)
        {
            Type propertyType = propertyInfo.PropertyType;
            JsonPropertyAttribute jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();

            Type baseClass = typifyingAttribute.InheritanceRoot ?? classType.GetDeclaringClass(propertyInfo.Name); //get base declaring class for the current property. Manual declaration of root class is prioritized.  
            TypifyingPropertyAttribute baseAttribute = baseClass.GetProperty(propertyInfo.Name)?.GetCustomAttribute<TypifyingPropertyAttribute>();

            if (baseAttribute == null)
            {
                throw new Exception($"[{nameof(PropertyDataProvider)}.{nameof(InitializeCache)}] {baseClass.Name} has no property with {nameof(TypifyingPropertyAttribute)} and \"{propertyInfo.Name}\" {nameof(propertyInfo.Name)}.\nReferencing class - {classType.FullName}.");
            }

            if (!BaseToPropertyData.TryGetValue(baseClass, out TypifyingPropertyData propertyData)) //get or create TypifyingPropertyData from / in BaseToPropertyData.
            {
                propertyData = new TypifyingPropertyData(propertyType, propertyInfo.Name, jsonProperty?.PropertyName, baseAttribute);
                BaseToPropertyData[baseClass] = propertyData;
            }

            if (classType.IsAbstract) //if class is abstract - its impossible to create instance of it, so
            {
                if (classType != baseClass) //if current class is abstract class, and current class is not a class which defines current TypifyingProperty -> add this class to the list of abstract classes for post-processing. 
                {
                    AbstractDefiningData.Add((classType, propertyData));
                }

                return;
            }

            //create an instance of non-abstract class and get the value of its property marked with TypifyingPropertyAttribute.
            object classInstance = Activator.CreateInstance(classType, true);
            object propertyValue = propertyInfo.GetValue(classInstance);
            //add to corresponding TypifyingPropertyData.
            propertyData.ValuesData[propertyValue] = classType;
        }

        /// <summary>
        /// Processes properties marked with <see cref="TypifiedPropertyAttribute"/>, capturing their serialization details.
        /// The method adds information about <see cref="TypifiedPropertyAttribute"/> to the <see cref="TypifiedDefiningData"/>, which is essential
        /// for handling custom serialization and deserialization behaviors of these composed properties based on <see cref="TypifyingPropertyAttribute"/> of the holder class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> object of the property with <see cref="TypifiedPropertyAttribute"/>.</param>
        /// <param name="classType">The Type of the class that contains the the property with <see cref="TypifiedPropertyAttribute"/>.</param>
        /// <param name="typifiedAttribute">The <see cref="TypifiedPropertyAttribute"/> applied to the property.</param>
        private static void ProcessTypifiedProperty(PropertyInfo propertyInfo, Type classType, TypifiedPropertyAttribute typifiedAttribute)
        {
            Type propertyType = propertyInfo.PropertyType;
            JsonPropertyAttribute jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            
            if (!TypifiedDefiningData.TryGetValue(classType, out List<TypifiedPropertyData> propertyData))
            {
                propertyData = new List<TypifiedPropertyData>();
                TypifiedDefiningData[classType] = propertyData;
            }

            TypifiedPropertyData data = new TypifiedPropertyData(propertyType, propertyInfo.Name, jsonProperty.PropertyName);
            propertyData.Add(data);
        }

        /// <summary>
        /// Iterates through <see cref="AbstractDefiningData"/> and establishes the mapping between abstract classes and their <see cref="TypifyingPropertyAttribute"/> value.
        /// This method is crucial for enabling polymorphic deserialization where abstract types of secondary inheritance (abstract class assigns value to the base abstract property) need to be resolved
        /// to their concrete implementations.
        /// </summary>
        private static void PostProcessAbstractClasses()
        {
            foreach (var data in AbstractDefiningData)
            {
                if(!BaseToPropertyData.TryGetValue(data.abstractType, out TypifyingPropertyData propertyData))
                {
                    continue;
                }
                
                Type implementingClass = propertyData.ValuesData.Values.FirstOrDefault(type => !type.IsAbstract);
                PropertyInfo propertyInfo = implementingClass?.GetProperty(data.propertyData.PropertyName);

                if (propertyInfo is null)
                {
                    continue;
                }
                
                //create an instance of non-abstract class and get the value of its property marked with TypifyingPropertyAttribute.
                object classObject = Activator.CreateInstance(implementingClass, true);
                object value = propertyInfo.GetValue(classObject);
                //add to corresponding TypifyingPropertyData.
                data.propertyData.ValuesData[value] = data.abstractType;
            }
        }

        /// <summary>
        /// Finalizes the processing of <see cref="TypifiedPropertyAttribute"/> by incorporating them into the <see cref="BaseToPropertyData"/>.
        /// This method finds corresponding <see cref="TypifyingPropertyData"/> for all <see cref="TypifiedPropertyAttribute"/>s and adds them in. 
        /// </summary>
        private static void PostProcessTypifiedProperties()
        {
            foreach (var data in TypifiedDefiningData)
            {
                if (!BaseToPropertyData.TryGetValue(data.Key, out TypifyingPropertyData propertyData))
                {
                    continue;
                }

                propertyData.TypifiedProperties.AddRange(data.Value);
            }
        }
    }
}