﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Testing.Platform.Acceptance.IntegrationTests.Helpers;

namespace Microsoft.Testing.Platform.Acceptance.IntegrationTests;

[TestGroup]
public class DotnetTestCliTests : AcceptanceTestBase
{
    private const string AssetName = "MSTestProject";
    private readonly AcceptanceFixture _acceptanceFixture;

    public DotnetTestCliTests(ITestExecutionContext testExecutionContext, AcceptanceFixture acceptanceFixture)
        : base(testExecutionContext)
    {
        _acceptanceFixture = acceptanceFixture;
    }

    [ArgumentsProvider(nameof(GetBuildMatrixTfmBuildConfiguration))]
    public async Task DotnetTest_Should_Execute_Tests(string tfm, BuildConfiguration buildConfiguration)
    {
        using TestAsset generator = await TestAsset.GenerateAssetAsync(
            AssetName,
            CurrentMSTestSourceCode
            .PatchCodeWithReplace("$TargetFramework$", $"<TargetFramework>{tfm}</TargetFramework>")
            .PatchCodeWithReplace("$MicrosoftNETTestSdkVersion$", MicrosoftNETTestSdkVersion)
            .PatchCodeWithReplace("$MSTestVersion$", MSTestVersion)
            .PatchCodeWithReplace("$EnableMSTestRunner$", string.Empty)
            .PatchCodeWithReplace("$OutputType$", string.Empty)
            .PatchCodeWithReplace("$Extra$", string.Empty),
            addPublicFeeds: true);

        string binlogFile = Path.Combine(generator.TargetAssetPath, "msbuild.binlog");
        var compilationResult = await DotnetCli.RunAsync($"test -m:1 -nodeReuse:false {generator.TargetAssetPath}", _acceptanceFixture.NuGetGlobalPackagesFolder.Path);
        compilationResult.AssertOutputContains("Passed!  - Failed:     0, Passed:     1, Skipped:     0, Total:     1");
    }
}
