using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DysonCore.DynamicJson.PolymorphicParser;
using Newtonsoft.Json;

namespace DysonCore.DynamicJson.Editor.PolymorphicParser
{
    /// <summary>
    /// Provides a centralized data provider for managing <see cref="PropertyData"/> mappings used by polymorphic converter.
    /// It handles mappings of abstract types and their concrete implementations to facilitate correct type resolution during deserialization.
    /// </summary>
    internal static class PolymorphicCacheBuilder
    {
        
        /// <summary>
        /// Initializes the <see cref="PolymorphicCacheProvider"/> with data from the specified assemblies.
        /// Scans the provided assemblies to build the data mappings required for polymorphic deserialization.
        /// </summary>
        internal static Dictionary<TypeLazyReference, TypifyingPropertyData> GetData()
        {
            Dictionary<TypeLazyReference, TypifyingPropertyData> baseToPropertyData = new (); // Stores mappings from base types to their corresponding TypifyingPropertyData
            List<(TypeLazyReference abstractType, TypifyingPropertyData propertyData)> abstractDefiningData = new (); // Temporal list of tuples containing abstract types of secondary inheritance (when abstract class assigns value to the abstract property of base class) and associated TypifyingPropertyData.
            Dictionary<TypeLazyReference, List<TypifiedPropertyData>> typifiedDefiningData = new (); // Temporal dictionary of types to a list of TypifiedPropertyData, storing information for types that have been marked with TypifiedPropertyAttribute.
            
            Assembly[] assemblies = AssemblyUtils.GetPackageRuntimeAssembly().GetReferencingAssemblies();
            
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type classType in assembly.GetTypes())
                {
                    foreach (PropertyInfo propertyInfo in classType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        ProcessProperty(baseToPropertyData, abstractDefiningData, typifiedDefiningData, propertyInfo, (TypeLazyReference)classType);
                    }
                }
            }

            PostProcessAbstractClasses(baseToPropertyData, abstractDefiningData);
            PostProcessTypifiedProperties(baseToPropertyData, typifiedDefiningData);
            
            abstractDefiningData.Clear();
            typifiedDefiningData.Clear();
            return baseToPropertyData;
        }

        /// <summary>
        /// Processes a property to determine whether it has <see cref="TypifyingPropertyAttribute"/> or <see cref="TypifiedPropertyAttribute"/> and ensures that it is not marked as both.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object representing the property to process.</param>
        /// <param name="classType">The Type of the class that the property belongs to.</param>
        /// <exception cref="Exception">Throws <see cref="Exception"/> if single property marked with both <see cref="TypifyingPropertyAttribute"/> and <see cref="TypifiedPropertyAttribute"/></exception>
        private static void ProcessProperty(Dictionary<TypeLazyReference, TypifyingPropertyData> baseToPropertyData, List<(TypeLazyReference abstractType, TypifyingPropertyData propertyData)> abstractDefiningData, Dictionary<TypeLazyReference, List<TypifiedPropertyData>> typifiedDefiningData, PropertyInfo propertyInfo, TypeLazyReference classType)
        {
            TypifyingPropertyAttribute typifyingAttribute = propertyInfo.GetCustomAttribute<TypifyingPropertyAttribute>();
            TypifiedPropertyAttribute typifiedAttribute = propertyInfo.GetCustomAttribute<TypifiedPropertyAttribute>();

            bool isTypifying = typifyingAttribute != null;
            bool isTypified = typifiedAttribute != null;

            if (isTypifying && isTypified)
            {
                throw new Exception($"[{nameof(PolymorphicCacheBuilder)}.{nameof(ProcessProperty)}] {classType.TypeName}.{propertyInfo.Name} property has {nameof(TypifyingPropertyAttribute)} and {nameof(TypifiedPropertyAttribute)} at the same time!");
            }

            if (isTypifying)
            {
                ProcessTypifyingProperty(baseToPropertyData, abstractDefiningData, propertyInfo, classType, typifyingAttribute);
            }

            if (isTypified)
            {
                ProcessTypifiedProperty(typifiedDefiningData, propertyInfo, classType);
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
        private static void ProcessTypifyingProperty(Dictionary<TypeLazyReference, TypifyingPropertyData> baseToPropertyData, List<(TypeLazyReference abstractType, TypifyingPropertyData propertyData)> abstractDefiningData, PropertyInfo propertyInfo, TypeLazyReference classType, TypifyingPropertyAttribute typifyingAttribute)
        {
            TypeLazyReference propertyType = (TypeLazyReference)propertyInfo.PropertyType;
            JsonPropertyAttribute jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();

            TypeLazyReference baseType = (TypeLazyReference)(typifyingAttribute.InheritanceRoot ?? classType.Type.GetDeclaringClass(propertyInfo.Name)); //get base declaring class for the current property. Manual declaration of root class is prioritized.  
            TypifyingPropertyAttribute baseAttribute = baseType.Type.GetProperty(propertyInfo.Name)?.GetCustomAttribute<TypifyingPropertyAttribute>();

            if (baseAttribute == null)
            {
                throw new Exception($"[{nameof(PolymorphicCacheBuilder)}.{nameof(ProcessTypifyingProperty)}] {baseType.TypeName} has no property with {nameof(TypifyingPropertyAttribute)} and \"{propertyInfo.Name}\" {nameof(propertyInfo.Name)}.\nReferencing class - {classType.TypeName}.");
            }

            if (!baseToPropertyData.TryGetValue(baseType, out TypifyingPropertyData propertyData)) //get or create TypifyingPropertyData from / in BaseToPropertyData.
            {
                propertyData = new TypifyingPropertyData(propertyType, propertyInfo.Name, jsonProperty?.PropertyName, baseAttribute);
                baseToPropertyData[baseType] = propertyData;
            }

            if (classType.Type.IsAbstract) //if class is abstract - its impossible to create instance of it, so
            {
                if (classType.Equals(baseType)) //if current class is abstract class, and current class is not a class which defines current TypifyingProperty -> add this class to the list of abstract classes for post-processing. 
                {
                    abstractDefiningData.Add((classType, propertyData));
                }

                return;
            }

            //create an instance of non-abstract class and get the value of its property marked with TypifyingPropertyAttribute.
            object classInstance = Activator.CreateInstance(classType.Type, true);
            object propertyValue = propertyInfo.GetValue(classInstance);

            if (propertyValue == null)
            {
                return;
            }
            
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
        private static void ProcessTypifiedProperty(Dictionary<TypeLazyReference, List<TypifiedPropertyData>> typifiedDefiningData, PropertyInfo propertyInfo, TypeLazyReference classType)
        {
            TypeLazyReference propertyType = (TypeLazyReference)propertyInfo.PropertyType;
            JsonPropertyAttribute jsonProperty = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            
            if (!typifiedDefiningData.TryGetValue(classType, out List<TypifiedPropertyData> propertyData))
            {
                propertyData = new List<TypifiedPropertyData>();
                typifiedDefiningData[classType] = propertyData;
            }

            TypifiedPropertyData data = new TypifiedPropertyData(propertyType, propertyInfo.Name, jsonProperty.PropertyName);
            propertyData.Add(data);
        }

        /// <summary>
        /// Iterates through <see cref="abstractDefiningData"/> and establishes the mapping between abstract classes and their <see cref="TypifyingPropertyAttribute"/> value.
        /// This method is crucial for enabling polymorphic deserialization where abstract types of secondary inheritance (abstract class assigns value to the base abstract property) need to be resolved
        /// to their concrete implementations.
        /// </summary>
        private static void PostProcessAbstractClasses(Dictionary<TypeLazyReference, TypifyingPropertyData> baseToPropertyData, List<(TypeLazyReference abstractType, TypifyingPropertyData propertyData)> abstractDefiningData)
        {
            foreach ((TypeLazyReference abstractType, TypifyingPropertyData propertyData) data in abstractDefiningData)
            {
                if(!baseToPropertyData.TryGetValue(data.abstractType, out TypifyingPropertyData propertyData))
                {
                    continue;
                }
                
                TypeLazyReference implementingClass = propertyData.ValuesData.Values.FirstOrDefault(typeWrapper => !typeWrapper.Type.IsAbstract);
                PropertyInfo propertyInfo = implementingClass?.Type.GetProperty(data.propertyData.PropertyName);

                if (propertyInfo is null)
                {
                    continue;
                }
                
                //create an instance of non-abstract class and get the value of its property marked with TypifyingPropertyAttribute.
                object classObject = Activator.CreateInstance(implementingClass.Type, true);
                object value = propertyInfo.GetValue(classObject);
                //add to corresponding TypifyingPropertyData.
                data.propertyData.ValuesData[value] = data.abstractType;
            }
        }

        /// <summary>
        /// Finalizes the processing of <see cref="TypifiedPropertyAttribute"/> by incorporating them into the <see cref="baseToPropertyData"/>.
        /// This method finds corresponding <see cref="TypifyingPropertyData"/> for all <see cref="TypifiedPropertyAttribute"/>s and adds them in. 
        /// </summary>
        private static void PostProcessTypifiedProperties(Dictionary<TypeLazyReference, TypifyingPropertyData> baseToPropertyData, Dictionary<TypeLazyReference, List<TypifiedPropertyData>> typifiedDefiningData)
        {
            foreach (KeyValuePair<TypeLazyReference, List<TypifiedPropertyData>> data in typifiedDefiningData)
            {
                if (!baseToPropertyData.TryGetValue(data.Key, out TypifyingPropertyData propertyData))
                {
                    continue;
                }

                propertyData.TypifiedProperties.AddRange(data.Value);
            }
        }
    }
}