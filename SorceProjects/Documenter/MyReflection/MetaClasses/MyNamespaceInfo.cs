/*
 * Copyright 2007 - 2009 Marek Stуj
 * 
 * This file is part of ImmDoc .NET.
 *
 * ImmDoc .NET is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * ImmDoc .NET is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with ImmDoc .NET; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using BestCS.DocNet.Documenters;
using Mono.Cecil;

namespace BestCS.DocNet.MyReflection.MetaClasses
{
  class MyNamespaceInfo : MetaClass, ISummarisableMember
  {
    public const string GLOBAL_NAMESPACE_NAME = "[GLOBAL]";

    private string assemblyName;
    private Dictionary<NamespaceMembersGroups, Dictionary<string, MetaClass>> membersGroups;
    private Dictionary<NamespaceMembersGroups, Dictionary<string, string>> genericNamesMappings;

    #region Constructor(s)

    public MyNamespaceInfo(string name, string assemblyName)
    {
      this.name = name;
      this.assemblyName = assemblyName;
      this.membersGroups = new Dictionary<NamespaceMembersGroups, Dictionary<string, MetaClass>>();
    }

    #endregion

    #region Public methods

    public void AddType(TypeDefinition typeDefinition)
    {
      if (typeDefinition.IsValueType && !typeDefinition.IsEnum)
      {
        MyStructureInfo myStructureInfo = new MyStructureInfo(typeDefinition, assemblyName);
        Dictionary<string, MetaClass> membersGroup;
        NamespaceMembersGroups namespaceMembersGroupType;

        if (myStructureInfo.IsPublic)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PublicStructures;
        }
        else if (myStructureInfo.IsProtectedInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedInternalStructures;
        }
        else if (myStructureInfo.IsProtected)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedStructures;
        }
        else if (myStructureInfo.IsInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.InternalStructures;
        }
        else if (myStructureInfo.IsPrivate)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PrivateStructures;
        }
        else
        {
          Debug.Assert(false, "Impossible! Visibility of a type is not supported.");
          return;
        }

        membersGroup = GetMembersGroup(namespaceMembersGroupType);

        if (!membersGroup.ContainsKey(myStructureInfo.Name))
        {
          membersGroup.Add(myStructureInfo.Name, myStructureInfo);

          AddGenericNameMappingIfNeeded(myStructureInfo, namespaceMembersGroupType);
        }
        else
        {
          Logger.Warning("Structure named '{0}' has already been added to namespace {1}.", myStructureInfo.Name, name);
        }
      }
      else if (typeDefinition.IsInterface)
      {
        MyInterfaceInfo myInterfaceInfo = new MyInterfaceInfo(typeDefinition, assemblyName);
        Dictionary<string, MetaClass> membersGroup;
        NamespaceMembersGroups namespaceMembersGroupType;

        if (myInterfaceInfo.IsPublic)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PublicInterfaces;
        }
        else if (myInterfaceInfo.IsProtectedInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedInternalInterfaces;
        }
        else if (myInterfaceInfo.IsProtected)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedInterfaces;
        }
        else if (myInterfaceInfo.IsInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.InternalInterfaces;
        }
        else if (myInterfaceInfo.IsPrivate)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PrivateInterfaces;
        }
        else
        {
          Debug.Assert(false, "Impossible! Visibility of a type is not supported.");
          return;
        }

        membersGroup = GetMembersGroup(namespaceMembersGroupType);

        if (!membersGroup.ContainsKey(myInterfaceInfo.Name))
        {
          membersGroup.Add(myInterfaceInfo.Name, myInterfaceInfo);

          AddGenericNameMappingIfNeeded(myInterfaceInfo, namespaceMembersGroupType);
        }
        else
        {
          Logger.Warning("Interface named '{0}' has already been added to namespace {1}.", myInterfaceInfo.Name, name);
        }
      }
      else if (typeDefinition.IsEnum)
      {
        MyEnumerationInfo myEnumerationInfo = new MyEnumerationInfo(typeDefinition, assemblyName);
        Dictionary<string, MetaClass> membersGroup;
        NamespaceMembersGroups namespaceMembersGroupType;

        if (myEnumerationInfo.IsPublic)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PublicEnumerations;
        }
        else if (myEnumerationInfo.IsProtectedInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedInternalEnumerations;
        }
        else if (myEnumerationInfo.IsProtected)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedEnumerations;
        }
        else if (myEnumerationInfo.IsInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.InternalEnumerations;
        }
        else if (myEnumerationInfo.IsPrivate)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PrivateEnumerations;
        }
        else
        {
          Debug.Assert(false, "Impossible! Visibility of a type is not supported.");
          return;
        }

        membersGroup = GetMembersGroup(namespaceMembersGroupType);

        if (!membersGroup.ContainsKey(myEnumerationInfo.Name))
        {
          membersGroup.Add(myEnumerationInfo.Name, myEnumerationInfo);

          AddGenericNameMappingIfNeeded(myEnumerationInfo, namespaceMembersGroupType);
        }
        else
        {
          Logger.Warning("Enumeration named '{0}' has already been added to namespace {1}.", myEnumerationInfo.Name, name);
        }
      }
      else if (typeDefinition.IsClass && !Utils.IsDelegate(typeDefinition))
      {
        MyClassInfo myClassInfo = new MyClassInfo(typeDefinition, assemblyName);
        Dictionary<string, MetaClass> membersGroup;
        NamespaceMembersGroups namespaceMembersGroupType;

        if (myClassInfo.IsPublic)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PublicClasses;
        }
        else if (myClassInfo.IsProtectedInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedInternalClasses;
        }
        else if (myClassInfo.IsProtected)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedClasses;
        }
        else if (myClassInfo.IsInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.InternalClasses;
        }
        else if (myClassInfo.IsPrivate)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PrivateClasses;
        }
        else
        {
          Debug.Assert(false, "Impossible! Visibility of a type is not supported.");
          return;
        }

        membersGroup = GetMembersGroup(namespaceMembersGroupType);

        if (!membersGroup.ContainsKey(myClassInfo.Name))
        {
          membersGroup.Add(myClassInfo.Name, myClassInfo);

          AddGenericNameMappingIfNeeded(myClassInfo, namespaceMembersGroupType);
        }
        else
        {
          Logger.Warning("Class named '{0}' has already been added to namespace {1}.", myClassInfo.Name, name);
        }
      }
      else if (Utils.IsDelegate(typeDefinition))
      {
        MyDelegateInfo myDelegateInfo = new MyDelegateInfo(typeDefinition, assemblyName);
        Dictionary<string, MetaClass> membersGroup;
        NamespaceMembersGroups namespaceMembersGroupType;

        if (myDelegateInfo.IsPublic)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PublicDelegates;
        }
        else if (myDelegateInfo.IsProtectedInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedInternalDelegates;
        }
        else if (myDelegateInfo.IsProtected)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.ProtectedDelegates;
        }
        else if (myDelegateInfo.IsInternal)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.InternalDelegates;
        }
        else if (myDelegateInfo.IsPrivate)
        {
          namespaceMembersGroupType = NamespaceMembersGroups.PrivateDelegates;
        }
        else
        {
          Debug.Assert(false, "Impossible! Visibility of a type is not supported.");
          return;
        }

        membersGroup = GetMembersGroup(namespaceMembersGroupType);

        if (!membersGroup.ContainsKey(myDelegateInfo.Name))
        {
          membersGroup.Add(myDelegateInfo.Name, myDelegateInfo);

          AddGenericNameMappingIfNeeded(myDelegateInfo, namespaceMembersGroupType);
        }
        else
        {
          Logger.Warning("Delegate named '{0}' has already been added to namespace {1}.", myDelegateInfo.Name, name);
        }
      }
      else
      {
        Logger.Warning("Unrecognized type: {0}.", typeDefinition.FullName);
      }
    }

    public MyClassInfo FindMember(NamespaceMembersGroups namespaceMembersGroupType, string memberName)
    {
      if (!membersGroups.ContainsKey(namespaceMembersGroupType))
      {
        return null;
      }

      Dictionary<string, MetaClass> membersGroup = membersGroups[namespaceMembersGroupType];
      MetaClass member = null;

      if (!membersGroup.ContainsKey(memberName))
      {
        // try indirect search (for generic types)
        if (genericNamesMappings == null || !genericNamesMappings.ContainsKey(namespaceMembersGroupType))
        {
          return null;
        }

        Dictionary<string, string> genericNamesMappingsForGroup = genericNamesMappings[namespaceMembersGroupType];
        if (!genericNamesMappingsForGroup.ContainsKey(memberName))
        {
          return null;
        }

        memberName = genericNamesMappingsForGroup[memberName];

        if (!membersGroup.ContainsKey(memberName))
        {
          return null;
        }
      }

      member = membersGroup[memberName];
      Debug.Assert(member is MyClassInfo, "Impossible! All namespace member inherit from MyClassInfo.");

      return (MyClassInfo)member;
    }

    public int GetMembersCount(NamespaceMembersGroups namespaceMembersGroupType)
    {
      if (!membersGroups.ContainsKey(namespaceMembersGroupType))
      {
        return 0;
      }

      return membersGroups[namespaceMembersGroupType].Count;
    }

    public Dictionary<string, MetaClass> GetMembers(NamespaceMembersGroups namespaceMembersGroupType)
    {
      if (!membersGroups.ContainsKey(namespaceMembersGroupType))
      {
        return null;
      }

      return membersGroups[namespaceMembersGroupType];
    }

    public static string NamespaceMembersGroupToString(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return "Публичные Классы"; }
        case NamespaceMembersGroups.PublicStructures: { return "Публичные Структуры"; }
        case NamespaceMembersGroups.PublicInterfaces: { return "Публичные Интерфейсы"; }
        case NamespaceMembersGroups.PublicDelegates: { return "Публичные Делегаты"; }
        case NamespaceMembersGroups.PublicEnumerations: { return "Публичные Перечни"; }
        case NamespaceMembersGroups.ProtectedInternalClasses: { return "Защищённые Внутренние Классы"; }
        case NamespaceMembersGroups.ProtectedInternalStructures: { return "Защищённые Внутренние Структуры"; }
        case NamespaceMembersGroups.ProtectedInternalInterfaces: { return "Защищённые Внутренние Интерфейсы"; }
        case NamespaceMembersGroups.ProtectedInternalDelegates: { return "Защищённые Внутренние Делегаты"; }
        case NamespaceMembersGroups.ProtectedInternalEnumerations: { return "Защищённые Внутренние Перечни"; }
        case NamespaceMembersGroups.ProtectedClasses: { return "Защищённые Классы"; }
        case NamespaceMembersGroups.ProtectedStructures: { return "Защищённые Структуры"; }
        case NamespaceMembersGroups.ProtectedInterfaces: { return "Защищённые Интерфейсы"; }
        case NamespaceMembersGroups.ProtectedDelegates: { return "Защищённые Делегаты"; }
        case NamespaceMembersGroups.ProtectedEnumerations: { return "Защищённые Перечни"; }
        case NamespaceMembersGroups.InternalClasses: { return "Внутренние Классы"; }
        case NamespaceMembersGroups.InternalStructures: { return "Внутренние Структуры"; }
        case NamespaceMembersGroups.InternalInterfaces: { return "Внутренние Интерфейсы"; }
        case NamespaceMembersGroups.InternalDelegates: { return "Внутренние Делегаты"; }
        case NamespaceMembersGroups.InternalEnumerations: { return "Внутренние Перечни"; }
        case NamespaceMembersGroups.PrivateClasses: { return "Приватные Классы"; }
        case NamespaceMembersGroups.PrivateStructures: { return "Приватные Структуры"; }
        case NamespaceMembersGroups.PrivateInterfaces: { return "Приватные Интерфейсы"; }
        case NamespaceMembersGroups.PrivateDelegates: { return "Приватные Делегаты"; }
        case NamespaceMembersGroups.PrivateEnumerations: { return "Приватные Перечни"; }

        default:
          {
            Debug.Assert(false, "Impossible! Couldn't recognize type of a namespace member.");

            return null;
          }
      }
    }

    public static string GetBaseGroupName(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return "Классы"; }
        case NamespaceMembersGroups.PublicStructures: { return "Структуры"; }
        case NamespaceMembersGroups.PublicInterfaces: { return "Интерфейсы"; }
        case NamespaceMembersGroups.PublicDelegates: { return "Делегаты"; }
        case NamespaceMembersGroups.PublicEnumerations: { return "Перечни"; }
        case NamespaceMembersGroups.ProtectedInternalClasses: { return "Классы"; }
        case NamespaceMembersGroups.ProtectedInternalStructures: { return "Структуры"; }
        case NamespaceMembersGroups.ProtectedInternalInterfaces: { return "Интерфейсы"; }
        case NamespaceMembersGroups.ProtectedInternalDelegates: { return "Делегаты"; }
        case NamespaceMembersGroups.ProtectedInternalEnumerations: { return "Перечни"; }
        case NamespaceMembersGroups.ProtectedClasses: { return "Классы"; }
        case NamespaceMembersGroups.ProtectedStructures: { return "Структуры"; }
        case NamespaceMembersGroups.ProtectedInterfaces: { return "Интерфейсы"; }
        case NamespaceMembersGroups.ProtectedDelegates: { return "Делегаты"; }
        case NamespaceMembersGroups.ProtectedEnumerations: { return "Перечни"; }
        case NamespaceMembersGroups.InternalClasses: { return "Классы"; }
        case NamespaceMembersGroups.InternalStructures: { return "Структуры"; }
        case NamespaceMembersGroups.InternalInterfaces: { return "Интерфейсы"; }
        case NamespaceMembersGroups.InternalDelegates: { return "Делегаты"; }
        case NamespaceMembersGroups.InternalEnumerations: { return "Перечни"; }
        case NamespaceMembersGroups.PrivateClasses: { return "Классы"; }
        case NamespaceMembersGroups.PrivateStructures: { return "Структуры"; }
        case NamespaceMembersGroups.PrivateInterfaces: { return "Интерфейсы"; }
        case NamespaceMembersGroups.PrivateDelegates: { return "Делегаты"; }
        case NamespaceMembersGroups.PrivateEnumerations: { return "Перечни"; }

        default:
          {
            Debug.Assert(false, "Impossible! Couldn't recognize type of a namespace member.");

            return null;
          }
      }
    }

    public static bool IsMembersGroupTypePublic(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return true; }
        case NamespaceMembersGroups.PublicStructures: { return true; }
        case NamespaceMembersGroups.PublicInterfaces: { return true; }
        case NamespaceMembersGroups.PublicDelegates: { return true; }
        case NamespaceMembersGroups.PublicEnumerations: { return true; }

        default: { return false; }
      }
    }

    public bool HasProtectedGroupOfTheSameType(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return GetMembersCount(NamespaceMembersGroups.ProtectedClasses) > 0; }
        case NamespaceMembersGroups.PublicStructures: { return GetMembersCount(NamespaceMembersGroups.ProtectedStructures) > 0; }
        case NamespaceMembersGroups.PublicInterfaces: { return GetMembersCount(NamespaceMembersGroups.ProtectedInterfaces) > 0; }
        case NamespaceMembersGroups.PublicDelegates: { return GetMembersCount(NamespaceMembersGroups.ProtectedDelegates) > 0; }
        case NamespaceMembersGroups.PublicEnumerations: { return GetMembersCount(NamespaceMembersGroups.ProtectedEnumerations) > 0; }

        default: return false;
      }
    }

    public bool HasProtectedInternalGroupOfTheSameType(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return GetMembersCount(NamespaceMembersGroups.ProtectedInternalClasses) > 0; }
        case NamespaceMembersGroups.PublicStructures: { return GetMembersCount(NamespaceMembersGroups.ProtectedInternalStructures) > 0; }
        case NamespaceMembersGroups.PublicInterfaces: { return GetMembersCount(NamespaceMembersGroups.ProtectedInternalInterfaces) > 0; }
        case NamespaceMembersGroups.PublicDelegates: { return GetMembersCount(NamespaceMembersGroups.ProtectedInternalDelegates) > 0; }
        case NamespaceMembersGroups.PublicEnumerations: { return GetMembersCount(NamespaceMembersGroups.ProtectedInternalEnumerations) > 0; }

        default: return false;
      }
    }

    public bool HasInternalGroupOfTheSameType(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return GetMembersCount(NamespaceMembersGroups.InternalClasses) > 0; }
        case NamespaceMembersGroups.PublicStructures: { return GetMembersCount(NamespaceMembersGroups.InternalStructures) > 0; }
        case NamespaceMembersGroups.PublicInterfaces: { return GetMembersCount(NamespaceMembersGroups.InternalInterfaces) > 0; }
        case NamespaceMembersGroups.PublicDelegates: { return GetMembersCount(NamespaceMembersGroups.InternalDelegates) > 0; }
        case NamespaceMembersGroups.PublicEnumerations: { return GetMembersCount(NamespaceMembersGroups.InternalEnumerations) > 0; }

        default: return false;
      }
    }

    public bool HasPrivateGroupOfTheSameType(NamespaceMembersGroups membersGroupType)
    {
      switch (membersGroupType)
      {
        case NamespaceMembersGroups.PublicClasses: { return GetMembersCount(NamespaceMembersGroups.PrivateClasses) > 0; }
        case NamespaceMembersGroups.PublicStructures: { return GetMembersCount(NamespaceMembersGroups.PrivateStructures) > 0; }
        case NamespaceMembersGroups.PublicInterfaces: { return GetMembersCount(NamespaceMembersGroups.PrivateInterfaces) > 0; }
        case NamespaceMembersGroups.PublicDelegates: { return GetMembersCount(NamespaceMembersGroups.PrivateDelegates) > 0; }
        case NamespaceMembersGroups.PublicEnumerations: { return GetMembersCount(NamespaceMembersGroups.PrivateEnumerations) > 0; }

        default: return false;
      }
    }

    #endregion

    #region Public properties

    public string AssemblyName
    {
      get { return assemblyName; }
    }

    public bool HasMembers
    {
      get
      {
        return membersGroups.Keys.Count > 0;
      }
    }

    #endregion

    #region Private helper methods

    private Dictionary<string, MetaClass> GetMembersGroup(NamespaceMembersGroups namespaceMembersGroupType)
    {
      Dictionary<string, MetaClass> membersGroup;

      if (!membersGroups.ContainsKey(namespaceMembersGroupType))
      {
        membersGroup = new Dictionary<string, MetaClass>();
        membersGroups[namespaceMembersGroupType] = membersGroup;
      }
      else
      {
        membersGroup = membersGroups[namespaceMembersGroupType];
      }

      return membersGroup;
    }

    private Dictionary<string, string> GetGenericNamesMappingsForGroup(NamespaceMembersGroups namespaceMembersGroupType)
    {
      if (genericNamesMappings == null)
      {
        genericNamesMappings = new Dictionary<NamespaceMembersGroups, Dictionary<string, string>>();
      }

      Dictionary<string, string> genericNamesMappingsForGroup;

      if (!genericNamesMappings.ContainsKey(namespaceMembersGroupType))
      {
        genericNamesMappingsForGroup = new Dictionary<string, string>();
        genericNamesMappings[namespaceMembersGroupType] = genericNamesMappingsForGroup;
      }
      else
      {
        genericNamesMappingsForGroup = genericNamesMappings[namespaceMembersGroupType];
      }

      return genericNamesMappingsForGroup;
    }

    private void AddGenericNameMappingIfNeeded(MyClassInfo myClassInfo, NamespaceMembersGroups namespaceMembersGroupType)
    {
      if (!myClassInfo.Name.Contains("<"))
      {
        return;
      }

      Dictionary<string, string> genericNamesMappingsForGroup = GetGenericNamesMappingsForGroup(namespaceMembersGroupType);
      string xmlName = Utils.ConvertNameToXmlDocForm(myClassInfo.Name);

      genericNamesMappingsForGroup[xmlName] = myClassInfo.Name;
    }

    #endregion

    #region ISummarisableMember Members

    public string DisplayableName
    {
      get { return Name; }
    }

    #endregion

    #region MetaClass overrides

    public override string GetMetaName()
    {
      return "Пространство Имён";
    }

    #endregion

    #region Enumeration

    internal IEnumerator<ISummarisableMember> GetEnumerator(NamespaceMembersGroups namespaceMembersGroupType)
    {
      if (!membersGroups.ContainsKey(namespaceMembersGroupType))
      {
        Debug.Assert(false, "Impossible! Couldn't recognize members group ('" + namespaceMembersGroupType + "').");
        yield break;
      }

      Dictionary<string, MetaClass> membersGroup = membersGroups[namespaceMembersGroupType];
      List<string> sortedKeys = new List<string>();

      sortedKeys.AddRange(membersGroup.Keys);

      sortedKeys.Sort();

      foreach (string key in sortedKeys)
      {
        yield return (ISummarisableMember)membersGroup[key];
      }
    }

    public IEnumerable<MetaClass> GetEnumerator()
    {
      List<MetaClass> sortedMembers = new List<MetaClass>();
      int namespaceMembersGroupIndex = 0;

      while (Enum.IsDefined(typeof(NamespaceMembersGroups), namespaceMembersGroupIndex))
      {
        NamespaceMembersGroups namespaceMembersGroup = (NamespaceMembersGroups)namespaceMembersGroupIndex;

        if (GetMembersCount(namespaceMembersGroup) > 0)
        {
          Dictionary<string, MetaClass> membersGroup = GetMembers(namespaceMembersGroup);

          foreach (MetaClass member in membersGroup.Values)
          {
            sortedMembers.Add(member);
          }
        }

        namespaceMembersGroupIndex++;
      }

      sortedMembers.Sort(new Comparison<MetaClass>(MembersComparison));

      return sortedMembers;
    }

    private int MembersComparison(MetaClass m1, MetaClass m2)
    {
      return m1.Name.CompareTo(m2.Name);
    }

    #endregion
  }
}
