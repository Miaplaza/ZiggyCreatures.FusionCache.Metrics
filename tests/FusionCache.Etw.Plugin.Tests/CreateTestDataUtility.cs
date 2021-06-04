﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using FusionCache.Example.Domain.Model;
using Xunit;
using Xunit.Abstractions;

namespace FusionCache.Etw.Plugin.Tests
{

    public class CreateTestDataUtility
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CreateTestDataUtility(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Data in MockDomainCertData.json and MockEmailtoIpData.json was generated by Makaroo.com
        /// This test utility correlates the domains in MockDomainCertData.json to the email addresses in
        /// MockEmailToIpData.json.  This data will be used to build example apps with caching and metrics needs.
        /// </summary>
        [Fact]
        public void fixupMockEmailToIpData_Json()
        {
            //_testOutputHelper.WriteLine(File.ReadAllText("../../../../../examples/data/MockDomainCertData.json"));
            
            var domains = JsonSerializer.Deserialize<List<DomainCertData>>(File.ReadAllText("../../../../../examples/data/MockDomainCertData.json"));
            
            
        }
    }
}
