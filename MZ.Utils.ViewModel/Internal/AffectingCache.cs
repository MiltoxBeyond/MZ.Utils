using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MZ.Utils.ViewModel.Internal
{
    /// <summary>
    /// Internal caching class to help with relationship management between properties of classes to minimize reflection repetition.
    /// </summary>
    internal class AffectingCache : Dictionary<string, AffectingCache.AffectingLookup>
    {
        /// <summary>
        /// Internal Caching class to hold properties and their affected properties.
        /// </summary>
        internal class AffectingLookup : Dictionary<string, List<string>>
        {

        }

        //Add types
        public AffectingLookup Add(Type type)
        {
            var key = type.FullName;
            if (ContainsKey(key))
                throw new ArgumentException("Unable to add duplicate key to type cache.");
            var result = GetAffected(type);
            Add(key, result);
            return result;
        }


        /// <summary>
        /// Get Affected Properties and combine with reverse lookup of affected by properties to simplify interactions.
        /// </summary>
        /// <param name="type">Type to generate the relationship map</param>
        /// <returns>Relationship AffectingLookup for mapped interactions between properties</returns>
        internal static AffectingLookup GetAffected(Type type)
        {
            AffectingLookup lookup = new AffectingLookup();
            var properties = type.GetProperties().Where(p => p.CanWrite || p.CanRead);
            var affects = properties.Where(p => p.GetCustomAttribute<AffectsAttribute>() != null);
            var affectedByList = properties.Where(p => p.GetCustomAttribute<AffectedByAttribute>() != null);

            foreach (var affect in affects)
            {
                var affectList = affect.GetCustomAttribute<AffectsAttribute>().Affects.ToList();
                lookup.Add(affect.Name, affectList);
            }

            foreach (var affectedProperty in affectedByList)
            {
                var affectingPropertyList = affectedProperty.GetCustomAttribute<AffectedByAttribute>()?.AffectedBy;

                foreach (var affectingProperty in affectingPropertyList)
                {
                    if (!lookup.ContainsKey(affectingProperty))
                    {
                        lookup[affectingProperty] = new List<string>();
                    }
                    lookup[affectingProperty].Add(affectedProperty.Name);
                }
            }

            return lookup;
        }

        /// <summary>
        /// Lookup Shortcut
        /// </summary>
        /// <param name="type">The type to lookup otherwise build cache from</param>
        /// <returns>The Affecting Lookup List</returns>
        public AffectingLookup this[Type type] => ContainsKey(type.FullName) ? this[type.FullName] : Add(type);
    }
}
