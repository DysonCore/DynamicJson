using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DysonCore.PolymorphicJson.Attributes;
using DysonCore.PolymorphicJson.Models;
using DysonCore.PolymorphicJson.Utils;
using Newtonsoft.Json;

namespace DysonCore.PolymorphicJson.Providers
{
    internal static class PropertyDataProvider
    {
        internal static Dictionary<Type, TypifyingPropertyData> BaseToPropertyData { get; } = new ();
        
        private static readonly List<(Type abstractType, TypifyingPropertyData propertyData)> AbstractDefiningData = new ();
        private static readonly Dictionary<Type, List<TypifiedPropertyData>> TypifiedDefiningData = new ();
        private static bool _initialized;
        
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
        /// and populates the BaseToPropertyData dictionary.
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
                ProcessTypifiedAttribute(propertyInfo, classType, typifiedAttribute);
            }
        }


        private static void ProcessTypifyingProperty(PropertyInfo propertyInfo, Type classType, TypifyingPropertyAttribute typifyingAttribute)
        {
            Type propertyType = propertyInfo.PropertyType;
            JsonPropertyAttribute jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();

            Type baseClass = typifyingAttribute.InheritanceRoot ?? classType.GetDeclaringClass(propertyInfo.Name);
            TypifyingPropertyAttribute baseAttribute = baseClass.GetProperty(propertyInfo.Name)?.GetCustomAttribute<TypifyingPropertyAttribute>();

            if (baseAttribute == null)
            {
                throw new Exception($"[{nameof(PropertyDataProvider)}.{nameof(InitializeCache)}] {baseClass.Name} has no property with {nameof(TypifyingPropertyAttribute)} and \"{propertyInfo.Name}\" {nameof(propertyInfo.Name)}.\nReferencing class - {classType.FullName}.");
            }

            if (!BaseToPropertyData.TryGetValue(baseClass, out TypifyingPropertyData propertyData))
            {
                propertyData = new TypifyingPropertyData(propertyType, propertyInfo.Name, jsonProperty?.PropertyName, baseAttribute);
                BaseToPropertyData[baseClass] = propertyData;
            }

            if (classType.IsAbstract)
            {
                if (classType != baseClass)
                {
                    AbstractDefiningData.Add((classType, propertyData));
                }

                return;
            }

            object classInstance = Activator.CreateInstance(classType, true);
            object propertyValue = propertyInfo.GetValue(classInstance);

            propertyData.ValuesData[propertyValue] = classType;
        }

        private static void ProcessTypifiedAttribute(PropertyInfo propertyInfo, Type classType, TypifiedPropertyAttribute typifiedAttribute)
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

                object classObject = Activator.CreateInstance(implementingClass, true);
                object value = propertyInfo.GetValue(classObject);
                
                data.propertyData.ValuesData[value] = data.abstractType;
            }
        }

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