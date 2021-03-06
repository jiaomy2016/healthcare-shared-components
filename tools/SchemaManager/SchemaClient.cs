﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SchemaManager.Exceptions;
using SchemaManager.Model;
using SchemaManager.Utils;

namespace SchemaManager
{
    internal class SchemaClient : ISchemaClient
    {
        private static HttpClient _httpClient;

        public SchemaClient(Uri serverUri)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = serverUri;
        }

        public async Task<List<CurrentVersion>> GetCurrentVersionInformationAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(RelativeUrl(UrlConstants.Current), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var responseBodyAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CurrentVersion>>(responseBodyAsString);
            }
            else
            {
                throw new SchemaManagerException(string.Format(Resources.CurrentDefaultErrorDescription, response.StatusCode));
            }
        }

        public async Task<string> GetScriptAsync(Uri scriptUri, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(scriptUri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new SchemaManagerException(string.Format(Resources.ScriptNotFound, response.StatusCode));
            }
        }

        public async Task<CompatibleVersion> GetCompatibilityAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(RelativeUrl(UrlConstants.Compatibility), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var responseBodyAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CompatibleVersion>(responseBodyAsString);
            }
            else
            {
                throw new SchemaManagerException(string.Format(Resources.CompatibilityDefaultErrorMessage, response.StatusCode));
            }
        }

        public async Task<List<AvailableVersion>> GetAvailabilityAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(RelativeUrl(UrlConstants.Availability), cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var responseBodyAsString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<AvailableVersion>>(responseBodyAsString);
            }
            else
            {
                throw new SchemaManagerException(string.Format(Resources.AvailableVersionsDefaultErrorMessage, response.StatusCode));
            }
        }

        public async Task<string> GetDiffScriptAsync(Uri diffUri, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(diffUri, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new SchemaManagerException(string.Format(Resources.ScriptNotFound, response.StatusCode));
            }
        }

        private Uri RelativeUrl(string url)
        {
            return new Uri(url, UriKind.Relative);
        }
    }
}
