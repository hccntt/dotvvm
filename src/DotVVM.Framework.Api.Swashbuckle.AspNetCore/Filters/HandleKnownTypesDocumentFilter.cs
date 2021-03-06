﻿using System;
using System.Linq;
using System.Reflection;
using DotVVM.Core.Common;
using DotVVM.Framework.ViewModel;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DotVVM.Framework.Api.Swashbuckle.AspNetCore.Filters
{
    public class HandleKnownTypesDocumentFilter : IDocumentFilter
    {
        private readonly IOptions<DotvvmApiOptions> apiOptions;
        private readonly DefaultPropertySerialization propertySerialization;

        public HandleKnownTypesDocumentFilter(IOptions<DotvvmApiOptions> apiOptions)
        {
            this.apiOptions = apiOptions;
            this.propertySerialization = new DefaultPropertySerialization();
        }

        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            var knownTypes = apiOptions.Value;
            foreach (var schema in swaggerDoc.Definitions.Values)
            {
                if (schema.Extensions.TryGetValue(ApiConstants.DotvvmTypeKey, out var objType) && objType is Type underlayingType)
                {
                    if (knownTypes.IsKnownType(underlayingType))
                    {
                        var name = CreateProperName(underlayingType, swaggerDoc);
                        schema.Extensions.Add(ApiConstants.DotvvmKnownTypeKey, name);

                        SetDotvvmNameToProperties(schema, underlayingType);
                    }
                }
            }

            foreach (var definition in swaggerDoc.Definitions)
            {
                definition.Value.Extensions.Remove(ApiConstants.DotvvmTypeKey);
            }
        }

        private void SetDotvvmNameToProperties(Schema schema, Type underlayingType)
        {
            if (schema.Properties == null)
            {
                return;
            }

            foreach (var property in schema.Properties)
            {
                SetDotvvmNameToProperty(underlayingType, property.Key, property.Value);
            }
        }

        private void SetDotvvmNameToProperty(Type type, string propertyName, Schema targetSchema)
        {
            var propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            if (propertyInfo != null)
            {
                targetSchema.Extensions.Add(ApiConstants.DotvvmNameKey, propertySerialization.ResolveName(propertyInfo));
            }
        }

        public string CreateProperName(Type type, SwaggerDocument swaggerDoc)
        {
            if (type.GetGenericArguments().Length == 0)
            {
                return CreateNameWithNamespace(type);
            }

            var genericArguments = type.GetGenericArguments().Select(t => CreateNameForGenericParameter(t, swaggerDoc));
            var unmangledName = GetNameWithoutGenericArity(type);

            return type.Namespace + '.' + unmangledName + '<' + string.Join(",", genericArguments) + '>';
        }

        public string CreateNameForGenericParameter(Type type, SwaggerDocument swaggerDoc)
        {
            var definition = swaggerDoc.Definitions
                .Where(d => d.Value.Extensions.TryGetValue(ApiConstants.DotvvmTypeKey, out var objType) && (Type)objType == type)
                .FirstOrDefault();

            return definition.Key ?? type.FullName;
        }

        public static string GetNameWithoutGenericArity(Type type) => type.Name.Substring(0, type.Name.IndexOf('`'));

        private static string CreateNameWithNamespace(Type type) => type.Namespace + '.' + type.Name;
    }
}
