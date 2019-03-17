﻿/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Service;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.DataProvider.WebAPI;
using MDD4All.SpecIF.ServiceDataProvider;
using System;
using System.Collections.Generic;

namespace MDD4All.SpecIF.DataProvider.Integration
{
	public class SpecIfIntegrationMetadataReader : AbstractSpecIfMetadataReader
	{

		private SpecIfServiceDataProvider _descriptionProvider;

		public SpecIfIntegrationMetadataReader(SpecIfServiceDataProvider descriptionProvider)
		{
			_descriptionProvider = descriptionProvider;
			InitializeReaders();
		}

		private void InitializeReaders()
		{
			List<SpecIfServiceDescription> serviceDescriptions = _descriptionProvider.GetAvailableServices();

			foreach(SpecIfServiceDescription serviceDescription in serviceDescriptions)
			{
				if(serviceDescription.MetadataRead == true)
				{
					SpecIfWebApiMetadataReader metadataReader = new SpecIfWebApiMetadataReader(serviceDescription.ServiceAddress + ":" + serviceDescription.ServicePort);

					metadataReader.DataSourceDescription = serviceDescription;

					_metadataReaders.Add(serviceDescription.ID, metadataReader);
				}
			}
		}

		private Dictionary<string, ISpecIfMetadataReader> _metadataReaders = new Dictionary<string, ISpecIfMetadataReader>();

		public void AddMetadataReader(ISpecIfMetadataReader metadataReader, string id)
		{
			if(!_metadataReaders.ContainsKey(id))
			{
				_metadataReaders.Add(id, metadataReader);
			}
		}

		public override List<DataType> GetAllDataTypes()
		{
			List<DataType> result = new List<DataType>();

			foreach(KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				List<DataType> part = reader.Value.GetAllDataTypes();

				result.AddRange(part);
			}

			return result;
		}

		public override List<HierarchyClass> GetAllHierarchyClasses()
		{
			List<HierarchyClass> result = new List<HierarchyClass>();

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				List<HierarchyClass> part = reader.Value.GetAllHierarchyClasses();

				result.AddRange(part);
			}

			return result;
		}

		public override List<PropertyClass> GetAllPropertyClasses()
		{
			List<PropertyClass> result = new List<PropertyClass>();

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				List<PropertyClass> part = reader.Value.GetAllPropertyClasses();

				result.AddRange(part);
			}

			return result;
		}

		public override List<ResourceClass> GetAllResourceClasses()
		{
			List<ResourceClass> result = new List<ResourceClass>();

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				List<ResourceClass> part = reader.Value.GetAllResourceClasses();

				result.AddRange(part);
			}

			return result;
		}

		public override DataType GetDataTypeById(string id)
		{
			DataType result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				DataType dataType = reader.Value.GetDataTypeById(id);

				if(dataType != null)
				{
					result = dataType;
					break;
				}

			}

			return result;
		}

		

		public override HierarchyClass GetHierarchyClassByKey(Key key)
		{
			HierarchyClass result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				HierarchyClass hierarchyClass = reader.Value.GetHierarchyClassByKey(key);

				if (hierarchyClass != null)
				{
					result = hierarchyClass;
					break;
				}

			}

			return result;
		}

		public override ResourceClass GetResourceClassByKey(Key key)
		{
			ResourceClass result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				ResourceClass resourceClass = reader.Value.GetResourceClassByKey(key);

				if (resourceClass != null)
				{
					result = resourceClass;
					break;
				}

			}

			return result;
		}

		public override StatementClass GetStatementClassByKey(Key key)
		{
			StatementClass result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				StatementClass statementClass = reader.Value.GetStatementClassByKey(key);

				if (statementClass != null)
				{
					result = statementClass;
					break;
				}

			}

			return result;
		}

		public override PropertyClass GetPropertyClassByKey(Key key)
		{
			PropertyClass result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> reader in _metadataReaders)
			{
				PropertyClass propertyClass = reader.Value.GetPropertyClassByKey(key);

				if (propertyClass != null)
				{
					result = propertyClass;
					break;
				}

			}

			return result;
		}

		public override int GetLatestPropertyClassRevision(string propertyClassID)
		{
			int result = 1;

			ISpecIfMetadataReader provider = FindDataProviderForPropertyClass(propertyClassID);

			if (provider != null)
			{
				result = provider.GetLatestPropertyClassRevision(propertyClassID);
			}

			return result;
		}

		public override int GetLatestResourceClassRevision(string resourceClassID)
		{
			int result = 1;

			ISpecIfMetadataReader provider = FindDataProviderForResourceClass(resourceClassID);

			if (provider != null)
			{
				result = provider.GetLatestResourceClassRevision(resourceClassID);
			}

			return result;
		}

		public override int GetLatestStatementClassRevision(string statementClassID)
		{
			int result = 1;

			ISpecIfMetadataReader provider = FindDataProviderForStatementClass(statementClassID);

			if (provider != null)
			{
				result = provider.GetLatestStatementClassRevision(statementClassID);
			}

			return result;
		}

		public override int GetLatestHierarchyClassRevision(string hierarchyClassID)
		{
			int result = 1;

			ISpecIfMetadataReader provider = FindDataProviderForHierarchyClass(hierarchyClassID);

			if (provider != null)
			{
				result = provider.GetLatestHierarchyClassRevision(hierarchyClassID);
			}

			return result;
		}

		private ISpecIfMetadataReader FindDataProviderForStatementClass(string id)
		{
			ISpecIfMetadataReader result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> provider in _metadataReaders)
			{
				if (provider.Value.GetStatementClassByKey(new Key() { ID = id, Revision = 0 }) != null)
				{
					result = provider.Value;
					break;
				}
			}

			return result;
		}

		private ISpecIfMetadataReader FindDataProviderForPropertyClass(string id)
		{
			ISpecIfMetadataReader result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> provider in _metadataReaders)
			{
				if (provider.Value.GetPropertyClassByKey(new Key() { ID = id, Revision = 0 }) != null)
				{
					result = provider.Value;
					break;
				}
			}

			return result;
		}

		private ISpecIfMetadataReader FindDataProviderForResourceClass(string id)
		{
			ISpecIfMetadataReader result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> provider in _metadataReaders)
			{
				if (provider.Value.GetResourceClassByKey(new Key() { ID = id, Revision = 0 }) != null)
				{
					result = provider.Value;
					break;
				}
			}

			return result;
		}

		private ISpecIfMetadataReader FindDataProviderForHierarchyClass(string id)
		{
			ISpecIfMetadataReader result = null;

			foreach (KeyValuePair<string, ISpecIfMetadataReader> provider in _metadataReaders)
			{
				if (provider.Value.GetHierarchyClassByKey(new Key() { ID = id, Revision = 0 }) != null)
				{
					result = provider.Value;
					break;
				}
			}

			return result;
		}
	}
}
