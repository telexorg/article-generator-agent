﻿using ArticleAgent.Common;
using ArticleAgent.Constants;
using ArticleAgent.Helper;
using ArticleAgent.Models;
using Microsoft.Extensions.Options;
using System.Security.Principal;

namespace ArticleAgent.Data
{
   
    public class DataContext
    {
        private readonly string _baseUrl;
        private readonly HttpHelper _httpHelper;
        private const string CollectionName = "article_agent_db";
        private readonly TaskContextAccessor _taskContextAccessor;
        private readonly ILogger<DataContext> _logger;

        public DataContext(IOptions<TelexApiSettings> options, HttpHelper httphelper, TaskContextAccessor contextAccessor, ILogger<DataContext> logger)
        {
            _baseUrl = options.Value.BaseUrl;
            _taskContextAccessor = contextAccessor;
            _httpHelper = httphelper;
            _logger = logger;
            _baseUrl += "/agent_db/collections";
        }

        public TaskContext TaskContext =>
            _taskContextAccessor.GetTaskContext();

        public async Task<TelexApiResponse<T?>> CreateCollection<T>()
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = _baseUrl,
                Body = new
                {
                    collection_name = CollectionName,
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }


        public async Task<TelexApiResponse<List<T?>>> GetAll<T>(Dictionary<string, object> filter = null)
        {
            if (filter == null)
            {
                filter = new Dictionary<string, object>();
            }

            filter["tag"] = CollectionType.ResolveTagName<T>();

            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"{_baseUrl}/{CollectionName}/documents",
                Body = new
                {
                    Filter = filter
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                return TelexApiResponse<List<T>>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<List<T>>.ExtractResponse(responseContent);

        }


        public async Task<TelexApiResponse<T?>> GetSingle<T>(string id)
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Get,
                Url = $"{_baseUrl}/{CollectionName}/documents/{id}",
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();


            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }
            return TelexApiResponse<T>.ExtractResponse(responseContent);

        }


        public async Task<TelexApiResponse<T?>> AddAsync<T>(T document) where T : IEntity
        {
            string tagName = CollectionType.ResolveTagName<T>();
            document.Tag = tagName;

            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = $"{_baseUrl}/{CollectionName}/documents",
                Body = new
                {
                    Document = document
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }

        public async Task<TelexApiResponse<T?>> UpdateAsync<T>(string id, object document)
        {

            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Put,
                Url = $"{_baseUrl}/{CollectionName}/documents/{id}",
                Body = new
                {
                    Document = document
                },
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }


        public async Task<TelexApiResponse<T?>> DeleteAsync<T>(string id)
        {
            var apiRequest = new ApiRequest()
            {
                Method = HttpMethod.Delete,
                Url = $"{_baseUrl}/{CollectionName}/documents/{id}",
                Headers = PrepareOrgHeader()
            };

            var response = await _httpHelper.SendRequestAsync(apiRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Fallback: if something goes wrong with the HTTP call itself
            if (!response.IsSuccessStatusCode)
            {
                var telexResponse = TelexApiResponse<T>.ErrorResponse(responseContent);
            }

            return TelexApiResponse<T>.ExtractResponse(responseContent);
        }

        private Dictionary<string, string> PrepareOrgHeader()
        {
            if (string.IsNullOrEmpty(TaskContext?.AuthToken))
            {
                _logger.LogError("Auth Token not found in Task Context");
                throw new KeyNotFoundException(nameof(TaskContext.AuthToken));
            }

            return new Dictionary<string, string>()
            {
               {TelexApiSettings.Header, TaskContext.AuthToken}
            };
        }

    }

   
}
