﻿using System;
using DotVVM.Samples.Tests.Base;
using DotVVM.Testing.Abstractions;
using Riganti.Selenium.Core;
using Riganti.Selenium.Core.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace DotVVM.Samples.Tests.Feature
{
    public class ParameterBindingTests : AppSeleniumTest
    {
        public ParameterBindingTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Feature_ParameterBinding_ParameterBinding()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ParameterBinding_ParameterBinding + "/123?B=abc");
                browser.Wait();

                AssertUI.InnerTextEquals(browser.Single(".root-a"), "123");
                AssertUI.InnerTextEquals(browser.Single(".root-b"), "abc");
                AssertUI.InnerTextEquals(browser.Single(".nested-a"), "123");
                AssertUI.InnerTextEquals(browser.Single(".nested-b"), "abc");
            });
        }

        [Fact]
        public void Feature_ParameterBinding_OptionalParameterBinding()
        {
            base.RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ParameterBinding_OptionalParameterBinding);

                ValidateDefaultRouteLinkState(browser);
                AssertUI.TextEmpty(browser.First("#Result"));

                browser.First("#opt1_empty").Click();

                ValidateDefaultRouteLinkState(browser);
                AssertUI.TextEmpty(browser.First("#Result"));

                browser.First("#opt1_param_ID2A2").Click();
                ValidateDefaultRouteLinkState(browser, "/5");
                AssertUI.TextEquals(browser.First("#Result"), "Id:5");

                browser.First("#opt2_param_2").Click();
                ValidateDefaultRouteLinkState(browser, "/3");
                AssertUI.TextEquals(browser.First("#Result"), "Id:3");
            });
        }

        private void ValidateDefaultRouteLinkState(IBrowserWrapper browser, string suffix = "")
        {
            AssertUI.HyperLinkEquals(browser.First("#opt1_empty"), "FeatureSamples/ParameterBinding/OptionalParameterBinding" + suffix, UrlKind.Relative, UriComponents.PathAndQuery);
            AssertUI.HyperLinkEquals(browser.First("#opt1_param_empty"), "FeatureSamples/ParameterBinding/OptionalParameterBinding" + suffix, UrlKind.Relative, UriComponents.PathAndQuery);
            AssertUI.HyperLinkEquals(browser.First("#opt1_param_A2"), "FeatureSamples/ParameterBinding/OptionalParameterBinding" + suffix, UrlKind.Relative, UriComponents.PathAndQuery);

            AssertUI.HyperLinkEquals(browser.First("#opt1_param_ID2"), "FeatureSamples/ParameterBinding/OptionalParameterBinding/4", UrlKind.Relative, UriComponents.PathAndQuery);
            AssertUI.HyperLinkEquals(browser.First("#opt1_param_ID2A2"), "FeatureSamples/ParameterBinding/OptionalParameterBinding/5", UrlKind.Relative, UriComponents.PathAndQuery);

            var suffixWithDefaultValue = string.IsNullOrEmpty(suffix) ? "/300" : suffix;
            AssertUI.HyperLinkEquals(browser.First("#opt2_empty"), "FeatureSamples/ParameterBinding/OptionalParameterBinding2" + suffixWithDefaultValue, UrlKind.Relative, UriComponents.PathAndQuery);
            AssertUI.HyperLinkEquals(browser.First("#opt2_param_A"), "FeatureSamples/ParameterBinding/OptionalParameterBinding2" + suffixWithDefaultValue, UrlKind.Relative, UriComponents.PathAndQuery);
            AssertUI.HyperLinkEquals(browser.First("#opt2_param_2"), "FeatureSamples/ParameterBinding/OptionalParameterBinding2/3", UrlKind.Relative, UriComponents.PathAndQuery);
            AssertUI.HyperLinkEquals(browser.First("#opt2_param_ID2A2"), "FeatureSamples/ParameterBinding/OptionalParameterBinding2/4", UrlKind.Relative, UriComponents.PathAndQuery);
        }
    }
}
